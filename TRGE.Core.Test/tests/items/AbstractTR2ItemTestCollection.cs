using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace TRGE.Core.Test
{
    public abstract class AbstractTR2ItemTestCollection : BaseTestCollection
    {
        protected abstract int ScriptFileIndex { get; }
        internal abstract Dictionary<string, List<TRItem>> ManualBonusData { get; }

        [TestMethod]
        [TestSequence(0)]
        protected virtual void TestLoadLevels()
        {
            TR23ScriptManager sm = TRGameflowEditor.Instance.GetScriptManager(_validScripts[ScriptFileIndex]) as TR23ScriptManager;
            Assert.IsTrue(sm.LevelManager.ItemProvider is TR2ItemProvider);
        }

        [TestMethod]
        protected void TestRandomiseLevels()
        {
            TR23ScriptManager sm = TRGameflowEditor.Instance.GetScriptManager(_validScripts[ScriptFileIndex]) as TR23ScriptManager;
            try
            {
                List<MutableTuple<string, string, List<MutableTuple<ushort, TRItemCategory, string, int>>>> originalBonusData = sm.LevelBonusData;

                sm.BonusOrganisation = Organisation.Random;
                sm.BonusRNG = new RandomGenerator(RandomGenerator.Type.Date);
                sm.RandomiseBonuses();

                List<MutableTuple<string, string, List<MutableTuple<ushort, TRItemCategory, string, int>>>> bonusData = sm.LevelBonusData;
                CollectionAssert.AreNotEqual(originalBonusData, bonusData);

                HashSet<ushort> weapons = new HashSet<ushort>();
                foreach (MutableTuple<string, string, List<MutableTuple<ushort, TRItemCategory, string, int>>> levelBonusData in bonusData)
                {
                    foreach (MutableTuple<ushort, TRItemCategory, string, int> bonusItem in levelBonusData.Item3)
                    {
                        if (bonusItem.Item4 > 0)
                        {
                            int levelWeaponCount = 0;
                            if (bonusItem.Item2 == TRItemCategory.Weapon)
                            {
                                levelWeaponCount++;
                                if (!weapons.Add(bonusItem.Item1))
                                {
                                    Assert.Fail(string.Format("{0} already seen", bonusItem.Item3));
                                }
                                if (levelWeaponCount > 1)
                                {
                                    Assert.Fail(string.Format("More than one weapon in {0}", levelBonusData.Item2));
                                }
                            }

                            //Console.WriteLine(levelBonusData.Item2 + ": " + bonusItem.Item3 + " x " + bonusItem.Item4);
                        }
                    }
                }
            }
            finally
            {
                TRGameflowEditor.Instance.CloseScriptManager(sm);
            }
        }

        [TestMethod]
        protected void TestReorganiseLevels()
        {
            TR23ScriptManager sm = TRGameflowEditor.Instance.GetScriptManager(_validScripts[ScriptFileIndex]) as TR23ScriptManager;
            try
            {
                sm.BonusOrganisation = Organisation.Manual;
                
                Dictionary<string, List<TRItem>> originalBonusData = (sm.LevelManager as TR23LevelManager).GetLevelBonusItems();
                CollectionAssert.AreNotEqual(originalBonusData, ManualBonusData);

                List<MutableTuple<string, string, List<MutableTuple<ushort, TRItemCategory, string, int>>>> newBonusData = ConvertManualBonusData(sm);
                sm.LevelBonusData = newBonusData;

                originalBonusData = (sm.LevelManager as TR23LevelManager).GetLevelBonusItems();
                foreach (string levelFile in ManualBonusData.Keys)
                {
                    Assert.IsTrue(originalBonusData.ContainsKey(levelFile));
                    CollectionAssert.AreEquivalent(ManualBonusData[levelFile], originalBonusData[levelFile]);
                }
            }
            finally
            {
                TRGameflowEditor.Instance.CloseScriptManager(sm);
            }
        }

        private List<MutableTuple<string, string, List<MutableTuple<ushort, TRItemCategory, string, int>>>> ConvertManualBonusData(TR23ScriptManager sm)
        {
            List<MutableTuple<string, string, List<MutableTuple<ushort, TRItemCategory, string, int>>>> ret = new List<MutableTuple<string, string, List<MutableTuple<ushort, TRItemCategory, string, int>>>>();
            foreach (string levelFile in ManualBonusData.Keys)
            {
                List<MutableTuple<ushort, TRItemCategory, string, int>> levelData = new List<MutableTuple<ushort, TRItemCategory, string, int>>();
                foreach (TRItem item in ManualBonusData[levelFile])
                {
                    levelData.Add(new MutableTuple<ushort, TRItemCategory, string, int>(item.ID, item.Category, item.Name, 1));
                }
                ret.Add(new MutableTuple<string, string, List<MutableTuple<ushort, TRItemCategory, string, int>>>(levelFile, string.Empty, levelData));
            }

            //copy over whatever is there by default
            foreach (var n in sm.LevelBonusData)
            {
                if (!ManualBonusData.ContainsKey(n.Item1))
                {
                    ret.Add(n);
                }
            }

            return ret;
        }
    }
}