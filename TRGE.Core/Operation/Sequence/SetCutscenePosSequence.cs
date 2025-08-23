namespace TRGE.Core;

public class SetCutscenePosSequence : BaseLevelSequence 
{
    public int? X { get; set; }
    public int? Y { get; set; }
    public int? Z { get; set; }
}

public class SetCutsceneAngleSequence : BaseLevelSequence
{
    public int Value { get; set; }
}
