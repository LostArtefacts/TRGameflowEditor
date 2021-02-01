using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace TRGE.Core
{
    public abstract class AbstractTRScriptEditor
    {
        protected TRScriptIOArgs _io;

        public FileInfo OriginalFile
        {
            get => _io.OriginalFile;
            set => _io.OriginalFile = value;
        }

        public FileInfo BackupFile
        {
            get => _io.BackupFile;
            set => _io.BackupFile = value;
        }

        public FileInfo ConfigFile
        {
            get => _io.ConfigFile;
            set => _io.ConfigFile = value;
        }

        public DirectoryInfo OutputDirectory
        {
            get => _io.OutputDirectory;
            set => _io.OutputDirectory = value;
        }

        public TREdition Edition => Script.Edition;
        internal bool AllowSuccessiveEdits { get; set; }

        internal AbstractTRLevelManager LevelManager { get; private set; }
        internal AbstractTRFrontEnd FrontEnd => Script.FrontEnd;
        internal AbstractTRScript Script { get; private set; }

        protected TRScriptOpenOption _openOption;
        protected Dictionary<string, object> _config;

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

            ReadConfig();
        }

        private void LevelManagerLevelModified(object sender, TRScriptedLevelEventArgs e)
        {
            LevelModified?.Invoke(this, e);
        }

        private void LoadConfig()
        {
            _config = ConfigFile.Exists ? JsonConvert.DeserializeObject<Dictionary<string, object>>(File.ReadAllText(ConfigFile.FullName)) : null;
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
                        OriginalFile.CopyTo(BackupFile.FullName, true);
                        ConfigFile.Delete();
                        break;
                    //overwrite the original file with the backup
                    case TRScriptOpenOption.RestoreBackup:
                        BackupFile.CopyTo(OriginalFile.FullName, true);
                        break;
                }

                _openOption = TRScriptOpenOption.Default;
            }
        }

        protected void ReadConfig()
        {
            if (_config != null)
            {
                Dictionary<string, object> levelSeq = JsonConvert.DeserializeObject<Dictionary<string, object>>(_config["LevelSequencing"].ToString());
                LevelSequencingOrganisation = (Organisation)Enum.ToObject(typeof(Organisation), levelSeq["Organisation"]);
                LevelSequencingRNG = new RandomGenerator(JsonConvert.DeserializeObject<Dictionary<string, object>>(levelSeq["RNG"].ToString()));
                //note that even if the levels were randomised, this would have been done after saving the config file
                //so reloading the sequencing will either just restore defaults or set it to a manual sequence if the user
                //picked that at one point in the previous edit
                LevelSequencing = JsonConvert.DeserializeObject<List<Tuple<string, string>>>(levelSeq["Data"].ToString());

                Dictionary<string, object> trackInfo = JsonConvert.DeserializeObject<Dictionary<string, object>>(_config["GameTracks"].ToString());
                GameTrackOrganisation = (Organisation)Enum.ToObject(typeof(Organisation), trackInfo["Organisation"]);
                GameTrackRNG = new RandomGenerator(JsonConvert.DeserializeObject<Dictionary<string, object>>(trackInfo["RNG"].ToString()));
                //see note above
                GameTrackData = JsonConvert.DeserializeObject<List<MutableTuple<string, string, ushort>>>(trackInfo["Data"].ToString());

                Dictionary<string, object> secretInfo = JsonConvert.DeserializeObject<Dictionary<string, object>>(_config["SecretSupport"].ToString());
                LevelSecretSupportOrganisation = (Organisation)Enum.ToObject(typeof(Organisation), secretInfo["Organisation"]);
                LevelSecretSupport = JsonConvert.DeserializeObject<List<MutableTuple<string, string, bool>>>(secretInfo["Data"].ToString());

                FrontEndHasFMV = bool.Parse(_config["FrontEndFMVOn"].ToString());
            }
            else
            {
                LevelSequencingOrganisation = Organisation.Default;
                LevelSequencingRNG = new RandomGenerator(RandomGenerator.Type.Date);
                GameTrackOrganisation = Organisation.Default;
                GameTrackRNG = new RandomGenerator(RandomGenerator.Type.Date);
                LevelSecretSupportOrganisation = Organisation.Default;
            }

            ApplyConfig();
        }

        internal void Save()
        {
            _config = new Dictionary<string, object>
            {
                ["Version"] = Assembly.GetExecutingAssembly().GetName().Version,
                ["Original"] = OriginalFile.FullName,
                ["CheckSumOnSave"] = string.Empty,
                ["FrontEndFMVOn"] = FrontEndHasFMV,
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
                }
            };

            AbstractTRScript backupScript = LoadBackupScript();
            if (LevelSequencingOrganisation == Organisation.Random)
            {
                LevelManager.RandomiseSequencing(backupScript.Levels);
            }
            else if (LevelSequencingOrganisation == Organisation.Default)
            {
                LevelManager.RestoreSequencing(backupScript.Levels);
            }

            if (GameTrackOrganisation == Organisation.Random)
            {
                LevelManager.RandomiseGameTracks(backupScript.Levels);
            }
            else if (GameTrackOrganisation == Organisation.Default)
            {
                LevelManager.RestoreGameTracks(backupScript);
            }

            if (LevelSecretSupportOrganisation == Organisation.Default)
            {
                LevelManager.RestoreSecretSupport(backupScript.Levels);
            }

            SaveImpl();
            LevelManager.Save();

            string localCopyPath = Path.Combine(OutputDirectory.FullName, OriginalFile.Name);
            Script.Write(localCopyPath);
            //File.Copy(localCopyPath, OriginalFile.FullName, true);

            _config["CheckSumOnSave"] = new FileInfo(localCopyPath)/*OriginalFile*/.Checksum();

            _config.Sort(delegate(string s1, string s2)
            {
                object o1 = _config[s1];
                object o2 = _config[s2];

                if (o1 is Dictionary<string, object> && !(o2 is Dictionary<string, object>))
                {
                    return 1;
                }
                if (o2 is Dictionary<string, object> && !(o1 is Dictionary<string, object>))
                {
                    return -1;
                }

                return s1.CompareTo(s2);
            });

            File.WriteAllText(ConfigFile.FullName, JsonConvert.SerializeObject(_config, Formatting.Indented));

            //reload at this point to reset the randomised information, to prevent this being available without playing the game
            //Initialise(CreateScript());
        }

        public void Restore()
        {
            BackupFile.CopyTo(OriginalFile.FullName, true);
            while (File.Exists(ConfigFile.FullName))
            {
                ConfigFile.Delete(); //issue #39
            }
            ConfigFile = new FileInfo(ConfigFile.FullName);
            Initialise(Script);
        }

        internal AbstractTRScript LoadBackupScript()
        {
            AbstractTRScript script = CreateScript();
            script.Read(BackupFile);
            return script;
        }

        internal List<AbstractTRScriptedLevel> GetOriginalLevels()
        {
            return LoadBackupScript().Levels;
        }

        internal abstract AbstractTRScript CreateScript();

        protected abstract void SaveImpl();
        protected abstract void ApplyConfig();

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
            get => LevelManager.GetTrackData();
            set => LevelManager.SetTrackData(value);
        }

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
                    throw new ArgumentException("Randomisation of level secret support is not implemented.");
                }
                LevelManager.SecretSupportOrganisation = value;
            }
        }

        public List<MutableTuple<string, string, bool>> LevelSecretSupport
        {
            get => LevelManager.GetSecretSupport();
            set => LevelManager.SetSecretSupport(value);
        }
    }
}