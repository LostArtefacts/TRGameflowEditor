using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using TRGE.Core;

namespace TRGE.Coord
{
    public class TRCoord
    {
        private static TRCoord _instance;
        public static TRCoord Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new TRCoord();
                }
                return _instance;
            }
        }

        private const string _configDirectoryName = "TRGE";
        private const string _globalConfigFileName = "config.json";

        private string _rootConfigDirectory;
        private readonly TRFileCoord _fileCoord;

        private AbstractTRScriptManager _scriptManager;

        private TRCoord()
        {
            RootConfigDirectory = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

            Dictionary<string, object> config = LoadConfig();
            _fileCoord = new TRFileCoord();
            _fileCoord.SetConfig(config == null ? null : (config.ContainsKey("History") ? config["History"] : null));

            _fileCoord.FileHistoryAdded += TRFileHistoryAdded;
            _fileCoord.FileHistoryChanged += TRFileHistoryChanged;
            TRDownloader.ResourceDownloading += TRResourceDownloading;
        }

        #region Global Config
        /// <summary>
        /// The root config directory under which the application ConfigDirectory is stored. The default is Environment.SpecialFolder.LocalApplicationData.
        /// </summary>
        public string RootConfigDirectory
        {
            get => _rootConfigDirectory;
            set
            {
                _rootConfigDirectory = value;
                TRInterop.ConfigDirectory = Directory.CreateDirectory(ConfigDirectory).FullName;
            }
        }

        /// <summary>
        /// The specific directory under RootConfigDirectory where global configuration is stored.
        /// </summary>
        public string ConfigDirectory => Path.Combine(_rootConfigDirectory, _configDirectoryName);

        /// <summary>
        /// The path to the global configuration JSON file.
        /// </summary>
        public string ConfigFilePath => Path.Combine(ConfigDirectory, _globalConfigFileName);

        private Dictionary<string, object> LoadConfig()
        {
            Dictionary<string, object> config = null;
            if (File.Exists(ConfigFilePath))
            {
                config = JsonConvert.DeserializeObject<Dictionary<string, object>>(File.ReadAllText(ConfigFilePath));
            }

            return config;
        }

        private void StoreConfig()
        {
            Dictionary<string, object> config = new Dictionary<string, object>
            {
                ["History"] = _fileCoord.GetConfig()
            };

            File.WriteAllText(ConfigFilePath, JsonConvert.SerializeObject(config, Formatting.Indented));
        }
        #endregion

        #region Event Bubbles
        public event EventHandler<TRFileHistoryEventArgs> FileHistoryAdded;
        public event EventHandler FileHistoryChanged;

        private void TRFileHistoryAdded(object sender, TRFileHistoryEventArgs e)
        {
            FileHistoryAdded?.Invoke(this, e);
            StoreConfig();
        }

        private void TRFileHistoryChanged(object sender, EventArgs e)
        {
            FileHistoryChanged?.Invoke(this, e);
            StoreConfig();
        }

        public event EventHandler<TRDownloadEventArgs> ResourceDownloading;

        private void TRResourceDownloading(object sender, TRDownloadEventArgs e)
        {
            ResourceDownloading?.Invoke(this, e);
        }
        #endregion

        public AbstractTRScriptManager OpenScript(string scriptFile, TRScriptOpenOption openOption = TRScriptOpenOption.Default)
        {
            return OpenScript(new FileInfo(scriptFile), openOption);
        }

        public AbstractTRScriptManager OpenScript(FileInfo scriptFile, TRScriptOpenOption openOption = TRScriptOpenOption.Default)
        {
            _scriptManager = _fileCoord.GetScriptManager(scriptFile, openOption);
            _scriptManager.LevelModified += ScriptManagerLevelModified;

            return _scriptManager;
        }

        private void ScriptManagerLevelModified(object sender, TRScriptedLevelEventArgs e)
        {
            
        }

        public void SaveScript(AbstractTRScriptManager scriptMan)
        {
            scriptMan.Save();
        }

        public void RestoreScript(AbstractTRScriptManager scriptMan)
        {
            scriptMan.Restore();
        }

        public void ClearHistory()
        {
            _fileCoord.ClearHistory();
        }
    }
}