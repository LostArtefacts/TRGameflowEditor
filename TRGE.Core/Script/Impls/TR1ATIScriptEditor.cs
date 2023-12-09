namespace TRGE.Core;

public class TR1ATIScriptEditor : AbstractTRScriptEditor
{
    internal TR1ATIScriptEditor(TRScriptIOArgs ioArgs, TRScriptOpenOption openOption)
        : base(ioArgs, openOption) { }

    protected override void ApplyConfig(Config config) { }

    protected override void SaveImpl(AbstractTRScript backupScript, AbstractTRLevelManager backupLevelManager) { }

    internal override AbstractTRScript CreateScript()
    {
        return new TR1ATIScript();
    }

    protected override void ProcessGameMode(AbstractTRScript backupScript, AbstractTRLevelManager backupLevelManager)
    {
        throw new NotImplementedException();
    }
}