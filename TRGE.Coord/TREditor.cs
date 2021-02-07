using System;
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
                _scriptEditor.SaveStateChanged += Editor_SaveStateChanged;
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
                    _levelEditor.SaveStateChanged += Editor_SaveStateChanged;
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

        public string BackupDirectory => _scriptEditor.BackupFile.DirectoryName;
        public string TargetDirectory => _targetDirectory;

        public TREdition Edition => _scriptEditor.Edition;

        private readonly string _outputDirectory;
        private readonly string _targetDirectory;

        public event EventHandler<TRSaveEventArgs> SaveProgressChanged;

        internal TREditor(string outputDirectory, string targetDirectory)
        {
            _outputDirectory = outputDirectory;
            _targetDirectory = targetDirectory;
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
            TRSaveEventArgs e = new TRSaveEventArgs
            {
                ProgressTarget = ScriptEditor.LevelManager.LevelCount + 1,
                ProgressValue = 0
            };

            ScriptEditor.Save(e);

            if (LevelEditor != null)
            {
                LevelEditor.Save(ScriptEditor, e);
            }

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
    }
}