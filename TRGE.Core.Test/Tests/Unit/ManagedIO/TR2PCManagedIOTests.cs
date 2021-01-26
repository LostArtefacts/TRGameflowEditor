using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using TRGE.Coord;

namespace TRGE.Core.Test
{
    [TestClass]
    public class TR2PCManagedIOTests : AbstractTR23ManagedIOTestCollection
    {
        protected override int ScriptFileIndex => 0;

        [TestMethod]
        protected void TestBonusRandomisation()
        {
            List<MutableTuple<string, string, List<MutableTuple<ushort, TRItemCategory, string, int>>>> bonusData;
            TREditor editor = TRCoord.Instance.Open(_validScripts[ScriptFileIndex]);
            TR23ScriptEditor sm = editor.ScriptEditor as TR23ScriptEditor;
            bonusData = sm.LevelBonusData;

            sm.BonusOrganisation = Organisation.Random;
            sm.BonusRNG = new RandomGenerator(RandomGenerator.Type.UnixTime);

            editor.Save();
            
            sm = TRCoord.Instance.Open(_validScripts[ScriptFileIndex]).ScriptEditor as TR23ScriptEditor;
            
            //CollectionAssert.AreEqual is failing here for some reason, hence the more manual approach
            List<MutableTuple<string, string, List<MutableTuple<ushort, TRItemCategory, string, int>>>> newBonusData = sm.LevelBonusData;
            Assert.AreEqual(bonusData.Count, newBonusData.Count);
            for (int i = 0; i < bonusData.Count; i++)
            {
                MutableTuple<string, string, List<MutableTuple<ushort, TRItemCategory, string, int>>> m1 = bonusData[i];
                MutableTuple<string, string, List<MutableTuple<ushort, TRItemCategory, string, int>>> m2 = newBonusData[i];
                Assert.AreEqual(m1.Item1, m2.Item1);
                Assert.AreEqual(m1.Item2, m2.Item2);
                Assert.AreEqual(m1.Item3.Count, m2.Item3.Count);
                for (int j = 0; j < m1.Item3.Count; j++)
                {
                    MutableTuple<ushort, TRItemCategory, string, int> t1 = m1.Item3[j];
                    MutableTuple<ushort, TRItemCategory, string, int> t2 = m2.Item3[j];
                    Assert.AreEqual(t1.Item1, t2.Item1);
                    Assert.AreEqual(t1.Item2, t2.Item2);
                    Assert.AreEqual(t1.Item3, t2.Item3);
                    Assert.AreEqual(t1.Item4, t2.Item4);
                }
            }
        }
    }
}