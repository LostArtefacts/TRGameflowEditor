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

        private void ScriptManagerLevelModified(object sender, TRScriptedLevelEventArgs e)
        {
            
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