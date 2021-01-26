using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;
using TRGE.Coord;

namespace TRGE.Core.Test
{
    [TestClass]
    public class TR2PCItemTests : AbstractTR2ItemTestCollection
    {
        protected override int ScriptFileIndex => 0;

        internal override Dictionary<string, List<TRItem>> ManualBonusData => new Dictionary<string, List<TRItem>>
        {
            { 
                AbstractTRScriptedLevel.CreateID(@"data\wall.TR2"), new List<TRItem>
                {
                    ExpectedItems[2], ExpectedItems[15]
                }
            },
            {
                AbstractTRScriptedLevel.CreateID(@"data\boat.TR2"), new List<TRItem>
                {
                    ExpectedItems[6], ExpectedItems[13], ExpectedItems[14]
                }
            }
        };

        [TestMethod]
        protected void TestRandomiseItemsOutput()
        {
            TR23ScriptEditor sm = TRCoord.Instance.Open(_validScripts[ScriptFileIndex]).ScriptEditor as TR23ScriptEditor;
            try
            {
                sm.BonusOrganisation = Organisation.Random;
                sm.BonusRNG = new RandomGenerator(RandomGenerator.Type.UnixTime);
                int r = sm.BonusRNG.Value;
                sm.BonusRNG.RNGType = RandomGenerator.Type.Custom;
                sm.BonusRNG.Value = r;

                List<string> output = new List<string>
                {
                    "Index,Level,Item Type,Item,Quantity"
                };

                for (int i = 0; i < 10; i++)
                {
                    if (i > 0)
                    {
                        ++sm.BonusRNG.Value;
                    }

                    sm.RandomiseBonuses();

                    foreach (MutableTuple<string, string, List<MutableTuple<ushort, TRItemCategory, string, int>>> levelBonusData in sm.LevelBonusData)
                    {
                        foreach (MutableTuple<ushort, TRItemCategory, string, int> bonusItem in levelBonusData.Item3)
                        {
                            if (bonusItem.Item4 > 0)
                            {
                                output.Add(i + "," + levelBonusData.Item2 + "," + bonusItem.Item2 + "," + bonusItem.Item3 + "," + bonusItem.Item4);
                            }
                        }
                    }
                }

                File.WriteAllLines("TR2BonusRandomisation.csv", output.ToArray());
            }
            finally
            {
                
            }
        }
    }
}