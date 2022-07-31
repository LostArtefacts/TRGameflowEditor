using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace TRGE.Core
{
    public class TR1ScriptEditor : AbstractTRScriptEditor
    {
        internal TR1ScriptEditor(TRScriptIOArgs ioArgs, TRScriptOpenOption openOption)
            : base(ioArgs, openOption) { }

        protected override void ApplyConfig(Config config)
        {
            if (config == null)
            {
                UnarmedLevelOrganisation = Organisation.Default;
                UnarmedLevelRNG = new RandomGenerator(RandomGenerator.Type.Date);
                RandomUnarmedLevelCount = Math.Max(1, (LevelManager as TR1LevelManager).GetUnarmedLevelCount());

                AmmolessLevelOrganisation = Organisation.Default;
                AmmolessLevelRNG = new RandomGenerator(RandomGenerator.Type.Date);
                RandomAmmolessLevelCount = Math.Max(1, (LevelManager as TR1LevelManager).GetAmmolessLevelCount());

                return;
            }

            Config unarmedLevels = config.GetSubConfig("UnarmedLevels");
            UnarmedLevelOrganisation = unarmedLevels.GetOrganisation("Organisation");
            UnarmedLevelRNG = new RandomGenerator(unarmedLevels.GetSubConfig("RNG"));
            RandomUnarmedLevelCount = Math.Min(unarmedLevels.GetUInt("RandomCount"), (uint)LevelManager.EnabledLevelCount);
            //see note in base.LoadConfig re restoring randomised - same applies for Unarmed
            if (unarmedLevels.ContainsKey("Data"))
            {
                UnarmedLevelData = JsonConvert.DeserializeObject<List<MutableTuple<string, string, bool>>>(unarmedLevels.GetString("Data"));
            }

            Config ammolessLevels = config.GetSubConfig("AmmolessLevels");
            AmmolessLevelOrganisation = ammolessLevels.GetOrganisation("Organisation");
            AmmolessLevelRNG = new RandomGenerator(ammolessLevels.GetSubConfig("RNG"));
            RandomAmmolessLevelCount = Math.Min(ammolessLevels.GetUInt("RandomCount"), (uint)LevelManager.EnabledLevelCount);
            //see note in base.LoadConfig re restoring randomised - same applies for Ammoless
            if (ammolessLevels.ContainsKey("Data"))
            {
                AmmolessLevelData = JsonConvert.DeserializeObject<List<MutableTuple<string, string, bool>>>(ammolessLevels.GetString("Data"));
            }

            LevelsHaveCutScenes = config.GetBool("LevelCutScenesOn");
            LevelsHaveFMV = config.GetBool("LevelFMVsOn");
            DemosEnabled = config.GetBool("DemosOn");
            DemoTime = config.GetUInt("DemoTime");
            MainMenuPicture = config.GetString("MainMenuPicture");
            SavegameFmtLegacy = config.GetString("SavegameFmtLegacy");
            SavegameFmtBson = config.GetString("SavegameFmtBson");
            EnableGameModes = config.GetBool("EnableGameModes");
            EnableSaveCrystals = config.GetBool("EnableSaveCrystals");
            DemoTime = config.GetDouble("DemoTime");
            WaterColor = config.GetArray<double>("WaterColor");
            DrawDistanceFade = config.GetDouble("DrawDistanceFade");
            DrawDistanceMax = config.GetDouble("DrawDistanceMax");

            StartLaraHitpoints = config.GetInt(nameof(StartLaraHitpoints));
            DisableHealingBetweenLevels = config.GetBool(nameof(DisableHealingBetweenLevels));
            DisableMedpacks = config.GetBool(nameof(DisableMedpacks));
            DisableMagnums = config.GetBool(nameof(DisableMagnums));
            DisableUzis = config.GetBool(nameof(DisableUzis));
            DisableShotgun = config.GetBool(nameof(DisableShotgun));
            EnableDeathsCounter = config.GetBool(nameof(EnableDeathsCounter));
            EnableEnemyHealthbar = config.GetBool(nameof(EnableEnemyHealthbar));
            EnableEnhancedLook = config.GetBool(nameof(EnableEnhancedLook));
            EnableShotgunFlash = config.GetBool(nameof(EnableShotgunFlash));
            FixShotgunTargeting = config.GetBool(nameof(FixShotgunTargeting));
            EnableNumericKeys = config.GetBool(nameof(EnableNumericKeys));
            EnableTr3Sidesteps = config.GetBool(nameof(EnableTr3Sidesteps));
            EnableCheats = config.GetBool(nameof(EnableCheats));
            EnableBraid = config.GetBool(nameof(EnableBraid));
            EnableDetailedStats = config.GetBool(nameof(EnableDetailedStats));
            EnableCompassStats = config.GetBool(nameof(EnableCompassStats));
            EnableTotalStats = config.GetBool(nameof(EnableTotalStats));
            EnableTimerInInventory = config.GetBool(nameof(EnableTimerInInventory));
            EnableSmoothBars = config.GetBool(nameof(EnableSmoothBars));
            EnableFadeEffects = config.GetBool(nameof(EnableFadeEffects));
            MenuStyle = (TRMenuStyle)config.GetEnum(nameof(MenuStyle), typeof(TRMenuStyle), TRMenuStyle.PC);
            HealthbarShowingMode = (TRHealthbarMode)config.GetEnum(nameof(HealthbarShowingMode), typeof(TRHealthbarMode), TRHealthbarMode.FlashingOrDefault);
            HealthbarLocation = (TRUILocation)config.GetEnum(nameof(HealthbarLocation), typeof(TRUILocation), TRUILocation.TopLeft);
            HealthbarColor = (TRUIColour)config.GetEnum(nameof(HealthbarColor), typeof(TRUIColour), TRUIColour.Red);
            AirbarShowingMode = (TRAirbarMode)config.GetEnum(nameof(AirbarShowingMode), typeof(TRAirbarMode), TRAirbarMode.Default);
            AirbarLocation = (TRUILocation)config.GetEnum(nameof(AirbarLocation), typeof(TRUILocation), TRUILocation.TopRight);
            AirbarColor = (TRUIColour)config.GetEnum(nameof(AirbarColor), typeof(TRUIColour), TRUIColour.Blue);
            EnemyHealthbarLocation = (TRUILocation)config.GetEnum(nameof(EnemyHealthbarLocation), typeof(TRUILocation), TRUILocation.BottomLeft);
            EnemyHealthbarColor = (TRUIColour)config.GetEnum(nameof(EnemyHealthbarColor), typeof(TRUIColour), TRUIColour.Grey);
            FixTihocanSecretSound = config.GetBool(nameof(FixTihocanSecretSound));
            FixPyramidSecretTrigger = config.GetBool(nameof(FixPyramidSecretTrigger));
            FixSecretsKillingMusic = config.GetBool(nameof(FixSecretsKillingMusic));
            FixDescendingGlitch = config.GetBool(nameof(FixDescendingGlitch));
            FixWallJumpGlitch = config.GetBool(nameof(FixWallJumpGlitch));
            FixBridgeCollision = config.GetBool(nameof(FixBridgeCollision));
            FixQwopGlitch = config.GetBool(nameof(FixQwopGlitch));
            FixAlligatorAi = config.GetBool(nameof(FixAlligatorAi));
            ChangePierreSpawn = config.GetBool(nameof(ChangePierreSpawn));
            FovValue = config.GetInt(nameof(FovValue));
            FovVertical = config.GetBool(nameof(FovVertical));
            DisableFmv = config.GetBool(nameof(DisableFmv));
            DisableCine = config.GetBool(nameof(DisableCine));
            DisableMusicInMenu = config.GetBool(nameof(DisableMusicInMenu));
            DisableMusicInInventory = config.GetBool(nameof(DisableMusicInInventory));
            DisableTrexCollision = config.GetBool(nameof(DisableTrexCollision));
            ResolutionWidth = config.GetInt(nameof(ResolutionWidth));
            ResolutionHeight = config.GetInt(nameof(ResolutionHeight));
            EnableRoundShadow = config.GetBool(nameof(EnableRoundShadow));
            Enable3dPickups = config.GetBool(nameof(Enable3dPickups));
            ScreenshotFormat = (TRScreenshotFormat)config.GetEnum(nameof(ScreenshotFormat), typeof(TRScreenshotFormat), TRScreenshotFormat.JPG);
            AnisotropyFilter = config.GetDouble(nameof(AnisotropyFilter));
            WalkToItems = config.GetBool(nameof(WalkToItems));
            MaximumSaveSlots = config.GetInt(nameof(MaximumSaveSlots));
            RevertToPistols = config.GetBool(nameof(RevertToPistols));
        }

        protected override void SaveImpl()
        {
            _config["UnarmedLevels"] = new Config
            {
                ["Organisation"] = (int)UnarmedLevelOrganisation,
                ["RNG"] = UnarmedLevelRNG.ToJson(),
                ["RandomCount"] = RandomUnarmedLevelCount,
                ["Data"] = UnarmedLevelData
            };

            _config["AmmolessLevels"] = new Config
            {
                ["Organisation"] = (int)AmmolessLevelOrganisation,
                ["RNG"] = AmmolessLevelRNG.ToJson(),
                ["RandomCount"] = RandomAmmolessLevelCount,
                ["Data"] = AmmolessLevelData
            };

            _config["LevelCutScenesOn"] = LevelsHaveCutScenes;
            _config["LevelFMVsOn"] = LevelsHaveFMV;
            _config["DemosOn"] = DemosEnabled;
            _config["DemoTime"] = DemoTime;
            _config["MainMenuPicture"] = MainMenuPicture;
            _config["SavegameFmtLegacy"] = SavegameFmtLegacy;
            _config["SavegameFmtBson"] = SavegameFmtBson;
            _config["EnableGameModes"] = EnableGameModes;
            _config["EnableSaveCrystals"] = EnableSaveCrystals;
            _config["DemoTime"] = DemoTime;
            _config["WaterColor"] = WaterColor;
            _config["DrawDistanceFade"] = DrawDistanceFade;
            _config["DrawDistanceMax"] = DrawDistanceMax;
            _config[nameof(StartLaraHitpoints)] = StartLaraHitpoints;
            _config[nameof(DisableHealingBetweenLevels)] = DisableHealingBetweenLevels;
            _config[nameof(DisableMedpacks)] = DisableMedpacks;
            _config[nameof(DisableMagnums)] = DisableMagnums;
            _config[nameof(DisableUzis)] = DisableUzis;
            _config[nameof(DisableShotgun)] = DisableShotgun;
            _config[nameof(EnableDeathsCounter)] = EnableDeathsCounter;
            _config[nameof(EnableEnemyHealthbar)] = EnableEnemyHealthbar;
            _config[nameof(EnableEnhancedLook)] = EnableEnhancedLook;
            _config[nameof(EnableShotgunFlash)] = EnableShotgunFlash;
            _config[nameof(FixShotgunTargeting)] = FixShotgunTargeting;
            _config[nameof(EnableNumericKeys)] = EnableNumericKeys;
            _config[nameof(EnableTr3Sidesteps)] = EnableTr3Sidesteps;
            _config[nameof(EnableCheats)] = EnableCheats;
            _config[nameof(EnableBraid)] = EnableBraid;
            _config[nameof(EnableDetailedStats)] = EnableDetailedStats;
            _config[nameof(EnableCompassStats)] = EnableCompassStats;
            _config[nameof(EnableTotalStats)] = EnableTotalStats;
            _config[nameof(EnableTimerInInventory)] = EnableTimerInInventory;
            _config[nameof(EnableSmoothBars)] = EnableSmoothBars;
            _config[nameof(EnableFadeEffects)] = EnableFadeEffects;
            _config[nameof(MenuStyle)] = MenuStyle;
            _config[nameof(HealthbarShowingMode)] = HealthbarShowingMode;
            _config[nameof(HealthbarLocation)] = HealthbarLocation;
            _config[nameof(HealthbarColor)] = HealthbarColor;
            _config[nameof(AirbarShowingMode)] = AirbarShowingMode;
            _config[nameof(AirbarLocation)] = AirbarLocation;
            _config[nameof(AirbarColor)] = AirbarColor;
            _config[nameof(EnemyHealthbarLocation)] = EnemyHealthbarLocation;
            _config[nameof(EnemyHealthbarColor)] = EnemyHealthbarColor;
            _config[nameof(FixTihocanSecretSound)] = FixTihocanSecretSound;
            _config[nameof(FixPyramidSecretTrigger)] = FixPyramidSecretTrigger;
            _config[nameof(FixSecretsKillingMusic)] = FixSecretsKillingMusic;
            _config[nameof(FixDescendingGlitch)] = FixDescendingGlitch;
            _config[nameof(FixWallJumpGlitch)] = FixWallJumpGlitch;
            _config[nameof(FixBridgeCollision)] = FixBridgeCollision;
            _config[nameof(FixQwopGlitch)] = FixQwopGlitch;
            _config[nameof(FixAlligatorAi)] = FixAlligatorAi;
            _config[nameof(ChangePierreSpawn)] = ChangePierreSpawn;
            _config[nameof(FovValue)] = FovValue;
            _config[nameof(FovVertical)] = FovVertical;
            _config[nameof(DisableFmv)] = DisableFmv;
            _config[nameof(DisableCine)] = DisableCine;
            _config[nameof(DisableMusicInMenu)] = DisableMusicInMenu;
            _config[nameof(DisableMusicInInventory)] = DisableMusicInInventory;
            _config[nameof(DisableTrexCollision)] = DisableTrexCollision;
            _config[nameof(ResolutionWidth)] = ResolutionWidth;
            _config[nameof(ResolutionHeight)] = ResolutionHeight;
            _config[nameof(EnableRoundShadow)] = EnableRoundShadow;
            _config[nameof(Enable3dPickups)] = Enable3dPickups;
            _config[nameof(ScreenshotFormat)] = ScreenshotFormat;
            _config[nameof(AnisotropyFilter)] = AnisotropyFilter;
            _config[nameof(WalkToItems)] = WalkToItems;
            _config[nameof(MaximumSaveSlots)] = MaximumSaveSlots;
            _config[nameof(RevertToPistols)] = RevertToPistols;

            AbstractTRScript backupScript = LoadBackupScript();
            AbstractTRScript randoBaseScript = LoadRandomisationBaseScript(); // #42

            List<AbstractTRScriptedLevel> randoBaseLevels = randoBaseScript.Levels;
            TR1LevelManager backupLevelManager = TRScriptedLevelFactory.GetLevelManager(backupScript) as TR1LevelManager; //#65, #86
            TR1LevelManager currentLevelManager = LevelManager as TR1LevelManager;

            if (AmmolessLevelOrganisation == Organisation.Random)
            {
                if (TRInterop.RandomisationSupported)
                {
                    currentLevelManager.RandomiseAmmolessLevels(randoBaseLevels);
                }
                else
                {
                    currentLevelManager.SetAmmolessLevelData(backupLevelManager.GetAmmolessLevelData(backupLevelManager.Levels)); //#65 lock to that of the original file; #86
                }
            }
            else if (AmmolessLevelOrganisation == Organisation.Default)
            {
                currentLevelManager.SetAmmolessLevelData(backupLevelManager.GetAmmolessLevelData(backupLevelManager.Levels));
            }

            if (UnarmedLevelOrganisation == Organisation.Random)
            {
                if (TRInterop.RandomisationSupported)
                {
                    currentLevelManager.RandomiseUnarmedLevels(randoBaseLevels);
                }
                else
                {
                    currentLevelManager.SetUnarmedLevelData(backupLevelManager.GetUnarmedLevelData(backupLevelManager.Levels)); //#65 lock to that of the original file; #86
                }
            }
            else if (UnarmedLevelOrganisation == Organisation.Default)
            {
                currentLevelManager.SetUnarmedLevelData(backupLevelManager.GetUnarmedLevelData(backupLevelManager.Levels));
            }
            else
            {
                currentLevelManager.SetUnarmedLevelData(UnarmedLevelData); //TODO: Fix this - it's in place to ensure the event is triggered for any listeners
            }
        }

        internal override AbstractTRScript CreateScript()
        {
            return new TR1Script();
        }

        internal override Config ExportConfig()
        {
            Config config = base.ExportConfig();
            if (!TRInterop.RandomisationSupported)
            {
                if (UnarmedLevelOrganisation == Organisation.Random)
                {
                    config["UnarmedLevels"] = new Config
                    {
                        ["Organisation"] = (int)Organisation.Default,
                        ["RNG"] = UnarmedLevelRNG.ToJson(),
                        ["RandomCount"] = RandomUnarmedLevelCount
                    };
                }

                if (AmmolessLevelOrganisation == Organisation.Random)
                {
                    config["AmmolessLevels"] = new Config
                    {
                        ["Organisation"] = (int)Organisation.Default,
                        ["RNG"] = AmmolessLevelRNG.ToJson(),
                        ["RandomCount"] = RandomAmmolessLevelCount
                    };
                }
            }
            return config;
        }

        public List<MutableTuple<string, string, bool>> UnarmedLevelData
        {
            get => (LevelManager as TR1LevelManager).GetUnarmedLevelData(LoadBackupScript().Levels);
            set => (LevelManager as TR1LevelManager).SetUnarmedLevelData(value);
        }

        public Organisation UnarmedLevelOrganisation
        {
            get => (LevelManager as TR1LevelManager).UnarmedLevelOrganisation;
            set => (LevelManager as TR1LevelManager).UnarmedLevelOrganisation = value;
        }

        public RandomGenerator UnarmedLevelRNG
        {
            get => (LevelManager as TR1LevelManager).UnarmedLevelRNG;
            set => (LevelManager as TR1LevelManager).UnarmedLevelRNG = value;
        }

        public uint RandomUnarmedLevelCount
        {
            get => (LevelManager as TR1LevelManager).RandomUnarmedLevelCount;
            set => (LevelManager as TR1LevelManager).RandomUnarmedLevelCount = value;
        }

        internal void RandomiseUnarmedLevels()
        {
            (LevelManager as TR1LevelManager).RandomiseUnarmedLevels(LoadRandomisationBaseScript().Levels);
        }

        internal List<TR1ScriptedLevel> GetUnarmedLevels()
        {
            return (LevelManager as TR1LevelManager).GetUnarmedLevels();
        }

        public List<MutableTuple<string, string, bool>> AmmolessLevelData
        {
            get => (LevelManager as TR1LevelManager).GetAmmolessLevelData(LoadBackupScript().Levels);
            set => (LevelManager as TR1LevelManager).SetAmmolessLevelData(value);
        }

        public Organisation AmmolessLevelOrganisation
        {
            get => (LevelManager as TR1LevelManager).AmmolessLevelOrganisation;
            set => (LevelManager as TR1LevelManager).AmmolessLevelOrganisation = value;
        }

        public RandomGenerator AmmolessLevelRNG
        {
            get => (LevelManager as TR1LevelManager).AmmolessLevelRNG;
            set => (LevelManager as TR1LevelManager).AmmolessLevelRNG = value;
        }

        public uint RandomAmmolessLevelCount
        {
            get => (LevelManager as TR1LevelManager).RandomAmmolessLevelCount;
            set => (LevelManager as TR1LevelManager).RandomAmmolessLevelCount = value;
        }

        internal void RandomiseAmmolessLevels()
        {
            (LevelManager as TR1LevelManager).RandomiseAmmolessLevels(LoadRandomisationBaseScript().Levels);
        }

        internal List<TR1ScriptedLevel> GetAmmolessLevels()
        {
            return (LevelManager as TR1LevelManager).GetAmmolessLevels();
        }

        public bool LevelsHaveCutScenes
        {
            get => (LevelManager as TR1LevelManager).GetLevelsHaveCutScenes();
            set => (LevelManager as TR1LevelManager).SetLevelsHaveCutScenes(value);
        }

        public bool LevelsSupportCutScenes => (LevelManager as TR1LevelManager).GetLevelsSupportCutScenes();

        public bool LevelsHaveFMV
        {
            get => (LevelManager as TR1LevelManager).GetLevelsHaveFMV();
            set => (LevelManager as TR1LevelManager).SetLevelsHaveFMV(value);
        }

        public bool LevelsSupportFMVs => (LevelManager as TR1LevelManager).GetLevelsSupportFMVs();      

        public bool DemosEnabled
        {
            get => !(Script as TR1Script).DisableDemo;
            set => (Script as TR1Script).DisableDemo = !value;
        }

        public string MainMenuPicture
        {
            get => (Script as TR1Script).MainMenuPicture;
            set => (Script as TR1Script).MainMenuPicture = value;
        }

        public string SavegameFmtLegacy
        {
            get => (Script as TR1Script).SavegameFmtLegacy;
            set => (Script as TR1Script).SavegameFmtLegacy = value;
        }

        public string SavegameFmtBson
        {
            get => (Script as TR1Script).SavegameFmtBson;
            set => (Script as TR1Script).SavegameFmtBson = value;
        }

        public bool EnableGameModes
        {
            get => (Script as TR1Script).EnableGameModes;
            set => (Script as TR1Script).EnableGameModes = value;
        }

        public bool EnableSaveCrystals
        {
            get => (Script as TR1Script).EnableSaveCrystals;
            set => (Script as TR1Script).EnableSaveCrystals = value;
        }

        public double DemoTime
        {
            get => (Script as TR1Script).DemoDelay;
            set => (Script as TR1Script).DemoDelay = value;
        }

        public double[] WaterColor
        {
            get => (Script as TR1Script).WaterColor;
            set => (Script as TR1Script).WaterColor = value;
        }

        public double DrawDistanceFade
        {
            get => (Script as TR1Script).DrawDistanceFade;
            set => (Script as TR1Script).DrawDistanceFade = value;
        }

        public double DrawDistanceMax
        {
            get => (Script as TR1Script).DrawDistanceMax;
            set => (Script as TR1Script).DrawDistanceMax = value;
        }

        public int StartLaraHitpoints
        {
            get => (Script as TR1Script).StartLaraHitpoints;
            set => (Script as TR1Script).StartLaraHitpoints = value;
        }

        public bool DisableHealingBetweenLevels
        {
            get => (Script as TR1Script).DisableHealingBetweenLevels;
            set => (Script as TR1Script).DisableHealingBetweenLevels = value;
        }

        public bool DisableMedpacks
        {
            get => (Script as TR1Script).DisableMedpacks;
            set => (Script as TR1Script).DisableMedpacks = value;
        }

        public bool DisableMagnums
        {
            get => (Script as TR1Script).DisableMagnums;
            set => (Script as TR1Script).DisableMagnums = value;
        }

        public bool DisableUzis
        {
            get => (Script as TR1Script).DisableUzis;
            set => (Script as TR1Script).DisableUzis = value;
        }

        public bool DisableShotgun
        {
            get => (Script as TR1Script).DisableShotgun;
            set => (Script as TR1Script).DisableShotgun = value;
        }

        public bool EnableDeathsCounter
        {
            get => (Script as TR1Script).EnableDeathsCounter;
            set => (Script as TR1Script).EnableDeathsCounter = value;
        }

        public bool EnableEnemyHealthbar
        {
            get => (Script as TR1Script).EnableEnemyHealthbar;
            set => (Script as TR1Script).EnableEnemyHealthbar = value;
        }

        public bool EnableEnhancedLook
        {
            get => (Script as TR1Script).EnableEnhancedLook;
            set => (Script as TR1Script).EnableEnhancedLook = value;
        }

        public bool EnableShotgunFlash
        {
            get => (Script as TR1Script).EnableShotgunFlash;
            set => (Script as TR1Script).EnableShotgunFlash = value;
        }

        public bool FixShotgunTargeting
        {
            get => (Script as TR1Script).FixShotgunTargeting;
            set => (Script as TR1Script).FixShotgunTargeting = value;
        }

        public bool EnableNumericKeys
        {
            get => (Script as TR1Script).EnableNumericKeys;
            set => (Script as TR1Script).EnableNumericKeys = value;
        }

        public bool EnableTr3Sidesteps
        {
            get => (Script as TR1Script).EnableTr3Sidesteps;
            set => (Script as TR1Script).EnableTr3Sidesteps = value;
        }

        public bool EnableCheats
        {
            get => (Script as TR1Script).EnableCheats;
            set => (Script as TR1Script).EnableCheats = value;
        }

        public bool EnableBraid
        {
            get => (Script as TR1Script).EnableBraid;
            set => (Script as TR1Script).EnableBraid = value;
        }

        public bool EnableDetailedStats
        {
            get => (Script as TR1Script).EnableDetailedStats;
            set => (Script as TR1Script).EnableDetailedStats = value;
        }

        public bool EnableCompassStats
        {
            get => (Script as TR1Script).EnableCompassStats;
            set => (Script as TR1Script).EnableCompassStats = value;
        }

        public bool EnableTotalStats
        {
            get => (Script as TR1Script).EnableTotalStats;
            set => (Script as TR1Script).EnableTotalStats = value;
        }

        public bool EnableTimerInInventory
        {
            get => (Script as TR1Script).EnableTimerInInventory;
            set => (Script as TR1Script).EnableTimerInInventory = value;
        }

        public bool EnableSmoothBars
        {
            get => (Script as TR1Script).EnableSmoothBars;
            set => (Script as TR1Script).EnableSmoothBars = value;
        }

        public bool EnableFadeEffects
        {
            get => (Script as TR1Script).EnableFadeEffects;
            set => (Script as TR1Script).EnableFadeEffects = value;
        }

        public TRMenuStyle MenuStyle
        {
            get => (Script as TR1Script).MenuStyle;
            set => (Script as TR1Script).MenuStyle = value;
        }

        public TRHealthbarMode HealthbarShowingMode
        {
            get => (Script as TR1Script).HealthbarShowingMode;
            set => (Script as TR1Script).HealthbarShowingMode = value;
        }

        public TRUILocation HealthbarLocation
        {
            get => (Script as TR1Script).HealthbarLocation;
            set => (Script as TR1Script).HealthbarLocation = value;
        }

        public TRUIColour HealthbarColor
        {
            get => (Script as TR1Script).HealthbarColor;
            set => (Script as TR1Script).HealthbarColor = value;
        }

        public TRAirbarMode AirbarShowingMode
        {
            get => (Script as TR1Script).AirbarShowingMode;
            set => (Script as TR1Script).AirbarShowingMode = value;
        }

        public TRUILocation AirbarLocation
        {
            get => (Script as TR1Script).AirbarLocation;
            set => (Script as TR1Script).AirbarLocation = value;
        }

        public TRUIColour AirbarColor
        {
            get => (Script as TR1Script).AirbarColor;
            set => (Script as TR1Script).AirbarColor = value;
        }

        public TRUILocation EnemyHealthbarLocation
        {
            get => (Script as TR1Script).EnemyHealthbarLocation;
            set => (Script as TR1Script).EnemyHealthbarLocation = value;
        }

        public TRUIColour EnemyHealthbarColor
        {
            get => (Script as TR1Script).EnemyHealthbarColor;
            set => (Script as TR1Script).EnemyHealthbarColor = value;
        }

        public bool FixTihocanSecretSound
        {
            get => (Script as TR1Script).FixTihocanSecretSound;
            set => (Script as TR1Script).FixTihocanSecretSound = value;
        }

        public bool FixPyramidSecretTrigger
        {
            get => (Script as TR1Script).FixPyramidSecretTrigger;
            set => (Script as TR1Script).FixPyramidSecretTrigger = value;
        }

        public bool FixSecretsKillingMusic
        {
            get => (Script as TR1Script).FixSecretsKillingMusic;
            set => (Script as TR1Script).FixSecretsKillingMusic = value;
        }

        public bool FixDescendingGlitch
        {
            get => (Script as TR1Script).FixDescendingGlitch;
            set => (Script as TR1Script).FixDescendingGlitch = value;
        }

        public bool FixWallJumpGlitch
        {
            get => (Script as TR1Script).FixWallJumpGlitch;
            set => (Script as TR1Script).FixWallJumpGlitch = value;
        }

        public bool FixBridgeCollision
        {
            get => (Script as TR1Script).FixBridgeCollision;
            set => (Script as TR1Script).FixBridgeCollision = value;
        }

        public bool FixQwopGlitch
        {
            get => (Script as TR1Script).FixQwopGlitch;
            set => (Script as TR1Script).FixQwopGlitch = value;
        }

        public bool FixAlligatorAi
        {
            get => (Script as TR1Script).FixAlligatorAi;
            set => (Script as TR1Script).FixAlligatorAi = value;
        }

        public bool ChangePierreSpawn
        {
            get => (Script as TR1Script).ChangePierreSpawn;
            set => (Script as TR1Script).ChangePierreSpawn = value;
        }

        public int FovValue
        {
            get => (Script as TR1Script).FovValue;
            set => (Script as TR1Script).FovValue = value;
        }

        public bool FovVertical
        {
            get => (Script as TR1Script).FovVertical;
            set => (Script as TR1Script).FovVertical = value;
        }

        public bool DisableFmv
        {
            get => (Script as TR1Script).DisableFmv;
            set => (Script as TR1Script).DisableFmv = value;
        }

        public bool DisableCine
        {
            get => (Script as TR1Script).DisableCine;
            set => (Script as TR1Script).DisableCine = value;
        }

        public bool DisableMusicInMenu
        {
            get => (Script as TR1Script).DisableMusicInMenu;
            set => (Script as TR1Script).DisableMusicInMenu = value;
        }

        public bool DisableMusicInInventory
        {
            get => (Script as TR1Script).DisableMusicInInventory;
            set => (Script as TR1Script).DisableMusicInInventory = value;
        }

        public bool DisableTrexCollision
        {
            get => (Script as TR1Script).DisableTrexCollision;
            set => (Script as TR1Script).DisableTrexCollision = value;
        }

        public int ResolutionWidth
        {
            get => (Script as TR1Script).ResolutionWidth;
            set => (Script as TR1Script).ResolutionWidth = value;
        }

        public int ResolutionHeight
        {
            get => (Script as TR1Script).ResolutionHeight;
            set => (Script as TR1Script).ResolutionHeight = value;
        }

        public bool EnableRoundShadow
        {
            get => (Script as TR1Script).EnableRoundShadow;
            set => (Script as TR1Script).EnableRoundShadow = value;
        }

        public bool Enable3dPickups
        {
            get => (Script as TR1Script).Enable3dPickups;
            set => (Script as TR1Script).Enable3dPickups = value;
        }

        public TRScreenshotFormat ScreenshotFormat
        {
            get => (Script as TR1Script).ScreenshotFormat;
            set => (Script as TR1Script).ScreenshotFormat = value;
        }

        public double AnisotropyFilter
        {
            get => (Script as TR1Script).AnisotropyFilter;
            set => (Script as TR1Script).AnisotropyFilter = value;
        }

        public bool WalkToItems
        {
            get => (Script as TR1Script).WalkToItems;
            set => (Script as TR1Script).WalkToItems = value;
        }

        public int MaximumSaveSlots
        {
            get => (Script as TR1Script).MaximumSaveSlots;
            set => (Script as TR1Script).MaximumSaveSlots = value;
        }

        public bool RevertToPistols
        {
            get => (Script as TR1Script).RevertToPistols;
            set => (Script as TR1Script).RevertToPistols = value;
        }
    }
}