using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using TRGE.Core;

namespace TRGE.Coord
{
    public class TREditor
    {
        public static readonly string[] TargetFileExtensions = new string[] { "*.dat", "*.tr2", "*.psx" };

        private AbstractTRScriptEditor _scriptEditor;
        public AbstractTRScriptEditor ScriptEditor
        {
            get => _scriptEditor;
            internal set
            {
                _scriptEditor = value;
                _scriptEditor.LevelModified += ScriptEditorLevelModified;
            }
        }

        private AbstractTRLevelEditor _levelEditor;
        public AbstractTRLevelEditor LevelEditor
        {
            get => _levelEditor;
            internal set
            {
                if ((_levelEditor = value) != null)
                {
                    _levelEditor.Initialise(ScriptEditor);
                }
            }
        }

        public bool AllowSuccessiveEdits 
        {
            get => _scriptEditor.AllowSuccessiveEdits && (LevelEditor == null || LevelEditor.AllowSuccessiveEdits);
            set
            {
                _scriptEditor.AllowSuccessiveEdits = value;
                if (LevelEditor != null)
                {
                    LevelEditor.AllowSuccessiveEdits = value;
                }
            }
        }

        public TREdition Edition => _scriptEditor.Edition;
        public string BackupDirectory => _scriptEditor.BackupFile.DirectoryName;
        public string TargetDirectory => _targetDirectory;
        private readonly string _outputDirectory;
        private readonly string _targetDirectory;

        public event EventHandler<TRSaveEventArgs> SaveProgressChanged;

        public bool IsExportPossible => ScriptEditor.IsExportPossible && (LevelEditor == null || LevelEditor.IsExportPossible);

        internal TREditor(string outputDirectory, string targetDirectory)
        {
            _outputDirectory = outputDirectory;
            _targetDirectory = targetDirectory;
        }

        internal static void ValidateCompatibility(AbstractTRScript script, string directoryPath)
        {
            if (TRLevelEditorFactory.EditionSupportsLevelEditing(script.Edition))
            {
                AbstractTRLevelEditor.ValidateCompatibility(script.Levels, directoryPath);
            }
        }

        private void ScriptEditorLevelModified(object sender, TRScriptedLevelEventArgs e)
        {
            if (LevelEditor != null)
            {
                LevelEditor.ScriptedLevelModified(e);
            }
        }

        private void Editor_SaveStateChanged(object sender, TRSaveEventArgs e)
        {
            FireSaveProgressChanged(e);
        }

        private void FireSaveProgressChanged(TRSaveEventArgs e)
        {
            SaveProgressChanged?.Invoke(this, e);
        }

        public void Save()
        {
            TRSaveMonitor monitor = new TRSaveMonitor(new TRSaveEventArgs
            {
                ProgressTarget = ScriptEditor.GetSaveTargetCount() + 
                (LevelEditor == null ? 0 : LevelEditor.GetSaveTargetCount())
            });
            monitor.SaveStateChanged += Editor_SaveStateChanged;

            ScriptEditor.Save(monitor);

            if (LevelEditor != null)
            {
                LevelEditor.Save(ScriptEditor, monitor);
            }

            monitor.FireSaveStateChanged(0, TRSaveCategory.Commit);

            DirectoryInfo outputDir = new DirectoryInfo(_outputDirectory);
            DirectoryInfo targetDir = new DirectoryInfo(_targetDirectory);
            outputDir.Copy(targetDir, true, TargetFileExtensions);

            ScriptEditor.Initialise();
            if (LevelEditor != null)
            {
                LevelEditor.Initialise(ScriptEditor);
            }
        }

        public void Restore()
        {
            ScriptEditor.Restore();
            if (LevelEditor != null)
            {
                LevelEditor.Restore();
            }
        }

        public void ExportSettings(string filePath)
        {
            if (!IsExportPossible)
            {
                throw new InvalidOperationException();
            }

            Dictionary<string, object> config = new Dictionary<string, object>
            {
                ["TRGE"] = ScriptEditor.ExportConfig()
            };
            if (LevelEditor != null)
            {
                config["TRLE"] = LevelEditor.ExportConfig();
            }

            new FileInfo(filePath).WriteCompressedText(JsonConvert.SerializeObject(config, Formatting.None));
        }

        public void ImportSettings(string filePath)
        {
            Dictionary<string, object> config = JsonConvert.DeserializeObject<Dictionary<string, object>>(new FileInfo(filePath).ReadCompressedText());
            if (config.ContainsKey("TRGE"))
            {
                ScriptEditor.ImportConfig(JsonConvert.DeserializeObject<Dictionary<string, object>>(config["TRGE"].ToString()));
            }
            if (config.ContainsKey("TRLE") && LevelEditor != null)
            {
                LevelEditor.ImportConfig(JsonConvert.DeserializeObject<Dictionary<string, object>>(config["TRLE"].ToString()));
            }
        }
    }
}