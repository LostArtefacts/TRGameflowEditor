namespace TRGE.Core;

public class TRRScriptEditor : AbstractTRScriptEditor
{
    internal TRRScriptEditor(TRScriptIOArgs ioArgs, TRScriptOpenOption openOption)
        : base(ioArgs, openOption) { }

    protected override void ApplyConfig(Config config) { }

    protected override void SaveImpl(AbstractTRScript backupScript, AbstractTRLevelManager backupLevelManager) { }

    internal override AbstractTRScript CreateScript()
    {
        return new TRRScript(TRVersion.Unknown);
    }

    protected override void ProcessGameMode(AbstractTRScript backupScript, AbstractTRLevelManager backupLevelManager)
    {
        throw new NotImplementedException();
    }
}
