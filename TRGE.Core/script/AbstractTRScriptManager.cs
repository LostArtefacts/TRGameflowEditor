using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
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
            string folder = Path.Combine(TRGameflowEditor.Instance.GetAppDataPath(), _editsFolder, Hashing.CreateMD5(OriginalFilePath, Encoding.UTF8));
            Directory.CreateDirectory(folder);
            return folder;
        }

        protected void ReadConfig()
        {
            string configFile = Path.Combine(GetEditFolder(), _configFile);
            Dictionary<string, object> config = File.Exists(configFile) ? JsonConvert.DeserializeObject<Dictionary<string, object>>(File.ReadAllText(configFile)) : null;
            if (config != null)
            {
                Dictionary<string, object> levelSeq = JsonConvert.DeserializeObject<Dictionary<string, object>>(config["LevelSeq"].ToString());
                LevelOrganisation = (Organisation)Enum.ToObject(typeof(Organisation), levelSeq["Org"]);
                LevelRNG = new RandomGenerator(JsonConvert.DeserializeObject<Dictionary<string, object>>(levelSeq["RNG"].ToString()));
                if (LevelOrganisation == Organisation.Manual)
                {
                    LevelSequencing = JsonConvert.DeserializeObject<List<Tuple<string, string>>>(levelSeq["Manual"].ToString());
                }

                Dictionary<string, object> bonuses = JsonConvert.DeserializeObject<Dictionary<string, object>>(config["Bonuses"].ToString());
                BonusOrganisation = (Organisation)Enum.ToObject(typeof(Organisation), bonuses["Org"]);
                BonusRNG = new RandomGenerator(JsonConvert.DeserializeObject<Dictionary<string, object>>(bonuses["RNG"].ToString()));
                if (BonusOrganisation == Organisation.Manual)
                {
                    LevelBonusData = JsonConvert.DeserializeObject<List<MutableTuple<string, string, List<MutableTuple<ushort, TRItemCategory, string, int>>>>>(levelSeq["Manual"].ToString());
                }

                FrontEndHasFMV = bool.Parse(config["FrontEndFMVOn"].ToString());
            }

            LoadConfig(config);
        }

        public void Save(string filePath)
        {

        }

        public void Restore()
        {
            File.Copy(BackupFilePath, OriginalFilePath, true);
            Initialise(OriginalFilePath, Script);
        }

        internal abstract AbstractTRScript LoadBackupScript();
        protected abstract void PrepareSave();
        protected abstract void LoadConfig(Dictionary<string, object> config);
        protected abstract Dictionary<string, object> CreateConfig();

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
            LevelManager.RandomiseLevels(LoadBackupScript().Levels);
        }

        internal bool CanRandomiseBonues => LevelManager.CanRandomiseBonuses;
        public Organisation BonusOrganisation
        {
            get => LevelManager.BonusOrganisation;
            set => LevelManager.BonusOrganisation = value;
        }

        public RandomGenerator BonusRNG
        {
            get => LevelManager.BonusRNG;
            set => LevelManager.BonusRNG = value;
        }

        public List<MutableTuple<string, string, List<MutableTuple<ushort, TRItemCategory, string, int>>>> LevelBonusData
        {
            get => LevelManager.GetLevelBonusData();
            set => LevelManager.SetLevelBonusData(value);
        }

        internal void RandomiseBonuses()
        {
            LevelManager.RandomiseBonuses();
        }

        public bool FrontEndHasFMV
        {
            get => FrontEnd.HasFMV;
            set => FrontEnd.HasFMV = value;
        }
    }
}