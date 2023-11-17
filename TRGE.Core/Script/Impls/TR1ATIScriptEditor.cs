namespace TRGE.Core;

public class TR1ATIScriptEditor : AbstractTRScriptEditor
{
    internal TR1ATIScriptEditor(TRScriptIOArgs ioArgs, TRScriptOpenOption openOption)
        : base(ioArgs, openOption) { }

    protected override void ApplyConfig(Config config) { }

    protected override void SaveImpl() { }

    internal override AbstractTRScript CreateScript()
    {
        return new TR1ATIScript();
    }
}