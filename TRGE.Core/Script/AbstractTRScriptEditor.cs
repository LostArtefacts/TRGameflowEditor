using Newtonsoft.Json;

namespace TRGE.Core;

public abstract class AbstractTRScriptEditor : AbstractTRGEEditor
{
    protected TRScriptIOArgs _io;

    public FileInfo OriginalFile
    {
        get => _io.TRScriptFile;
        set => _io.TRScriptFile = value;
    }

    public FileInfo BackupFile
    {
        get => _io.TRScriptBackupFile;
        set => _io.TRScriptBackupFile = value;
    }

    public FileInfo TRConfigFile => _io.TRConfigFile;
    public FileInfo BackupTRConfigFile => _io.TRConfigBackupFile;

    public FileInfo InternalConfigFile
    {
        get => _io.InternalConfigFile;
        set => _io.InternalConfigFile = value;
    }

    internal override string ConfigFilePath => InternalConfigFile?.FullName;

    public DirectoryInfo WIPOutputDirectory
    {
        get => _io.WIPOutputDirectory;
        set => _io.WIPOutputDirectory = value;
    }

    public DirectoryInfo OutputDirectory
    {
        get => _io.OutputDirectory;
        set => _io.OutputDirectory = value;
    }

    public DirectoryInfo BackupDirectory
    {
        get => _io.BackupDirectory;
        set => _io.BackupDirectory = value;
    }

    public TREdition Edition => Script.Edition;

    public AbstractTRAudioProvider AudioProvider => LevelManager.AudioProvider;
    public AbstractTRLevelManager LevelManager { get; private set; }
    public AbstractTRFrontEnd FrontEnd => Script.FrontEnd;
    public AbstractTRScriptedLevel AssaultLevel => LevelManager.AssaultLevel;
    public AbstractTRScript Script { get; private set; }

    public IReadOnlyList<AbstractTRScriptedLevel> Levels => LevelManager.Levels;
    public IReadOnlyList<AbstractTRScriptedLevel> ScriptedLevels => LevelManager.GetOriginalSequencedLevels(LoadBackupScript().Levels);
    public IReadOnlyList<AbstractTRScriptedLevel> EnabledScriptedLevels => LevelManager.GetOriginalSequencedLevels(LevelManager.Levels, true);

    protected TRScriptOpenOption _openOption;

    internal event EventHandler<TRScriptedLevelEventArgs> LevelModified;

    public AbstractTRScriptEditor GoldEditor { get; set; }
    public GameMode GameMode { get; set; }

    internal AbstractTRScriptEditor(TRScriptIOArgs ioArgs, TRScriptOpenOption openOption)
    {
        _io = ioArgs;
        _openOption = openOption;
        Initialise();
    }

    internal void Initialise()
    {
        Initialise(CreateScript());
    }

    private void Initialise(AbstractTRScript script)
    {
        LoadConfig();

        (Script = script).Read(BackupFile);
        if (BackupTRConfigFile != null)
        {
            Script.ReadConfig(BackupTRConfigFile);
        }

        TRPatchTester.Test(Edition, _io);

        LevelManager = TRScriptedLevelFactory.GetLevelManager(script);
        LevelManager.LevelModified += LevelManagerLevelModified;

        ReadConfig(_config);
    }

    private void LevelManagerLevelModified(object sender, TRScriptedLevelEventArgs e)
    {
        LevelModified?.Invoke(this, e);
    }

    private void LoadConfig()
    {
        _config = ConfigFilePath != null ? Config.Read(ConfigFilePath) : null;
        //issue #36
        if (_config != null && OriginalFile != null && !OriginalFile.Checksum().Equals(_config["CheckSumOnSave"]))
        {
            switch (_openOption)
            {
                //callers should make a different OpenOption and try to open the script again
                case TRScriptOpenOption.Default:
                    throw new ChecksumMismatchException();
                //overwrite the existing backup with the "new" original file, delete the config as though we have never opened the file before
                case TRScriptOpenOption.DiscardBackup:
                    File.Copy(OriginalFile.FullName, BackupFile.FullName, true);
                    while (File.Exists(InternalConfigFile.FullName))
                    {
                        InternalConfigFile.Delete(); //issue #39
                    }
                    _config = null;
                    break;
                //overwrite the original file with the backup
                case TRScriptOpenOption.RestoreBackup:
                    File.Copy(BackupFile.FullName, OriginalFile.FullName, true);
                    break;
            }

            _openOption = TRScriptOpenOption.Default;
        }
    }

    protected override void ReadConfig(Config config)
    {
        if (config != null)
        {
            Config configEdition = config.GetSubConfig("Edition");
            TREdition checkEdition = TREdition.From(configEdition.GetHardware("Hardware"), configEdition.GetTRVersion("Version"));
            if (checkEdition == null || !checkEdition.Equals(Edition))
            {
                throw new EditionMismatchException("The TR edition in the configuration file does not match the edition of the current script file.");
            }

            GameMode = config.GetGameMode(nameof(GameMode));
            Config levelSeq = config.GetSubConfig("LevelSequencing");
            LevelSequencingOrganisation = levelSeq.GetOrganisation("Organisation");
            LevelSequencingRNG = new RandomGenerator(levelSeq.GetSubConfig("RNG"));
            //note that even if the levels were randomised, this would have been done after saving the config file
            //so reloading the sequencing will either just restore defaults or set it to a manual sequence if the user
            //picked that at one point in the previous edit
            if (levelSeq.ContainsKey("Data"))
            {
                LevelSequencing = JsonConvert.DeserializeObject<List<Tuple<string, string>>>(levelSeq.GetString("Data"));
            }

            if (config.ContainsKey("EnabledLevelStatus"))
            {
                Config enabledLevels = config.GetSubConfig("EnabledLevelStatus");
                EnabledLevelOrganisation = enabledLevels.GetOrganisation("Organisation");
                EnabledLevelRNG = new RandomGenerator(enabledLevels.GetSubConfig("RNG"));
                //see note above
                if (enabledLevels.ContainsKey("Data"))
                {
                    EnabledLevelStatus = JsonConvert.DeserializeObject<List<MutableTuple<string, string, bool>>>(enabledLevels.GetString("Data"));
                }
                RandomEnabledLevelCount = enabledLevels.GetUInt("RandomCount");
            }
            else
            {
                EnabledLevelOrganisation = Organisation.Default;
                EnabledLevelRNG = new RandomGenerator(RandomGenerator.Type.Date);
                RandomEnabledLevelCount = (uint)LevelManager.LevelCount;
            }

            Config trackInfo = config.GetSubConfig("GameTracks");
            GameTrackOrganisation = trackInfo.GetOrganisation("Organisation");
            GameTrackRNG = new RandomGenerator(trackInfo.GetSubConfig("RNG"));
            //see note above
            if (trackInfo.ContainsKey("Data"))
            {
                GameTrackData = JsonConvert.DeserializeObject<List<MutableTuple<string, string, ushort>>>(trackInfo.GetString("Data"));
            }
            RandomGameTracksIncludeBlank = trackInfo.GetBool("IncludeBlank");

            Config secretInfo = config.GetSubConfig("SecretSupport");
            LevelSecretSupportOrganisation = secretInfo.GetOrganisation("Organisation");
            LevelSecretSupport = JsonConvert.DeserializeObject<List<MutableTuple<string, string, bool>>>(secretInfo.GetString("Data"));

            Config sunsetInfo = config.GetSubConfig("Sunsets");
            LevelSunsetOrganisation = sunsetInfo.GetOrganisation("Organisation");
            LevelSunsetRNG = new RandomGenerator(sunsetInfo.GetSubConfig("RNG"));
            //see note above
            if (sunsetInfo.ContainsKey("Data"))
            {
                LevelSunsetData = JsonConvert.DeserializeObject<List<MutableTuple<string, string, bool>>>(sunsetInfo.GetString("Data"));
            }
            RandomSunsetLevelCount = sunsetInfo.GetUInt("RandomCount");

            FrontEndHasFMV = config.GetBool("FrontEndFMVOn");
        }
        else
        {
            GameMode = GameMode.Normal;
            LevelSequencingOrganisation = Organisation.Default;
            LevelSequencingRNG = new RandomGenerator(RandomGenerator.Type.Date);
            EnabledLevelOrganisation = Organisation.Default;
            EnabledLevelRNG = new RandomGenerator(RandomGenerator.Type.Date);
            RandomEnabledLevelCount = (uint)LevelManager.LevelCount;
            GameTrackOrganisation = Organisation.Default;
            GameTrackRNG = new RandomGenerator(RandomGenerator.Type.Date);
            RandomGameTracksIncludeBlank = false;
            LevelSecretSupportOrganisation = Organisation.Default;
            LevelSunsetOrganisation = Organisation.Default;
            LevelSunsetRNG = new RandomGenerator(RandomGenerator.Type.Date);
            RandomSunsetLevelCount = LevelManager.GetSunsetLevelCount();
        }

        ApplyConfig(config);
    }

    public override int GetSaveTargetCount()
    {
        return 1;
    }

    public int GetTotalLevelCount(GameMode mode)
    {
        return GetLevelCount(mode, l => true);
    }

    public int GetDefaultUnarmedLevelCount(GameMode mode)
    {
        return GetLevelCount(mode, l => l.RemovesWeapons);
    }

    public int GetDefaultAmmolessLevelCount(GameMode mode)
    {
        return GetLevelCount(mode, l => l.RemovesAmmo);
    }

    public int GetDefaultSunsetLevelCount(GameMode mode)
    {
        return GetLevelCount(mode, l => l.HasSunset);
    }

    public int GetLevelCount(GameMode mode, Func<AbstractTRScriptedLevel, bool> propertyFunc)
    {
        AbstractTRLevelManager backupLevelManager = TRScriptedLevelFactory.GetLevelManager(LoadBackupScript());
        if (mode == GameMode.Normal || GoldEditor == null)
        {
            return backupLevelManager.Levels.Where(l => propertyFunc(l)).Count();
        }

        int count = GoldEditor.LevelManager.Levels.Where(l => propertyFunc(l)).Count();
        if (mode == GameMode.Combined)
        {
            count += backupLevelManager.Levels.Where(l => propertyFunc(l)).Count();
        }

        return count;
    }

    internal void Save(/*TRSaveMonitor monitor*/)
    {
        //monitor.FireSaveStateChanged(0, TRSaveCategory.Scripting);

        _config = new Config
        {
            ["App"] = new Config
            {
                ["Tag"] = TRInterop.TaggedVersion,
                ["Version"] = TRInterop.ExecutingVersion
            },
            ["Edition"] = Edition.ToJson(),
            ["Original"] = OriginalFile?.FullName,
            ["CheckSumOnSave"] = string.Empty,
            ["FrontEndFMVOn"] = FrontEndHasFMV,
            [nameof(GameMode)] = (int)GameMode,
            ["LevelSequencing"] = new Config
            {
                ["Organisation"] = (int)LevelSequencingOrganisation,
                ["RNG"] = LevelSequencingRNG.ToJson(),
                ["Data"] = LevelSequencing
            },
            ["EnabledLevelStatus"] = new Config
            {
                ["Organisation"] = (int)EnabledLevelOrganisation,
                ["RNG"] = EnabledLevelRNG.ToJson(),
                ["Data"] = EnabledLevelStatus,
                ["RandomCount"] = RandomEnabledLevelCount
            },
            ["GameTracks"] = new Config
            {
                ["Organisation"] = (int)GameTrackOrganisation,
                ["RNG"] = GameTrackRNG.ToJson(),
                ["Data"] = GameTrackData,
                ["IncludeBlank"] = RandomGameTracksIncludeBlank
            },
            ["SecretSupport"] = new Config
            {
                ["Organisation"] = (int)LevelSecretSupportOrganisation,
                ["Data"] = LevelSecretSupport
            },
            ["Sunsets"] = new Config
            {
                ["Organisation"] = (int)LevelSunsetOrganisation,
                ["RNG"] = LevelSunsetRNG.ToJson(),
                ["RandomCount"] = RandomSunsetLevelCount,
                ["Data"] = LevelSunsetData
            }
        };

        AbstractTRScript backupScript = LoadBackupScript();
        AbstractTRLevelManager backupLevelManager = TRScriptedLevelFactory.GetLevelManager(backupScript);

        if (GoldEditor != null)
        {
            ProcessGameMode(backupScript, backupLevelManager);
        }

        if (LevelSequencingOrganisation == Organisation.Random)
        {
            if (TRInterop.RandomisationSupported)
            {
                LevelManager.RandomiseSequencing(backupLevelManager.Levels);
            }
            else
            {
                LevelManager.SetSequencing(backupLevelManager.GetSequencing()); //#65 lock to that of the original file; #86 ensure backup script is used and not what's in the original folder
            }
        }
        else if (LevelSequencingOrganisation == Organisation.Default)
        {
            LevelManager.SetSequencing(backupLevelManager.GetSequencing());
        }

        if (EnabledLevelOrganisation == Organisation.Random)
        {
            if (TRInterop.RandomisationSupported)
            {
                LevelManager.RandomiseEnabledLevels();
            }
            else
            {
                LevelManager.SetEnabledLevelStatus(backupLevelManager.GetEnabledLevelStatus());
            }
        }
        else if (EnabledLevelOrganisation == Organisation.Default)
        {
            LevelManager.SetEnabledLevelStatus(backupLevelManager.GetEnabledLevelStatus());
        }
        else if (EnabledLevelOrganisation == Organisation.Manual)
        {
            LevelManager.SetEnabledLevelStatus(EnabledLevelStatus);
        }

        if (GameTrackOrganisation == Organisation.Random)
        {
            if (TRInterop.RandomisationSupported)
            {
                LevelManager.RandomiseGameTracks(backupLevelManager.Levels);
            }
            else
            {
                LevelManager.SetTrackData(backupLevelManager.GetTrackData(backupLevelManager.Levels)); //#65 lock to that of the original file; #86 ensure backup script is used and not what's in the original folder
            }
        }
        else if (GameTrackOrganisation == Organisation.Default)
        {
            LevelManager.SetTrackData(backupLevelManager.GetTrackData(backupLevelManager.Levels));
        }

        if (LevelSecretSupportOrganisation == Organisation.Default)
        {
            LevelManager.RestoreSecretSupport(backupLevelManager.Levels);
        }

        if (LevelSunsetOrganisation == Organisation.Random)
        {
            if (TRInterop.RandomisationSupported)
            {
                LevelManager.RandomiseSunsets(backupLevelManager.Levels);
            }
            else
            {
                LevelManager.SetSunsetData(backupLevelManager.GetSunsetData(backupLevelManager.Levels)); //#65 lock to that of the original file; #86 ensure backup script is used and not what's in the original folder
            }
        }
        else if (LevelSunsetOrganisation == Organisation.Default)
        {
            LevelManager.SetSunsetData(backupLevelManager.GetSunsetData(backupLevelManager.Levels));
        }
        else
        {
            LevelManager.SetSunsetData(LevelSunsetData); //TODO: Fix this - it's in place to ensure the event is triggered for any listeners
        }

        SaveImpl(backupScript, backupLevelManager);

        LevelManager.Save();

        WriteScript();
    }

    protected abstract void ProcessGameMode(AbstractTRScript backupScript, AbstractTRLevelManager backupLevelManager);

    public void SaveScript()
    {
        // Commit level specifics
        LevelManager.UpdateScript();

        WriteScript();
    }

    private void WriteScript()
    {
        if (OriginalFile != null)
        {
            string outputPath = GetScriptWIPOutputPath();
            Script.Write(outputPath);
        }

        if (TRConfigFile != null)
        {
            Script.WriteConfig(GetTRConfigWIPOutputPath(), TRConfigFile.FullName);
        }
    }

    /// <summary>
    /// Called on a successful save transaction from TREditor, so it is safe to write the current config
    /// to disk at this stage.
    /// </summary>
    internal sealed override void SaveComplete()
    {
        if (OriginalFile != null)
        {
            _config["CheckSumOnSave"] = new FileInfo(GetScriptWIPOutputPath()).Checksum();
        }
        _config.Write(InternalConfigFile.FullName);

        if (Edition.ExportLevelData)
        {
            // Store sequencing data for third-party tools to access/compare differences
            string sequenceData = Path.Combine(Path.GetDirectoryName(OriginalFile.FullName), "../", "LEVELINFO.DAT");
            using BinaryWriter bw = new(new FileStream(sequenceData, FileMode.Create));
            byte levelCount = (byte)LevelManager.EnabledLevelCount;
            AbstractTRScriptedLevel assaultLevel = null;// AssaultLevel;
            if (assaultLevel != null)
            {
                levelCount++;
            }
            bw.Write(levelCount);
            assaultLevel?.SerializeToMain(bw);

            foreach (AbstractTRScriptedLevel level in LevelManager.Levels)
            {
                if (level.Enabled)
                {
                    level.SerializeToMain(bw);
                }
            }
        }
    }

    internal override Config ExportConfig()
    {
        Config config = base.ExportConfig();
        config.Remove("CheckSumOnSave");
        config.Remove("Original");

        if (!TRInterop.RandomisationSupported)
        {
            if (LevelSequencingOrganisation == Organisation.Random)
            {
                config["LevelSequencing"] = new Config
                {
                    ["Organisation"] = (int)Organisation.Default,
                    ["RNG"] = LevelSequencingRNG.ToJson()
                };
            }

            if (GameTrackOrganisation == Organisation.Random)
            {
                config["GameTracks"] = new Config
                {
                    ["Organisation"] = (int)Organisation.Default,
                    ["RNG"] = GameTrackRNG.ToJson()
                };
            }

            if (LevelSunsetOrganisation == Organisation.Random)
            {
                config["Sunsets"] = new Config
                {
                    ["Organisation"] = (int)Organisation.Default,
                    ["RNG"] = LevelSunsetRNG.ToJson(),
                    ["RandomCount"] = RandomSunsetLevelCount
                };
            }
        }
        return config;
    }

    internal override void Restore()
    {
        if (OriginalFile != null)
        {
            BackupFile.CopyTo(OriginalFile.FullName, true);
        }

        if (TRConfigFile != null)
        {
            BackupTRConfigFile.CopyTo(TRConfigFile.FullName, true);
        }

        while (File.Exists(InternalConfigFile.FullName))
        {
            InternalConfigFile.Delete(); //issue #39
        }
        InternalConfigFile = new FileInfo(InternalConfigFile.FullName);
        Initialise(Script);
    }

    protected string GetScriptWIPOutputPath()
    {
        return Path.Combine(WIPOutputDirectory.FullName, OriginalFile.Name);
    }

    protected string GetTRConfigWIPOutputPath()
    {
        return Path.Combine(WIPOutputDirectory.FullName, TRConfigFile.Name);
    }

    internal string GetScriptOutputPath()
    {
        return Path.Combine(OutputDirectory.FullName, OriginalFile.Name);
    }

    internal string GetConfigOutputPath()
    {
        return TRConfigFile == null ? null : Path.Combine(OutputDirectory.FullName, TRConfigFile.Name);
    }

    internal AbstractTRScript LoadBackupScript()
    {
        AbstractTRScript script = LoadScript(BackupFile?.FullName);
        if (BackupTRConfigFile != null)
        {
            script.ReadConfig(BackupTRConfigFile);
        }
        return script;
    }

    internal AbstractTRScript LoadRandomisationBaseScript()
    {
        return LoadBackupScript();
    }

    internal AbstractTRScript LoadScript(string filePath)
    {
        AbstractTRScript script = CreateScript();
        script.Read(filePath);
        return script;
    }

    internal List<AbstractTRScriptedLevel> GetOriginalLevels()
    {
        return LoadBackupScript().Levels;
    }

    internal abstract AbstractTRScript CreateScript();

    protected abstract void SaveImpl(AbstractTRScript backupScript, AbstractTRLevelManager backupLevelManager);

    public bool GymAvailable => Edition.AssaultCourseSupported;

    public Organisation LevelSequencingOrganisation
    {
        get => LevelManager.SequencingOrganisation;
        set => LevelManager.SequencingOrganisation = value;
    }

    public RandomGenerator LevelSequencingRNG
    {
        get => LevelManager.SequencingRNG;
        set => LevelManager.SequencingRNG = value;
    }

    public List<Tuple<string, string>> LevelSequencing
    {
        get => LevelManager.GetSequencing();
        set => LevelManager.SetSequencing(value);
    }

    internal void RandomiseLevels()
    {
        LevelManager.RandomiseSequencing(LoadBackupScript().Levels);
    }

    public Organisation EnabledLevelOrganisation
    {
        get => LevelManager.EnabledOrganisation;
        set => LevelManager.EnabledOrganisation = value;
    }

    public RandomGenerator EnabledLevelRNG
    {
        get => LevelManager.EnabledRNG;
        set => LevelManager.EnabledRNG = value;
    }

    public List<MutableTuple<string, string, bool>> EnabledLevelStatus
    {
        get => LevelManager.GetEnabledLevelStatus();
        set => LevelManager.SetEnabledLevelStatus(value);
    }

    public uint RandomEnabledLevelCount
    {
        get => LevelManager.RandomEnabledCount;
        set => LevelManager.RandomEnabledCount = value;
    }

    internal void RandomiseEnabledLevels()
    {
        LevelManager.RandomiseEnabledLevels();
    }

    public bool FrontEndHasFMV
    {
        get => FrontEnd.HasFMV;
        set => FrontEnd.HasFMV = value;
    }

    public Organisation GameTrackOrganisation
    {
        get => LevelManager.GameTrackOrganisation;
        set => LevelManager.GameTrackOrganisation = value;
    }

    public RandomGenerator GameTrackRNG
    {
        get => LevelManager.GameTrackRNG;
        set => LevelManager.GameTrackRNG = value;
    }

    public bool RandomGameTracksIncludeBlank
    {
        get => LevelManager.RandomGameTracksIncludeBlank;
        set => LevelManager.RandomGameTracksIncludeBlank = value;
    }

    public virtual List<MutableTuple<string, string, ushort>> GameTrackData
    {
        get => LevelManager.GetTrackData(LoadBackupScript().Levels);
        set => LevelManager.SetTrackData(value);
    }

    public IReadOnlyList<Tuple<ushort, string>> AllGameTracks => LevelManager.GetAllGameTracks();

    internal TRAudioTrack TitleTrack => LevelManager.AudioProvider.GetTrack(Script.TitleSoundID);
    internal TRAudioTrack SecretTrack => LevelManager.AudioProvider.GetTrack(Script.SecretSoundID);

    public byte[] GetTrackData(ushort trackID)
    {
        return LevelManager.AudioProvider.GetTrackData(trackID);
    }

    internal void RandomiseGameTracks()
    {
        LevelManager.RandomiseGameTracks(LoadBackupScript().Levels);
    }

    public Organisation LevelSecretSupportOrganisation
    {
        get => LevelManager.SecretSupportOrganisation;
        set
        {
            if (value == Organisation.Random)
            {
                throw new NotImplementedException("Randomisation of level secret support is not implemented.");
            }
            LevelManager.SecretSupportOrganisation = value;
        }
    }

    public List<MutableTuple<string, string, bool>> LevelSecretSupport
    {
        get => LevelManager.GetSecretSupport(LoadBackupScript().Levels);
        set => LevelManager.SetSecretSupport(value);
    }

    public bool AllLevelsHaveSecrets
    {
        get => LevelManager.GetAllLevelsHaveSecrets();
        set
        {
            LevelManager.SetAllLevelsHaveSecrets(value);
            LevelSecretSupportOrganisation = Organisation.Manual;
        }
    }

    public bool CanSetSunsets => LevelManager.CanSetSunsets;

    public Organisation LevelSunsetOrganisation
    {
        get => LevelManager.SunsetOrganisation;
        set => LevelManager.SunsetOrganisation = value;
    }

    public RandomGenerator LevelSunsetRNG
    {
        get => LevelManager.SunsetRNG;
        set => LevelManager.SunsetRNG = value;
    }

    public uint RandomSunsetLevelCount
    {
        get => LevelManager.RandomSunsetCount;
        set => LevelManager.RandomSunsetCount = value;
    }

    public List<MutableTuple<string, string, bool>> LevelSunsetData
    {
        get => LevelManager.GetSunsetData(LoadBackupScript().Levels);
        set => LevelManager.SetSunsetData(value);
    }
}