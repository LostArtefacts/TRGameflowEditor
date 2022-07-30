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

        public bool HasRain { get; set; }
        public bool HasSnow { get; set; }
        public bool HasColdWater { get; set; }

        public override void SetDefaults()
        {
            base.SetDefaults();

            HasRain = HasSnow = HasColdWater = false;

            switch (OriginalSequence)
            {
                case 1:
                case 9:
                    // Jungle & Thames
                    HasRain = true;
                    break;
                case 16:
                    // Antarctica
                    HasSnow = true;
                    HasColdWater = true;
                    break;
                case 17:
                    // Antarctica & RX-Tech
                    HasColdWater = true;
                    break;
                case 19:
                    // Willie
                    HasSnow = true;
                    break;
            }
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