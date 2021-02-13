using TRGE.Core;

namespace TRGE.Coord
{
    internal static class TRLevelEditorFactory
    {
        internal static AbstractTRLevelEditor GetLevelEditor(TRDirectoryIOArgs io, TREdition edition)
        {
            switch (edition.Version)
            {
                case TRVersion.TR2:
                case TRVersion.TR2G:
                    return new TR2LevelEditor(io);
                default:
                    return null;
            }
        }

        internal static bool EditionSupportsLevelEditing(TREdition edition)
        {
            switch (edition.Version)
            {
                case TRVersion.TR2:
                case TRVersion.TR2G:
                    return true;
                default:
                    return false;
            }
        }
    }
}