namespace TRGE.Core;

internal static class TRScriptedLevelFactory
{
    internal static AbstractTRLevelManager GetLevelManager(AbstractTRScript script)
    {
        if (script.Edition.Remastered)
        {
            return new TRRLevelManager(script as TRRScript);
        }

        return script.Edition.Version switch
        {
            TRVersion.TR1 or TRVersion.TR2 => new TRXLevelManager(script as TRXScript),
            TRVersion.TR3 or TRVersion.TR3G => new TR3LevelManager(script as TR3Script),
            _ => null,
        };
    }
}