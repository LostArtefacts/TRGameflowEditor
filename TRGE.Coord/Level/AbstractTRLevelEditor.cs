using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TRGE.Core;

namespace TRGE.Coord
{
    public abstract class AbstractTRLevelEditor : AbstractTRGEEditor
    {
        protected readonly TRDirectoryIOArgs _io;
        protected readonly Dictionary<string, ISet<TRScriptedLevelEventArgs>> _levelModifications;

        internal override string ConfigFilePath => _io.ConfigFile.FullName;

        public AbstractTRLevelEditor(TRDirectoryIOArgs io)
        {
            _io = io;
            _levelModifications = new Dictionary<string, ISet<TRScriptedLevelEventArgs>>();
            ReadConfig(_config = Config.Read(_io.ConfigFile.FullName));
        }

        protected sealed override void ReadConfig(Config config)
        {
            if (config != null)
            {
                AllowSuccessiveEdits = config.GetBool("Successive");
                ApplyConfig(config);
            }
        }

        internal sealed override Config ExportConfig()
        {
            Config config = base.ExportConfig();
            StoreConfig(config);
            return config;
        }

        /// <summary>
        /// The supplied dictionary has been loaded from disk from the previous edit, so values
        /// can be assigned locally as necessary.
        /// </summary>
        /// <param name="config">The configuration dictionary loaded from disk.</param>
        protected override void ApplyConfig(Config config) { }

        /// <summary>
        /// Any custom values to be saved between edits should be added to the supplied dictionary.
        /// </summary>
        /// <param name="config">The current configuration dictionary.</param>
        protected virtual void StoreConfig(Config config) { }

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
            _config = new Config
            {
                ["App"] = new Config
                {
                    ["Tag"] = TRInterop.TaggedVersion,
                    ["Version"] = TRInterop.ExecutingVersion
                },
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
                if (monitor.IsCancelled)
                {
                    return;
                }
            }

            SaveImpl(scriptEditor, monitor);

            StoreConfig(_config);
        }

        /// <summary>
        /// Called on a successful save transaction from TREditor, so it is safe to write the current config
        /// to disk at this stage.
        /// </summary>
        internal sealed override void SaveComplete()
        {
            _config.Write(_io.ConfigFile.FullName);
            //_io.ConfigFile.WriteCompressedText(JsonConvert.SerializeObject(_config, Formatting.None)); //#48
        }

        internal void ScriptedLevelModified(TRScriptedLevelEventArgs e)
        {
            if (ShouldHandleModification(e))
            {
                _levelModifications[e.LevelID].Add(e);
            }
        }

        public sealed override int GetSaveTargetCount()
        {
            return _levelModifications.Count + GetSaveTarget(_levelModifications.Count);
        }

        /// <summary>
        /// Called when initialising a save. The returned value should represent the
        /// number of steps that will be involved in the save progress for this class.
        /// </summary>
        /// <param name="numLevels">A count of the total number of levels for the current edit.</param>
        /// <returns>The numer of save steps for this class.</returns>
        protected virtual int GetSaveTarget(int numLevels)
        {
            return 0;
        }

        internal abstract bool ShouldHandleModification(TRScriptedLevelEventArgs e);
        internal abstract void ProcessModification(TRScriptedLevelEventArgs e);

        /// <summary>
        /// Called after any modifications have been actioned as received from AbstractTRScriptEditor edit events.
        /// </summary>
        /// <param name="scriptEditor">A reference to the current script editor.</param>
        /// <param name="monitor">The save monitor for publishing save progress.</param>
        protected virtual void SaveImpl(AbstractTRScriptEditor scriptEditor, TRSaveMonitor monitor) { }
        
        /// <summary>
        /// Depending on wheter AllowSuccessiveEdits is set this will either return the current
        /// file in the output directory or the file that was originally backed up.
        /// </summary>
        protected virtual string GetReadLevelFilePath(string levelFileName)
        {
            return Path.Combine(AllowSuccessiveEdits ?_io.OutputDirectory.FullName : _io.BackupDirectory.FullName, levelFileName);
        }

        /// <summary>
        /// All writes are sent to the temporary WIP directory and are only moved to the output
        /// and target directories at the end of the save chain. See TREditor.
        /// </summary>
        protected virtual string GetWriteLevelFilePath(string levelFileName)
        {
            return Path.Combine(_io.WIPOutputDirectory.FullName, levelFileName);
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