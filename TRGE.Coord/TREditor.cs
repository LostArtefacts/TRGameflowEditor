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

        internal TRLevelEditor LevelEditor;

        private void ScriptEditorLevelModified(object sender, TRScriptedLevelEventArgs e)
        {
            if (LevelEditor != null)
            {
                LevelEditor.ScriptedLevelModified(e);
            }
        }

        public void Save()
        {
            _scriptEditor.Save();
        }

        public void Restore()
        {
            _scriptEditor.Restore();
        }
    }
}