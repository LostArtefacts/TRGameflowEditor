using System.Collections.Generic;
using System.Linq;
using TRGE.Core.Item.Enums;

namespace TRGE.Core
{
    public class TR3ScriptedLevel : TR2ScriptedLevel
    {
        // The number of secrets is hardcoded in TR3 based on the level sequence unfortunately.
        // The sequencing here starts from Lara's Home.
        private static readonly ushort[] _levelSecrets = new ushort[21]
        {
            0, 6, 4, 5, 0, 3, 3, 3, 1, 5, 5, 6, 1, 3, 2, 3, 3, 3, 3, 0, 0
        };

        public override bool HasSecrets
        {
            get => _levelSecrets[Sequence] > 0;
            set { }
        }

        public override ushort NumSecrets => _levelSecrets[Sequence];

        public void AddStartInventoryItem(TR3Items item, uint count = 1)
        {
            AddStartInventoryItem((ushort)item, count);
        }

        public void RemoveStartInventoryItem(TR3Items item, bool removeAll = false)
        {
            RemoveStartInventoryItem((ushort)item, removeAll);
        }

        public void SetStartInventoryItems(Dictionary<TR3Items, int> items)
        {
            SetStartInventoryItems(items.ToDictionary(item => (ushort)item.Key, item => item.Value));
        }
    }
}