using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace TRGE.Core.Test
{
    public abstract class AbstractTR3ItemTestCollection : AbstractTR23ItemTestCollection
    {
        internal override List<TRItem> ExpectedItems => new List<TRItem>
        {
            new TRItem(0,  TRItemCategory.Weapon, "Pistols"),
            new TRItem(1,  TRItemCategory.Weapon, "Shotgun"),
            new TRItem(2,  TRItemCategory.Weapon, "Desert Eagle"),
            new TRItem(3,  TRItemCategory.Weapon, "Uzis"),
            new TRItem(4,  TRItemCategory.Weapon, "Harpoon Gun"),
            new TRItem(5,  TRItemCategory.Weapon, "MP5"),
            new TRItem(6,  TRItemCategory.Weapon, "Rocket Launcher"),
            new TRItem(7,  TRItemCategory.Weapon, "Grenade Launcher"),
            new TRItem(8,  TRItemCategory.Ammo,   "Pistol Clips"),
            new TRItem(9,  TRItemCategory.Ammo,   "Shotgun Shells"),
            new TRItem(10, TRItemCategory.Ammo,   "Desert Eagle Clips"),
            new TRItem(11, TRItemCategory.Ammo,   "Uzi Clips"),
            new TRItem(12, TRItemCategory.Ammo,   "Harpoons"),
            new TRItem(13, TRItemCategory.Ammo,   "MP5 Clips"),
            new TRItem(14, TRItemCategory.Ammo,   "Rockets"),
            new TRItem(15, TRItemCategory.Ammo,   "Grenades"),
            new TRItem(16, TRItemCategory.Misc,   "Flare"),
            new TRItem(17, TRItemCategory.Health, "Small Medi Pack"),
            new TRItem(18, TRItemCategory.Health, "Large Medi Pack"),
            new TRItem(29, TRItemCategory.Health, "Savegame Crystal"),
            new TRItem(19, TRItemCategory.Pickup, "Pickup 1"),
            new TRItem(20, TRItemCategory.Pickup, "Pickup 2"),
            new TRItem(21, TRItemCategory.Pickup, "Puzzle 1"),
            new TRItem(22, TRItemCategory.Pickup, "Puzzle 2"),
            new TRItem(23, TRItemCategory.Pickup, "Puzzle 3"),
            new TRItem(24, TRItemCategory.Pickup, "Puzzle 4"),
            new TRItem(25, TRItemCategory.Pickup, "Key 1"),
            new TRItem(26, TRItemCategory.Pickup, "Key 2"),
            new TRItem(27, TRItemCategory.Pickup, "Key 3"),
            new TRItem(28, TRItemCategory.Pickup, "Key 4")
        };

        [TestMethod]
        [TestSequence(0)]
        protected override void TestLoadItems()
        {
            TR23ScriptManager sm = TRGameflowEditor.Instance.GetScriptManager(_validScripts[ScriptFileIndex]) as TR23ScriptManager;
            Assert.IsTrue(sm.LevelManager.ItemProvider is TR3ItemProvider);
            base.TestLoadItems();
        }
    }
}