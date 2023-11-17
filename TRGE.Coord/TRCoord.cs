using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using TRGE.Coord.Helpers;
using TRGE.Core;

[assembly: InternalsVisibleTo("TRGE.Core.Test")]
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
        private const string _globalConfigFileName = "config_{0}.json";

        private string _rootConfigDirectory;
        private readonly TRIOCoord _trioCoord;

        public IReadOnlyList<string> History => _trioCoord.History;

        private TRCoord()
        {
            RootConfigDirectory = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

            Dictionary<string, object> config = LoadConfig();
            _trioCoord = new TRIOCoord();
            _trioCoord.SetConfig(config == null ? null : (config.ContainsKey("History") ? config["History"] : null));

            _trioCoord.HistoryAdded += TRHistoryAdded;
            _trioCoord.HistoryChanged += TRHistoryChanged;
            TRDownloader.ResourceDownloading += TRResourceDownloading;
        }

        #region Global Config
        /// <summary>
        /// The root config directory under which the application ConfigDirectory is stored. The default is Environment.SpecialFolder.LocalApplicationData.
        /// </summary>
        public string RootConfigDirectory
        {
            get => _rootConfigDirectory;
            internal set
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
        public string ConfigFilePath => Path.Combine(ConfigDirectory, string.Format(_globalConfigFileName, TRInterop.ExecutingVersionName));

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
            Dictionary<string, object> config = new()
            {
                ["History"] = _trioCoord.GetConfig()
            };

            File.WriteAllText(ConfigFilePath, JsonConvert.SerializeObject(config, Formatting.Indented));
        }
        #endregion

        #region Event Bubbles
        public event EventHandler<TRHistoryEventArgs> HistoryAdded;
        public event EventHandler HistoryChanged;

        private void TRHistoryAdded(object sender, TRHistoryEventArgs e)
        {
            HistoryAdded?.Invoke(this, e);
            StoreConfig();
        }

        private void TRHistoryChanged(object sender, EventArgs e)
        {
            HistoryChanged?.Invoke(this, e);
            StoreConfig();
        }

        public event EventHandler<TRDownloadEventArgs> ResourceDownloading;

        private void TRResourceDownloading(object sender, TRDownloadEventArgs e)
        {
            ResourceDownloading?.Invoke(this, e);
        }
        #endregion

        public event EventHandler<TRBackupRestoreEventArgs> BackupProgressChanged
        {
            add => _trioCoord.BackupProgressChanged += value;
            remove => _trioCoord.BackupProgressChanged -= value;
        }

        public TREditor Open(FileInfo editFilePath, TRScriptOpenOption openOption = TRScriptOpenOption.Default, TRBackupChecksumOption checksumOption = TRBackupChecksumOption.PerformCheck)
        {
            return Open(editFilePath.FullName, openOption, checksumOption);
        }

        public TREditor Open(DirectoryInfo editFolderPath, TRScriptOpenOption openOption = TRScriptOpenOption.Default, TRBackupChecksumOption checksumOption = TRBackupChecksumOption.PerformCheck)
        {
            return Open(editFolderPath.FullName, openOption, checksumOption);
        }

        /// <summary>
        /// Uses the specified script file or directory path to create a TREditor instance, making all necessary
        /// backups and reloading previous configuration settings. If there is an issue with the previous configuration, 
        /// errorOption can be specified to restore the last backup or to start afresh.
        /// </summary>
        /// <param name="path">The full path to the script file or directory containing a script file and level files.</param>
        /// <param name="errorOption">The action to take if an error is detected with the previous configuration.</param>
        /// <param name="checksumOption">Level file checksum handling.</param>
        /// <returns></returns>
        public TREditor Open(string path, TRScriptOpenOption errorOption = TRScriptOpenOption.Default, TRBackupChecksumOption checksumOption = TRBackupChecksumOption.PerformCheck)
        {
            return _trioCoord.Open(path, errorOption, checksumOption);
        }

        public void ClearHistory()
        {
            _trioCoord.ClearHistory();
        }

        public void ClearCurrentBackup()
        {
            _trioCoord.ClearBackup();
        }
    }
}