using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using TRGE.Core;

namespace TRGE.Coord
{
    public abstract class AbstractTRLevelEditor : AbstractTRGEEditor
    {
        protected readonly TRDirectoryIOArgs _io;
        protected readonly Dictionary<string, ISet<TRScriptedLevelEventArgs>> _levelModifications;

        internal AbstractTRLevelEditor(TRDirectoryIOArgs io)
        {
            _io = io;
            _levelModifications = new Dictionary<string, ISet<TRScriptedLevelEventArgs>>();
            LoadConfig();
        }

        private void LoadConfig()
        {
            _config = File.Exists(_io.ConfigFile.FullName) ? JsonConvert.DeserializeObject<Dictionary<string, object>>(_io.ConfigFile.ReadCompressedText()) : null;
            ReadConfig(_config);
        }

        protected override void ReadConfig(Dictionary<string, object> config)
        {
            if (config != null)
            {
                AllowSuccessiveEdits = bool.Parse(config["Successive"].ToString());
                ApplyConfig(config);
            }
        }

        internal override Dictionary<string, object> ExportConfig()
        {
            Dictionary<string, object> config = base.ExportConfig();
            SaveConfig(config);
            return config;
        }

        protected virtual void SaveConfig(Dictionary<string, object> config) { }

        internal void Initialise(AbstractTRScriptEditor scriptEditor)
        {
            _levelModifications.Clear();
            foreach (AbstractTRScriptedLevel level in scriptEditor.LevelManager.Levels)
            {
                _levelModifications.Add(level.ID, new HashSet<TRScriptedLevelEventArgs>());
            }
        }

        internal void Save(AbstractTRScriptEditor scriptEditor, TRSaveMonitor monitor)
        {
            _config = new Dictionary<string, object>
            {
                ["Version"] = Assembly.GetExecutingAssembly().GetName().Version,
                ["Successive"] = AllowSuccessiveEdits
            };

            monitor.FireSaveStateChanged(0, TRSaveCategory.LevelFile);
            foreach (string levelID in _levelModifications.Keys)
            {
                foreach (TRScriptedLevelEventArgs mod in _levelModifications[levelID])
                {
                    ProcessModification(mod);
                }

                monitor.FireSaveStateChanged(1);
            }

            SaveImpl(scriptEditor, monitor);

            SaveConfig(_config);

            _io.ConfigFile.WriteCompressedText(JsonConvert.SerializeObject(_config, Formatting.None)); //#48
        }

        internal void ScriptedLevelModified(TRScriptedLevelEventArgs e)
        {
            if (ShouldHandleModification(e))
            {
                _levelModifications[e.LevelID].Add(e);
            }
        }

        public override int GetSaveTargetCount()
        {
            return _levelModifications.Count + GetSaveTarget(_levelModifications.Count);
        }

        protected virtual int GetSaveTarget(int numLevels)
        {
            return 0;
        }

        protected override void ApplyConfig(Dictionary<string, object> config) { }
        internal abstract bool ShouldHandleModification(TRScriptedLevelEventArgs e);
        internal abstract void ProcessModification(TRScriptedLevelEventArgs e);

        protected virtual void SaveImpl(AbstractTRScriptEditor scriptEditor, TRSaveMonitor monitor) { }
        
        /// <summary>
        /// Depending on wheter AllowSuccessiveEdits is set this will either return the current
        /// file in the target directory or the file that was originally backed up.
        /// </summary>
        protected virtual string GetReadLevelFilePath(string levelFileName)
        {
            return Path.Combine(AllowSuccessiveEdits ?_io.OutputDirectory.FullName : _io.BackupDirectory.FullName, levelFileName);
        }

        protected virtual string GetWriteLevelFilePath(string levelFileName)
        {
            return Path.Combine(_io.OutputDirectory.FullName, levelFileName);
        }

        /// <summary>
        /// This performs a check that each level defined in the script file is available as a level
        /// file in the specified directory. So, if a folder contains a TR2 script file but TR3 level
        /// files, then an exception is thrown. Equally, if the folder contains only a subset of the
        /// expected level files, an exception is thrown.
        /// </summary>
        internal static void ValidateCompatibility(List<AbstractTRScriptedLevel> levels, string folderPath)
        {
            List<AbstractTRScriptedLevel> faults = new List<AbstractTRScriptedLevel>();
            foreach (AbstractTRScriptedLevel level in levels)
            {
                if (!File.Exists(Path.Combine(folderPath, level.LevelFileBaseName)))
                {
                    faults.Add(level);
                }
            }

            if (faults.Count > 0)
            {
                StringBuilder sb = new StringBuilder("The following level files were not found in ").Append(folderPath).Append(".").AppendLine();
                foreach (AbstractTRScriptedLevel level in faults)
                {
                    sb.AppendLine().Append(level.Name).Append(" (").Append(level.LevelFileBaseName).Append(")");
                }
                throw new ScriptedLevelMismatchException(sb.ToString());
            }
        }
    }
}