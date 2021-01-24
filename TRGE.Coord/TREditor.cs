using TRGE.Core;

namespace TRGE.Coord
{
    public class TREditor
    {
        private AbstractTRScriptManager _scriptManager;
        public AbstractTRScriptManager ScriptManager
        {
            get => _scriptManager;
            internal set
            {
                _scriptManager = value;
                _scriptManager.LevelModified += ScriptManagerLevelModified;
            }
        }

        internal TRLevelEditor LevelEditor;

        private void ScriptManagerLevelModified(object sender, TRScriptedLevelEventArgs e)
        {
            if (LevelEditor != null)
            {
                LevelEditor.LevelModified(e);
            }
        }

        public void Save()
        {
            _scriptManager.Save();
        }

        public void Restore()
        {
            _scriptManager.Restore();
        }
    }
}