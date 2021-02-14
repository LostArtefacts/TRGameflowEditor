using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace TRGE.Core
{
    public abstract class AbstractTRScriptEditor : AbstractTRGEEditor
    {
        protected TRScriptIOArgs _io;

        internal FileInfo OriginalFile
        {
            get => _io.OriginalFile;
            set => _io.OriginalFile = value;
        }

        internal FileInfo BackupFile
        {
            get => _io.BackupFile;
            set => _io.BackupFile = value;
        }

        internal FileInfo ConfigFile
        {
            get => _io.ConfigFile;
            set => _io.ConfigFile = value;
        }

        internal DirectoryInfo WIPOutputDirectory
        {
            get => _io.WIPOutputDirectory;
            set => _io.WIPOutputDirectory = value;
        }

        internal DirectoryInfo OutputDirectory
        {
            get => _io.OutputDirectory;
            set => _io.OutputDirectory = value;
        }

        public TREdition Edition => Script.Edition;

        internal AbstractTRLevelManager LevelManager { get; private set; }
        internal AbstractTRFrontEnd FrontEnd => Script.FrontEnd;
        internal AbstractTRScript Script { get; private set; }

        public IReadOnlyList<AbstractTRScriptedLevel> Levels => LevelManager.Levels;

        protected TRScriptOpenOption _openOption;

        internal event EventHandler<TRScriptedLevelEventArgs> LevelModified;

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
            _config = File.Exists(ConfigFile.FullName) ? JsonConvert.DeserializeObject<Dictionary<string, object>>(ConfigFile.ReadCompressedText()) : null;
            //issue #36
            if (_config != null && !OriginalFile.Checksum().Equals(_config["CheckSumOnSave"]))
            {
                switch (_openOption)
                {
                    //callers should make a different OpenOption and try to open the script again
                    case TRScriptOpenOption.Default:
                        throw new ChecksumMismatchException();
                    //overwrite the existing backup with the "new" original file, delete the config as though we have never opened the file before
                    case TRScriptOpenOption.DiscardBackup:
                        File.Copy(OriginalFile.FullName, BackupFile.FullName, true);
                        while (File.Exists(ConfigFile.FullName))
                        {
                            ConfigFile.Delete(); //issue #39
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

        protected override void ReadConfig(Dictionary<string, object> config)
        {
            if (config != null)
            {
                Dictionary<string, object> configEdition = JsonConvert.DeserializeObject<Dictionary<string, object>>(config["Edition"].ToString());
                TREdition checkEdition = TREdition.From
                (
                    (Hardware)Enum.ToObject(typeof(Hardware), configEdition["Hardware"]), 
                    (TRVersion)Enum.ToObject(typeof(TRVersion), configEdition["Version"])
                );
                if (checkEdition == null || !checkEdition.Equals(Edition))
                {
                    throw new EditionMismatchException("The TR edition in the configuration file does not match the edition of the current script file.");
                }

                Dictionary<string, object> levelSeq = JsonConvert.DeserializeObject<Dictionary<string, object>>(config["LevelSequencing"].ToString());
                LevelSequencingOrganisation = (Organisation)Enum.ToObject(typeof(Organisation), levelSeq["Organisation"]);
                LevelSequencingRNG = new RandomGenerator(JsonConvert.DeserializeObject<Dictionary<string, object>>(levelSeq["RNG"].ToString()));
                //note that even if the levels were randomised, this would have been done after saving the config file
                //so reloading the sequencing will either just restore defaults or set it to a manual sequence if the user
                //picked that at one point in the previous edit
                if (levelSeq.ContainsKey("Data"))
                {
                    LevelSequencing = JsonConvert.DeserializeObject<List<Tuple<string, string>>>(levelSeq["Data"].ToString());
                }

                Dictionary<string, object> trackInfo = JsonConvert.DeserializeObject<Dictionary<string, object>>(config["GameTracks"].ToString());
                GameTrackOrganisation = (Organisation)Enum.ToObject(typeof(Organisation), trackInfo["Organisation"]);
                GameTrackRNG = new RandomGenerator(JsonConvert.DeserializeObject<Dictionary<string, object>>(trackInfo["RNG"].ToString()));
                //see note above
                if (trackInfo.ContainsKey("Data"))
                {
                    GameTrackData = JsonConvert.DeserializeObject<List<MutableTuple<string, string, ushort>>>(trackInfo["Data"].ToString());
                }

                Dictionary<string, object> secretInfo = JsonConvert.DeserializeObject<Dictionary<string, object>>(config["SecretSupport"].ToString());
                LevelSecretSupportOrganisation = (Organisation)Enum.ToObject(typeof(Organisation), secretInfo["Organisation"]);
                LevelSecretSupport = JsonConvert.DeserializeObject<List<MutableTuple<string, string, bool>>>(secretInfo["Data"].ToString());

                Dictionary<string, object> sunsetInfo = JsonConvert.DeserializeObject<Dictionary<string, object>>(config["Sunsets"].ToString());
                LevelSunsetOrganisation = (Organisation)Enum.ToObject(typeof(Organisation), sunsetInfo["Organisation"]);
                LevelSunsetRNG = new RandomGenerator(JsonConvert.DeserializeObject<Dictionary<string, object>>(sunsetInfo["RNG"].ToString()));
                //see note above
                if (sunsetInfo.ContainsKey("Data"))
                {
                    LevelSunsetData = JsonConvert.DeserializeObject<List<MutableTuple<string, string, bool>>>(sunsetInfo["Data"].ToString());
                }
                RandomSunsetLevelCount = uint.Parse(sunsetInfo["RandomCount"].ToString());

                FrontEndHasFMV = bool.Parse(config["FrontEndFMVOn"].ToString());
                AllowSuccessiveEdits = bool.Parse(config["Successive"].ToString());
            }
            else
            {
                LevelSequencingOrganisation = Organisation.Default;
                LevelSequencingRNG = new RandomGenerator(RandomGenerator.Type.Date);
                GameTrackOrganisation = Organisation.Default;
                GameTrackRNG = new RandomGenerator(RandomGenerator.Type.Date);
                LevelSecretSupportOrganisation = Organisation.Default;
                LevelSunsetOrganisation = Organisation.Default;
                LevelSunsetRNG = new RandomGenerator(RandomGenerator.Type.Date);
                RandomSunsetLevelCount = LevelManager.GetSunsetLevelCount();

                AllowSuccessiveEdits = false;
            }

            ApplyConfig(config);
        }

        public override int GetSaveTargetCount()
        {
            return 1;
        }

        internal void Save(TRSaveMonitor monitor)
        {
            monitor.FireSaveStateChanged(0, TRSaveCategory.Scripting);

            _config = new Dictionary<string, object>
            {
                ["App"] = new Dictionary<string, object>
                {
                    ["Tag"] = TRInterop.TaggedVersion,
                    ["Version"] = TRInterop.ExecutingVersion
                },
                ["Edition"] = Edition.ToJson(),
                ["Original"] = OriginalFile.FullName,
                ["CheckSumOnSave"] = string.Empty,
                ["FrontEndFMVOn"] = FrontEndHasFMV,
                ["Successive"] = AllowSuccessiveEdits,
                ["LevelSequencing"] = new Dictionary<string, object>
                {
                    ["Organisation"] = (int)LevelSequencingOrganisation,
                    ["RNG"] = LevelSequencingRNG.ToJson(),
                    ["Data"] = LevelSequencing
                },
                ["GameTracks"] = new Dictionary<string, object>
                {
                    ["Organisation"] = (int)GameTrackOrganisation,
                    ["RNG"] = GameTrackRNG.ToJson(),
                    ["Data"] = GameTrackData
                },
                ["SecretSupport"] = new Dictionary<string, object>
                {
                    ["Organisation"] = (int)LevelSecretSupportOrganisation,
                    ["Data"] = LevelSecretSupport
                },
                ["Sunsets"] = new Dictionary<string, object>
                {
                    ["Organisation"] = (int)LevelSunsetOrganisation,
                    ["RNG"] = LevelSunsetRNG.ToJson(),
                    ["RandomCount"] = RandomSunsetLevelCount,
                    ["Data"] = LevelSunsetData
                }
            };

            AbstractTRScript backupScript = LoadBackupScript();
            AbstractTRScript randoBaseScript = LoadRandomisationBaseScript(); // #42
            AbstractTRLevelManager originalLevelManager = TRScriptedLevelFactory.GetLevelManager(LoadScript(OriginalFile.FullName)); //#65

            if (LevelSequencingOrganisation == Organisation.Random)
            {
                if (TRInterop.RandomisationSupported)
                {
                    LevelManager.RandomiseSequencing(randoBaseScript.Levels);
                }
                else
                {
                    LevelManager.SetSequencing(originalLevelManager.GetSequencing()); //#65 lock to that of the original file
                }
            }
            else if (LevelSequencingOrganisation == Organisation.Default)
            {
                LevelManager.RestoreSequencing(backupScript.Levels);
            }

            if (GameTrackOrganisation == Organisation.Random)
            {
                if (TRInterop.RandomisationSupported)
                {
                    LevelManager.RandomiseGameTracks(randoBaseScript.Levels);
                }
                else
                {
                    LevelManager.SetTrackData(originalLevelManager.GetTrackData(originalLevelManager.Levels)); //#65 lock to that of the original file
                }
            }
            else if (GameTrackOrganisation == Organisation.Default)
            {
                LevelManager.RestoreGameTracks(backupScript);
            }

            if (LevelSecretSupportOrganisation == Organisation.Default)
            {
                LevelManager.RestoreSecretSupport(backupScript.Levels);
            }

            if (LevelSunsetOrganisation == Organisation.Random)
            {
                if (TRInterop.RandomisationSupported)
                {
                    LevelManager.RandomiseSunsets(randoBaseScript.Levels);
                }
                else
                {
                    LevelManager.SetSunsetData(originalLevelManager.GetSunsetData(originalLevelManager.Levels)); //#65 lock to that of the original file
                }
            }
            else if (LevelSunsetOrganisation == Organisation.Default)
            {
                LevelManager.RestoreSunsetData(backupScript.Levels);
            }
            else
            {
                LevelManager.SetSunsetData(LevelSunsetData); //TODO: Fix this - it's in place to ensure the event is triggered for any listeners
            }

            SaveImpl();
            LevelManager.Save();

            string outputPath = GetScriptWIPOutputPath();
            Script.Write(outputPath);

            monitor.FireSaveStateChanged(1);
        }

        /// <summary>
        /// Called on a successful save transaction from TREditor, so it is safe to write the current config
        /// to disk at this stage.
        /// </summary>
        internal sealed override void SaveComplete()
        {
            _config["CheckSumOnSave"] = new FileInfo(GetScriptWIPOutputPath()).Checksum();
            ConfigFile.WriteCompressedText(JsonConvert.SerializeObject(_config, Formatting.None)); //#48
        }

        internal override Dictionary<string, object> ExportConfig()
        {
            Dictionary<string, object> config = base.ExportConfig();
            if (config.ContainsKey("CheckSumOnSave"))
            {
                config.Remove("CheckSumOnSave");
            }
            if (config.ContainsKey("Original"))
            {
                config.Remove("Original");
            }

            if (!TRInterop.RandomisationSupported)
            {
                if (LevelSequencingOrganisation == Organisation.Random)
                {
                    config["LevelSequencing"] = new Dictionary<string, object>
                    {
                        ["Organisation"] = (int)Organisation.Default,
                        ["RNG"] = LevelSequencingRNG.ToJson()
                    };
                }

                if (GameTrackOrganisation == Organisation.Random)
                {
                    config["GameTracks"] = new Dictionary<string, object>
                    {
                        ["Organisation"] = (int)Organisation.Default,
                        ["RNG"] = GameTrackRNG.ToJson()
                    };
                }

                if (LevelSunsetOrganisation == Organisation.Random)
                {
                    config["Sunsets"] = new Dictionary<string, object>
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
            BackupFile.CopyTo(OriginalFile.FullName, true);
            while (File.Exists(ConfigFile.FullName))
            {
                ConfigFile.Delete(); //issue #39
            }
            ConfigFile = new FileInfo(ConfigFile.FullName);
            Initialise(Script);
        }

        protected string GetScriptWIPOutputPath()
        {
            return Path.Combine(WIPOutputDirectory.FullName, OriginalFile.Name);
        }

        protected string GetScriptOutputPath()
        {
            return Path.Combine(OutputDirectory.FullName, OriginalFile.Name);
        }

        internal AbstractTRScript LoadBackupScript()
        {
            return LoadScript(BackupFile.FullName);
        }

        internal AbstractTRScript LoadOutputScript()
        {
            return LoadScript(GetScriptOutputPath());
        }

        internal AbstractTRScript LoadRandomisationBaseScript()
        {
            return AllowSuccessiveEdits ? LoadOutputScript() : LoadBackupScript();
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

        protected abstract void SaveImpl();

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
}