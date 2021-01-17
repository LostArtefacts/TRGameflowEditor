using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace TRGE.Core
{
    public abstract class AbstractTRScriptManager
    {
        protected const string _editsFolder = "Edits";
        protected const string _backupExt = ".bak";
        protected const string _configFile = "trge.json";

        public TREdition Edition => Script.Edition;
        public string OriginalFilePath { get; internal set; }
        public string BackupFilePath { get; internal set; }

        internal AbstractTRLevelManager LevelManager { get; private set; }
        internal AbstractTRFrontEnd FrontEnd { get; private set; }
        internal AbstractTRScript Script { get; private set; }
        
        internal AbstractTRScriptManager(string originalFilePath, AbstractTRScript script)
        {
            Initialise(originalFilePath, script);
        }

        private void Initialise(string originalFilePath, AbstractTRScript script)
        {
            OriginalFilePath = originalFilePath;
            CreateBackup();

            //(Script = script).Read(OriginalFilePath); //actually read from the backup path instead?
            (Script = script).Read(BackupFilePath); //actually read from the backup path instead?
            FrontEnd = script.FrontEnd;

            LevelManager = TRLevelFactory.GetLevelManager(script);

            ReadConfig();

            //TODO: if this is not the first time opening the file, run through
            //the config from the last time so the script is in the same
            //state, but we still have any inactive operations still in place
        }

        protected void CreateBackup()
        {
            string editFolder = GetEditFolder();
            FileInfo originalFile = new FileInfo(OriginalFilePath);
            FileInfo backupFile = new FileInfo(Path.Combine(editFolder, originalFile.Name + _backupExt));

            if (!backupFile.Exists)
            {
                File.Copy(originalFile.FullName, backupFile.FullName);
            }

            BackupFilePath = backupFile.FullName;
        }

        public string GetEditFolder()
        {
            string folder = Path.Combine(TRGameflowEditor.Instance.GetConfigDirectory(), _editsFolder, Hashing.CreateMD5(OriginalFilePath, Encoding.UTF8));
            Directory.CreateDirectory(folder);
            return folder;
        }

        internal string GetConfigFile()
        {
            return Path.Combine(GetEditFolder(), _configFile);
        }

        protected void ReadConfig()
        {
            string configFile = GetConfigFile();
            Dictionary<string, object> config = File.Exists(configFile) ? JsonConvert.DeserializeObject<Dictionary<string, object>>(File.ReadAllText(configFile)) : null;
            if (config != null)
            {
                Dictionary<string, object> levelSeq = JsonConvert.DeserializeObject<Dictionary<string, object>>(config["LevelSequencing"].ToString());
                LevelOrganisation = (Organisation)Enum.ToObject(typeof(Organisation), levelSeq["Organisation"]);
                LevelRNG = new RandomGenerator(JsonConvert.DeserializeObject<Dictionary<string, object>>(levelSeq["RNG"].ToString()));
                LevelSequencing = JsonConvert.DeserializeObject<List<Tuple<string, string>>>(levelSeq["Data"].ToString());

                FrontEndHasFMV = bool.Parse(config["FrontEndFMVOn"].ToString());
            }
            else
            {
                LevelOrganisation = Organisation.Default;
                LevelRNG = new RandomGenerator(RandomGenerator.Type.Date);
            }

            LoadConfig(config);
        }

        internal void Save(string filePath)
        {
            Dictionary<string, object> config = new Dictionary<string, object>
            {
                ["Version"] = Assembly.GetExecutingAssembly().GetName().Version,
                ["Original"] = OriginalFilePath,
                ["FrontEndFMVOn"] = FrontEndHasFMV,
                ["LevelSequencing"] = new Dictionary<string, object>
                {
                    ["Organisation"] = (int)LevelOrganisation,
                    ["RNG"] = LevelRNG.ToJson(),
                    ["Data"] = LevelSequencing
                }
            };

            AbstractTRScript backupScript = LoadBackupScript();
            if (LevelOrganisation == Organisation.Random)
            {
                LevelManager.RandomiseLevelSequencing(backupScript.Levels);
            }
            else if (LevelOrganisation == Organisation.Default)
            {
                LevelManager.RestoreLevelSequencing(backupScript.Levels);
            }

            SaveImpl(config);
            LevelManager.Save();

            File.WriteAllText(GetConfigFile(), JsonConvert.SerializeObject(config, Formatting.Indented));

            string localCopyPath = Path.Combine(GetEditFolder(), new FileInfo(filePath).Name);
            Script.Write(localCopyPath);
            File.Copy(localCopyPath, filePath, true);
        }

        public void Restore()
        {
            File.Copy(BackupFilePath, OriginalFilePath, true);
            Initialise(OriginalFilePath, Script);
        }

        internal abstract AbstractTRScript LoadBackupScript();
        protected abstract void SaveImpl(Dictionary<string, object> config);
        protected abstract void LoadConfig(Dictionary<string, object> config);

        public Organisation LevelOrganisation
        {
            get => LevelManager.LevelOrganisation;
            set => LevelManager.LevelOrganisation = value;
        }

        public RandomGenerator LevelRNG
        {
            get => LevelManager.LevelRNG;
            set => LevelManager.LevelRNG = value;
        }

        public List<Tuple<string, string>> LevelSequencing
        {
            get => LevelManager.GetLevelSequencing();
            set => LevelManager.SetLevelSequencing(value);
        }

        internal void RandomiseLevels()
        {
            LevelManager.RandomiseLevelSequencing(LoadBackupScript().Levels);
        }

        public bool FrontEndHasFMV
        {
            get => FrontEnd.HasFMV;
            set => FrontEnd.HasFMV = value;
        }
    }
}