using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using TRGE.Core;

namespace TRGE.Coord
{
    internal class TRIOCoord : ITRConfigProvider
    {
        internal enum OperationMode
        {
            File, Directory
        }

        protected const string _editDirectoryName = "Edits";
        protected const string _backupDirectoryName = "Backup";
        protected const string _outputDirectoryName = "Output";
        protected const string _scriptConfigFileName = "trge.csf";
        protected const string _dirConfigFileName = "trle.csf";

        protected OperationMode _mode;

        protected string _editDirectory;
        protected string _orignalScriptFile, _backupScriptFile, _scriptConfigFile;
        protected string _originalDirectory, _directoryConfigFile;

        internal string OriginalDirectory => _originalDirectory;
        internal string OutputDirectory => GetOutputDirectory();

        #region History Vars and Events
        protected readonly List<string> _history;
        internal IReadOnlyList<string> History => _history;

        internal event EventHandler<TRHistoryEventArgs> HistoryAdded;
        internal event EventHandler HistoryChanged;
        #endregion

        internal TRIOCoord()
        {
            _history = new List<string>();
        }

        internal TREditor Open(string path, TRScriptOpenOption openOption)
        {
            _mode = Directory.Exists(path) ? OperationMode.Directory : OperationMode.File;
            _orignalScriptFile = _mode == OperationMode.Directory ? FindScriptFile(path) : path;
            _originalDirectory = _mode == OperationMode.Directory ? path : new FileInfo(_orignalScriptFile).DirectoryName;
            _editDirectory = GetEditDirectory();
            CheckBackup();

            AbstractTRScriptEditor scriptEditor = GetScriptEditor(openOption);
            AbstractTRLevelEditor levelEditor = GetLevelEditor(scriptEditor);
            return new TREditor(OutputDirectory, OriginalDirectory)
            {
                ScriptEditor = scriptEditor,
                LevelEditor = levelEditor
            };
        }

        internal AbstractTRScriptEditor GetScriptEditor(TRScriptOpenOption openOption)
        {
            TRScriptIOArgs io = new TRScriptIOArgs
            {
                OriginalFile = new FileInfo(_orignalScriptFile),
                BackupFile = new FileInfo(_backupScriptFile),
                ConfigFile = new FileInfo(_scriptConfigFile),
                OutputDirectory = new DirectoryInfo(GetOutputDirectory())
            };
            AbstractTRScriptEditor scriptMan = TRScriptFactory.GetScriptEditor(io, openOption);

            UpdateFileHistory();

            return scriptMan;
        }

        internal AbstractTRLevelEditor GetLevelEditor(AbstractTRScriptEditor scriptEditor)
        {
            if (_mode == OperationMode.File)
            {
                return null;
            }

            TRDirectoryIOArgs io = new TRDirectoryIOArgs
            {
                OriginalDirectory = new DirectoryInfo(_originalDirectory),
                BackupDirectory = new DirectoryInfo(GetBackupDirectory()),
                ConfigFile = new FileInfo(_directoryConfigFile),
                OutputDirectory = new DirectoryInfo(GetOutputDirectory())
            };

            return TRLevelEditorFactory.GetLevelEditor(io, scriptEditor.Edition);
        }

        private string FindScriptFile(string path)
        {
            FileInfo fi = TRScriptFactory.FindScriptFile(new DirectoryInfo(path));
            if (fi == null)
            {
                throw new MissingScriptException(string.Format("No valid script file (.dat) was found in {0}.", path));
            }
            return fi.FullName;
        }

        protected void CheckBackup()
        {
            string backupDirectory = GetBackupDirectory();
            string outputDirectory = GetOutputDirectory();
            if (_mode == OperationMode.Directory)
            {
                DirectoryInfo backupDI = new DirectoryInfo(backupDirectory);
                DirectoryInfo outputDI = new DirectoryInfo(outputDirectory);

                DirectoryInfo originalDI = new DirectoryInfo(_originalDirectory);
                originalDI.Copy(backupDI, false, TREditor.TargetFileExtensions);
                backupDI.ClearExcept(_orignalScriptFile);
                backupDI.Copy(outputDI, false);

                _backupScriptFile = Path.Combine(backupDirectory, new FileInfo(_orignalScriptFile).Name);
            }
            else
            {
                FileInfo fi = new FileInfo(_orignalScriptFile);
                _backupScriptFile = Path.Combine(backupDirectory, fi.Name);
                if (!File.Exists(_backupScriptFile))
                {
                    File.Copy(_orignalScriptFile, _backupScriptFile);
                }

                string outputFile = Path.Combine(outputDirectory, fi.Name);
                if (!File.Exists(outputFile))
                {
                    File.Copy(_orignalScriptFile, outputFile);
                }
            }

            _scriptConfigFile = Path.Combine(_editDirectory, _scriptConfigFileName);
            _directoryConfigFile = Path.Combine(_editDirectory, _dirConfigFileName);
        }

        internal string GetEditDirectory()
        {
            DirectoryInfo topLevelEditDirectory = Directory.CreateDirectory(Path.Combine(TRCoord.Instance.ConfigDirectory, _editDirectoryName));
            return topLevelEditDirectory.CreateSubdirectory(HashingExtensions.CreateMD5(_orignalScriptFile)).FullName;
        }

        internal string GetBackupDirectory()
        {
            DirectoryInfo editDirectory = new DirectoryInfo(GetEditDirectory());
            return editDirectory.CreateSubdirectory(_backupDirectoryName).FullName;
        }

        internal string GetOutputDirectory()
        {
            DirectoryInfo editDirectory = new DirectoryInfo(GetEditDirectory());
            return editDirectory.CreateSubdirectory(_outputDirectoryName).FullName;
        }

        internal string GetOriginalDirectory()
        {
            return _originalDirectory;
        }

        #region History Methods
        protected void FireHistoryChanged()
        {
            HistoryChanged?.Invoke(this, EventArgs.Empty);
        }

        protected void FireHistoryAdded(string filePath)
        {
            HistoryAdded?.Invoke(this, new TRHistoryEventArgs(filePath));
        }

        protected void UpdateFileHistory()
        {
            string target = _mode == OperationMode.Directory ? _originalDirectory : _orignalScriptFile;

            int j = GetFileHistoryIndex(target);
            _history.Insert(0, target);
            if (j != -1)
            {
                _history.RemoveAt(j + 1);
            }

            while (_history.Count > 10)
            {
                _history.RemoveAt(_history.Count - 1);
            }
            FireHistoryAdded(target);
        }

        protected int GetFileHistoryIndex(string filePath)
        {
            filePath = filePath.ToLower();
            for (int i = 0; i < _history.Count; i++)
            {
                if (_history[i].ToLower().Equals(filePath))
                {
                    return i;
                }
            }
            return -1;
        }

        internal void ClearHistory()
        {
            string mainEditDirectory = Path.Combine(TRCoord.Instance.ConfigDirectory, _editDirectoryName);
            if (Directory.Exists(mainEditDirectory))
            {
                Directory.Delete(mainEditDirectory, true);
            }
            _history.Clear();
            FireHistoryChanged();
        }
        #endregion

        #region ITRConfigProvider
        public void SetConfig(object config)
        {
            if (config == null)
            {
                _history.Clear();
            }
            else
            {
                string[] history = JsonConvert.DeserializeObject<string[]>(config.ToString());
                foreach (string item in history)
                {
                    if (File.Exists(item) || Directory.Exists(item))
                    {
                        _history.Add(item);
                        FireHistoryAdded(item);
                    }
                }

                if (history.Length == 0)
                {
                    FireHistoryChanged();
                }
            }
        }

        public object GetConfig()
        {
            return _history.ToArray();
        }
        #endregion
    }
}