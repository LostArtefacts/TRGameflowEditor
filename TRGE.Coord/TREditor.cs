using System.IO;
using TRGE.Core;

namespace TRGE.Coord
{
    public class TREditor
    {
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

        internal AbstractTRLevelEditor LevelEditor;

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
            DirectoryInfo outputDir = new DirectoryInfo(_outputDirectory);
            DirectoryInfo targetDir = new DirectoryInfo(_targetDirectory);
            outputDir.Clear();

            ScriptEditor.Save();
            if (LevelEditor != null)
            {
                LevelEditor.Save(ScriptEditor);
            }

            outputDir.Copy(targetDir);
            ScriptEditor.Initialise();
        }

        public void Restore()
        {
            ScriptEditor.Restore();
        }
    }
}