using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace TRGE.Core
{
    public class TR23ScriptManager : AbstractTRScriptManager
    {
        internal TR23ScriptManager(string originalFilePath)
            : base(originalFilePath, new TR23Script()) { }

        internal TR23ScriptManager(string originalFilePath, TR23Script script)
            : base(originalFilePath, script) { }

        protected override Dictionary<string, object> CreateConfig()
        {
            throw new NotImplementedException();
        }

        protected override void LoadConfig(Dictionary<string, object> config)
        {
            if (config == null)
            {
                return;
            }

            Dictionary<string, object> unarmedLevels = JsonConvert.DeserializeObject<Dictionary<string, object>>(config["Unarmed"].ToString());
            UnarmedLevelOrganisation = (Organisation)Enum.ToObject(typeof(Organisation), unarmedLevels["Org"]);
            UnarmedLevelRNG = new RandomGenerator(JsonConvert.DeserializeObject<Dictionary<string, object>>(unarmedLevels["RNG"].ToString()));
            RandomUnarmedLevelCount = uint.Parse(unarmedLevels["RandomCount"].ToString());
            if (UnarmedLevelOrganisation == Organisation.Manual)
            {
                UnarmedLevelData = JsonConvert.DeserializeObject<List<MutableTuple<string, string, bool>>>(unarmedLevels["Manual"].ToString());
            }                

            Dictionary<string, object> ammolessLevels = JsonConvert.DeserializeObject<Dictionary<string, object>>(config["Ammoless"].ToString());
            AmmolessLevelOrganisation = (Organisation)Enum.ToObject(typeof(Organisation), ammolessLevels["Org"]);
            AmmolessLevelRNG = new RandomGenerator(JsonConvert.DeserializeObject<Dictionary<string, object>>(ammolessLevels["RNG"].ToString()));
            RandomAmmolessLevelCount = uint.Parse(ammolessLevels["RandomCount"].ToString());
            if (AmmolessLevelOrganisation == Organisation.Manual)
            {
                AmmolessLevelData = JsonConvert.DeserializeObject<List<MutableTuple<string, string, bool>>>(ammolessLevels["Manual"].ToString());
            }

            LevelsHaveCutScenes = bool.Parse(config["LevelCutScenesOn"].ToString());
            LevelsHaveFMV = bool.Parse(config["LevelFMVsOn"].ToString());
            LevelsHaveStartAnimation = bool.Parse(config["LevelStartAnimsOn"].ToString());
            CheatsEnabled = bool.Parse(config["CheatsOn"].ToString());
            DemosEnabled = bool.Parse(config["DemosOn"].ToString());
            DemoTime = uint.Parse(config["DemoTime"].ToString());
            IsDemoVersion = bool.Parse(config["DemoVersion"].ToString());
            DozyEnabled = bool.Parse(config["DozyOn"].ToString());
            GymEnabled = bool.Parse(config["GymOn"].ToString());
            LevelSelectEnabled = bool.Parse(config["LevelSelectOn"].ToString());
            OptionRingEnabled = bool.Parse(config["OptionRingOn"].ToString());
            SaveLoadEnabled = bool.Parse(config["SaveLoadOn"].ToString());
            ScreensizingEnabled = bool.Parse(config["ScreensizeOn"].ToString());
            TitleScreenEnabled = bool.Parse(config["TitlesOn"].ToString());
        }

        protected override void PrepareSave()
        {
            throw new NotImplementedException();
        }

        internal override AbstractTRScript LoadBackupScript()
        {
            TR23Script backupScript = new TR23Script();
            backupScript.Read(BackupFilePath);
            return backupScript;
        }

        /// <summary>
        /// A list of Tuples configured as follows:
        ///     Item1 => Level ID (string);
        ///     Item2 => Level Name (string);
        ///     Item3 => Is unarmed (bool).
        /// </summary>
        public List<MutableTuple<string, string, bool>> UnarmedLevelData
        {
            get => (LevelManager as TR23LevelManager).GetUnarmedLevelData();
            set => (LevelManager as TR23LevelManager).SetUnarmedLevelData(value);
        }

        public Organisation UnarmedLevelOrganisation
        {
            get => (LevelManager as TR23LevelManager).UnarmedLevelOrganisation;
            set => (LevelManager as TR23LevelManager).UnarmedLevelOrganisation = value;
        }

        public RandomGenerator UnarmedLevelRNG
        {
            get => (LevelManager as TR23LevelManager).UnarmedLevelRNG;
            set => (LevelManager as TR23LevelManager).UnarmedLevelRNG = value;
        }

        public uint RandomUnarmedLevelCount
        {
            get => (LevelManager as TR23LevelManager).RandomUnarmedLevelCount;
            set => (LevelManager as TR23LevelManager).RandomUnarmedLevelCount = value;
        }

        internal void RandomiseUnarmedLevels()
        {
            (LevelManager as TR23LevelManager).RandomiseUnarmedLevels(LoadBackupScript().Levels);
        }

        internal List<TR23Level> GetUnarmedLevels()
        {
            return (LevelManager as TR23LevelManager).GetUnarmedLevels();
        }

        /// <summary>
        /// A list of Tuples configured as follows:
        ///     Item1 => Level ID (string);
        ///     Item2 => Level Name (string);
        ///     Item3 => Ammo removed (bool).
        /// </summary>
        public List<MutableTuple<string, string, bool>> AmmolessLevelData
        {
            get => (LevelManager as TR23LevelManager).GetAmmolessLevelData();
            set => (LevelManager as TR23LevelManager).SetAmmolessLevelData(value);
        }

        public Organisation AmmolessLevelOrganisation
        {
            get => (LevelManager as TR23LevelManager).AmmolessLevelOrganisation;
            set => (LevelManager as TR23LevelManager).AmmolessLevelOrganisation = value;
        }

        public RandomGenerator AmmolessLevelRNG
        {
            get => (LevelManager as TR23LevelManager).AmmolessLevelRNG;
            set => (LevelManager as TR23LevelManager).AmmolessLevelRNG = value;
        }

        public uint RandomAmmolessLevelCount
        {
            get => (LevelManager as TR23LevelManager).RandomAmmolessLevelCount;
            set => (LevelManager as TR23LevelManager).RandomAmmolessLevelCount = value;
        }

        internal void RandomiseAmmolessLevels()
        {
            (LevelManager as TR23LevelManager).RandomiseAmmolessLevels(LoadBackupScript().Levels);
        }

        internal List<TR23Level> GetAmmolessLevels()
        {
            return (LevelManager as TR23LevelManager).GetAmmolessLevels();
        }

        public bool LevelsHaveCutScenes
        {
            get => (LevelManager as TR23LevelManager).GetLevelsHaveCutScenes();
            set => (LevelManager as TR23LevelManager).SetLevelsHaveCutScenes(value);
        }

        public bool LevelsHaveFMV
        {
            get => (LevelManager as TR23LevelManager).GetLevelsHaveFMV();
            set => (LevelManager as TR23LevelManager).SetLevelsHaveFMV(value);
        }

        public bool LevelsHaveStartAnimation
        {
            get => (LevelManager as TR23LevelManager).GetLevelsHaveStartAnimation();
            set => (LevelManager as TR23LevelManager).SetLevelsHaveStartAnimation(value);
        }

        public bool CheatsEnabled
        {
            get => !(Script as TR23Script).CheatsIgnored;
            set => (Script as TR23Script).CheatsIgnored = !value;
        }

        public bool DemosEnabled
        {
            get => !(Script as TR23Script).DemosDisabled;
            set => (Script as TR23Script).DemosDisabled = !value;
        }

        public uint DemoTime
        {
            get => (Script as TR23Script).DemoTimeSeconds;
            set => (Script as TR23Script).DemoTimeSeconds = value;
        }

        public bool IsDemoVersion
        {
            get => (Script as TR23Script).DemoVersion;
            set => (Script as TR23Script).DemoVersion = value;
        }

        public bool DozyEnabled
        {
            get => (Script as TR23Script).DozyEnabled;
            set => (Script as TR23Script).DozyEnabled = value;
        }

        public bool GymEnabled
        {
            get => (Script as TR23Script).GymEnabled;
            set => (Script as TR23Script).GymEnabled = value;
        }

        public bool LevelSelectEnabled
        {
            get => (Script as TR23Script).LevelSelectEnabled;
            set => (Script as TR23Script).LevelSelectEnabled = value;
        }

        public bool OptionRingEnabled
        {
            get => !(Script as TR23Script).OptionRingDisabled;
            set => (Script as TR23Script).OptionRingDisabled = !value;
        }

        public bool SaveLoadEnabled
        {
            get => !(Script as TR23Script).SaveLoadDisabled;
            set => (Script as TR23Script).SaveLoadDisabled = !value;
        }

        public bool ScreensizingEnabled
        {
            get => !(Script as TR23Script).ScreensizingDisabled;
            set => (Script as TR23Script).ScreensizingDisabled = !value;
        }

        public bool TitleScreenEnabled
        {
            get => !(Script as TR23Script).TitleDisabled;
            set => (Script as TR23Script).TitleDisabled = !value;
        }
    }
}