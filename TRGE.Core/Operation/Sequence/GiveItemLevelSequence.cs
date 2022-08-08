using TRGE.Core.Item.Enums;

namespace TRGE.Core
{
    public class GiveItemLevelSequence : BaseLevelSequence
    {
        public TR1Items ObjectId { get; set; }
        public int Quantity { get; set; }
    }
}