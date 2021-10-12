using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;
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
            TR23ScriptEditor sm = TRCoord.Instance.Open(_validScripts[ScriptFileIndex]).ScriptEditor as TR23ScriptEditor;
            Assert.IsTrue(sm.LevelManager.ItemProvider is TR2ItemProvider);
            base.TestLoadItems();
        }

        [TestMethod]
        [TestSequence(1)]
        protected void TestRandomiseItems()
        {
            TR23ScriptEditor sm = TRCoord.Instance.Open(_validScripts[ScriptFileIndex]).ScriptEditor as TR23ScriptEditor;
            List<MutableTuple<string, string, List<MutableTuple<ushort, TRItemCategory, string, int>>>> originalBonusData = sm.LevelSecretBonusData;

            sm.SecretBonusOrganisation = Organisation.Random;
            sm.SecretBonusRNG = new RandomGenerator(RandomGenerator.Type.Date);
            //sm.BonusRNG = new RandomGenerator(RandomGenerator.Type.UnixTime);
            sm.RandomiseBonuses();

            List<MutableTuple<string, string, List<MutableTuple<ushort, TRItemCategory, string, int>>>> bonusData = sm.LevelSecretBonusData;
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
                    }
                }
            }
        }

        [TestMethod]
        [TestSequence(2)]
        protected void TestRandomiseItemsReload()
        {
            // This simulates randomising with a particular seed, saving, then manually setting
            // the bonus items, saving, and finally randomising again with the first seed to 
            // prove randomisation is consistent.

            TREditor editor = TRCoord.Instance.Open(_validScripts[ScriptFileIndex]);
            TR23ScriptEditor sm = editor.ScriptEditor as TR23ScriptEditor;

            sm.SecretBonusOrganisation = Organisation.Random;
            sm.SecretBonusRNG = new RandomGenerator(RandomGenerator.Type.Date);
            editor.Save();
            byte[] firstRandoBytes = File.ReadAllBytes(_validScripts[ScriptFileIndex]);

            sm.LevelSecretBonusData = ConvertManualBonusData(sm);
            sm.SecretBonusOrganisation = Organisation.Manual;
            editor.Save();
            byte[] manualBytes = File.ReadAllBytes(_validScripts[ScriptFileIndex]);

            CollectionAssert.AreNotEqual(firstRandoBytes, manualBytes);

            sm.SecretBonusOrganisation = Organisation.Random;
            sm.SecretBonusRNG = new RandomGenerator(RandomGenerator.Type.Date);
            editor.Save();
            byte[] secondRandoBytes = File.ReadAllBytes(_validScripts[ScriptFileIndex]);

            CollectionAssert.AreEqual(firstRandoBytes, secondRandoBytes);
        }

        [TestMethod]
        protected void TestItemEnum()
        {
            TR23ScriptEditor sm = TRCoord.Instance.Open(_validScripts[ScriptFileIndex]).ScriptEditor as TR23ScriptEditor;
            foreach (TR2ScriptedLevel level in sm.LevelManager.Levels)
            {
                level.GetBonusItemIDs();
                level.GetStartInventoryItemIDs();
            }
        }

        [TestMethod]
        protected void TestReorganiseItems()
        {
            TR23ScriptEditor sm = TRCoord.Instance.Open(_validScripts[ScriptFileIndex]).ScriptEditor as TR23ScriptEditor;
            sm.SecretBonusOrganisation = Organisation.Manual;
                
            Dictionary<string, List<TRItem>> originalBonusData = (sm.LevelManager as TR23LevelManager).GetLevelBonusItems();
            CollectionAssert.AreNotEqual(originalBonusData, ManualBonusData);

            List<MutableTuple<string, string, List<MutableTuple<ushort, TRItemCategory, string, int>>>> newBonusData = ConvertManualBonusData(sm);
            sm.LevelSecretBonusData = newBonusData;

            originalBonusData = (sm.LevelManager as TR23LevelManager).GetLevelBonusItems();
            foreach (string levelFile in ManualBonusData.Keys)
            {
                Assert.IsTrue(originalBonusData.ContainsKey(levelFile));
                CollectionAssert.AreEquivalent(ManualBonusData[levelFile], originalBonusData[levelFile]);
            }
        }

        private List<MutableTuple<string, string, List<MutableTuple<ushort, TRItemCategory, string, int>>>> ConvertManualBonusData(TR23ScriptEditor sm)
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
            foreach (var n in sm.LevelSecretBonusData)
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