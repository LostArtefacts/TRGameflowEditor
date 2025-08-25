using Newtonsoft.Json;

namespace TRGE.Core;

public class TRXScriptEditor : AbstractTRScriptEditor, IUnarmedEditor, IAmmolessEditor, IDemoEditor, IHealthEditor
{
    internal TRXScriptEditor(TRScriptIOArgs ioArgs, TRScriptOpenOption openOption)
        : base(ioArgs, openOption) { }

    protected override void ApplyConfig(Config config)
    {
        if (config == null)
        {
            UnarmedLevelOrganisation = Organisation.Default;
            UnarmedLevelRNG = new RandomGenerator(RandomGenerator.Type.Date);
            RandomUnarmedLevelCount = Math.Max(1, (LevelManager as TRXLevelManager).GetUnarmedLevelCount());

            AmmolessLevelOrganisation = Organisation.Default;
            AmmolessLevelRNG = new RandomGenerator(RandomGenerator.Type.Date);
            RandomAmmolessLevelCount = Math.Max(1, (LevelManager as TRXLevelManager).GetAmmolessLevelCount());

            MedilessLevelOrganisation = Organisation.Default;
            MedilessLevelRNG = new RandomGenerator(RandomGenerator.Type.Date);
            RandomMedilessLevelCount = Math.Max(1, (LevelManager as TRXLevelManager).GetMedilessLevelCount());

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

        Config medilessLevels = config.GetSubConfig("MedilessLevels");
        MedilessLevelOrganisation = medilessLevels.GetOrganisation("Organisation");
        MedilessLevelRNG = new RandomGenerator(medilessLevels.GetSubConfig("RNG"));
        RandomMedilessLevelCount = medilessLevels.GetUInt("RandomCount");
        //see note in base.LoadConfig re restoring randomised - same applies for Ammoless
        if (medilessLevels.ContainsKey("Data"))
        {
            MedilessLevelData = JsonConvert.DeserializeObject<List<MutableTuple<string, string, bool>>>(medilessLevels.GetString("Data"));
        }

        LevelsHaveCutScenes = config.GetBool("LevelCutScenesOn");
        LevelsHaveFMV = config.GetBool("LevelFMVsOn");
        DemosEnabled = config.GetBool("DemosOn");
        MainMenuPicture = config.GetString("MainMenuPicture");

        StartLaraHitpoints = config.GetInt(nameof(StartLaraHitpoints), 1000);
        DisableHealingBetweenLevels = config.GetBool(nameof(DisableHealingBetweenLevels), false);
        DisableMedpacks = config.GetBool(nameof(DisableMedpacks), false);
        ConvertDroppedGuns = config.GetBool(nameof(ConvertDroppedGuns), false);
        EnableTR2ItemDrops = config.GetBool(nameof(EnableTR2ItemDrops), false);
        EnableKillerPushblocks = config.GetBool(nameof(EnableKillerPushblocks), true);
    }

    protected override void SaveImpl(AbstractTRScript backupScript, AbstractTRLevelManager backupLevelManager)
    {
        _config["UnarmedLevels"] = new Config
        {
            ["Organisation"] = (int)UnarmedLevelOrganisation,
            ["RNG"] = UnarmedLevelRNG.ToJson(),
            ["RandomCount"] = RandomUnarmedLevelCount,
            ["Data"] = (LevelManager as TRXLevelManager).GetUnarmedLevelData(backupLevelManager.Levels)
        };

        _config["AmmolessLevels"] = new Config
        {
            ["Organisation"] = (int)AmmolessLevelOrganisation,
            ["RNG"] = AmmolessLevelRNG.ToJson(),
            ["RandomCount"] = RandomAmmolessLevelCount,
            ["Data"] = (LevelManager as TRXLevelManager).GetAmmolessLevelData(backupLevelManager.Levels)
        };

        _config["MedilessLevels"] = new Config
        {
            ["Organisation"] = (int)MedilessLevelOrganisation,
            ["RNG"] = MedilessLevelRNG.ToJson(),
            ["RandomCount"] = RandomMedilessLevelCount,
            ["Data"] = (LevelManager as TRXLevelManager).GetMedilessLevelData(backupLevelManager.Levels) //MedilessLevelData
        };

        _config["LevelCutScenesOn"] = LevelsHaveCutScenes;
        _config["LevelFMVsOn"] = LevelsHaveFMV;
        _config["DemosOn"] = DemosEnabled;
        _config["MainMenuPicture"] = MainMenuPicture;
        _config[nameof(StartLaraHitpoints)] = StartLaraHitpoints;
        _config[nameof(DisableHealingBetweenLevels)] = DisableHealingBetweenLevels;
        _config[nameof(DisableMedpacks)] = DisableMedpacks;
        _config[nameof(ConvertDroppedGuns)] = ConvertDroppedGuns;
        _config[nameof(EnableTR2ItemDrops)] = EnableTR2ItemDrops;
        _config[nameof(EnableKillerPushblocks)] = EnableKillerPushblocks;

        List<AbstractTRScriptedLevel> randoBaseLevels = backupLevelManager.Levels;
        TRXLevelManager backupTR1LevelManager = backupLevelManager as TRXLevelManager;
        TRXLevelManager currentLevelManager = LevelManager as TRXLevelManager;

        if (AmmolessLevelOrganisation == Organisation.Random)
        {
            if (TRInterop.RandomisationSupported)
            {
                currentLevelManager.RandomiseAmmolessLevels(randoBaseLevels);
            }
            else
            {
                currentLevelManager.SetAmmolessLevelData(backupTR1LevelManager.GetAmmolessLevelData(backupLevelManager.Levels)); //#65 lock to that of the original file; #86
            }
        }
        else if (AmmolessLevelOrganisation == Organisation.Default)
        {
            currentLevelManager.SetAmmolessLevelData(backupTR1LevelManager.GetAmmolessLevelData(backupLevelManager.Levels));
        }

        if (MedilessLevelOrganisation == Organisation.Random)
        {
            if (TRInterop.RandomisationSupported)
            {
                currentLevelManager.RandomiseMedilessLevels(randoBaseLevels);
            }
            else
            {
                currentLevelManager.SetMedilessLevelData(backupTR1LevelManager.GetMedilessLevelData(backupLevelManager.Levels));
            }
        }
        else if (MedilessLevelOrganisation == Organisation.Default)
        {
            currentLevelManager.SetMedilessLevelData(backupTR1LevelManager.GetMedilessLevelData(backupLevelManager.Levels));
        }

        if (UnarmedLevelOrganisation == Organisation.Random)
        {
            if (TRInterop.RandomisationSupported)
            {
                currentLevelManager.RandomiseUnarmedLevels(randoBaseLevels);
            }
            else
            {
                currentLevelManager.SetUnarmedLevelData(backupTR1LevelManager.GetUnarmedLevelData(backupLevelManager.Levels)); //#65 lock to that of the original file; #86
            }
        }
        else if (UnarmedLevelOrganisation == Organisation.Default)
        {
            currentLevelManager.SetUnarmedLevelData(backupTR1LevelManager.GetUnarmedLevelData(backupLevelManager.Levels));
        }
        else
        {
            currentLevelManager.SetUnarmedLevelData(UnarmedLevelData); //TODO: Fix this - it's in place to ensure the event is triggered for any listeners
        }
    }

    internal override AbstractTRScript CreateScript()
    {
        return new TRXScript();
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

    protected override void ProcessGameMode(AbstractTRScript backupScript, AbstractTRLevelManager backupLevelManager)
    {
        TRXScript backupGoldScript = GoldEditor.LoadBackupScript() as TRXScript;

        LevelManager.RemoveLevels(l => true);
        MainMenuPicture = (backupScript as TRXScript).MainMenuPicture;

        if (GameMode == GameMode.Normal)
        {
            LevelManager.ImportLevels(backupScript.Levels);
            LevelManager.Levels[^1].IsFinalLevel = true;
        }
        else if (GameMode == GameMode.Gold)
        {
            MainMenuPicture = backupGoldScript.MainMenuPicture;
            LevelManager.ImportLevels(backupGoldScript.Levels);
        }
        else
        {
            LevelManager.ImportLevels(backupScript.Levels);
            TRXScriptedLevel finalLevel = LevelManager.Levels[^1] as TRXScriptedLevel;
            finalLevel.IsFinalLevel = false;
            int statsIndex = finalLevel.Sequences.FindIndex(s => s.Type == LevelSequenceType.Level_Stats);
            if (statsIndex == -1)
            {
                throw new Exception("Unable to combine regular/gold levels - original final level is misconfigured.");
            }

            BaseLevelSequence fmvSequence = finalLevel.Sequences.Find(s => s.Type == LevelSequenceType.Play_FMV);
            if (fmvSequence != null && finalLevel.Sequences.IndexOf(fmvSequence) < statsIndex)
            {
                fmvSequence = null;
            }

            for (int i = finalLevel.Sequences.Count - 1; i > statsIndex; i--)
            {
                finalLevel.Sequences.RemoveAt(i);
            }

            var statsMusicSeq = finalLevel.Sequences.Find(s => s.Type == LevelSequenceType.Play_Music);
            var firstMusicSeq = (LevelManager.Levels[0] as TRXScriptedLevel).Sequences.Find(s => s.Type == LevelSequenceType.Play_Music);
            if (statsMusicSeq != null && firstMusicSeq != null)
            {
                (statsMusicSeq as PlayMusicLevelSequence).MusicTrack = (firstMusicSeq as PlayMusicLevelSequence).MusicTrack;
            }

            LevelManager.ImportLevels(backupGoldScript.Levels);
            finalLevel = LevelManager.Levels[^1] as TRXScriptedLevel;
            finalLevel.IsFinalLevel = true;

            statsIndex = finalLevel.Sequences.FindIndex(s => s.Type == LevelSequenceType.Level_Stats);
            if (statsIndex != -1 && fmvSequence != null)
            {
                finalLevel.AddSequenceAfter(LevelSequenceType.Level_Stats, fmvSequence, false);
            }
        }

        LevelManager.SetLevelSequencing();
        backupLevelManager.RemoveLevels(l => true);
        backupLevelManager.ImportLevels(LevelManager.Levels);
    }

    public List<MutableTuple<string, string, bool>> UnarmedLevelData
    {
        get => (LevelManager as TRXLevelManager).GetUnarmedLevelData(LoadBackupScript().Levels);
        set => (LevelManager as TRXLevelManager).SetUnarmedLevelData(value);
    }

    public Organisation UnarmedLevelOrganisation
    {
        get => (LevelManager as TRXLevelManager).UnarmedLevelOrganisation;
        set => (LevelManager as TRXLevelManager).UnarmedLevelOrganisation = value;
    }

    public RandomGenerator UnarmedLevelRNG
    {
        get => (LevelManager as TRXLevelManager).UnarmedLevelRNG;
        set => (LevelManager as TRXLevelManager).UnarmedLevelRNG = value;
    }

    public uint RandomUnarmedLevelCount
    {
        get => (LevelManager as TRXLevelManager).RandomUnarmedLevelCount;
        set => (LevelManager as TRXLevelManager).RandomUnarmedLevelCount = value;
    }

    public List<MutableTuple<string, string, bool>> AmmolessLevelData
    {
        get => (LevelManager as TRXLevelManager).GetAmmolessLevelData(LoadBackupScript().Levels);
        set => (LevelManager as TRXLevelManager).SetAmmolessLevelData(value);
    }

    public Organisation AmmolessLevelOrganisation
    {
        get => (LevelManager as TRXLevelManager).AmmolessLevelOrganisation;
        set => (LevelManager as TRXLevelManager).AmmolessLevelOrganisation = value;
    }

    public RandomGenerator AmmolessLevelRNG
    {
        get => (LevelManager as TRXLevelManager).AmmolessLevelRNG;
        set => (LevelManager as TRXLevelManager).AmmolessLevelRNG = value;
    }

    public uint RandomAmmolessLevelCount
    {
        get => (LevelManager as TRXLevelManager).RandomAmmolessLevelCount;
        set => (LevelManager as TRXLevelManager).RandomAmmolessLevelCount = value;
    }

    internal void RandomiseAmmolessLevels()
    {
        (LevelManager as TRXLevelManager).RandomiseAmmolessLevels(LoadRandomisationBaseScript().Levels);
    }

    internal List<TRXScriptedLevel> GetAmmolessLevels()
    {
        return (LevelManager as TRXLevelManager).GetAmmolessLevels();
    }

    public List<MutableTuple<string, string, bool>> MedilessLevelData
    {
        get => (LevelManager as TRXLevelManager).GetMedilessLevelData(LoadBackupScript().Levels);
        set => (LevelManager as TRXLevelManager).SetMedilessLevelData(value);
    }

    public Organisation MedilessLevelOrganisation
    {
        get => (LevelManager as TRXLevelManager).MedilessLevelOrganisation;
        set => (LevelManager as TRXLevelManager).MedilessLevelOrganisation = value;
    }

    public RandomGenerator MedilessLevelRNG
    {
        get => (LevelManager as TRXLevelManager).MedilessLevelRNG;
        set => (LevelManager as TRXLevelManager).MedilessLevelRNG = value;
    }

    public uint RandomMedilessLevelCount
    {
        get => (LevelManager as TRXLevelManager).RandomMedilessLevelCount;
        set => (LevelManager as TRXLevelManager).RandomMedilessLevelCount = value;
    }

    internal void RandomiseMedilessLevels()
    {
        (LevelManager as TRXLevelManager).RandomiseMedilessLevels(LoadRandomisationBaseScript().Levels);
    }

    internal List<TRXScriptedLevel> GetMedilessLevels()
    {
        return (LevelManager as TRXLevelManager).GetMedilessLevels();
    }

    public bool LevelsHaveCutScenes
    {
        get => (LevelManager as TRXLevelManager).GetLevelsHaveCutScenes();
        set => (LevelManager as TRXLevelManager).SetLevelsHaveCutScenes(value);
    }

    public bool LevelsSupportCutScenes => (LevelManager as TRXLevelManager).GetLevelsSupportCutScenes();

    public bool LevelsHaveFMV
    {
        get => (LevelManager as TRXLevelManager).GetLevelsHaveFMV();
        set => (LevelManager as TRXLevelManager).SetLevelsHaveFMV(value);
    }

    public bool LevelsSupportFMVs => (LevelManager as TRXLevelManager).GetLevelsSupportFMVs();      

    public bool DemosEnabled
    {
        get => (Script as TRXScript).DemosEnabled;
        set => (Script as TRXScript).DemosEnabled = value;
    }

    public string MainMenuPicture
    {
        get => (Script as TRXScript).MainMenuPicture;
        set => (Script as TRXScript).MainMenuPicture = value;
    }

    public int StartLaraHitpoints
    {
        get => (Script as TRXScript).StartLaraHitpoints;
        set => (Script as TRXScript).StartLaraHitpoints = value;
    }

    public bool DisableMedpacks
    {
        get => (Script as TRXScript).DisableMedpacks;
        set => (Script as TRXScript).DisableMedpacks = value;
    }

    public bool DisableHealingBetweenLevels
    {
        get => (Script as TRXScript).DisableHealingBetweenLevels;
        set => (Script as TRXScript).DisableHealingBetweenLevels = value;
    }

    public bool ConvertDroppedGuns
    {
        get => (Script as TRXScript).ConvertDroppedGuns;
        set => (Script as TRXScript).ConvertDroppedGuns = value;
    }

    public bool EnableTR2ItemDrops
    {
        get => (Script as TRXScript).EnableTR2ItemDrops;
        set => (Script as TRXScript).EnableTR2ItemDrops = value;
    }

    public bool EnableKillerPushblocks
    {
        get => (Script as TRXScript).EnableKillerPushblocks;
        set => (Script as TRXScript).EnableKillerPushblocks = value;
    }
}
