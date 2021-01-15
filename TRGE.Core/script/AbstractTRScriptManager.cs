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

        internal AbstractTRLevelManager LevelManager { get; private set; }
        internal AbstractTRScript Script { get; private set; }
        public TREdition Edition => Script.Edition;
        public string OriginalFilePath { get; internal set; }
        public string BackupFilePath { get; internal set; }

        internal AbstractTRScriptManager(string originalFilePath, AbstractTRScript script)
        {
            Initialise(originalFilePath, script);
        }

        private void Initialise(string originalFilePath, AbstractTRScript script)
        {
            OriginalFilePath = originalFilePath;
            CreateBackup();

            (Script = script).Read(OriginalFilePath);

            LevelManager = TRLevelFactory.GetLevelManager(script);

            ReadConfig();
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
    }
}