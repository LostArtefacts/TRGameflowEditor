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

        /// <summary>
        /// The main folder name in the application settings folder that houses all edit information.
        /// </summary>
        protected const string _editDirectoryName = "Edits";
        /// <summary>
        /// The folder name under a specific edit folder where backups are stored.
        /// </summary>
        protected const string _backupDirectoryName = "Backup";
        /// <summary>
        /// The folder name under a specific edit folder where outputs are stored.
        /// </summary>
        protected const string _outputDirectoryName = "Output";
        /// <summary>
        /// The folder name under the outputs folder where temporary files are written to. Handlers should
        /// move these files to Output on successful completion of a save transaction.
        /// </summary>
        protected const string _wipDirectoryName = "WIP";
        /// <summary>
        /// The name of the config file that stores script editing information.
        /// </summary>
        protected const string _scriptConfigFileName = "trge.csf";
        /// <summary>
        /// The name of the config file that stores level editing information.
        /// </summary>
        protected const string _dirConfigFileName = "trle.csf";

        protected OperationMode _mode;

        protected string _editDirectory;
        protected string _orignalScriptFile, _backupScriptFile, _scriptConfigFile;
        protected string _originalDirectory, _directoryConfigFile;

        internal string OriginalDirectory => _originalDirectory;
        internal string OutputDirectory => GetOutputDirectory();
        internal string WIPOutputDirectory => GetWIPOutputDirectory();

        #region History Vars and Events
        protected readonly List<string> _history;
        internal IReadOnlyList<string> History => _history;

        internal event EventHandler<TRHistoryEventArgs> HistoryAdded;
        internal event EventHandler HistoryChanged;
        #endregion

        internal event EventHandler<TRBackupRestoreEventArgs> BackupProgressChanged;
        private TRBackupRestoreEventArgs _backupArgs;

        internal TRIOCoord()
        {
            _history = new List<string>();
        }

        internal TREditor Open(string path, TRScriptOpenOption openOption)
        {
            _mode = Directory.Exists(path) ? OperationMode.Directory : OperationMode.File;
            _orignalScriptFile = _mode == OperationMode.Directory ? FindScriptFile(path) : path;
            _originalDirectory = _mode == OperationMode.Directory ? path : new FileInfo(_orignalScriptFile).DirectoryName;

            // Verify that the script and level editors are compatible - this
            // should be done before any backups are performed.
            if (_mode == OperationMode.Directory)
            {
                TREditor.ValidateCompatibility(TRScriptFactory.OpenScript(_orignalScriptFile), _originalDirectory);
            }

            _editDirectory = GetEditDirectory();

            _backupArgs = new TRBackupRestoreEventArgs()
            {
                ProgressValue = 0,
                ProgressTarget = 1
            };
            CreateBackup();
            
            AbstractTRScriptEditor scriptEditor = GetScriptEditor(openOption);
            TidyBackup(scriptEditor);

            AbstractTRLevelEditor levelEditor = GetLevelEditor(scriptEditor);

            FireBackupProgressChanged(1);

            return new TREditor(WIPOutputDirectory, OutputDirectory, OriginalDirectory)
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
                WIPOutputDirectory = new DirectoryInfo(GetWIPOutputDirectory()),
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
                WIPOutputDirectory = new DirectoryInfo(GetWIPOutputDirectory()),
                OutputDirectory = new DirectoryInfo(GetOutputDirectory())
            };

            return TRLevelEditorFactory.GetLevelEditor(io, scriptEditor.Edition);
        }

        private string FindScriptFile(string path)
        {
            FileInfo fi = TRScriptFactory.FindScriptFile(new DirectoryInfo(path));
            if (fi == null)
            {
                throw new MissingScriptException(string.Format("No valid Tomb Raider script file (.dat) was found in {0}.", path));
            }
            return fi.FullName;
        }

        protected void CreateBackup()
        {
            string backupDirectory = GetBackupDirectory();
            string outputDirectory = GetOutputDirectory();
            if (_mode == OperationMode.Directory)
            {
                DirectoryInfo backupDI = new DirectoryInfo(backupDirectory);
                DirectoryInfo outputDI = new DirectoryInfo(outputDirectory);

                DirectoryInfo originalDI = new DirectoryInfo(_originalDirectory);

                FileInfo[] files = originalDI.GetFilteredFiles(TREditor.TargetFileExtensions);
                _backupArgs.ProgressTarget += files.Length * 2;
                FireBackupProgressChanged();

                Action<FileInfo> progressAction = new Action<FileInfo>
                (
                    fi => FireBackupProgressChanged(1)
                );

                backupDI.CopyInto(files, false, progressAction);
                outputDI.CopyInto(files, false, progressAction);

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

        private void FireBackupProgressChanged(int progress = 0)
        {
            _backupArgs.ProgressValue += progress;
            BackupProgressChanged?.Invoke(this, _backupArgs);
        }

        protected void TidyBackup(AbstractTRScriptEditor scriptEditor)
        {
            DirectoryInfo backupDI = new DirectoryInfo(GetBackupDirectory());
            DirectoryInfo outputDI = new DirectoryInfo(GetOutputDirectory());
            List<string> expectedFiles = new List<string>
            {
                scriptEditor.BackupFile.Name
            };
            foreach (AbstractTRScriptedLevel level in scriptEditor.Levels)
            {
                expectedFiles.Add(level.LevelFileBaseName);
            }

            backupDI.ClearExcept(expectedFiles, TREditor.TargetFileExtensions);
            outputDI.ClearExcept(expectedFiles, TREditor.TargetFileExtensions);
        }

        #region Directory Management
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

        internal string GetWIPOutputDirectory()
        {
            DirectoryInfo outputDirectory = new DirectoryInfo(GetOutputDirectory());
            return outputDirectory.CreateSubdirectory(_wipDirectoryName).FullName;
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
        #endregion

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