using Newtonsoft.Json;

namespace TRGE.Core;

public class TR1ScriptEditor : AbstractTRScriptEditor, IUnarmedEditor, IAmmolessEditor, IDemoEditor, IHealthEditor
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

            MedilessLevelOrganisation = Organisation.Default;
            MedilessLevelRNG = new RandomGenerator(RandomGenerator.Type.Date);
            RandomMedilessLevelCount = Math.Max(1, (LevelManager as TR1LevelManager).GetMedilessLevelCount());

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
            ["Data"] = (LevelManager as TR1LevelManager).GetUnarmedLevelData(backupLevelManager.Levels)
        };

        _config["AmmolessLevels"] = new Config
        {
            ["Organisation"] = (int)AmmolessLevelOrganisation,
            ["RNG"] = AmmolessLevelRNG.ToJson(),
            ["RandomCount"] = RandomAmmolessLevelCount,
            ["Data"] = (LevelManager as TR1LevelManager).GetAmmolessLevelData(backupLevelManager.Levels)
        };

        _config["MedilessLevels"] = new Config
        {
            ["Organisation"] = (int)MedilessLevelOrganisation,
            ["RNG"] = MedilessLevelRNG.ToJson(),
            ["RandomCount"] = RandomMedilessLevelCount,
            ["Data"] = (LevelManager as TR1LevelManager).GetMedilessLevelData(backupLevelManager.Levels) //MedilessLevelData
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
        TR1LevelManager backupTR1LevelManager = backupLevelManager as TR1LevelManager;
        TR1LevelManager currentLevelManager = LevelManager as TR1LevelManager;

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

    protected override void ProcessGameMode(AbstractTRScript backupScript, AbstractTRLevelManager backupLevelManager)
    {
        TR1Script backupGoldScript = GoldEditor.LoadBackupScript() as TR1Script;

        LevelManager.RemoveLevels(l => true);
        MainMenuPicture = (backupScript as TR1Script).MainMenuPicture;

        if (GameMode == GameMode.Normal)
        {
            LevelManager.ImportLevels(backupScript.Levels);
            LevelManager.Levels[^1].IsFinalLevel = true;
        }
        else if (GameMode == GameMode.Gold)
        {
            MainMenuPicture = backupGoldScript.MainMenuPicture;
            LevelManager.ImportLevels(backupGoldScript.Levels);
            //if (Edition.AssaultCourseSupported)
            //{
            //    foreach (TR1ScriptedLevel level in LevelManager.Levels.Cast<TR1ScriptedLevel>())
            //    {
            //        foreach (BaseLevelSequence sequence in level.Sequences)
            //        {
            //            if (sequence is LevelExitLevelSequence exit)
            //            {
            //                exit.LevelId++;
            //            }
            //        }
            //    }
            //}
        }
        else
        {
            LevelManager.ImportLevels(backupScript.Levels);
            TR1ScriptedLevel finalLevel = LevelManager.Levels[^1] as TR1ScriptedLevel;
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
            ////finalLevel.Sequences.Add(new LevelExitLevelSequence()
            ////{
            ////    Type = LevelSequenceType.Exit_To_Level,
            ////    LevelId = LevelManager.LevelCount + 1
            ////});

            LevelManager.ImportLevels(backupGoldScript.Levels);
            finalLevel = LevelManager.Levels[^1] as TR1ScriptedLevel;
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

    public List<MutableTuple<string, string, bool>> MedilessLevelData
    {
        get => (LevelManager as TR1LevelManager).GetMedilessLevelData(LoadBackupScript().Levels);
        set => (LevelManager as TR1LevelManager).SetMedilessLevelData(value);
    }

    public Organisation MedilessLevelOrganisation
    {
        get => (LevelManager as TR1LevelManager).MedilessLevelOrganisation;
        set => (LevelManager as TR1LevelManager).MedilessLevelOrganisation = value;
    }

    public RandomGenerator MedilessLevelRNG
    {
        get => (LevelManager as TR1LevelManager).MedilessLevelRNG;
        set => (LevelManager as TR1LevelManager).MedilessLevelRNG = value;
    }

    public uint RandomMedilessLevelCount
    {
        get => (LevelManager as TR1LevelManager).RandomMedilessLevelCount;
        set => (LevelManager as TR1LevelManager).RandomMedilessLevelCount = value;
    }

    internal void RandomiseMedilessLevels()
    {
        (LevelManager as TR1LevelManager).RandomiseMedilessLevels(LoadRandomisationBaseScript().Levels);
    }

    internal List<TR1ScriptedLevel> GetMedilessLevels()
    {
        return (LevelManager as TR1LevelManager).GetMedilessLevels();
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
        get => (Script as TR1Script).DemosEnabled;
        set => (Script as TR1Script).DemosEnabled = value;
    }

    public string MainMenuPicture
    {
        get => (Script as TR1Script).MainMenuPicture;
        set => (Script as TR1Script).MainMenuPicture = value;
    }

    public int StartLaraHitpoints
    {
        get => (Script as TR1Script).StartLaraHitpoints;
        set => (Script as TR1Script).StartLaraHitpoints = value;
    }

    public bool DisableMedpacks
    {
        get => (Script as TR1Script).DisableMedpacks;
        set => (Script as TR1Script).DisableMedpacks = value;
    }

    public bool DisableHealingBetweenLevels
    {
        get => (Script as TR1Script).DisableHealingBetweenLevels;
        set => (Script as TR1Script).DisableHealingBetweenLevels = value;
    }

    public bool ConvertDroppedGuns
    {
        get => (Script as TR1Script).ConvertDroppedGuns;
        set => (Script as TR1Script).ConvertDroppedGuns = value;
    }

    public bool EnableTR2ItemDrops
    {
        get => (Script as TR1Script).EnableTR2ItemDrops;
        set => (Script as TR1Script).EnableTR2ItemDrops = value;
    }

    public bool EnableKillerPushblocks
    {
        get => (Script as TR1Script).EnableKillerPushblocks;
        set => (Script as TR1Script).EnableKillerPushblocks = value;
    }
}
