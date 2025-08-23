namespace TRGE.Core;

public class DisplayPictureSequence : BaseLevelSequence
{
    public string Path { get; set; }
    public double? DisplayTime { get; set; }
    public double? FadeInTime { get; set; }
    public double? FadeOutTime { get; set; }
}
