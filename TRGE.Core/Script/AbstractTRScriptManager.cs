﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace TRGE.Core
{
    public abstract class AbstractTRScriptManager
    {
        public FileInfo OriginalFile { get; internal set; }
        public FileInfo BackupFile { get; internal set; }
        public FileInfo ConfigFile { get; internal set; }
        public TREdition Edition => Script.Edition;

        internal AbstractTRLevelManager LevelManager { get; private set; }
        internal AbstractTRFrontEnd FrontEnd => Script.FrontEnd;
        internal AbstractTRScript Script { get; private set; }

        protected TRScriptOpenOption _openOption;
        protected Dictionary<string, object> _config;

        internal event EventHandler<TRScriptedLevelEventArgs> LevelModified;

        internal AbstractTRScriptManager(FileInfo originalFile, FileInfo backupFile, FileInfo configFile, TRScriptOpenOption openOption)
        {
            ConfigFile = configFile;
            _openOption = openOption;
            Initialise(CreateScript(), originalFile, backupFile);
        }

        private void Initialise(AbstractTRScript script, FileInfo originalFile, FileInfo backupFile)
        {
            OriginalFile = originalFile;
            BackupFile = backupFile;

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
            if (_config != null && !Hashing.Checksum(OriginalFile.FullName).Equals(_config["CheckSumOnSave"]))
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
                LevelOrganisation = (Organisation)Enum.ToObject(typeof(Organisation), levelSeq["Organisation"]);
                LevelRNG = new RandomGenerator(JsonConvert.DeserializeObject<Dictionary<string, object>>(levelSeq["RNG"].ToString()));
                //note that even if the levels were randomised, this would have been done after saving the config file
                //so reloading the sequencing will either just restore defaults or set it to a manual sequence if the user
                //picked that at one point in the previous edit
                LevelSequencing = JsonConvert.DeserializeObject<List<Tuple<string, string>>>(levelSeq["Data"].ToString());

                Dictionary<string, object> trackInfo = JsonConvert.DeserializeObject<Dictionary<string, object>>(_config["GameTracks"].ToString());
                GameTrackOrganisation = (Organisation)Enum.ToObject(typeof(Organisation), trackInfo["Organisation"]);
                GameTrackRNG = new RandomGenerator(JsonConvert.DeserializeObject<Dictionary<string, object>>(trackInfo["RNG"].ToString()));
                //see note above
                GameTrackData = JsonConvert.DeserializeObject<List<MutableTuple<string, string, ushort>>>(trackInfo["Data"].ToString());

                FrontEndHasFMV = bool.Parse(_config["FrontEndFMVOn"].ToString());
            }
            else
            {
                LevelOrganisation = Organisation.Default;
                LevelRNG = new RandomGenerator(RandomGenerator.Type.Date);
                GameTrackOrganisation = Organisation.Default;
                GameTrackRNG = new RandomGenerator(RandomGenerator.Type.Date);
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
                    ["Organisation"] = (int)LevelOrganisation,
                    ["RNG"] = LevelRNG.ToJson(),
                    ["Data"] = LevelSequencing
                },
                ["GameTracks"] = new Dictionary<string, object>
                {
                    ["Organisation"] = (int)GameTrackOrganisation,
                    ["RNG"] = GameTrackRNG.ToJson(),
                    ["Data"] = GameTrackData
                }
            };

            AbstractTRScript backupScript = LoadBackupScript();
            if (LevelOrganisation == Organisation.Random)
            {
                LevelManager.RandomiseLevelSequencing(backupScript.Levels);
            }
            else if (LevelOrganisation == Organisation.Default)
            {
                LevelManager.RestoreLevelSequencing(backupScript.Levels);
            }

            if (GameTrackOrganisation == Organisation.Random)
            {
                //TODO: randomise
            }
            else if (GameTrackOrganisation == Organisation.Default)
            {
                //TODO: restore
            }

            SaveImpl();
            LevelManager.Save();

            string localCopyPath = Path.Combine(BackupFile.DirectoryName, OriginalFile.Name);
            Script.Write(localCopyPath);
            File.Copy(localCopyPath, OriginalFile.FullName, true);

            _config["CheckSumOnSave"] = Hashing.Checksum(OriginalFile.FullName);

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
        }

        public void Restore()
        {
            BackupFile.CopyTo(OriginalFile.FullName, true);
            Initialise(Script, OriginalFile, BackupFile);
        }

        internal AbstractTRScript LoadBackupScript()
        {
            AbstractTRScript script = CreateScript();
            script.Read(BackupFile);
            return script;
        }

        internal abstract AbstractTRScript CreateScript();

        protected abstract void SaveImpl();
        protected abstract void ApplyConfig();

        public Organisation LevelOrganisation
        {
            get => LevelManager.LevelOrganisation;
            set => LevelManager.LevelOrganisation = value;
        }

        public RandomGenerator LevelRNG
        {
            get => LevelManager.LevelRNG;
            set => LevelManager.LevelRNG = value;
        }

        public List<Tuple<string, string>> LevelSequencing
        {
            get => LevelManager.GetLevelSequencing();
            set => LevelManager.SetLevelSequencing(value);
        }

        internal void RandomiseLevels()
        {
            LevelManager.RandomiseLevelSequencing(LoadBackupScript().Levels);
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
            get => LevelManager.GetLevelTrackData();
            set => LevelManager.SetLevelTrackData(value);
        }

        internal TRAudioTrack TitleTrack => LevelManager.AudioProvider.GetTrack(Script.TitleSoundID);
        internal TRAudioTrack SecretTrack => LevelManager.AudioProvider.GetTrack(Script.SecretSoundID);

        public byte[] GetTrackData(ushort trackID)
        {
            return LevelManager.AudioProvider.GetTrackData(trackID);
        }
    }
}