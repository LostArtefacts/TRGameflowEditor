namespace TRGE.Core
{
    internal static class TRScriptedLevelFactory
    {
        internal static AbstractTRLevelManager GetLevelManager(AbstractTRScript script)
        {
            switch (script.Edition.Version)
            {
                case TRVersion.TR2:
                case TRVersion.TR2G:
                case TRVersion.TR3:
                case TRVersion.TR3G:
                    return new TR23LevelManager(script as TR23Script);
                default:
                    return null;
            }
        }
    }
}