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
        protected const string _backupExtension = ".bak";
        protected const string _scriptConfigFileName = "trge.json";
        protected const string _dirConfigFileName = "alt.json";

        protected OperationMode _mode;

        protected string _editDirectory;
        internal string EditDirectory => _editDirectory;

        protected string _orignalScriptFile, _backupScriptFile, _scriptConfigFile;
        internal string ScriptFile => _orignalScriptFile;

        protected string _originalDirectory, _directoryConfigFile;

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

        internal void Initialise(string path)
        {
            _mode = Directory.Exists(path) ? OperationMode.Directory : OperationMode.File;
            _orignalScriptFile = _mode == OperationMode.Directory ? FindScriptFile(path) : path;
            _originalDirectory = _mode == OperationMode.Directory ? path : new FileInfo(_orignalScriptFile).DirectoryName;
            _editDirectory = GetEditDirectory();
            CheckBackup();
        }

        internal AbstractTRScriptManager GetScriptManager(TRScriptOpenOption openOption)
        {
            AbstractTRScriptManager scriptMan = TRScriptFactory.GetScriptManager(_orignalScriptFile, _backupScriptFile, _scriptConfigFile, openOption);
            UpdateFileHistory();

            return scriptMan;
        }

        internal TRLevelEditor GetLevelEditor()
        {
            if (_mode == OperationMode.File)
            {
                return null;
            }
            return new TRLevelEditor(_originalDirectory, GetBackupDirectory());
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
            if (_mode == OperationMode.Directory)
            {
                DirectoryInfo backupDI = new DirectoryInfo(backupDirectory);
                new DirectoryInfo(_originalDirectory).Copy(backupDI, false, new string[] { "*.dat", "*.tr2", "*.psx" });
                backupDI.ClearExcept(ScriptFile);
                string scriptFile = Path.Combine(backupDirectory, new FileInfo(_orignalScriptFile).Name);
                File.Move(scriptFile, _backupScriptFile = scriptFile + _backupExtension);
            }
            else
            {
                string backupFile = Path.Combine(backupDirectory, new FileInfo(_orignalScriptFile).Name + _backupExtension);

                if (!File.Exists(backupFile))
                {
                    File.Copy(_orignalScriptFile, backupFile);
                }
                _backupScriptFile = backupFile;
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
                    if (File.Exists(item))
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