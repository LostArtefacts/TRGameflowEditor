using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using TRGE.Core.Item.Enums;

namespace TRGE.Core
{
    public class TR1Script : AbstractTRScript
    {
        private static readonly JsonSerializer _mainSerializer = new JsonSerializer
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
        public bool EnableGameModes { get; set; }
        public bool EnableSaveCrystals { get; set; }
        public double DemoDelay { get; set; }
        public double[] WaterColor { get; set; }
        public double DrawDistanceFade { get; set; }
        public double DrawDistanceMax { get; set; }
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
        public bool FixSecretsKillingMusic { get; set; }
        public bool FixDescendingGlitch { get; set; }
        public bool FixWallJumpGlitch { get; set; }
        public bool FixBridgeCollision { get; set; }
        public bool FixQwopGlitch { get; set; }
        public bool FixAlligatorAi { get; set; }
        public bool ChangePierreSpawn { get; set; }
        public int FovValue { get; set; }
        public bool FovVertical { get; set; }
        public bool DisableDemo { get; set; }
        public bool DisableFmv { get; set; }
        public bool DisableCine { get; set; }
        public bool DisableMusicInMenu { get; set; }
        public bool DisableMusicInInventory { get; set; }
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
            EnableGameModes = ReadBool(nameof(EnableGameModes), GameflowData).Value;
            EnableSaveCrystals = ReadBool(nameof(EnableSaveCrystals), GameflowData).Value;
            DemoDelay = ReadDouble(nameof(DemoDelay), GameflowData).Value;
            WaterColor = ReadArray<double>(nameof(WaterColor), GameflowData);
            DrawDistanceFade = ReadDouble(nameof(DrawDistanceFade), GameflowData).Value;
            DrawDistanceMax = ReadDouble(nameof(DrawDistanceMax), GameflowData).Value;
            Strings = ReadDictionary<string, string>(nameof(Strings), GameflowData);

            Levels = new List<AbstractTRScriptedLevel>();
            JArray levels = JArray.Parse(ReadString(nameof(Levels), GameflowData));

            int levelID = 0;
            foreach (JToken levelToken in levels)
            {
                JObject levelData = levelToken as JObject;
                TR1ScriptedLevel level = new TR1ScriptedLevel();

                level.Type = ReadEnum<LevelType>(nameof(level.Type), levelData);
                switch (level.Type)
                {
                    case LevelType.Gym:
                        AssaultLevel = level;
                        break;

                    case LevelType.Normal:
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
                level.Music = ReadInt(nameof(level.Music), levelData).Value;
                level.Demo = ReadBool(nameof(level.Demo), levelData);
                level.DrawDistanceFade = ReadDouble(nameof(level.DrawDistanceFade), levelData);
                level.DrawDistanceMax = ReadDouble(nameof(level.DrawDistanceMax), levelData);
                level.UnobtainableKills = ReadInt(nameof(level.UnobtainableKills), levelData);
                level.UnobtainablePickups = ReadInt(nameof(level.UnobtainablePickups), levelData);

                
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
                    switch (sequenceType)
                    {
                        case LevelSequenceType.Play_FMV:
                            sequence = new FMVLevelSequence
                            {
                                FmvPath = ReadString("FmvPath", sequenceData)
                            };
                            break;
                        case LevelSequenceType.Display_Picture:
                            sequence = new DisplayPictureLevelSequence
                            {
                                PicturePath = ReadString("PicturePath", sequenceData),
                                DisplayTime = ReadDouble("DisplayTime", sequenceData).Value
                            };
                            break;
                        case LevelSequenceType.Total_Stats:
                            sequence = new TotalStatsLevelSequence
                            {
                                PicturePath = ReadString("PicturePath", sequenceData)
                            };
                            break;
                        case LevelSequenceType.Level_Stats:
                        case LevelSequenceType.Exit_To_Level:
                        case LevelSequenceType.Exit_To_Cine:
                            sequence = new LevelExitLevelSequence
                            {
                                LevelId = ReadInt("LevelId", sequenceData).Value
                            };
                            break;                        
                        
                        case LevelSequenceType.Set_Cam_X:
                        case LevelSequenceType.Set_Cam_Y:
                        case LevelSequenceType.Set_Cam_Z:
                        case LevelSequenceType.Set_Cam_Angle:
                            sequence = new SetCamLevelSequence
                            {
                                Value = ReadInt("Value", sequenceData).Value
                            };
                            break;                        
                        case LevelSequenceType.Give_Item:
                            sequence = new GiveItemLevelSequence
                            {
                                ObjectId = (TR1Items)ReadInt("ObjectId", sequenceData).Value,
                                Quantity = ReadInt("Quantity", sequenceData).Value
                            };
                            break;
                        case LevelSequenceType.Play_Synced_Audio:
                            sequence = new PlaySyncedAudioLevelSequence
                            {
                                AudioId = ReadInt("AudioId", sequenceData).Value
                            };
                            break;
                        case LevelSequenceType.Mesh_Swap:
                            sequence = new MeshSwapLevelSequence
                            {
                                Object1ID = ReadInt("Object1Id", sequenceData).Value,
                                Object2ID = ReadInt("Object2Id", sequenceData).Value,
                                MeshID = ReadInt("MeshId", sequenceData).Value
                            };
                            break;
                        default:
                            sequence = new BaseLevelSequence();
                            break;
                    }

                    sequence.Type = sequenceType;
                    level.Sequences.Add(sequence);
                }

                level.SetDefaults();
                ++levelID;
            }

            Levels[Levels.Count - 1].IsFinalLevel = true;
        }

        public override void ReadConfigJson(string json)
        {
            ConfigData = JObject.Parse(json);

            StartLaraHitpoints = ReadInt(nameof(StartLaraHitpoints), ConfigData).Value;
            DisableHealingBetweenLevels = ReadBool(nameof(DisableHealingBetweenLevels), ConfigData).Value;
            DisableMedpacks = ReadBool(nameof(DisableMedpacks), ConfigData).Value;
            DisableMagnums = ReadBool(nameof(DisableMagnums), ConfigData).Value;
            DisableUzis = ReadBool(nameof(DisableUzis), ConfigData).Value;
            DisableShotgun = ReadBool(nameof(DisableShotgun), ConfigData).Value;
            EnableDeathsCounter = ReadBool(nameof(EnableDeathsCounter), ConfigData).Value;
            EnableEnemyHealthbar = ReadBool(nameof(EnableEnemyHealthbar), ConfigData).Value;
            EnableEnhancedLook = ReadBool(nameof(EnableEnhancedLook), ConfigData).Value;
            EnableShotgunFlash = ReadBool(nameof(EnableShotgunFlash), ConfigData).Value;
            FixShotgunTargeting = ReadBool(nameof(FixShotgunTargeting), ConfigData).Value;
            EnableNumericKeys = ReadBool(nameof(EnableNumericKeys), ConfigData).Value;
            EnableTr3Sidesteps = ReadBool(nameof(EnableTr3Sidesteps), ConfigData).Value;
            EnableCheats = ReadBool(nameof(EnableCheats), ConfigData).Value;
            EnableBraid = ReadBool(nameof(EnableBraid), ConfigData).Value;
            EnableDetailedStats = ReadBool(nameof(EnableDetailedStats), ConfigData).Value;
            EnableCompassStats = ReadBool(nameof(EnableCompassStats), ConfigData).Value;
            EnableTotalStats = ReadBool(nameof(EnableTotalStats), ConfigData).Value;
            EnableTimerInInventory = ReadBool(nameof(EnableTimerInInventory), ConfigData).Value;
            EnableSmoothBars = ReadBool(nameof(EnableSmoothBars), ConfigData).Value;
            EnableFadeEffects = ReadBool(nameof(EnableFadeEffects), ConfigData).Value;
            MenuStyle = ReadEnum<TRMenuStyle>(nameof(MenuStyle), ConfigData);
            HealthbarShowingMode = ReadEnum<TRHealthbarMode>(nameof(HealthbarShowingMode), ConfigData);
            HealthbarLocation = ReadEnum<TRUILocation>(nameof(HealthbarLocation), ConfigData);
            HealthbarColor = ReadEnum<TRUIColour>(nameof(HealthbarColor), ConfigData);
            AirbarShowingMode = ReadEnum<TRAirbarMode>(nameof(AirbarShowingMode), ConfigData);
            AirbarLocation = ReadEnum<TRUILocation>(nameof(AirbarLocation), ConfigData);
            AirbarColor = ReadEnum<TRUIColour>(nameof(AirbarColor), ConfigData);
            EnemyHealthbarLocation = ReadEnum<TRUILocation>(nameof(EnemyHealthbarLocation), ConfigData);
            EnemyHealthbarColor = ReadEnum<TRUIColour>(nameof(EnemyHealthbarColor), ConfigData);
            FixTihocanSecretSound = ReadBool(nameof(FixTihocanSecretSound), ConfigData).Value;
            FixPyramidSecretTrigger = ReadBool(nameof(FixPyramidSecretTrigger), ConfigData).Value;
            FixSecretsKillingMusic = ReadBool(nameof(FixSecretsKillingMusic), ConfigData).Value;
            FixDescendingGlitch = ReadBool(nameof(FixDescendingGlitch), ConfigData).Value;
            FixWallJumpGlitch = ReadBool(nameof(FixWallJumpGlitch), ConfigData).Value;
            FixBridgeCollision = ReadBool(nameof(FixBridgeCollision), ConfigData).Value;
            FixQwopGlitch = ReadBool(nameof(FixQwopGlitch), ConfigData).Value;
            FixAlligatorAi = ReadBool(nameof(FixAlligatorAi), ConfigData).Value;
            ChangePierreSpawn = ReadBool(nameof(ChangePierreSpawn), ConfigData).Value;
            FovValue = ReadInt(nameof(FovValue), ConfigData).Value;
            FovVertical = ReadBool(nameof(FovVertical), ConfigData).Value;
            DisableDemo = ReadBool(nameof(DisableDemo), ConfigData).Value;
            DisableFmv = ReadBool(nameof(DisableFmv), ConfigData).Value;
            DisableCine = ReadBool(nameof(DisableCine), ConfigData).Value;
            DisableMusicInMenu = ReadBool(nameof(DisableMusicInMenu), ConfigData).Value;
            DisableMusicInInventory = ReadBool(nameof(DisableMusicInInventory), ConfigData).Value;
            DisableTrexCollision = ReadBool(nameof(DisableTrexCollision), ConfigData).Value;
            ResolutionWidth = ReadInt(nameof(ResolutionWidth), ConfigData).Value;
            ResolutionHeight = ReadInt(nameof(ResolutionHeight), ConfigData).Value;
            EnableRoundShadow = ReadBool(nameof(EnableRoundShadow), ConfigData).Value;
            Enable3dPickups = ReadBool(nameof(Enable3dPickups), ConfigData).Value;
            ScreenshotFormat = ReadEnum<TRScreenshotFormat>(nameof(ScreenshotFormat), ConfigData);
            AnisotropyFilter = ReadDouble(nameof(AnisotropyFilter), ConfigData).Value;
            WalkToItems = ReadBool(nameof(WalkToItems), ConfigData).Value;
            MaximumSaveSlots = ReadInt(nameof(MaximumSaveSlots), ConfigData).Value;
            RevertToPistols = ReadBool(nameof(RevertToPistols), ConfigData).Value;
        }

        private string ReadString(string key, JObject data)
        {
            key = key.ToLowerSnake();
            if (!data.ContainsKey(key))
            {
                return null;
            }

            string result = data[key].ToString();
            data.Remove(key);
            return result;
        }

        private bool? ReadBool(string key, JObject data)
        {
            bool? result = null;
            string b = ReadString(key, data);
            if (b != null)
            {
                result = bool.Parse(b);
            }
            return result;
        }

        private int? ReadInt(string key, JObject data)
        {
            int? result = null;
            string i = ReadString(key, data);
            if (i != null)
            {
                result = int.Parse(i);
            }
            return result;
        }

        private double? ReadDouble(string key, JObject data)
        {
            double? result = null;
            string d = ReadString(key, data);
            if (d != null)
            {
                result = double.Parse(d);
            }
            return result;
        }

        private T ReadEnum<T>(string key, JObject data)
        {
            string val = ReadString(key, data);
            val = val.Replace("-", string.Empty);
            return (T)Enum.Parse(typeof(T), val, true);
        }

        private T[] ReadArray<T>(string key, JObject data)
        {
            string a = ReadString(key, data);
            return a.Length == 0 ? null : JsonConvert.DeserializeObject<T[]>(a);
        }

        private Dictionary<K, V> ReadDictionary<K, V>(string key, JObject data)
        {
            string d = ReadString(key, data);
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
            JObject data = new JObject();

            Write(nameof(MainMenuPicture), MainMenuPicture, data);
            Write(nameof(SavegameFmtLegacy), SavegameFmtLegacy, data);
            Write(nameof(SavegameFmtBson), SavegameFmtBson, data);
            Write(nameof(EnableGameModes), EnableGameModes, data);
            Write(nameof(EnableSaveCrystals), EnableSaveCrystals, data);
            Write(nameof(DemoDelay), DemoDelay, data);
            Write(nameof(WaterColor), WaterColor, data);
            Write(nameof(DrawDistanceFade), DrawDistanceFade, data);
            Write(nameof(DrawDistanceMax), DrawDistanceMax, data);

            Write(nameof(Levels), BuildLevels(), data);

            Write(nameof(Strings), Strings, data);

            // Add anything else from the original data that we may not have captured.
            data.Merge(GameflowData);

            return JsonConvert.SerializeObject(data, Formatting.Indented);
        }

        public override string SerialiseConfigToJson()
        {
            JObject data = new JObject();

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
            Write(nameof(FixSecretsKillingMusic), FixSecretsKillingMusic, data);
            Write(nameof(FixDescendingGlitch), FixDescendingGlitch, data);
            Write(nameof(FixWallJumpGlitch), FixWallJumpGlitch, data);
            Write(nameof(FixBridgeCollision), FixBridgeCollision, data);
            Write(nameof(FixQwopGlitch), FixQwopGlitch, data);
            Write(nameof(FixAlligatorAi), FixAlligatorAi, data);
            Write(nameof(ChangePierreSpawn), ChangePierreSpawn, data);
            Write(nameof(FovValue), FovValue, data);
            Write(nameof(FovVertical), FovVertical, data);
            Write(nameof(DisableDemo), DisableDemo, data);
            Write(nameof(DisableFmv), DisableFmv, data);
            Write(nameof(DisableCine), DisableCine, data);
            Write(nameof(DisableMusicInMenu), DisableMusicInMenu, data);
            Write(nameof(DisableMusicInInventory), DisableMusicInInventory, data);
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

            // Add anything else from the original data that we may not have captured.
            data.Merge(ConfigData);

            return JsonConvert.SerializeObject(data, Formatting.Indented);
        }

        private void Write<T>(string key, T value, JObject data)
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

        private void WriteKebab<T>(string key, T value, JObject data)
        {
            key = key.ToLowerSnake();
            data[key] = JToken.FromObject(value.ToString().ToLowerKebab());
        }

        private JArray BuildLevels()
        {
            JArray levels = new JArray();
            levels.Add(SerializeLevel(AssaultLevel as TR1ScriptedLevel));

            List<AbstractTRScriptedLevel> enabledLevels = Levels.FindAll(l => l.Enabled);
            List<AbstractTRScriptedLevel> cutsceneLevels = new List<AbstractTRScriptedLevel>();

            int cutsceneLevelID = enabledLevels.Count;
            foreach (TR1ScriptedLevel level in enabledLevels)
            {
                if (level.HasCutScene)
                {
                    LevelExitLevelSequence cutSequence = level.Sequences.Find(s => s.Type == LevelSequenceType.Exit_To_Cine) as LevelExitLevelSequence;
                    cutSequence.LevelId = ++cutsceneLevelID;
                    cutsceneLevels.Add(level.CutSceneLevel);
                }
                levels.Add(SerializeLevel(level));
            }

            foreach (TR1ScriptedLevel level in cutsceneLevels)
            {
                levels.Add(SerializeLevel(level));
            }

            levels.Add(SerializeLevel((FrontEnd as TR1FrontEnd).TitleLevel));
            levels.Add(SerializeLevel(_atiCurrent));

            return levels;
        }

        private JObject SerializeLevel(TR1ScriptedLevel level)
        {
            JObject levelObj = new JObject();

            Write("Title", level.Name, levelObj);
            Write("File", level.LevelFile, levelObj);
            Write(nameof(level.Type), level.Type/*.ToString().ToLower()*/, levelObj);
            Write(nameof(level.Music), level.Music, levelObj);

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

            JObject strings = new JObject();
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

            return levelObj;
        }

        private JArray BuildSequences(TR1ScriptedLevel level)
        {
            JArray sequences = new JArray();

            foreach (BaseLevelSequence sequence in level.Sequences)
            {
                JObject seq = JObject.FromObject(sequence, _mainSerializer);
                Write(nameof(seq.Type), sequence.Type/*.ToString().ToLower()*/, seq);
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
}