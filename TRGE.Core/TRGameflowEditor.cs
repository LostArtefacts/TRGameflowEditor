using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("TRGE.Core.Test")]
namespace TRGE.Core
{
    public class TRGameflowEditor
    {
        private static TRGameflowEditor _instance;

        public static TRGameflowEditor Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new TRGameflowEditor();
                }
                return _instance;
            }
        }

        private const string ConfigFile = "config.json";
        private readonly List<AbstractTRScriptManager> _activeScriptManagers;
        private readonly List<FileInfo> _fileHistory;
        public IReadOnlyList<FileInfo> FileHistory => _fileHistory;

        public event EventHandler<TRFileEventArgs> FileHistoryAdded;
        public event EventHandler FileHistoryChanged;

        private TRGameflowEditor()
        {
            _activeScriptManagers = new List<AbstractTRScriptManager>();

            _fileHistory = new List<FileInfo>();

            string configPath = GetConfigPath();
            if (File.Exists(configPath))
            {
                Dictionary<string, object> config = JsonConvert.DeserializeObject<Dictionary<string, object>>(File.ReadAllText(configPath));
                if (config.ContainsKey("History"))
                {
                    string[] history = JsonConvert.DeserializeObject<string[]>(config["History"].ToString());
                    foreach (string item in history)
                    {
                        if (File.Exists(item))
                        {
                            _fileHistory.Add(new FileInfo(item));
                            FireHistoryAdded(item);
                        }
                    }
                    FireHistoryChanged();
                }
            }
        }

        public AbstractTRScriptManager GetScriptManager(string filePath)
        {
            filePath = new FileInfo(filePath).FullName;
            foreach (AbstractTRScriptManager sm in _activeScriptManagers)
            {
                if (sm.OriginalFilePath.ToLower().Equals(filePath.ToLower()))
                {
                    return sm;
                }
            }

            AbstractTRScriptManager scriptMan = TRScriptFactory.GetScriptManager(filePath);
            _activeScriptManagers.Add(scriptMan);
            UpdateFileHistory(filePath);
            return scriptMan;
        }

        public void CloseScriptManager(AbstractTRScriptManager manager)
        {
            _activeScriptManagers.Remove(manager);
        }

        public void CloseAllScriptManagers()
        {
            _activeScriptManagers.Clear();
        }

        internal string GetAppDataPath()
        {
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "TRGE");
            Directory.CreateDirectory(path);
            return path;
        }

        public void ClearFileHistory()
        {
            _fileHistory.Clear();
            WriteConfig();
            FireHistoryChanged();
        }

        private void UpdateFileHistory(string filePath)
        {
            FileInfo fi = new FileInfo(filePath);
            int j = GetFileHistoryIndex(filePath);
            _fileHistory.Insert(0, fi);
            if (j != -1)
            {
                _fileHistory.RemoveAt(j + 1);
            }

            while (_fileHistory.Count > 10)
            {
                _fileHistory.RemoveAt(_fileHistory.Count - 1);
            }

            WriteConfig();
            FireHistoryAdded(filePath);
        }

        private int GetFileHistoryIndex(string filePath)
        {
            filePath = filePath.ToLower();
            for (int i = 0; i < _fileHistory.Count; i++)
            {
                if (_fileHistory[i].FullName.ToLower().Equals(filePath))
                {
                    return i;
                }
            }
            return -1;
        }

        private void FireHistoryChanged()
        {
            FileHistoryChanged?.Invoke(this, EventArgs.Empty);
        }

        private void FireHistoryAdded(string filePath)
        {
            FileHistoryAdded?.Invoke(this, new TRFileEventArgs(filePath));
        }

        private void WriteConfig()
        {
            string[] history = new string[_fileHistory.Count];
            for (int i = 0; i < history.Length; i++)
            {
                history[i] = _fileHistory[i].FullName;
            }

            Dictionary<string, object> config = new Dictionary<string, object>
            {
                ["History"] = history
            };

            File.WriteAllText(GetConfigPath(), JsonConvert.SerializeObject(config, Formatting.Indented));
        }

        private string GetConfigPath()
        {
            return Path.Combine(GetAppDataPath(), ConfigFile);
        }
    }
}