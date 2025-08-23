using TRGE.Core.Item.Enums;

namespace TRGE.Core;

public class MeshSwapLevelSequence : BaseLevelSequence
{
    public TR1Items Object1ID { get; set; }
    public TR1Items Object2ID { get; set; }
    public int MeshID { get; set; }
}