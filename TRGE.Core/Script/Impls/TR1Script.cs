using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using TRGE.Core.Item.Enums;

namespace TRGE.Core;

public class TR1Script : AbstractTRScript
{
    private static readonly JsonSerializer _mainSerializer = new()
    {
        ContractResolver = new BaseFirstContractResolver
        {
            NamingStrategy = new SnakeCaseNamingStrategy()
        }
    };

    public const uint Version = 1;

    #region Gameflow
    public string MainMenuPicture { get; set; }
    public string SavegameFmtLegacy { get; set; }
    public string SavegameFmtBson { get; set; }
    public double DemoDelay { get; set; }
    public double[] WaterColor { get; set; }
    public double DrawDistanceFade { get; set; }
    public double DrawDistanceMax { get; set; }
    public string[] Injections { get; set; }
    public bool ConvertDroppedGuns { get; set; }
    public Dictionary<string, string> Strings { get; set; }

    private TR1FrontEnd _frontEnd;
    private TR1ScriptedLevel _atiCurrent;
    public override AbstractTRFrontEnd FrontEnd => _frontEnd;
    public override AbstractTRScriptedLevel AssaultLevel { get; set; }
    public override List<AbstractTRScriptedLevel> Levels { get; set; }


    public override ushort TitleSoundID
    {
        get => _frontEnd.TrackID;
        set => _frontEnd.TrackID = value;
    }
    #endregion

    #region Config
    public int StartLaraHitpoints { get; set; }
    public bool DisableHealingBetweenLevels { get; set; }
    public bool DisableMedpacks { get; set; }
    public bool DisableMagnums { get; set; }
    public bool DisableUzis { get; set; }
    public bool DisableShotgun { get; set; }
    public bool EnableDeathsCounter { get; set; }
    public bool EnableEnemyHealthbar { get; set; }
    public bool EnableEnhancedLook { get; set; }
    public bool EnableShotgunFlash { get; set; }
    public bool FixShotgunTargeting { get; set; }
    public bool EnableNumericKeys { get; set; }
    public bool EnableTr3Sidesteps { get; set; }
    public bool EnableCheats { get; set; }
    public bool EnableBraid { get; set; }
    public bool EnableDetailedStats { get; set; }
    public bool EnableCompassStats { get; set; }
    public bool EnableTotalStats { get; set; }
    public bool EnableTimerInInventory { get; set; }
    public bool EnableSmoothBars { get; set; }
    public bool EnableFadeEffects { get; set; }
    public TRMenuStyle MenuStyle { get; set; }
    public TRHealthbarMode HealthbarShowingMode { get; set; }
    public TRUILocation HealthbarLocation { get; set; }
    public TRUIColour HealthbarColor { get; set; }
    public TRAirbarMode AirbarShowingMode { get; set; }
    public TRUILocation AirbarLocation { get; set; }
    public TRUIColour AirbarColor { get; set; }
    public TRUILocation EnemyHealthbarLocation { get; set; }
    public TRUIColour EnemyHealthbarColor { get; set; }
    public bool FixTihocanSecretSound { get; set; }
    public bool FixPyramidSecretTrigger { get; set; }
    public bool FixFloorDataIssues { get; set; }
    public bool FixSecretsKillingMusic { get; set; }
    public bool FixSpeechesKillingMusic { get; set; }
    public bool FixDescendingGlitch { get; set; }
    public bool FixWallJumpGlitch { get; set; }
    public bool FixBridgeCollision { get; set; }
    public bool FixQwopGlitch { get; set; }
    public bool FixAlligatorAi { get; set; }
    public bool ChangePierreSpawn { get; set; }
    public int FovValue { get; set; }
    public bool FovVertical { get; set; }
    public bool EnableDemo { get; set; }
    public bool EnableFmv { get; set; }
    public bool EnableCine { get; set; }
    public bool EnableMusicInMenu { get; set; }
    public bool EnableMusicInInventory { get; set; }
    public bool DisableTrexCollision { get; set; }
    public int ResolutionWidth { get; set; }
    public int ResolutionHeight { get; set; }
    public bool EnableRoundShadow { get; set; }
    public bool Enable3dPickups { get; set; }
    public TRScreenshotFormat ScreenshotFormat { get; set; }
    public double AnisotropyFilter { get; set; }
    public bool WalkToItems { get; set; }
    public int MaximumSaveSlots { get; set; }
    public bool RevertToPistols { get; set; }
    public bool EnableEnhancedSaves { get; set; }
    public bool EnablePitchedSounds { get; set; }
    public bool EnableJumpTwists { get; set; }
    public bool EnabledInvertedLook { get; set; }
    public int CameraSpeed { get; set; }
    public bool EnableSwingCancel { get; set; }
    public bool EnableTr2Jumping { get; set; }
    public bool EnableGameModes { get; set; }
    public bool EnableSaveCrystals { get; set; }
    public bool FixBearAi { get; set; }
    public bool LoadCurrentMusic { get; set; }
    public bool LoadMusicTriggers { get; set; }
    public bool EnableUwRoll { get; set; }
    public bool EnableEidosLogo { get; set; }
    public bool EnableBuffering { get; set; }
    public bool EnableLeanJumping { get; set; }
    public bool EnableConsole { get; set; }

    #endregion

    public JObject GameflowData { get; internal set; }
    public JObject ConfigData { get; internal set; }

    public override void ReadScriptJson(string json)
    {
        CalculateEdition();
        GameflowData = JObject.Parse(json);

        MainMenuPicture = ReadString(nameof(MainMenuPicture), GameflowData);
        SavegameFmtLegacy = ReadString(nameof(SavegameFmtLegacy), GameflowData);
        SavegameFmtBson = ReadString(nameof(SavegameFmtBson), GameflowData);
        DemoDelay = ReadDouble(nameof(DemoDelay), GameflowData);
        WaterColor = ReadArray<double>(nameof(WaterColor), GameflowData);
        DrawDistanceFade = ReadDouble(nameof(DrawDistanceFade), GameflowData);
        DrawDistanceMax = ReadDouble(nameof(DrawDistanceMax), GameflowData);
        Injections = ReadNullableArray<string>(nameof(Injections), GameflowData);
        ConvertDroppedGuns = ReadBool(nameof(ConvertDroppedGuns), GameflowData, false);
        Strings = ReadDictionary<string, string>(nameof(Strings), GameflowData);

        _additionalFiles.Add(MainMenuPicture);

        Levels = new List<AbstractTRScriptedLevel>();
        JArray levels = JArray.Parse(ReadString(nameof(Levels), GameflowData));

        int levelID = 0;
        foreach (JToken levelToken in levels)
        {
            JObject levelData = levelToken as JObject;
            TR1ScriptedLevel level = new();

            level.Type = ReadEnum<LevelType>(nameof(level.Type), levelData);
            switch (level.Type)
            {
                case LevelType.Gym:
                    AssaultLevel = level;
                    break;

                case LevelType.Normal:
                case LevelType.Bonus:
                    Levels.Add(level);
                    level.Sequence = level.OriginalSequence = (ushort)Levels.Count; // Gym always first
                    break;
                
                case LevelType.Cutscene:
                    TR1ScriptedLevel parentLevel = FindParentLevel(levelID);
                    if (parentLevel != null)
                    {
                        parentLevel.CutSceneLevel = level;
                    }
                    break;

                case LevelType.Title:
                    _frontEnd = new TR1FrontEnd
                    {
                        TitleLevel = level
                    };
                    break;

                case LevelType.Current:
                    _atiCurrent = level;
                    break;
            }

            level.Name = ReadString("Title", levelData);
            level.LevelFile = ReadString("File", levelData);
            level.Music = ReadInt(nameof(level.Music), levelData);
            level.Injections = ReadNullableArray<string>(nameof(level.Injections), levelData);
            level.InheritInjections = ReadNullableBool(nameof(level.InheritInjections), levelData);
            level.Demo = ReadNullableBool(nameof(level.Demo), levelData);
            level.DrawDistanceFade = ReadNullableDouble(nameof(level.DrawDistanceFade), levelData);
            level.DrawDistanceMax = ReadNullableDouble(nameof(level.DrawDistanceMax), levelData);
            level.UnobtainableKills = ReadNullableInt(nameof(level.UnobtainableKills), levelData);
            level.UnobtainablePickups = ReadNullableInt(nameof(level.UnobtainablePickups), levelData);
            level.LaraType = (uint?)ReadNullableInt(nameof(level.LaraType), levelData);

            level.ItemDrops = new();
            string dropsKey = nameof(level.ItemDrops).ToLowerSnake();
            if (levelData.ContainsKey(dropsKey))
            {
                JArray drops = JArray.Parse(ReadString(dropsKey, levelData));
                foreach (JToken dropToken in drops)
                {
                    JObject dropData = dropToken as JObject;
                    TR1ItemDrop drop = new();
                    drop.EnemyNum = ReadInt(nameof(drop.EnemyNum), dropData);
                    drop.ObjectIds = ReadArray<int>(nameof(drop.ObjectIds), dropData)
                        .Select(i => (TR1Items)i).ToList();
                    level.ItemDrops.Add(drop);
                }
            }
            
            Dictionary<string, string> strings = ReadDictionary<string, string>("Strings", levelData);
            foreach (string key in strings.Keys)
            {
                if (key.StartsWith("key"))
                {
                    level.AddKey(strings[key]);
                }
                else if (key.StartsWith("pickup"))
                {
                    level.AddPickup(strings[key]);
                }
                else if (key.StartsWith("puzzle"))
                {
                    level.AddPuzzle(strings[key]);
                }
            }


            level.Sequences = new List<BaseLevelSequence>();
            JArray sequences = JArray.Parse(ReadString(nameof(level.Sequence), levelData));
            foreach (JToken levelSequence in sequences)
            {
                JObject sequenceData = levelSequence as JObject;
                BaseLevelSequence sequence;
                LevelSequenceType sequenceType = ReadEnum<LevelSequenceType>(nameof(sequence.Type), sequenceData);
                sequence = sequenceType switch
                {
                    LevelSequenceType.Play_FMV => new FMVLevelSequence
                    {
                        FmvPath = ReadString("FmvPath", sequenceData)
                    },
                    LevelSequenceType.Display_Picture => new DisplayPictureLevelSequence
                    {
                        PicturePath = ReadString("PicturePath", sequenceData),
                        DisplayTime = ReadDouble("DisplayTime", sequenceData)
                    },
                    LevelSequenceType.Total_Stats => new TotalStatsLevelSequence
                    {
                        PicturePath = ReadString("PicturePath", sequenceData)
                    },
                    LevelSequenceType.Level_Stats or LevelSequenceType.Exit_To_Level or LevelSequenceType.Exit_To_Cine => new LevelExitLevelSequence
                    {
                        LevelId = ReadInt("LevelId", sequenceData)
                    },
                    LevelSequenceType.Set_Cam_X or LevelSequenceType.Set_Cam_Y or LevelSequenceType.Set_Cam_Z or LevelSequenceType.Set_Cam_Angle => new SetCamLevelSequence
                    {
                        Value = ReadInt("Value", sequenceData)
                    },
                    LevelSequenceType.Give_Item => new GiveItemLevelSequence
                    {
                        ObjectId = (TR1Items)ReadInt("ObjectId", sequenceData),
                        Quantity = ReadInt("Quantity", sequenceData)
                    },
                    LevelSequenceType.Play_Synced_Audio => new PlaySyncedAudioLevelSequence
                    {
                        AudioId = ReadInt("AudioId", sequenceData)
                    },
                    LevelSequenceType.Mesh_Swap => new MeshSwapLevelSequence
                    {
                        Object1ID = ReadInt("Object1Id", sequenceData),
                        Object2ID = ReadInt("Object2Id", sequenceData),
                        MeshID = ReadInt("MeshId", sequenceData)
                    },
                    LevelSequenceType.Setup_Bacon_Lara => new SetupBaconLaraSequence
                    {
                        AnchorRoom = ReadInt("AnchorRoom", sequenceData)
                    },
                    _ => new BaseLevelSequence(),
                };
                sequence.Type = sequenceType;
                level.Sequences.Add(sequence);
            }

            level.SetDefaults();
            ++levelID;
        }

        Levels[^1].IsFinalLevel = true;
    }

    public override void ReadConfigJson(string json)
    {
        ConfigData = JObject.Parse(json);

        StartLaraHitpoints          = ReadInt(nameof(StartLaraHitpoints), ConfigData, 1000);
        DisableHealingBetweenLevels = ReadBool(nameof(DisableHealingBetweenLevels), ConfigData, false);
        DisableMedpacks             = ReadBool(nameof(DisableMedpacks), ConfigData, false);
        DisableMagnums              = ReadBool(nameof(DisableMagnums), ConfigData, false);
        DisableUzis                 = ReadBool(nameof(DisableUzis), ConfigData, false);
        DisableShotgun              = ReadBool(nameof(DisableShotgun), ConfigData, false);
        EnableDeathsCounter         = ReadBool(nameof(EnableDeathsCounter), ConfigData, true);
        EnableEnemyHealthbar        = ReadBool(nameof(EnableEnemyHealthbar), ConfigData, true);
        EnableEnhancedLook          = ReadBool(nameof(EnableEnhancedLook), ConfigData, true);
        EnableShotgunFlash          = ReadBool(nameof(EnableShotgunFlash), ConfigData, true);
        FixShotgunTargeting         = ReadBool(nameof(FixShotgunTargeting), ConfigData, true);
        EnableNumericKeys           = ReadBool(nameof(EnableNumericKeys), ConfigData, true);
        EnableTr3Sidesteps          = ReadBool(nameof(EnableTr3Sidesteps), ConfigData, true);
        EnableCheats                = ReadBool(nameof(EnableCheats), ConfigData, false);
        EnableBraid                 = ReadBool(nameof(EnableBraid), ConfigData, false);
        EnableDetailedStats         = ReadBool(nameof(EnableDetailedStats), ConfigData, true);
        EnableCompassStats          = ReadBool(nameof(EnableCompassStats), ConfigData, true);
        EnableTotalStats            = ReadBool(nameof(EnableTotalStats), ConfigData, true);
        EnableTimerInInventory      = ReadBool(nameof(EnableTimerInInventory), ConfigData, true);
        EnableSmoothBars            = ReadBool(nameof(EnableSmoothBars), ConfigData, true);
        EnableFadeEffects           = ReadBool(nameof(EnableFadeEffects), ConfigData, true);
        MenuStyle                   = ReadEnum(nameof(MenuStyle), ConfigData, TRMenuStyle.PC);
        HealthbarShowingMode        = ReadEnum(nameof(HealthbarShowingMode), ConfigData, TRHealthbarMode.FlashingOrDefault);
        HealthbarLocation           = ReadEnum(nameof(HealthbarLocation), ConfigData, TRUILocation.TopLeft);
        HealthbarColor              = ReadEnum(nameof(HealthbarColor), ConfigData, TRUIColour.Red);
        AirbarShowingMode           = ReadEnum(nameof(AirbarShowingMode), ConfigData, TRAirbarMode.Default);
        AirbarLocation              = ReadEnum(nameof(AirbarLocation), ConfigData, TRUILocation.TopRight);
        AirbarColor                 = ReadEnum(nameof(AirbarColor), ConfigData, TRUIColour.Blue);
        EnemyHealthbarLocation      = ReadEnum(nameof(EnemyHealthbarLocation), ConfigData, TRUILocation.BottomLeft);
        EnemyHealthbarColor         = ReadEnum(nameof(EnemyHealthbarColor), ConfigData, TRUIColour.Grey);
        FixTihocanSecretSound       = ReadBool(nameof(FixTihocanSecretSound), ConfigData, true);
        FixPyramidSecretTrigger     = ReadBool(nameof(FixPyramidSecretTrigger), ConfigData, true);
        FixFloorDataIssues          = ReadBool(nameof(FixFloorDataIssues), ConfigData, true);
        FixSecretsKillingMusic      = ReadBool(nameof(FixSecretsKillingMusic), ConfigData, true);
        FixSpeechesKillingMusic     = ReadBool(nameof(FixSpeechesKillingMusic), ConfigData, true);
        FixDescendingGlitch         = ReadBool(nameof(FixDescendingGlitch), ConfigData, false);
        FixWallJumpGlitch           = ReadBool(nameof(FixWallJumpGlitch), ConfigData, false);
        FixBridgeCollision          = ReadBool(nameof(FixBridgeCollision), ConfigData, true);
        FixQwopGlitch               = ReadBool(nameof(FixQwopGlitch), ConfigData, false);
        FixAlligatorAi              = ReadBool(nameof(FixAlligatorAi), ConfigData, true);
        ChangePierreSpawn           = ReadBool(nameof(ChangePierreSpawn), ConfigData, true);
        FovValue                    = ReadInt(nameof(FovValue), ConfigData, 65);
        FovVertical                 = ReadBool(nameof(FovVertical), ConfigData, true);
        EnableDemo                  = ReadBool(nameof(EnableDemo), ConfigData, true);
        EnableFmv                   = ReadBool(nameof(EnableFmv), ConfigData, true);
        EnableCine                  = ReadBool(nameof(EnableCine), ConfigData, true);
        EnableMusicInMenu           = ReadBool(nameof(EnableMusicInMenu), ConfigData, true);
        EnableMusicInInventory      = ReadBool(nameof(EnableMusicInInventory), ConfigData, true);
        DisableTrexCollision        = ReadBool(nameof(DisableTrexCollision), ConfigData, false);
        ResolutionWidth             = ReadInt(nameof(ResolutionWidth), ConfigData, -1);
        ResolutionHeight            = ReadInt(nameof(ResolutionHeight), ConfigData, -1);
        EnableRoundShadow           = ReadBool(nameof(EnableRoundShadow), ConfigData, true);
        Enable3dPickups             = ReadBool(nameof(Enable3dPickups), ConfigData, true);
        ScreenshotFormat            = ReadEnum(nameof(ScreenshotFormat), ConfigData, TRScreenshotFormat.JPG);
        AnisotropyFilter            = ReadDouble(nameof(AnisotropyFilter), ConfigData, 16);
        WalkToItems                 = ReadBool(nameof(WalkToItems), ConfigData, false);
        MaximumSaveSlots            = ReadInt(nameof(MaximumSaveSlots), ConfigData, 25);
        RevertToPistols             = ReadBool(nameof(RevertToPistols), ConfigData, false);
        EnableEnhancedSaves         = ReadBool(nameof(EnableEnhancedSaves), ConfigData, true);
        EnablePitchedSounds         = ReadBool(nameof(EnablePitchedSounds), ConfigData, true);
        EnableJumpTwists            = ReadBool(nameof(EnableJumpTwists), ConfigData, true);
        EnabledInvertedLook         = ReadBool(nameof(EnabledInvertedLook), ConfigData, false);
        CameraSpeed                 = ReadInt(nameof(CameraSpeed), ConfigData, 5);
        EnableSwingCancel           = ReadBool(nameof(EnableSwingCancel), ConfigData, true);
        EnableTr2Jumping            = ReadBool(nameof(EnableTr2Jumping), ConfigData, false);

        EnableGameModes             = ReadBool(nameof(EnableGameModes), ConfigData, true);
        EnableSaveCrystals          = ReadBool(nameof(EnableSaveCrystals), ConfigData, false);
        LoadMusicTriggers           = ReadBool(nameof(LoadMusicTriggers), ConfigData, true);
        LoadCurrentMusic            = ReadBool(nameof(LoadCurrentMusic), ConfigData, true);
        FixBearAi                   = ReadBool(nameof(FixBearAi), ConfigData, true);
        EnableUwRoll                = ReadBool(nameof(EnableUwRoll), ConfigData, false);
        EnableEidosLogo             = ReadBool(nameof(EnableEidosLogo), ConfigData, true);
        EnableBuffering             = ReadBool(nameof(EnableBuffering), ConfigData, false);
        EnableLeanJumping           = ReadBool(nameof(EnableLeanJumping), ConfigData, false);
        EnableConsole               = ReadBool(nameof(EnableConsole), ConfigData, true);
    }

    private static string ReadString(string key, JObject data)
    {
        string result = ReadString(key, data, null) ?? throw new ArgumentException("Invalid/missing value for " + key);
        return result;
    }

    private static string ReadString(string key, JObject data, string defaultValue)
    {
        key = key.ToLowerSnake();
        if (!data.ContainsKey(key))
        {
            return defaultValue;
        }

        string result = data[key].ToString();
        data.Remove(key);
        return result;
    }

    private static int ReadInt(string key, JObject data)
    {
        int result = ReadInt(key, data, int.MinValue);
        if (result == int.MinValue)
        {
            throw new ArgumentException("Invalid/missing value for " + key);
        }
        return result;
    }

    private static int? ReadNullableInt(string key, JObject data)
    {
        int result = ReadInt(key, data, int.MinValue);
        if (result == int.MinValue)
        {
            return null;
        }
        return result;
    }

    private static int ReadInt(string key, JObject data, int defaultValue)
    {
        key = key.ToLowerSnake();
        if (!data.ContainsKey(key))
        {
            return defaultValue;
        }

        string result = data[key].ToString();
        data.Remove(key);

        return int.TryParse(result, out int value) ? value : defaultValue;
    }

    private static bool? ReadNullableBool(string key, JObject data)
    {
        key = key.ToLowerSnake();
        if (!data.ContainsKey(key))
        {
            return null;
        }

        return ReadBool(key, data, false);
    }

    private static bool ReadBool(string key, JObject data, bool defaultValue)
    {
        key = key.ToLowerSnake();
        if (!data.ContainsKey(key))
        {
            return defaultValue;
        }

        string result = data[key].ToString();
        data.Remove(key);

        return bool.TryParse(result, out bool value) ? value : defaultValue;
    }

    private static double ReadDouble(string key, JObject data)
    {
        double result = ReadDouble(key, data, double.NaN);
        if (double.IsNaN(result))
        {
            throw new ArgumentException("Invalid/missing value for " + key);
        }
        return result;
    }

    private static double? ReadNullableDouble(string key, JObject data)
    {
        double result = ReadDouble(key, data, double.MinValue);
        if (result == double.MinValue)
        {
            return null;
        }
        return result;
    }

    private static double ReadDouble(string key, JObject data, double defaultValue)
    {
        key = key.ToLowerSnake();
        if (!data.ContainsKey(key))
        {
            return defaultValue;
        }

        string result = data[key].ToString();
        data.Remove(key);

        return double.TryParse(result, out double value) ? value : defaultValue;
    }

    private static T ReadEnum<T>(string key, JObject data)
    {
        key = key.ToLowerSnake();
        if (!data.ContainsKey(key))
        {
            throw new ArgumentException("Invalid/missing value for " + key);
        }

        return ReadEnum(key, data, default(T));
    }

    private static T ReadEnum<T>(string key, JObject data, T defaultValue)
    {
        key = key.ToLowerSnake();
        if (!data.ContainsKey(key))
        {
            return defaultValue;
        }

        string result = data[key].ToString();
        data.Remove(key);

        result = result.Replace("-", string.Empty);
        return (T)Enum.Parse(typeof(T), result, true);
    }

    private static T[] ReadArray<T>(string key, JObject data)
    {
        string a = ReadString(key, data, string.Empty);
        return a.Length == 0 ? null : JsonConvert.DeserializeObject<T[]>(a);
    }

    private static T[] ReadNullableArray<T>(string key, JObject data)
    {
        string arr = ReadString(key, data, null);
        if (arr == null)
        {
            return null;
        }
        return arr.Length == 0 ? null : JsonConvert.DeserializeObject<T[]>(arr);
    }

    private static Dictionary<K, V> ReadDictionary<K, V>(string key, JObject data)
    {
        string d = ReadString(key, data, string.Empty);
        return d.Length == 0 ? null : JsonConvert.DeserializeObject<Dictionary<K, V>>(d);
    }

    private TR1ScriptedLevel FindParentLevel(int cutsceneLevelID)
    {
        foreach (AbstractTRScriptedLevel level in Levels)
        {
            TR1ScriptedLevel lvl = level as TR1ScriptedLevel;
            BaseLevelSequence sequence = lvl.Sequences.Find(s => s.Type == LevelSequenceType.Exit_To_Cine);
            if (sequence != null && (sequence as LevelExitLevelSequence).LevelId == cutsceneLevelID)
            {
                return lvl;
            }
        }

        return null;
    }

    protected override void CalculateEdition()
    {
        Edition = TREdition.TR1PC.Clone();
    }

    protected override void Stamp()
    {
        Strings["HEADING_INVENTORY"] = ApplyStamp(Strings["HEADING_INVENTORY"]);
        Strings["INV_ITEM_GAME"] = ApplyStamp(Strings["INV_ITEM_GAME"]);
    }

    public override string SerialiseScriptToJson()
    {
        JObject data = new();

        Write(nameof(MainMenuPicture), MainMenuPicture, data);
        Write(nameof(SavegameFmtLegacy), SavegameFmtLegacy, data);
        Write(nameof(SavegameFmtBson), SavegameFmtBson, data);
        Write(nameof(DemoDelay), DemoDelay, data);
        Write(nameof(WaterColor), WaterColor, data);
        Write(nameof(DrawDistanceFade), DrawDistanceFade, data);
        Write(nameof(DrawDistanceMax), DrawDistanceMax, data);
        Write(nameof(ConvertDroppedGuns), ConvertDroppedGuns, data);
        if (Injections != null)
        {
            Write(nameof(Injections), Injections, data);
        }

        Write(nameof(Levels), BuildLevels(), data);

        Write(nameof(Strings), Strings, data);

        // Add anything else from the original data that we may not have captured.
        data.Merge(GameflowData);

        return JsonConvert.SerializeObject(data, Formatting.Indented);
    }

    public override string SerialiseConfigToJson(string existingData)
    {
        JObject data = new();

        Write(nameof(StartLaraHitpoints), StartLaraHitpoints, data);
        Write(nameof(DisableHealingBetweenLevels), DisableHealingBetweenLevels, data);
        Write(nameof(DisableMedpacks), DisableMedpacks, data);
        Write(nameof(DisableMagnums), DisableMagnums, data);
        Write(nameof(DisableUzis), DisableUzis, data);
        Write(nameof(DisableShotgun), DisableShotgun, data);
        Write(nameof(EnableDeathsCounter), EnableDeathsCounter, data);
        Write(nameof(EnableEnemyHealthbar), EnableEnemyHealthbar, data);
        Write(nameof(EnableEnhancedLook), EnableEnhancedLook, data);
        Write(nameof(EnableShotgunFlash), EnableShotgunFlash, data);
        Write(nameof(FixShotgunTargeting), FixShotgunTargeting, data);
        Write(nameof(EnableNumericKeys), EnableNumericKeys, data);
        Write(nameof(EnableTr3Sidesteps), EnableTr3Sidesteps, data);
        Write(nameof(EnableCheats), EnableCheats, data);
        Write(nameof(EnableBraid), EnableBraid, data);
        Write(nameof(EnableDetailedStats), EnableDetailedStats, data);
        Write(nameof(EnableCompassStats), EnableCompassStats, data);
        Write(nameof(EnableTotalStats), EnableTotalStats, data);
        Write(nameof(EnableTimerInInventory), EnableTimerInInventory, data);
        Write(nameof(EnableSmoothBars), EnableSmoothBars, data);
        Write(nameof(EnableFadeEffects), EnableFadeEffects, data);
        WriteKebab(nameof(MenuStyle), MenuStyle, data);
        WriteKebab(nameof(HealthbarShowingMode), HealthbarShowingMode, data);
        WriteKebab(nameof(HealthbarLocation), HealthbarLocation, data);
        WriteKebab(nameof(HealthbarColor), HealthbarColor, data);
        WriteKebab(nameof(AirbarShowingMode), AirbarShowingMode, data);
        WriteKebab(nameof(AirbarLocation), AirbarLocation, data);
        WriteKebab(nameof(AirbarColor), AirbarColor, data);
        WriteKebab(nameof(EnemyHealthbarLocation), EnemyHealthbarLocation, data);
        WriteKebab(nameof(EnemyHealthbarColor), EnemyHealthbarColor, data);
        Write(nameof(FixTihocanSecretSound), FixTihocanSecretSound, data);
        Write(nameof(FixPyramidSecretTrigger), FixPyramidSecretTrigger, data);
        Write(nameof(FixFloorDataIssues), FixFloorDataIssues, data);
        Write(nameof(FixSecretsKillingMusic), FixSecretsKillingMusic, data);
        Write(nameof(FixSpeechesKillingMusic), FixSpeechesKillingMusic, data);
        Write(nameof(FixDescendingGlitch), FixDescendingGlitch, data);
        Write(nameof(FixWallJumpGlitch), FixWallJumpGlitch, data);
        Write(nameof(FixBridgeCollision), FixBridgeCollision, data);
        Write(nameof(FixQwopGlitch), FixQwopGlitch, data);
        Write(nameof(FixAlligatorAi), FixAlligatorAi, data);
        Write(nameof(ChangePierreSpawn), ChangePierreSpawn, data);
        Write(nameof(FovValue), FovValue, data);
        Write(nameof(FovVertical), FovVertical, data);
        Write(nameof(EnableDemo), EnableDemo, data);
        Write(nameof(EnableFmv), EnableFmv, data);
        Write(nameof(EnableCine), EnableCine, data);
        Write(nameof(EnableMusicInMenu), EnableMusicInMenu, data);
        Write(nameof(EnableMusicInInventory), EnableMusicInInventory, data);
        Write(nameof(DisableTrexCollision), DisableTrexCollision, data);
        Write(nameof(ResolutionWidth), ResolutionWidth, data);
        Write(nameof(ResolutionHeight), ResolutionHeight, data);
        Write(nameof(EnableRoundShadow), EnableRoundShadow, data);
        Write(nameof(Enable3dPickups), Enable3dPickups, data);
        WriteKebab(nameof(ScreenshotFormat), ScreenshotFormat, data);
        Write(nameof(AnisotropyFilter), AnisotropyFilter, data);
        Write(nameof(WalkToItems), WalkToItems, data);
        Write(nameof(MaximumSaveSlots), MaximumSaveSlots, data);
        Write(nameof(RevertToPistols), RevertToPistols, data);
        Write(nameof(EnableEnhancedSaves), EnableEnhancedSaves, data);
        Write(nameof(EnablePitchedSounds), EnablePitchedSounds, data);
        Write(nameof(EnableJumpTwists), EnableJumpTwists, data);
        Write(nameof(EnabledInvertedLook), EnabledInvertedLook, data);
        Write(nameof(CameraSpeed), CameraSpeed, data);
        Write(nameof(EnableSwingCancel), EnableSwingCancel, data);
        Write(nameof(EnableTr2Jumping), EnableTr2Jumping, data);
        Write(nameof(EnableGameModes), EnableGameModes, data);
        Write(nameof(EnableSaveCrystals), EnableSaveCrystals, data);
        Write(nameof(FixBearAi), FixBearAi, data);
        Write(nameof(LoadCurrentMusic), LoadCurrentMusic, data);
        Write(nameof(LoadMusicTriggers), LoadMusicTriggers, data);
        Write(nameof(EnableUwRoll), EnableUwRoll, data);
        Write(nameof(EnableEidosLogo), EnableEidosLogo, data);
        Write(nameof(EnableBuffering), EnableBuffering, data);
        Write(nameof(EnableLeanJumping), EnableLeanJumping, data);
        Write(nameof(EnableConsole), EnableConsole, data);

        // The existing data will have been re-read at this stage (T1M stores runtime config
        // in the same file so this may well have changed between saves in TRGE). Re-scan this
        // data, but in any case fall back to what was read when initially loaded.
        if (existingData != null)
        {
            JObject existingExternalData = GetExistingExternalData(data, existingData);
            if (existingExternalData != null)
            {
                ConfigData = existingExternalData;
            }
        }

        data.Merge(ConfigData);

        return JsonConvert.SerializeObject(data, Formatting.Indented);
    }

    private static JObject GetExistingExternalData(JObject convertedData, string externalJsonData)
    {
        try
        {
            JObject externalData = JObject.Parse(externalJsonData);
            foreach (KeyValuePair<string, JToken> pair in convertedData)
            {
                if (externalData.ContainsKey(pair.Key))
                {
                    externalData.Remove(pair.Key);
                }
            }
            return externalData;
        }
        catch
        {
            return null;
        }
    }

    private static void Write<T>(string key, T value, JObject data)
    {
        key = key.ToLowerSnake();
        if (value is Enum)
        {
            data[key] = JToken.FromObject(value.ToString().ToLower());
        }
        else
        {
            data[key] = JToken.FromObject(value);
        }
    }

    private static void WriteKebab<T>(string key, T value, JObject data)
    {
        key = key.ToLowerSnake();
        data[key] = JToken.FromObject(value.ToString().ToLowerKebab());
    }

    private JArray BuildLevels()
    {
        JArray levels = new()
        {
            SerializeLevel(AssaultLevel as TR1ScriptedLevel)
        };

        List<AbstractTRScriptedLevel> enabledLevels = Levels.FindAll(l => l.Enabled);
        List<AbstractTRScriptedLevel> cutsceneLevels = new();

        int cutsceneLevelID = enabledLevels.Count;
        foreach (TR1ScriptedLevel level in enabledLevels.Cast<TR1ScriptedLevel>())
        {
            if (level.HasCutScene)
            {
                LevelExitLevelSequence cutSequence = level.Sequences.Find(s => s.Type == LevelSequenceType.Exit_To_Cine) as LevelExitLevelSequence;
                cutSequence.LevelId = ++cutsceneLevelID;
                cutsceneLevels.Add(level.CutSceneLevel);
            }
            levels.Add(SerializeLevel(level));
        }

        foreach (TR1ScriptedLevel level in cutsceneLevels.Cast<TR1ScriptedLevel>())
        {
            levels.Add(SerializeLevel(level));
        }

        levels.Add(SerializeLevel((FrontEnd as TR1FrontEnd).TitleLevel));
        levels.Add(SerializeLevel(_atiCurrent));

        return levels;
    }

    private JObject SerializeLevel(TR1ScriptedLevel level)
    {
        JObject levelObj = new();

        Write("Title", level.Name, levelObj);
        Write("File", level.LevelFile, levelObj);
        Write(nameof(level.Type), level.Type, levelObj);
        Write(nameof(level.Music), level.Music, levelObj);

        if (level.Injections != null)
        {
            Write(nameof(level.Injections), level.Injections, levelObj);
        }
        if (level.InheritInjections.HasValue)
        {
            Write(nameof(level.InheritInjections), level.InheritInjections.Value, levelObj);
        }
        if (level.ItemDrops?.Count > 0)
        {
            Write(nameof(level.ItemDrops), BuildItemDrops(level), levelObj);
        }
        if (level.WaterColor != null)
        {
            Write(nameof(level.WaterColor), level.WaterColor, levelObj);
        }
        if (level.DrawDistanceFade.HasValue)
        {
            Write(nameof(level.DrawDistanceFade), level.DrawDistanceFade.Value, levelObj);
        }
        if (level.DrawDistanceMax.HasValue)
        {
            Write(nameof(level.DrawDistanceMax), level.DrawDistanceMax.Value, levelObj);
        }

        Write("Sequence", BuildSequences(level), levelObj);

        JObject strings = new();
        for (int i = 0; i < level.Keys.Count; i++)
        {
            Write("key" + (i + 1), level.Keys[i], strings);
        }
        for (int i = 0; i < level.Pickups.Count; i++)
        {
            Write("pickup" + (i + 1), level.Pickups[i], strings);
        }
        for (int i = 0; i < level.Puzzles.Count; i++)
        {
            Write("puzzle" + (i + 1), level.Puzzles[i], strings);
        }

        Write("Strings", strings, levelObj);

        if (level.UnobtainableKills.HasValue)
        {
            Write(nameof(level.UnobtainableKills), level.UnobtainableKills.Value, levelObj);
        }
        if (level.UnobtainablePickups.HasValue)
        {
            Write(nameof(level.UnobtainablePickups), level.UnobtainablePickups.Value, levelObj);
        }
        if (level.Demo.HasValue)
        {
            Write(nameof(level.Demo), level.Demo.Value, levelObj);
        }
        if (level.LaraType.HasValue)
        {
            Write(nameof(level.LaraType), level.LaraType.Value, levelObj);
        }

        return levelObj;
    }

    private static JArray BuildItemDrops(TR1ScriptedLevel level)
    {
        JArray items = new();

        foreach (TR1ItemDrop drop in level.ItemDrops)
        {
            items.Add(JObject.FromObject(drop, _mainSerializer));
        }

        return items;
    }

    private JArray BuildSequences(TR1ScriptedLevel level)
    {
        JArray sequences = new();

        foreach (BaseLevelSequence sequence in level.Sequences)
        {
            if (sequence.Type == LevelSequenceType.Fix_Pyramid_Secret
                && Edition.ExeVersion >= new Version(2, 14))
            {
                // Legacy sequence type, which will prevent T1M launching if present in 2.14+
                continue;
            }

            JObject seq = JObject.FromObject(sequence, _mainSerializer);
            Write(nameof(seq.Type), sequence.Type, seq);
            sequences.Add(seq);
        }

        return sequences;
    }

    #region Obsolete
    public override string[] GameStrings1 { get; set; }
    public override string[] GameStrings2 { get; set; }
    public override byte Language { get; set; }
    public override ushort SecretSoundID { get; set; }
    #endregion
}