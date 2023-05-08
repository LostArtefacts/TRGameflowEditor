using System.Collections.Generic;
using System.IO;
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

        private static readonly List<ushort> _ogRainLevels = new List<ushort> { 1, 3, 9, 12, 20 };
        private static readonly List<ushort> _ogSnowLevels = new List<ushort> { 16, 19 };
        private static readonly List<ushort> _ogColdLevels = new List<ushort> { 16, 17 };

        public override bool HasSecrets
        {
            get => _levelSecrets[Sequence] > 0;
            set { }
        }

        public override ushort NumSecrets
        {
            get => _levelSecrets[Sequence];
            set { }
        }

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

        public virtual bool HasRain
        {
            get => _ogRainLevels.Contains(Sequence);
            set { }
        }

        public virtual bool HasSnow
        {
            get => _ogSnowLevels.Contains(Sequence);
            set { }
        }

        public virtual bool HasColdWater
        {
            get => _ogColdLevels.Contains(Sequence);
            set { }
        }
        public override void SerializeToMain(BinaryWriter writer)
        {
            base.SerializeToMain(writer);

            writer.Write((byte)(HasRain ? 1 : 0));
            writer.Write((byte)(HasSnow ? 1 : 0));
            writer.Write((byte)(HasColdWater ? 1 : 0));
        }
    }
}