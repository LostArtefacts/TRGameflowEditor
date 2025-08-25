using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using TRGE.Core.Item;
using TRGE.Core.Item.Enums;
using TRGE.Core.Script;

namespace TRGE.Core;

public class TRXScript : AbstractTRScript, IStringSplitScript
{
    private static readonly JsonSerializer _mainSerializer = new()
    {
        NullValueHandling = NullValueHandling.Ignore,
        ContractResolver = new BaseFirstContractResolver
        {
            NamingStrategy = new SnakeCaseNamingStrategy()
        }
    };

    public const uint Version = 1;

    #region Gameflow
    // Only properties we're interested in
    public string MainMenuPicture { get; set; }    
    public string[] Injections { get; set; }
    public bool EnableTR2ItemDrops { get; set; }
    public bool ConvertDroppedGuns { get; set; }
    public bool EnableKillerPushblocks { get; set; }

    private TRXFrontEnd _frontEnd;
    private TRXScriptedLevel _atiCurrent;
    public override AbstractTRFrontEnd FrontEnd => _frontEnd;
    public override AbstractTRScriptedLevel AssaultLevel { get; set; }
    public override List<AbstractTRScriptedLevel> Levels { get; set; } = [];

    public override ushort TitleSoundID
    {
        get => _frontEnd.TrackID;
        set => _frontEnd.TrackID = value;
    }

    public bool DemosEnabled
    {
        get => !EnforcedConfig.TryGetValue("enable_demo", out object val) || bool.Parse(val.ToString());
        set => SetEnforcedConfig("enable_demo", value, true);
    }

    public int StartLaraHitpoints
    {
        get => EnforcedConfig.TryGetValue("start_lara_hitpoints", out object val) ? int.Parse(val.ToString()) : 1000;
        set => SetEnforcedConfig("start_lara_hitpoints", value, 1000);
    }

    public bool DisableMedpacks
    {
        get => !EnforcedConfig.TryGetValue("disable_medpacks", out object val) || bool.Parse(val.ToString());
        set => SetEnforcedConfig("disable_medpacks", value, true);
    }

    public bool DisableHealingBetweenLevels
    {
        get => !EnforcedConfig.TryGetValue("disable_healing_between_levels", out object val) || bool.Parse(val.ToString());
        set => SetEnforcedConfig("disable_healing_between_levels", value, true);
    }

    private void SetEnforcedConfig(string name, object value, object defaultValue)
    {
        if (value == defaultValue)
        {
            EnforcedConfig.Remove(name);
        }
        else
        {
            EnforcedConfig[name] = value;
        }
    }

    public void EnforceConfig(string name, object value, bool hide = false)
    {
        EnforcedConfig[name] = value;
        if (hide)
        {
            HiddenConfig.Add(name);
        }
    }
    #endregion

    public JObject GameflowData { get; internal set; }
    public Dictionary<string, object> EnforcedConfig { get; internal set; } = [];
    public List<string> HiddenConfig { get; internal set; } = [];
    public string LanguageName { get; set; }
    public Dictionary<string, string> BaseStrings { get; set; } = [];
    public Dictionary<string, string> GameStrings { get; set; } = [];
    public Dictionary<int, TRXObjectText> ObjectStrings { get; set; } = [];

    public override void ReadScriptJson(string json)
    {
        GameflowData = JObject.Parse(json);
        CalculateEdition();

        MainMenuPicture = ReadString(nameof(MainMenuPicture), GameflowData);
        Injections = ReadNullableArray<string>(nameof(Injections), GameflowData);
        ConvertDroppedGuns = ReadBool(nameof(ConvertDroppedGuns), GameflowData, false);
        EnableTR2ItemDrops = ReadBool(nameof(EnableTR2ItemDrops), GameflowData, false);
        EnableKillerPushblocks = ReadBool(nameof(EnableKillerPushblocks), GameflowData, false);
        EnforcedConfig = ReadDictionary<string, object>(nameof(EnforcedConfig), GameflowData);
        HiddenConfig = ReadArray<string>(nameof(HiddenConfig), GameflowData)?.ToList();
        BaseStrings = ReadDictionary<string, string>(nameof(BaseStrings), GameflowData);

        AddAdditionalBackupFile(MainMenuPicture);
        AddAdditionalBackupFile("cfg/base_strings.json5");
        if (Edition.Version == TRVersion.TR1)
        {
            AddAdditionalBackupFile("cfg/tr1/strings.json5");
            AddAdditionalBackupFile("cfg/tr1-ub/strings.json5", "strings_ub.json5");
        }
        else if (Edition.Version == TRVersion.TR2)
        {
            AddAdditionalBackupFile("cfg/tr2/strings.json5");
            AddAdditionalBackupFile("cfg/tr2-gm/strings.json5", "strings_gm.json5");
        }

        Levels.Clear();
        JArray levels = JArray.Parse(ReadString(nameof(Levels), GameflowData));

        foreach (JToken levelToken in levels)
        {
            var data = levelToken as JObject;
            var level = new TRXScriptedLevel(Edition.Version);

            level.Type = ReadEnum(nameof(level.Type), data, LevelType.Normal);
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

                case LevelType.Current:
                    _atiCurrent = level;
                    break;

                case LevelType.Dummy:
                    continue;
            }

            ReadLevel(level, data);
        }

        AmendGold();
        Levels[^1].IsFinalLevel = true;

        if (GameflowData.ContainsKey("title"))
        {
            var title = new TRXScriptedLevel(Edition.Version);
            _frontEnd = new()
            {
                TitleLevel = title,
            };
            ReadLevel(title, JObject.Parse(ReadString("title", GameflowData)));
        }

        if (GameflowData.ContainsKey("cutscenes"))
        {
            var cutscenes = JArray.Parse(ReadString("cutscenes", GameflowData));
            for (int i = 0; i < cutscenes.Count; i++)
            {
                var parentLevel = Levels.Cast<TRXScriptedLevel>()
                    .First(l => l.Sequences.Any(s => s is PlayCutsceneSequence p && p.CutsceneId == i));
                if (parentLevel is not null)
                {
                    var cutscene = new TRXScriptedLevel(Edition.Version);
                    parentLevel.CutSceneLevel = cutscene;
                    ReadLevel(cutscene, cutscenes[i] as JObject);
                }
            }
        }

        if (GameflowData.ContainsKey("demos"))
        {
            var demos = JArray.Parse(ReadString("demos", GameflowData));
            foreach (JToken demoToken in demos)
            {
                var path = ReadString("Path", demoToken as JObject);
                if (Levels.Find(l => l.LevelFile == path) is TRXScriptedLevel level)
                {
                    level.Demo = true;
                }
            }
        }
    }

    private void AmendGold()
    {
        if (Edition.Version != TRVersion.TR2)
        {
            return;
        }

        var levels = Levels.Cast<TRXScriptedLevel>();
        var bonus = levels.FirstOrDefault(l => l.Type == LevelType.Bonus);
        if (bonus == null)
        {
            return;
        }

        var prev = levels.FirstOrDefault(l => l.Sequence == bonus.Sequence - 1);
        if (prev == null)
        {
            return;
        }

        bonus.Type = LevelType.Normal;
        var prevLoopIdx = prev.Sequences.FindIndex(s => s.Type == LevelSequenceType.Loop_Game);
        var bonusLoopIdx = bonus.Sequences.FindIndex(s => s.Type == LevelSequenceType.Loop_Game);
        if (prevLoopIdx == -1 || bonusLoopIdx == -1)
        {
            return;
        }

        var a = new List<BaseLevelSequence>();
        var b = new List<BaseLevelSequence>();
        for (int i = prev.Sequences.Count - 1; i > prevLoopIdx; i--)
        {
            b.Add(prev.Sequences[i]);
            prev.Sequences.RemoveAt(i);
        }
        for (int i =  bonus.Sequences.Count - 1; i > bonusLoopIdx; i--)
        {
            a.Add(bonus.Sequences[i]);
            bonus.Sequences.RemoveAt(i);
        }

        a.Reverse();
        b.Reverse();
        prev.Sequences.AddRange(a);
        bonus.Sequences.AddRange(b);
    }

    private void ReadLevel(TRXScriptedLevel level, JObject levelData)
    {
        level.LevelFile = ReadString("Path", levelData);
        level.MusicTrack = ReadInt(nameof(level.MusicTrack), levelData);
        level.Injections = ReadNullableArray<string>(nameof(level.Injections), levelData);
        level.InheritInjections = ReadNullableBool(nameof(level.InheritInjections), levelData);
        level.Demo = ReadNullableBool(nameof(level.Demo), levelData);
        level.FogStart = ReadNullableDouble(nameof(level.FogStart), levelData);
        level.FogEnd = ReadNullableDouble(nameof(level.FogEnd), levelData);
        level.UnobtainableKills = ReadNullableInt(nameof(level.UnobtainableKills), levelData);
        level.UnobtainablePickups = ReadNullableInt(nameof(level.UnobtainablePickups), levelData);
        if (ReadString(nameof(level.LaraType), levelData, null) is string type)
        {
            level.LaraType = (int)TRXNaming.GetTR1Type(type);
        }

        level.ItemDrops = [];
        string dropsKey = nameof(level.ItemDrops).ToLowerSnake();
        if (levelData.ContainsKey(dropsKey))
        {
            JArray drops = JArray.Parse(ReadString(dropsKey, levelData));
            foreach (JToken dropToken in drops)
            {
                JObject dropData = dropToken as JObject;
                TR1ItemDrop drop = new();
                drop.EnemyNum = ReadInt(nameof(drop.EnemyNum), dropData);
                drop.ObjectIds = [.. ReadArray<int>(nameof(drop.ObjectIds), dropData)];
                level.ItemDrops.Add(drop);
            }
        }

        level.Sequences = [];
        JArray sequences = JArray.Parse(ReadString(nameof(level.Sequence), levelData));
        foreach (JToken levelSequence in sequences)
        {
            JObject sequenceData = levelSequence as JObject;
            BaseLevelSequence sequence;
            LevelSequenceType sequenceType = ReadEnum(nameof(sequence.Type), sequenceData, LevelSequenceType.Unknown);
            sequence = sequenceType switch
            {
                LevelSequenceType.Play_FMV => new FMVLevelSequence
                {
                    FmvId = ReadInt("FmvId", sequenceData),
                },
                LevelSequenceType.Display_Picture or LevelSequenceType.Loading_Screen => new DisplayPictureSequence
                {
                    Path = ReadString(nameof(DisplayPictureSequence.Path), sequenceData),
                    DisplayTime = ReadNullableDouble(nameof(DisplayPictureSequence.DisplayTime), sequenceData),
                    FadeInTime = ReadNullableDouble(nameof(DisplayPictureSequence.FadeInTime), sequenceData),
                    FadeOutTime = ReadNullableDouble(nameof(DisplayPictureSequence.FadeOutTime), sequenceData),
                    Legal = ReadNullableBool(nameof(DisplayPictureSequence.Legal), sequenceData),
                },
                LevelSequenceType.Total_Stats => new TotalStatsLevelSequence
                {
                    BackgroundPath = ReadString("BackgroundPath", sequenceData),
                },
                LevelSequenceType.Play_Cutscene => new PlayCutsceneSequence
                {
                    CutsceneId = ReadInt("CutsceneId", sequenceData),
                },
                LevelSequenceType.Set_Cutscene_Pos => new SetCutscenePosSequence
                {
                    X = ReadNullableInt("X", sequenceData),
                    Y = ReadNullableInt("Y", sequenceData),
                    Z = ReadNullableInt("Z", sequenceData),
                },
                LevelSequenceType.Set_Cutscene_Angle => new SetCutsceneAngleSequence
                {
                    Value = ReadNullableInt("Value", sequenceData)
                        ?? ReadNullableInt("Angle", sequenceData) ?? 0,
                },
                LevelSequenceType.Give_Item or LevelSequenceType.Add_Secret_Reward => new GiveItemLevelSequence
                {
                    ObjectId = ReadItem("ObjectId", sequenceData),
                    Quantity = ReadNullableInt("Quantity", sequenceData) ?? 1,
                },
                LevelSequenceType.Play_Music => new PlayMusicLevelSequence
                {
                    MusicTrack = ReadInt("MusicTrack", sequenceData),
                },
                LevelSequenceType.Mesh_Swap => new MeshSwapLevelSequence
                {
                    Object1ID = ReadItem("Object1Id", sequenceData),
                    Object2ID = ReadItem("Object2Id", sequenceData),
                    MeshID = ReadInt("MeshId", sequenceData)
                },
                LevelSequenceType.Setup_Bacon_Lara => new SetupBaconLaraSequence
                {
                    AnchorRoom = ReadInt("AnchorRoom", sequenceData)
                },
                LevelSequenceType.Set_Lara_Start_Anim => new SetStartAnimSequence
                {
                    Anim = ReadInt("Anim", sequenceData),
                },
                LevelSequenceType.Disable_Floor => new DisableFloorSequence
                {
                    Height = ReadInt("Height", sequenceData),
                },
                _ => new BaseLevelSequence(),
            };

            if (sequenceType != LevelSequenceType.Unknown)
            {
                sequence.Type = sequenceType;
                level.Sequences.Add(sequence);
            }
        }

        level.SetDefaults();
    }

    public void ReadStrings(string directoryName)
    {
        var data = JObject.Parse(File.ReadAllText(Path.Combine(directoryName, "base_strings.json5")));
        LanguageName = ReadString(nameof(LanguageName), data);
        BaseStrings = ReadDictionary<string, string>("GameStrings", data);

        bool gold = Levels.Count < 6; // Hmm
        string goldSuffix = Edition.Version == TRVersion.TR1 ? "_ub" : "_gm";
        data = JObject.Parse(File.ReadAllText(Path.Combine(directoryName, $"strings{(gold ? goldSuffix : string.Empty)}.json5")));
        JArray levels = JArray.Parse(ReadString(nameof(Levels), data));
        if (AssaultLevel == null && Edition.Version == TRVersion.TR1)
        {
            levels.Insert(0, null);
        }

        int levelID = 0;
        foreach (JToken levelToken in levels)
        {
            if (levelToken is not JObject levelData)
            {
                ++levelID;
                continue;
            }

            string levelName = ReadString("title", levelData);
            if (levelName == "Dummy")
            {
                continue;
            }

            TRXScriptedLevel level = null;
            if (levelID == 0)
            {
                level = AssaultLevel as TRXScriptedLevel;
            }
            else if (levelID - 1 < Levels.Count)
            {
                level = Levels[levelID - 1] as TRXScriptedLevel;
            }
            else if (levelID - 1 == levels.Count)
            {
                level = _frontEnd.TitleLevel;
            }
            else if (levelID == levels.Count)
            {
                level = _atiCurrent;
            }

            ++levelID;
            if (level == null)
            {
                continue;
            }

            level.Name = levelName;
            level.ObjectText = ReadObjectText("objects", levelData);

            for (int i = 1; i <= 4; i++)
            {
                level.Keys.Add(string.Empty);
                level.Puzzles.Add(string.Empty);
                if (i < 3)
                {
                    level.Pickups.Add(string.Empty);
                }
            }

            var keyBase = Edition.Version == TRVersion.TR1 ? (int)TR1Items.Key1_S_P : (int)TR2Items.Key1_S_P;
            var puzBase = Edition.Version == TRVersion.TR1 ? (int)TR1Items.Puzzle1_S_P : (int)TR2Items.Puzzle1_S_P;
            var picBase = Edition.Version == TRVersion.TR1 ? (int)TR1Items.Quest1_S_P : (int)TR2Items.Quest1_S_P;
            for (int i = 0; i < 4; i++)
            {
                if (level.ObjectText.TryGetValue(keyBase + i, out var text))
                {
                    level.Keys[i] = text.Name;
                }
            }
            for (int i = 0; i < 4; i++)
            {
                if (level.ObjectText.TryGetValue(puzBase + i, out var text))
                {
                    level.Puzzles[i] = text.Name;
                }
            }
            for (int i = 0; i < 2; i++)
            {
                if (level.ObjectText.TryGetValue(picBase + i, out var text))
                {
                    level.Pickups[i] = text.Name;
                }
            }
        }

        ObjectStrings = ReadObjectText("objects", data);
        GameStrings = ReadDictionary<string, string>("GameStrings", data);

        if (data.ContainsKey("cutscenes"))
        {
            var cutscenes = JArray.Parse(ReadString("cutscenes", data));
            for (int i = 0; i < cutscenes.Count; i++)
            {
                var cutLevel = Levels.Cast<TRXScriptedLevel>()
                    .Where(l => l.Sequences.Any(s => s is PlayCutsceneSequence p && p.CutsceneId == i))
                    .Select(l => l.CutSceneLevel).First();
                if (cutLevel is not null)
                {
                    cutLevel.Name = ReadString("title", cutscenes[i] as JObject);
                }
            }
        }
    }

    private Dictionary<int, TRXObjectText> ReadObjectText(string key, JObject data)
    {
        var objData = ReadDictionary<string, Dictionary<string, object>>(key, data);
        var objText = new Dictionary<int, TRXObjectText>();
        foreach (var (k, v) in objData)
        {
            var type = TRXNaming.GetType(k, Edition.Version);
            if (type == -1)
            {
                continue;
            }

            objText[type] = new();
            if (v.TryGetValue("names", out var names) && names is JArray arr && arr.Count > 0)
            {
                objText[type].Name = arr[0].ToString();
            }
            if (v.TryGetValue("name", out var name))
            {
                objText[type].Name = name.ToString();
            }
            if (v.TryGetValue("description", out var desc))
            {
                objText[type].Description = desc.ToString();
            }
        }
        return objText;
    }

    private static string ReadString(string key, JObject data)
    {
        return ReadString(key, data, null)
            ?? throw new ArgumentException("Invalid/missing value for " + key);
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

    private int ReadItem(string key, JObject data)
    {
        string result = ReadString(key, data);
        if (int.TryParse(result, out int value))
        {
            return value;
        }
        return TRXNaming.GetType(result, Edition.Version);
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
        return d.Length == 0 ? [] : JsonConvert.DeserializeObject<Dictionary<K, V>>(d);
    }

    protected override void CalculateEdition()
    {
        var guessData = ReadString("savegame_fmt_bson", GameflowData, string.Empty).ToLower();
        if (guessData.Contains("_tr1_") || guessData.Contains("_trub_"))
        {
            Edition = TREdition.TR1PC.Clone();
        }
        else if (guessData.Contains("_tr2_") || guessData.Contains("_trgm_"))
        {
            Edition = TREdition.TR2PC.Clone();
        }
        else
        {
            throw new UnsupportedScriptException();
        }

        GameflowData["savegame_fmt_bson"] = guessData;
    }

    protected override void Stamp()
    {
        BaseStrings["HEADING_INVENTORY"] = ApplyStamp(BaseStrings["HEADING_INVENTORY"]);
        int type = Edition.Version == TRVersion.TR1 ? (int)TR1Items.PassportOpen_M_H : (int)TR2Items.PassportOpen_M_H;
        ObjectStrings[type].Name = ApplyStamp(ObjectStrings[type].Name);
    }

    public override string SerialiseScriptToJson()
    {
        JObject data = [];

        Write(nameof(MainMenuPicture), MainMenuPicture, data);
        if (Edition.Version == TRVersion.TR1)
        {
            Write(nameof(ConvertDroppedGuns), ConvertDroppedGuns, data);
            Write(nameof(EnableTR2ItemDrops), EnableTR2ItemDrops, data);
        }
        Write(nameof(EnableKillerPushblocks), EnableKillerPushblocks, data);

        if (Injections != null)
        {
            Write(nameof(Injections), Injections, data);
        }

        {
            var title = SerializeLevel((FrontEnd as TRXFrontEnd).TitleLevel);
            Write(nameof(title), title, data);
        }

        WriteLevels(data);

        Write(nameof(EnforcedConfig), EnforcedConfig, data);
        Write(nameof(HiddenConfig), HiddenConfig, data);

        // Add anything else from the original data that we may not have captured.
        data.Merge(GameflowData);

        return JsonConvert.SerializeObject(data, Formatting.Indented);
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

    private void WriteLevels(JObject data)
    {
        JArray levels = [];
        JArray cutscenes = [];
        JArray demos = [];

        if (AssaultLevel is TRXScriptedLevel gym)
        {
            levels.Add(SerializeLevel(gym));
        }

        var enabledLevels = Levels.FindAll(l => l.Enabled).Cast<TRXScriptedLevel>();
        foreach (TRXScriptedLevel level in enabledLevels)
        {
            levels.Add(SerializeLevel(level));
            if (level.HasCutScene)
            {
                var cutSequence = level.Sequences.OfType<PlayCutsceneSequence>().First();
                cutSequence.CutsceneId = cutscenes.Count;
                cutscenes.Add(SerializeLevel(level.CutSceneLevel as TRXScriptedLevel));
            }            
            if (level.Demo ?? false)
            {
                demos.Add(SerializeLevel(level, true));
            }
        }

        if (Edition.Version == TRVersion.TR1)
        {
            levels.Add(SerializeLevel(_atiCurrent));
        }

        Write(nameof(levels), levels, data);
        Write(nameof(demos), demos, data);
        Write(nameof(cutscenes), cutscenes, data);
    }

    private JObject SerializeLevel(TRXScriptedLevel level, bool demoFormat = false)
    {
        JObject levelObj = [];

        Write("Path", level.LevelFile, levelObj);
        if (level.Type != LevelType.Normal && level.Type != LevelType.Dummy)
        {
            Write(nameof(level.Type), level.Type, levelObj);
        }
        Write(nameof(level.MusicTrack), level.MusicTrack == ushort.MaxValue ? -1 : level.MusicTrack, levelObj);

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
        if (level.FogStart.HasValue)
        {
            Write(nameof(level.FogStart), level.FogStart.Value, levelObj);
        }
        if (level.FogEnd.HasValue)
        {
            Write(nameof(level.FogEnd), level.FogEnd.Value, levelObj);
        }

        Write("Sequence", BuildSequences(level, demoFormat), levelObj);

        if (level.UnobtainableKills.HasValue)
        {
            Write(nameof(level.UnobtainableKills), level.UnobtainableKills.Value, levelObj);
        }
        if (level.UnobtainablePickups.HasValue)
        {
            Write(nameof(level.UnobtainablePickups), level.UnobtainablePickups.Value, levelObj);
        }
        if (level.LaraType.HasValue)
        {
            Write(nameof(level.LaraType), TRXNaming.GetName(level.LaraType.Value, Edition.Version), levelObj);
        }

        return levelObj;
    }

    private static JArray BuildItemDrops(TRXScriptedLevel level)
    {
        JArray items = [];

        foreach (TR1ItemDrop drop in level.ItemDrops)
        {
            items.Add(JObject.FromObject(drop, _mainSerializer));
        }

        return items;
    }

    private static JArray BuildSequences(TRXScriptedLevel level, bool demoFormat = false)
    {
        JArray sequences = [];

        foreach (var sequence in level.Sequences.Where(s => !demoFormat || s.Type != LevelSequenceType.Play_FMV))
        {
            JObject seq = JObject.FromObject(sequence, _mainSerializer);
            if (sequence is SetCutsceneAngleSequence ang)
            {
                seq["angle"] = ang.Value;
                seq["value"] = ang.Value;
            }
            Write(nameof(seq.Type), sequence.Type, seq);
            sequences.Add(seq);
        }

        return sequences;
    }

    public void WriteStrings(string directoryName)
    {
        {
            var data = new JObject();
            Write(nameof(LanguageName), LanguageName, data);
            Write(nameof(GameStrings), BaseStrings, data);

            File.WriteAllText(Path.Combine(directoryName, "base_strings.json5"),
                JsonConvert.SerializeObject(data, Formatting.Indented));
        }

        {
            var data = new JObject();
            JArray levels = [];
            JArray cutscenes = [];
            JArray demos = [];

            if (AssaultLevel is TRXScriptedLevel gym)
            {
                levels.Add(SerializeLevelText(gym));
            }

            var enabledLevels = Levels.FindAll(l => l.Enabled).Cast<TRXScriptedLevel>();
            foreach (TRXScriptedLevel level in enabledLevels)
            {
                levels.Add(SerializeLevelText(level));
                if (level.HasCutScene)
                {
                    cutscenes.Add(SerializeLevelText(level.CutSceneLevel as TRXScriptedLevel));
                }
                if (level.Demo ?? false)
                {
                    demos.Add(SerializeLevelText(level));
                }
            }

            if (Edition.Version == TRVersion.TR1)
            {
                levels.Add(SerializeLevelText(_atiCurrent));
            }

            Write(nameof(levels), levels, data);
            Write(nameof(demos), demos, data);
            Write(nameof(cutscenes), cutscenes, data);
            if (SerializeObjectText(ObjectStrings) is JObject objText)
            {
                Write("objects", objText, data);
            }
            Write(nameof(GameStrings), GameStrings, data);

            File.WriteAllText(Path.Combine(directoryName, "strings.json5"),
                JsonConvert.SerializeObject(data, Formatting.Indented));
        }
    }

    private JObject SerializeLevelText(TRXScriptedLevel level)
    {
        var data = new JObject();
        if (level.Name != null)
        {
            Write("title", level.Name, data);
        }
        if (SerializeObjectText(level.ObjectText, level) is JObject objText)
        {
            Write("objects", objText, data);
        }
        
        return data;
    }

    private JObject SerializeObjectText(Dictionary<int, TRXObjectText> text, TRXScriptedLevel level = null)
    {
        if (level == null && (text == null || text.Count == 0))
        {
            return null;
        }

        if (level != null)
        {
            var keyBase = Edition.Version == TRVersion.TR1 ? (int)TR1Items.Key1_S_P : (int)TR2Items.Key1_S_P;
            var puzBase = Edition.Version == TRVersion.TR1 ? (int)TR1Items.Puzzle1_S_P : (int)TR2Items.Puzzle1_S_P;
            var picBase = Edition.Version == TRVersion.TR1 ? (int)TR1Items.Quest1_S_P : (int)TR2Items.Quest1_S_P;
            level.ObjectText ??= [];
            for (int i = 0; i < 4 && i < level.Keys.Count; i++)
            {
                if (level.Keys[i].Length == 0)
                {
                    continue;
                }
                if (!level.ObjectText.TryGetValue(keyBase + i, out var objText))
                {
                    level.ObjectText[keyBase + i] = objText = new();
                }
                objText.Name = level.Keys[i];
            }
            for (int i = 0; i < 4 && i < level.Puzzles.Count; i++)
            {
                if (level.Puzzles[i].Length == 0)
                {
                    continue;
                }
                if (!level.ObjectText.TryGetValue(puzBase + i, out var objText))
                {
                    level.ObjectText[puzBase + i] = objText = new();
                }
                objText.Name = level.Puzzles[i];
            }
            for (int i = 0; i < 4 && i < level.Pickups.Count; i++)
            {
                if (level.Pickups[i].Length == 0)
                {
                    continue;
                }
                if (!level.ObjectText.TryGetValue(picBase + i, out var objText))
                {
                    level.ObjectText[picBase + i] = objText = new();
                }
                objText.Name = level.Pickups[i];
            }
            text = level.ObjectText;
        }

        if (text == null || text.Count == 0)
        {
            return null;
        }

        var data = new JObject();
        foreach (var (k, v) in text)
        {
            var objData = new JObject();
            if (v.Name != null)
            {
                Write(nameof(v.Name), v.Name, objData);
            }
            if (v.Description != null)
            {
                Write(nameof(v.Description), v.Description, objData);
            }
            Write(TRXNaming.GetName(k, Edition.Version), objData, data);
        }
        return data;
    }

    #region Obsolete
    public override string[] GameStrings1 { get; set; }
    public override string[] GameStrings2 { get; set; }
    public override byte Language { get; set; }
    public override ushort SecretSoundID { get; set; }
    #endregion
}
