namespace TRGE.Core;

public class TRXFrontEnd : AbstractTRFrontEnd
{
    public TRXScriptedLevel TitleLevel { get; set; }

    public override bool HasFMV
    {
        get => TitleLevel.HasFMV;
        set => TitleLevel.HasFMV = value;
    }

    public ushort TrackID
    {
        get => TitleLevel.TrackID;
        set => TitleLevel.TrackID = value;
    }

    protected override TROpDef GetOpDefFor(ushort scriptData)
    {
        return null;
    }
}