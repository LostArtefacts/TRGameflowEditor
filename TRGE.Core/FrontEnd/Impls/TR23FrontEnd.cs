namespace TRGE.Core;

public class TR23FrontEnd : AbstractTRFrontEnd
{
    public override bool HasFMV
    {
        get => HasActiveOperation(TR23OpDefs.FMV);
        set => SetOperationActive(TR23OpDefs.FMV, value);
    }

    protected override TROpDef GetOpDefFor(ushort scriptData)
    {
        return TR23OpDefs.Get(scriptData);
    }
}