using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using TRGE.Coord;

namespace TRGE.Core.Test
{
    public abstract class AbstractTR2ItemTestCollection : AbstractTR23ItemTestCollection
    {
        internal abstract Dictionary<string, List<TRItem>> ManualBonusData { get; }

        internal override List<TRItem> ExpectedItems => new List<TRItem>
        {
            new TRItem(0,  TRItemCategory.Weapon, "Pistols"),
            new TRItem(1,  TRItemCategory.Weapon, "Shotgun"),
            new TRItem(2,  TRItemCategory.Weapon, "Automatic Pistols"),
            new TRItem(3,  TRItemCategory.Weapon, "Uzis"),
            new TRItem(4,  TRItemCategory.Weapon, "Harpoon Gun"),
            new TRItem(5,  TRItemCategory.Weapon, "M16"),
            new TRItem(6,  TRItemCategory.Weapon, "Grenade Launcher"),
            new TRItem(7,  TRItemCategory.Ammo,   "Pistol Clips"),
            new TRItem(8,  TRItemCategory.Ammo,   "Shotgun Shells"),
            new TRItem(9,  TRItemCategory.Ammo,   "Automatic Pistol Clips"),
            new TRItem(10, TRItemCategory.Ammo,   "Uzi Clips"),
            new TRItem(11, TRItemCategory.Ammo,   "Harpoons"),
            new TRItem(12, TRItemCategory.Ammo,   "M16 Clips"),
            new TRItem(13, TRItemCategory.Ammo,   "Grenades"),
            new TRItem(14, TRItemCategory.Misc,   "Flare"),
            new TRItem(15, TRItemCategory.Health, "Small Medi Pack"),
            new TRItem(16, TRItemCategory.Health, "Large Medi Pack"),
            new TRItem(17, TRItemCategory.Pickup, "Pickup 1"),
            new TRItem(18, TRItemCategory.Pickup, "Pickup 2"),
            new TRItem(19, TRItemCategory.Pickup, "Puzzle 1"),
            new TRItem(20, TRItemCategory.Pickup, "Puzzle 2"),
            new TRItem(21, TRItemCategory.Pickup, "Puzzle 3"),
            new TRItem(22, TRItemCategory.Pickup, "Puzzle 4"),
            new TRItem(23, TRItemCategory.Pickup, "Key 1"),
            new TRItem(24, TRItemCategory.Pickup, "Key 2"),
            new TRItem(25, TRItemCategory.Pickup, "Key 3"),
            new TRItem(26, TRItemCategory.Pickup, "Key 4")
        };

        [TestMethod]
        [TestSequence(0)]
        protected override void TestLoadItems()
        {
            TR23ScriptManager sm = TRCoord.Instance.OpenScript(_validScripts[ScriptFileIndex]) as TR23ScriptManager;
            Assert.IsTrue(sm.LevelManager.ItemProvider is TR2ItemProvider);
            base.TestLoadItems();
        }

        [TestMethod]
        protected void TestRandomiseItems()
        {
            TR23ScriptManager sm = TRCoord.Instance.OpenScript(_validScripts[ScriptFileIndex]) as TR23ScriptManager;
            List<MutableTuple<string, string, List<MutableTuple<ushort, TRItemCategory, string, int>>>> originalBonusData = sm.LevelBonusData;

                sm.BonusOrganisation = Organisation.Random;
                sm.BonusRNG = new RandomGenerator(RandomGenerator.Type.Date);
                //sm.BonusRNG = new RandomGenerator(RandomGenerator.Type.UnixTime);
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

                        //System.Console.WriteLine(levelBonusData.Item2 + "," + bonusItem.Item3 + "," + bonusItem.Item4);
                    }
                }
            }
        }

        [TestMethod]
        protected void TestReorganiseItems()
        {
            TR23ScriptManager sm = TRCoord.Instance.OpenScript(_validScripts[ScriptFileIndex]) as TR23ScriptManager;
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