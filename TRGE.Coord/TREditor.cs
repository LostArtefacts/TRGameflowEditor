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

        public AbstractTRLevelEditor LevelEditor { get; internal set; }

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

        public void Save()
        {
            ScriptEditor.Save();
            if (LevelEditor != null)
            {
                LevelEditor.Save(ScriptEditor);
            }

            DirectoryInfo outputDir = new DirectoryInfo(_outputDirectory);
            DirectoryInfo targetDir = new DirectoryInfo(_targetDirectory);
            outputDir.Copy(targetDir, true, TargetFileExtensions);

            ScriptEditor.Initialise();
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