using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;

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

        private const string _resourceURLBase = "https://github.com/lahm86/TRGameflowEditor/raw/main/Resources/";

        private const string _configFile = "config.json";
        private string _configDirectory;

        private readonly List<AbstractTRScriptManager> _activeScriptManagers;
        private readonly List<FileInfo> _fileHistory;

        public IReadOnlyList<FileInfo> FileHistory => _fileHistory;

        public event EventHandler<TRFileEventArgs> FileHistoryAdded;
        public event EventHandler FileHistoryChanged;

        public event EventHandler<TRDownloadEventArgs> ResourceDownloading;

        private TRGameflowEditor()
        {
            _activeScriptManagers = new List<AbstractTRScriptManager>();

            _fileHistory = new List<FileInfo>();

            _configDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData));
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

        public void Save(AbstractTRScriptManager scriptMan, string filePath)
        {
            scriptMan.Save(filePath);
            //TODO:what if file path has changed...history update?
        }

        public void CloseScriptManager(AbstractTRScriptManager manager)
        {
            _activeScriptManagers.Remove(manager);
        }

        public void CloseAllScriptManagers()
        {
            _activeScriptManagers.Clear();
        }

        internal string GetConfigDirectory()
        {
            string path = Path.Combine(_configDirectory, "TRGE");
            Directory.CreateDirectory(path);
            return path;
        }

        internal void SetConfigDirectory(string newDirectory)
        {
            _configDirectory = newDirectory;
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
            return Path.Combine(GetConfigDirectory(), _configFile);
        }

        //TODO: this remains untested
        internal bool DownloadResourceFile(string urlPath, string targetFile)
        {
            string url = _resourceURLBase + urlPath;

            TRDownloadEventArgs args = new TRDownloadEventArgs
            {
                URL = url,
                TargetFile = targetFile
            };

            ResourceDownloading?.Invoke(this, args);

            try
            {
                HttpWebRequest req = WebRequest.CreateHttp(url);
                using (WebResponse response = req.GetResponse())
                using (Stream receiveStream = response.GetResponseStream())
                using (FileStream ouputStream = File.OpenWrite(targetFile))
                {
                    args.DownloadLength = response.ContentLength;
                    args.Status = TRDownloadStatus.Downloading;
                    ResourceDownloading?.Invoke(this, args);

                    byte[] buffer = new byte[1024];
                    int size;
                    while ((size = receiveStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        ouputStream.Write(buffer, 0, size);
                        args.DownloadProgress += size;
                        args.DownloadDifference = size;
                        ResourceDownloading?.Invoke(this, args);
                    }

                    args.Status = TRDownloadStatus.Completed;
                }
            }
            catch (Exception e)
            {
                args.Exception = e;
                args.Status = TRDownloadStatus.Failed;
            }

            ResourceDownloading?.Invoke(this, args);

            return args.Status == TRDownloadStatus.Completed;
        }
    }
}