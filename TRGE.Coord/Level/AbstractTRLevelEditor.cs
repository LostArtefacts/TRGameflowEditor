using TRGE.Core;

namespace TRGE.Coord
{
    public abstract class AbstractTRLevelEditor
    {
        protected readonly TRDirectoryIOArgs _io;

        internal AbstractTRLevelEditor(TRDirectoryIOArgs io)
        {
            _io = io;
        }

        internal abstract void ScriptedLevelModified(TRScriptedLevelEventArgs e);
        internal abstract void Save(AbstractTRScriptEditor scriptEditor);
        internal abstract void Restore();
    }
}