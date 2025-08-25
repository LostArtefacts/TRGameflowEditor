namespace TRGE.Core;

public class TR3FrontEnd : AbstractTRFrontEnd
{
    public override bool HasFMV
    {
        get => HasActiveOperation(TR3OpDefs.FMV);
        set => SetOperationActive(TR3OpDefs.FMV, value);
    }

    protected override TROpDef GetOpDefFor(ushort scriptData)
    {
        return TR3OpDefs.Get(scriptData);
    }
}