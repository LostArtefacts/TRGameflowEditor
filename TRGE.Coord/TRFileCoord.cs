using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using TRGE.Core;

namespace TRGE.Coord
{
    internal class TRFileCoord : ITRConfigProvider
    {
        private const string _editDirectoryName = "Edits";
        private const string _backupDirectoryName = "Backup";
        private const string _backupExtension = ".bak";
        private const string _configFileName = "trge.json";

        private readonly List<FileInfo> _fileHistory;
        internal IReadOnlyList<FileInfo> FileHistory => _fileHistory;

        internal event EventHandler<TRFileHistoryEventArgs> FileHistoryAdded;
        internal event EventHandler FileHistoryChanged;

        private DirectoryInfo _editDirectory;
        internal DirectoryInfo EditDirectory => _editDirectory;

        private FileInfo _orignalScriptFile, _backupScriptFile, _configFile;
        internal FileInfo ScriptFile => _orignalScriptFile;

        internal TRFileCoord()
        {
            _fileHistory = new List<FileInfo>();
        }

        internal AbstractTRScriptManager GetScriptManager(FileInfo scriptFile, TRScriptOpenOption openOption)
        {
            _orignalScriptFile = scriptFile;
            _editDirectory = GetEditDirectory();
            CheckBackup();

            AbstractTRScriptManager scriptMan = TRScriptFactory.GetScriptManager(_orignalScriptFile, _backupScriptFile, _configFile, openOption);

            UpdateFileHistory();

            return scriptMan;
        }

        private DirectoryInfo GetEditDirectory()
        {
            DirectoryInfo topLevelEditDirectory = Directory.CreateDirectory(Path.Combine(TRCoord.Instance.ConfigDirectory, _editDirectoryName));
            return topLevelEditDirectory.CreateSubdirectory(Hashing.CreateMD5(_orignalScriptFile.FullName));
        }

        private void CheckBackup()
        {
            DirectoryInfo backupDirectory = _editDirectory.CreateSubdirectory(_backupDirectoryName);
            FileInfo backupFile = new FileInfo(Path.Combine(backupDirectory.FullName, _orignalScriptFile.Name + _backupExtension));

            if (!backupFile.Exists)
            {
                File.Copy(_orignalScriptFile.FullName, backupFile.FullName);
            }
            _backupScriptFile = backupFile;
            _configFile = new FileInfo(Path.Combine(_editDirectory.FullName, _configFileName));
        }

        private void FireHistoryChanged()
        {
            FileHistoryChanged?.Invoke(this, EventArgs.Empty);
        }

        private void FireHistoryAdded(FileInfo file)
        {
            FileHistoryAdded?.Invoke(this, new TRFileHistoryEventArgs(file));
        }

        private void UpdateFileHistory()
        {
            int j = GetFileHistoryIndex(_orignalScriptFile);
            _fileHistory.Insert(0, _orignalScriptFile);
            if (j != -1)
            {
                _fileHistory.RemoveAt(j + 1);
            }

            while (_fileHistory.Count > 10)
            {
                _fileHistory.RemoveAt(_fileHistory.Count - 1);
            }
            FireHistoryAdded(_orignalScriptFile);
        }

        private int GetFileHistoryIndex(FileInfo file)
        {
            string filePath = file.FullName.ToLower();
            for (int i = 0; i < _fileHistory.Count; i++)
            {
                if (_fileHistory[i].FullName.ToLower().Equals(filePath))
                {
                    return i;
                }
            }
            return -1;
        }

        internal void ClearHistory()
        {
            _fileHistory.Clear();
            FireHistoryChanged();
        }

        public void SetConfig(object config)
        {
            if (config == null)
            {
                _fileHistory.Clear();
            }
            else
            {
                string[] history = JsonConvert.DeserializeObject<string[]>(config.ToString());
                foreach (string item in history)
                {
                    if (File.Exists(item))
                    {
                        FileInfo fi = new FileInfo(item);
                        _fileHistory.Add(fi);
                        FireHistoryAdded(fi);
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
            string[] history = new string[_fileHistory.Count];
            for (int i = 0; i < history.Length; i++)
            {
                history[i] = _fileHistory[i].FullName;
            }
            return history;
        }
    }
}