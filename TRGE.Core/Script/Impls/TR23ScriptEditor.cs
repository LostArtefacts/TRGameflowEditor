using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace TRGE.Core
{
    public class TR23ScriptEditor : AbstractTRScriptEditor
    {
        internal TR23ScriptEditor(TRScriptIOArgs ioArgs, TRScriptOpenOption openOption)
            : base(ioArgs, openOption) { }        

        protected override void ApplyConfig()
        {
            if (_config == null)
            {
                UnarmedLevelOrganisation = Organisation.Default;
                UnarmedLevelRNG = new RandomGenerator(RandomGenerator.Type.Date);
                RandomUnarmedLevelCount = Math.Max(1, (LevelManager as TR23LevelManager).GetUnarmedLevelCount());

                AmmolessLevelOrganisation = Organisation.Default;
                AmmolessLevelRNG = new RandomGenerator(RandomGenerator.Type.Date);
                RandomAmmolessLevelCount = Math.Max(1, (LevelManager as TR23LevelManager).GetAmmolessLevelCount());

                BonusOrganisation = Organisation.Default;
                BonusRNG = new RandomGenerator(RandomGenerator.Type.Date);

                return;
            }

            Dictionary<string, object> unarmedLevels = JsonConvert.DeserializeObject<Dictionary<string, object>>(_config["UnarmedLevels"].ToString());
            UnarmedLevelOrganisation = (Organisation)Enum.ToObject(typeof(Organisation), unarmedLevels["Organisation"]);
            UnarmedLevelRNG = new RandomGenerator(JsonConvert.DeserializeObject<Dictionary<string, object>>(unarmedLevels["RNG"].ToString()));
            RandomUnarmedLevelCount = uint.Parse(unarmedLevels["RandomCount"].ToString());
            //see note in base.LoadConfig re restoring randomised - same applies for Unarmed
            UnarmedLevelData = JsonConvert.DeserializeObject<List<MutableTuple<string, string, bool>>>(unarmedLevels["Data"].ToString());

            Dictionary<string, object> ammolessLevels = JsonConvert.DeserializeObject<Dictionary<string, object>>(_config["AmmolessLevels"].ToString());
            AmmolessLevelOrganisation = (Organisation)Enum.ToObject(typeof(Organisation), ammolessLevels["Organisation"]);
            AmmolessLevelRNG = new RandomGenerator(JsonConvert.DeserializeObject<Dictionary<string, object>>(ammolessLevels["RNG"].ToString()));
            RandomAmmolessLevelCount = uint.Parse(ammolessLevels["RandomCount"].ToString());
            //see note in base.LoadConfig re restoring randomised - same applies for Ammoless
            AmmolessLevelData = JsonConvert.DeserializeObject<List<MutableTuple<string, string, bool>>>(ammolessLevels["Data"].ToString());

            if (CanOrganiseBonuses)
            {
                Dictionary<string, object> bonuses = JsonConvert.DeserializeObject<Dictionary<string, object>>(_config["BonusSetup"].ToString());
                BonusOrganisation = (Organisation)Enum.ToObject(typeof(Organisation), bonuses["Organisation"]);
                BonusRNG = new RandomGenerator(JsonConvert.DeserializeObject<Dictionary<string, object>>(bonuses["RNG"].ToString()));
                LevelBonusData = JsonConvert.DeserializeObject<List<MutableTuple<string, string, List<MutableTuple<ushort, TRItemCategory, string, int>>>>>(bonuses["Data"].ToString());
            }

            LevelsHaveCutScenes = bool.Parse(_config["LevelCutScenesOn"].ToString());
            LevelsHaveFMV = bool.Parse(_config["LevelFMVsOn"].ToString());
            LevelsHaveStartAnimation = bool.Parse(_config["LevelStartAnimsOn"].ToString());
            CheatsEnabled = bool.Parse(_config["CheatsOn"].ToString());
            DemosEnabled = bool.Parse(_config["DemosOn"].ToString());
            DemoTime = uint.Parse(_config["DemoTime"].ToString());
            IsDemoVersion = bool.Parse(_config["DemoVersion"].ToString());
            DozyEnabled = bool.Parse(_config["DozyOn"].ToString());
            GymEnabled = bool.Parse(_config["GymOn"].ToString());
            LevelSelectEnabled = bool.Parse(_config["LevelSelectOn"].ToString());
            OptionRingEnabled = bool.Parse(_config["OptionRingOn"].ToString());
            SaveLoadEnabled = bool.Parse(_config["SaveLoadOn"].ToString());
            ScreensizingEnabled = bool.Parse(_config["ScreensizeOn"].ToString());
            TitleScreenEnabled = bool.Parse(_config["TitlesOn"].ToString());
        }

        protected override void SaveImpl()
        {
            //for unarmed, ammoless and bonus data, save the config before
            //running any randomisation, otherwise when the script is
            //reloaded the random order will be available
            _config["UnarmedLevels"] = new Dictionary<string, object>
            {
                ["Organisation"] = (int)UnarmedLevelOrganisation,
                ["RNG"] = UnarmedLevelRNG.ToJson(),
                ["RandomCount"] = RandomUnarmedLevelCount,
                ["Data"] = UnarmedLevelData
            };

            _config["AmmolessLevels"] = new Dictionary<string, object>
            {
                ["Organisation"] = (int)AmmolessLevelOrganisation,
                ["RNG"] = AmmolessLevelRNG.ToJson(),
                ["RandomCount"] = RandomAmmolessLevelCount,
                ["Data"] = AmmolessLevelData
            };

            if (CanOrganiseBonuses)
            {
                _config["BonusSetup"] = new Dictionary<string, object>
                {
                    ["Organisation"] = (int)BonusOrganisation,
                    ["RNG"] = BonusRNG.ToJson(),
                    ["Data"] = LevelBonusData
                };
            }

            _config["LevelCutScenesOn"] = LevelsHaveCutScenes;
            _config["LevelFMVsOn"] = LevelsHaveFMV;
            _config["LevelStartAnimsOn"] = LevelsHaveStartAnimation;
            _config["CheatsOn"] = CheatsEnabled;
            _config["DemosOn"] = DemosEnabled;
            _config["DemoTime"] = DemoTime;
            _config["DemoVersion"] = IsDemoVersion;
            _config["DozyOn"] = DozyEnabled;
            _config["GymOn"] = GymEnabled;
            _config["LevelSelectOn"] = LevelSelectEnabled;
            _config["OptionRingOn"] = OptionRingEnabled;
            _config["SaveLoadOn"] = SaveLoadEnabled;
            _config["ScreensizeOn"] = ScreensizingEnabled;
            _config["TitlesOn"] = TitleScreenEnabled;

            AbstractTRScript backupScript = LoadBackupScript();
            List<AbstractTRScriptedLevel> backupLevels = backupScript.Levels;

            TR23LevelManager levelMan = LevelManager as TR23LevelManager;
            /*if (BonusOrganisation == Organisation.Random)
            {
                levelMan.RandomiseBonuses(backupLevels);
            }
            else if (BonusOrganisation == Organisation.Default)
            {
                levelMan.RestoreBonuses(backupLevels);
            }*/

            if (AmmolessLevelOrganisation == Organisation.Random)
            {
                levelMan.RandomiseAmmolessLevels(backupLevels);
            }
            else if (AmmolessLevelOrganisation == Organisation.Default)
            {
                levelMan.RestoreAmmolessLevels(backupLevels);
            }

            if (UnarmedLevelOrganisation == Organisation.Random)
            {
                levelMan.RandomiseUnarmedLevels(backupLevels);
            }
            else if (UnarmedLevelOrganisation == Organisation.Default)
            {
                levelMan.RestoreUnarmedLevels(backupLevels);
            }

            //should occur after unarmed organisation
            if (BonusOrganisation == Organisation.Random)
            {
                levelMan.RandomiseBonuses(backupLevels);
            }
            else if (BonusOrganisation == Organisation.Default)
            {
                levelMan.RestoreBonuses(backupLevels);
            }
        }

        internal override AbstractTRScript CreateScript()
        {
            return new TR23Script();
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

        internal List<TR23ScriptedLevel> GetUnarmedLevels()
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

        internal List<TR23ScriptedLevel> GetAmmolessLevels()
        {
            return (LevelManager as TR23LevelManager).GetAmmolessLevels();
        }

        public bool CanOrganiseBonuses => (LevelManager as TR23LevelManager).CanOrganiseBonuses;
        public Organisation BonusOrganisation
        {
            get => (LevelManager as TR23LevelManager).BonusOrganisation;
            set => (LevelManager as TR23LevelManager).BonusOrganisation = value;
        }

        public RandomGenerator BonusRNG
        {
            get => (LevelManager as TR23LevelManager).BonusRNG;
            set => (LevelManager as TR23LevelManager).BonusRNG = value;
        }

        public List<MutableTuple<string, string, List<MutableTuple<ushort, TRItemCategory, string, int>>>> LevelBonusData
        {
            get
            {
                if (CanOrganiseBonuses)
                {
                    return (LevelManager as TR23LevelManager).GetLevelBonusData();
                }
                return null;
            }
            set
            {
                if (CanOrganiseBonuses)
                {
                    (LevelManager as TR23LevelManager).SetLevelBonusData(value);
                }
            }
        }

        internal void RandomiseBonuses()
        {
            (LevelManager as TR23LevelManager).RandomiseBonuses(LoadBackupScript().Levels);
        }

        public bool LevelsHaveCutScenes
        {
            get => (LevelManager as TR23LevelManager).GetLevelsHaveCutScenes();
            set => (LevelManager as TR23LevelManager).SetLevelsHaveCutScenes(value);
        }

        public bool LevelsSupportCutScenes => (LevelManager as TR23LevelManager).GetLevelsSupportCutScenes();

        public bool LevelsHaveFMV
        {
            get => (LevelManager as TR23LevelManager).GetLevelsHaveFMV();
            set => (LevelManager as TR23LevelManager).SetLevelsHaveFMV(value);
        }

        public bool LevelsSupportFMVs => (LevelManager as TR23LevelManager).GetLevelsSupportFMVs();

        public bool LevelsHaveStartAnimation
        {
            get => (LevelManager as TR23LevelManager).GetLevelsHaveStartAnimation();
            set => (LevelManager as TR23LevelManager).SetLevelsHaveStartAnimation(value);
        }

        public bool LevelsSupportStartAnimations => (LevelManager as TR23LevelManager).GetLevelsSupportStartAnimations();

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

        public bool DozySupported => (Script as TR23Script).DozyViable;

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