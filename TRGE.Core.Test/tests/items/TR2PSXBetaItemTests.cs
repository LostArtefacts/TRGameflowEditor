using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace TRGE.Core.Test
{
    [TestClass]
    public class TR2PSXBetaItemTests : AbstractTR2ItemTestCollection
    {
        protected override int ScriptFileIndex => 3;
        internal override Dictionary<string, List<TRItem>> ManualBonusData => new Dictionary<string, List<TRItem>>
        {
            { Hashing.CreateMD5(@"data\venice.PSX"), new List<TRItem>
                {
                    new TRItem(2, TRItemCategory.Weapon, "Automatic Pistols"),
                    new TRItem(15, TRItemCategory.Health, "Small Medi Kit")
                }
            },
            { Hashing.CreateMD5(@"data\floating.PSX"), new List<TRItem>
                {
                    new TRItem(6, TRItemCategory.Weapon, "Grenade Launcher"),
                    new TRItem(13, TRItemCategory.Ammo, "Grenades"),
                    new TRItem(14, TRItemCategory.Misc, "Flares")
                }
            }
        };
    }
}