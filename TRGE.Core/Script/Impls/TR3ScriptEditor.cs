using Newtonsoft.Json;

namespace TRGE.Core;

public class TR3ScriptEditor : AbstractTRScriptEditor, IUnarmedEditor, IAmmolessEditor, ISecretRewardEditor, IDemoEditor
{
    private bool _addGymWeapons, _addGymSkidoo;

    internal TR3ScriptEditor(TRScriptIOArgs ioArgs, TRScriptOpenOption openOption)
        : base(ioArgs, openOption) { }        

    protected override void ApplyConfig(Config config)
    {
        if (config == null)
        {
            UnarmedLevelOrganisation = Organisation.Default;
            UnarmedLevelRNG = new RandomGenerator(RandomGenerator.Type.Date);
            RandomUnarmedLevelCount = Math.Max(1, (LevelManager as TR3LevelManager).GetUnarmedLevelCount());

            AmmolessLevelOrganisation = Organisation.Default;
            AmmolessLevelRNG = new RandomGenerator(RandomGenerator.Type.Date);
            RandomAmmolessLevelCount = Math.Max(1, (LevelManager as TR3LevelManager).GetAmmolessLevelCount());

            SecretBonusOrganisation = Organisation.Default;
            SecretBonusRNG = new RandomGenerator(RandomGenerator.Type.Date);

            AddGymWeapons = AddGymSkidoo = false;

            return;
        }

        Config unarmedLevels = config.GetSubConfig("UnarmedLevels");
        UnarmedLevelOrganisation = unarmedLevels.GetOrganisation("Organisation");
        UnarmedLevelRNG = new RandomGenerator(unarmedLevels.GetSubConfig("RNG"));
        RandomUnarmedLevelCount = unarmedLevels.GetUInt("RandomCount");
        //see note in base.LoadConfig re restoring randomised - same applies for Unarmed
        if (unarmedLevels.ContainsKey("Data"))
        {
            UnarmedLevelData = JsonConvert.DeserializeObject<List<MutableTuple<string, string, bool>>>(unarmedLevels.GetString("Data"));
        }

        Config ammolessLevels = config.GetSubConfig("AmmolessLevels");
        AmmolessLevelOrganisation = ammolessLevels.GetOrganisation("Organisation");
        AmmolessLevelRNG = new RandomGenerator(ammolessLevels.GetSubConfig("RNG"));
        RandomAmmolessLevelCount = ammolessLevels.GetUInt("RandomCount");
        //see note in base.LoadConfig re restoring randomised - same applies for Ammoless
        if (ammolessLevels.ContainsKey("Data"))
        {
            AmmolessLevelData = JsonConvert.DeserializeObject<List<MutableTuple<string, string, bool>>>(ammolessLevels.GetString("Data"));
        }

        if (CanOrganiseBonuses)
        {
            Config bonuses = config.GetSubConfig("BonusSetup");
            if (bonuses == null) // Can be configured on the fly via TRInterop, but TRGE may reset this
            {
                SecretBonusOrganisation = Organisation.Default;
                SecretBonusRNG = new RandomGenerator(RandomGenerator.Type.Date);
            }
            else
            {
                SecretBonusOrganisation = bonuses.GetOrganisation("Organisation");
                SecretBonusRNG = new RandomGenerator(bonuses.GetSubConfig("RNG"));
                if (bonuses.ContainsKey("Data"))
                {
                    LevelSecretBonusData = JsonConvert.DeserializeObject<List<MutableTuple<string, string, List<MutableTuple<ushort, TRItemCategory, string, int>>>>>(bonuses.GetString("Data"));
                }
            }
        }

        LevelsHaveCutScenes = config.GetBool("LevelCutScenesOn");
        LevelsHaveFMV = config.GetBool("LevelFMVsOn");
        LevelsHaveStartAnimation = config.GetBool("LevelStartAnimsOn");
        CheatsEnabled = config.GetBool("CheatsOn");
        DemosEnabled = config.GetBool("DemosOn");
        DemoTime = config.GetUInt("DemoTime");
        IsDemoVersion = config.GetBool("DemoVersion");
        DozyEnabled = config.GetBool("DozyOn");
        GymEnabled = config.GetBool("GymOn");
        AddGymWeapons = config.GetBool("GymWeapons");
        AddGymSkidoo = config.GetBool("GymSkidoo");
        LevelSelectEnabled = config.GetBool("LevelSelectOn");
        OptionRingEnabled = config.GetBool("OptionRingOn");
        SaveLoadEnabled = config.GetBool("SaveLoadOn");
        ScreensizingEnabled = config.GetBool("ScreensizeOn");
        TitleScreenEnabled = config.GetBool("TitlesOn");
    }

    protected override void SaveImpl(AbstractTRScript backupScript, AbstractTRLevelManager backupLevelManager)
    {
        //for unarmed, ammoless and bonus data, save the config before
        //running any randomisation, otherwise when the script is
        //reloaded the random order will be available
        _config["UnarmedLevels"] = new Config
        {
            ["Organisation"] = (int)UnarmedLevelOrganisation,
            ["RNG"] = UnarmedLevelRNG.ToJson(),
            ["RandomCount"] = RandomUnarmedLevelCount,
            ["Data"] = (LevelManager as TR3LevelManager).GetUnarmedLevelData(backupLevelManager.Levels)
        };

        _config["AmmolessLevels"] = new Config
        {
            ["Organisation"] = (int)AmmolessLevelOrganisation,
            ["RNG"] = AmmolessLevelRNG.ToJson(),
            ["RandomCount"] = RandomAmmolessLevelCount,
            ["Data"] = (LevelManager as TR3LevelManager).GetAmmolessLevelData(backupLevelManager.Levels)
        };

        if (CanOrganiseBonuses)
        {
            _config["BonusSetup"] = new Config
            {
                ["Organisation"] = (int)SecretBonusOrganisation,
                ["RNG"] = SecretBonusRNG.ToJson(),
                ["Data"] = (LevelManager as TR3LevelManager).GetLevelBonusData(backupLevelManager.Levels)
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
        _config["GymWeapons"] = AddGymWeapons;
        _config["GymSkidoo"] = AddGymSkidoo;
        _config["LevelSelectOn"] = LevelSelectEnabled;
        _config["OptionRingOn"] = OptionRingEnabled;
        _config["SaveLoadOn"] = SaveLoadEnabled;
        _config["ScreensizeOn"] = ScreensizingEnabled;
        _config["TitlesOn"] = TitleScreenEnabled;

        List<AbstractTRScriptedLevel> randoBaseLevels = backupLevelManager.Levels;
        TR3LevelManager backupTR23LevelManager = backupLevelManager as TR3LevelManager;
        TR3LevelManager currentLevelManager = LevelManager as TR3LevelManager;
        
        if (AmmolessLevelOrganisation == Organisation.Random)
        {
            if (TRInterop.RandomisationSupported)
            {
                currentLevelManager.RandomiseAmmolessLevels(randoBaseLevels);
            }
            else
            {
                currentLevelManager.SetAmmolessLevelData(backupTR23LevelManager.GetAmmolessLevelData(backupLevelManager.Levels)); //#65 lock to that of the original file; #86
            }
        }
        else if (AmmolessLevelOrganisation == Organisation.Default)
        {
            currentLevelManager.SetAmmolessLevelData(backupTR23LevelManager.GetAmmolessLevelData(backupLevelManager.Levels));
        }

        if (UnarmedLevelOrganisation == Organisation.Random)
        {
            if (TRInterop.RandomisationSupported)
            {
                currentLevelManager.RandomiseUnarmedLevels(randoBaseLevels);
            }
            else
            {
                currentLevelManager.SetUnarmedLevelData(backupTR23LevelManager.GetUnarmedLevelData(backupLevelManager.Levels)); //#65 lock to that of the original file; #86
            }
        }
        else if (UnarmedLevelOrganisation == Organisation.Default)
        {
            currentLevelManager.SetUnarmedLevelData(backupTR23LevelManager.GetUnarmedLevelData(backupLevelManager.Levels));
        }
        else
        {
            currentLevelManager.SetUnarmedLevelData(UnarmedLevelData); //TODO: Fix this - it's in place to ensure the event is triggered for any listeners
        }

        //should occur after unarmed organisation and does not use any other script
        //as basis, as order of levels is essential
        if (SecretBonusOrganisation == Organisation.Random)
        {
            if (TRInterop.RandomisationSupported)
            {
                currentLevelManager.RandomiseBonuses();
            }
            else
            {
                currentLevelManager.SetLevelBonusData(backupTR23LevelManager.GetLevelBonusData(backupLevelManager.Levels)); //#65 lock to that of the original file
            }
        }
        else if (SecretBonusOrganisation == Organisation.Default)
        {
            currentLevelManager.SetLevelBonusData(backupTR23LevelManager.GetLevelBonusData(backupLevelManager.Levels));
        }

        TR3ScriptedLevel gym = currentLevelManager.AssaultLevel as TR3ScriptedLevel;
        currentLevelManager.MakeSkidooAvailable(gym, AddGymSkidoo);
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

            if (CanOrganiseBonuses && SecretBonusOrganisation == Organisation.Random)
            {
                config["BonusSetup"] = new Config
                {
                    ["Organisation"] = (int)Organisation.Default,
                    ["RNG"] = SecretBonusRNG.ToJson()
                };
            }
        }
        return config;
    }

    internal override AbstractTRScript CreateScript()
    {
        return new TR3Script();
    }

    protected override void ProcessGameMode(AbstractTRScript backupScript, AbstractTRLevelManager backupLevelManager)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// A list of Tuples configured as follows:
    ///     Item1 => Level ID (string);
    ///     Item2 => Level Name (string);
    ///     Item3 => Is unarmed (bool).
    /// </summary>
    public List<MutableTuple<string, string, bool>> UnarmedLevelData
    {
        get => (LevelManager as TR3LevelManager).GetUnarmedLevelData(LoadBackupScript().Levels);
        set => (LevelManager as TR3LevelManager).SetUnarmedLevelData(value);
    }

    public Organisation UnarmedLevelOrganisation
    {
        get => (LevelManager as TR3LevelManager).UnarmedLevelOrganisation;
        set => (LevelManager as TR3LevelManager).UnarmedLevelOrganisation = value;
    }

    public RandomGenerator UnarmedLevelRNG
    {
        get => (LevelManager as TR3LevelManager).UnarmedLevelRNG;
        set => (LevelManager as TR3LevelManager).UnarmedLevelRNG = value;
    }

    public uint RandomUnarmedLevelCount
    {
        get => (LevelManager as TR3LevelManager).RandomUnarmedLevelCount;
        set => (LevelManager as TR3LevelManager).RandomUnarmedLevelCount = value;
    }

    internal void RandomiseUnarmedLevels()
    {
        (LevelManager as TR3LevelManager).RandomiseUnarmedLevels(LoadRandomisationBaseScript().Levels);
    }

    internal List<TR3ScriptedLevel> GetUnarmedLevels()
    {
        return (LevelManager as TR3LevelManager).GetUnarmedLevels();
    }

    /// <summary>
    /// A list of Tuples configured as follows:
    ///     Item1 => Level ID (string);
    ///     Item2 => Level Name (string);
    ///     Item3 => Ammo removed (bool).
    /// </summary>
    public List<MutableTuple<string, string, bool>> AmmolessLevelData
    {
        get => (LevelManager as TR3LevelManager).GetAmmolessLevelData(LoadBackupScript().Levels);
        set => (LevelManager as TR3LevelManager).SetAmmolessLevelData(value);
    }

    public Organisation AmmolessLevelOrganisation
    {
        get => (LevelManager as TR3LevelManager).AmmolessLevelOrganisation;
        set => (LevelManager as TR3LevelManager).AmmolessLevelOrganisation = value;
    }

    public RandomGenerator AmmolessLevelRNG
    {
        get => (LevelManager as TR3LevelManager).AmmolessLevelRNG;
        set => (LevelManager as TR3LevelManager).AmmolessLevelRNG = value;
    }

    public uint RandomAmmolessLevelCount
    {
        get => (LevelManager as TR3LevelManager).RandomAmmolessLevelCount;
        set => (LevelManager as TR3LevelManager).RandomAmmolessLevelCount = value;
    }

    internal void RandomiseAmmolessLevels()
    {
        (LevelManager as TR3LevelManager).RandomiseAmmolessLevels(LoadRandomisationBaseScript().Levels);
    }

    internal List<TR3ScriptedLevel> GetAmmolessLevels()
    {
        return (LevelManager as TR3LevelManager).GetAmmolessLevels();
    }

    public bool CanOrganiseBonuses => (LevelManager as TR3LevelManager).CanOrganiseBonuses || TRInterop.SecretRewardsSupported;
    public Organisation SecretBonusOrganisation
    {
        get => (LevelManager as TR3LevelManager).BonusOrganisation;
        set => (LevelManager as TR3LevelManager).BonusOrganisation = value;
    }

    public RandomGenerator SecretBonusRNG
    {
        get => (LevelManager as TR3LevelManager).BonusRNG;
        set => (LevelManager as TR3LevelManager).BonusRNG = value;
    }

    public List<MutableTuple<string, string, List<MutableTuple<ushort, TRItemCategory, string, int>>>> LevelSecretBonusData
    {
        get
        {
            if (CanOrganiseBonuses)
            {
                return (LevelManager as TR3LevelManager).GetLevelBonusData(LoadBackupScript().Levels);
            }
            return null;
        }
        set
        {
            if (CanOrganiseBonuses)
            {
                (LevelManager as TR3LevelManager).SetLevelBonusData(value);
            }
        }
    }

    internal void RandomiseBonuses()
    {
        (LevelManager as TR3LevelManager).RandomiseBonuses(/*LoadBackupScript().Levels*/);
    }

    public bool LevelsHaveCutScenes
    {
        get => (LevelManager as TR3LevelManager).GetLevelsHaveCutScenes();
        set => (LevelManager as TR3LevelManager).SetLevelsHaveCutScenes(value);
    }

    public bool LevelsSupportCutScenes => (LevelManager as TR3LevelManager).GetLevelsSupportCutScenes();

    public bool LevelsHaveFMV
    {
        get => (LevelManager as TR3LevelManager).GetLevelsHaveFMV();
        set => (LevelManager as TR3LevelManager).SetLevelsHaveFMV(value);
    }

    public bool LevelsSupportFMVs => (LevelManager as TR3LevelManager).GetLevelsSupportFMVs();

    public bool LevelsHaveStartAnimation
    {
        get => (LevelManager as TR3LevelManager).GetLevelsHaveStartAnimation();
        set => (LevelManager as TR3LevelManager).SetLevelsHaveStartAnimation(value);
    }

    public bool LevelsSupportStartAnimations => (LevelManager as TR3LevelManager).GetLevelsSupportStartAnimations();

    public bool CheatsEnabled
    {
        get => !(Script as TR3Script).CheatsIgnored;
        set => (Script as TR3Script).CheatsIgnored = !value;
    }

    public int DefaultDemoCount
    {
        get => (LoadBackupScript() as TR3Script).NumDemoLevels;
    }

    public bool DemosSupported => DefaultDemoCount > 0;

    public bool DemosEnabled
    {
        get => DemosSupported && (Script as TR3Script).NumDemoLevels > 0;
        set => (Script as TR3Script).DemoLevels = value ? (LoadBackupScript() as TR3Script).DemoLevels : null;
    }

    public uint DemoTime
    {
        get => (Script as TR3Script).DemoTimeSeconds;
        set => (Script as TR3Script).DemoTimeSeconds = value;
    }

    public bool IsDemoVersion
    {
        get => (Script as TR3Script).DemoVersion;
        set => (Script as TR3Script).DemoVersion = value;
    }

    public bool DozyEnabled
    {
        get => (Script as TR3Script).DozyEnabled;
        set => (Script as TR3Script).DozyEnabled = value;
    }

    public bool DozySupported => (Script as TR3Script).DozyViable;

    public bool GymEnabled
    {
        get => (Script as TR3Script).GymEnabled;
        set => (Script as TR3Script).GymEnabled = value;
    }

    public bool SkidooAvailable => Edition.Version == TRVersion.TR2;

    public bool GymWeaponsAvailable => Edition.Version == TRVersion.TR2;

    public bool AddGymWeapons
    {
        get => _addGymWeapons;
        set => _addGymWeapons = GymAvailable && value;
    }

    public bool AddGymSkidoo
    {
        get => _addGymSkidoo;
        set => _addGymSkidoo = GymAvailable && SkidooAvailable && value;
    }

    public bool LevelSelectEnabled
    {
        get => (Script as TR3Script).LevelSelectEnabled;
        set => (Script as TR3Script).LevelSelectEnabled = value;
    }

    public bool OptionRingEnabled
    {
        get => !(Script as TR3Script).OptionRingDisabled;
        set => (Script as TR3Script).OptionRingDisabled = !value;
    }

    public bool SaveLoadEnabled
    {
        get => !(Script as TR3Script).SaveLoadDisabled;
        set => (Script as TR3Script).SaveLoadDisabled = !value;
    }

    public bool ScreensizingEnabled
    {
        get => !(Script as TR3Script).ScreensizingDisabled;
        set => (Script as TR3Script).ScreensizingDisabled = !value;
    }

    public bool TitleScreenEnabled
    {
        get => !(Script as TR3Script).TitleDisabled;
        set => (Script as TR3Script).TitleDisabled = !value;
    }
}