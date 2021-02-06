using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using TRGE.Core;

namespace TRGE.Coord
{
    public abstract class AbstractTRLevelEditor
    {
        protected readonly TRDirectoryIOArgs _io;
        protected Dictionary<string, object> _config;

        internal bool AllowSuccessiveEdits { get; set; }

        internal AbstractTRLevelEditor(TRDirectoryIOArgs io)
        {
            _io = io;
            LoadConfig();
        }

        private void LoadConfig()
        {
            _config = _io.ConfigFile.Exists ? JsonConvert.DeserializeObject<Dictionary<string, object>>(_io.ConfigFile.ReadCompressedText()) : null;
            if (_config != null)
            {
                AllowSuccessiveEdits = bool.Parse(_config["Successive"].ToString());
                ApplyConfig();
            }
        }

        internal void Save(AbstractTRScriptEditor scriptEditor)
        {
            _config = new Dictionary<string, object>
            {
                ["Version"] = Assembly.GetExecutingAssembly().GetName().Version,
                ["Successive"] = AllowSuccessiveEdits
            };

            SaveImpl(scriptEditor);

            _io.ConfigFile.WriteCompressedText(JsonConvert.SerializeObject(_config, Formatting.None)); //#48
        }

        protected virtual void ApplyConfig() { }
        internal abstract void ScriptedLevelModified(TRScriptedLevelEventArgs e);
        internal abstract void SaveImpl(AbstractTRScriptEditor scriptEditor);
        internal abstract void Restore();

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
    }
}