using System.Collections.Generic;

namespace TRGE.Core
{
    internal class TR2ItemProvider : AbstractTR23ItemProvider
    {
        public TRItem Autos { get; protected set; }
        public TRItem M16 { get; protected set; }
        public TRItem AutoAmmo { get; protected set; }
        public TRItem M16Ammo { get; protected set; }

        internal TR2ItemProvider(IReadOnlyList<string> gameStrings)
            :base(gameStrings)
        {
            _weapons.Add(Autos = new TRItem(2, TRItemCategory.Weapon, gameStrings[38]));
            _weapons.Add(M16 = new TRItem(5, TRItemCategory.Weapon, gameStrings[41]));
            _weapons.Add(GLauncher = new TRItem(6, TRItemCategory.Weapon, gameStrings[42]));

            PistolAmmo = new TRItem(7, TRItemCategory.Ammo, gameStrings[44]);

            _ammo.Add(ShotgunAmmo = new TRItem(8, TRItemCategory.Ammo, gameStrings[45]));
            _ammo.Add(AutoAmmo = new TRItem(9, TRItemCategory.Ammo, gameStrings[46]));
            _ammo.Add(UziAmmo = new TRItem(10, TRItemCategory.Ammo, gameStrings[47]));
            _ammo.Add(HarpoonAmmo = new TRItem(11, TRItemCategory.Ammo, gameStrings[48]));
            _ammo.Add(M16Ammo = new TRItem(12, TRItemCategory.Ammo, gameStrings[49]));
            _ammo.Add(Grenades = new TRItem(13, TRItemCategory.Ammo, gameStrings[50]));

            _miscItems.Add(Flare = new TRItem(14, TRItemCategory.Misc, gameStrings[43]));

            _miscItems.Add(SmallMedi = new TRItem(15, TRItemCategory.Health, gameStrings[51]));
            _miscItems.Add(LargeMedi = new TRItem(16, TRItemCategory.Health, gameStrings[52]));

            _pickups.Add(Pickup1 = new TRItem(17, TRItemCategory.Pickup, gameStrings[53] + " 1"));
            _pickups.Add(Pickup2 = new TRItem(18, TRItemCategory.Pickup, gameStrings[53] + " 2"));
            _pickups.Add(Puzzle1 = new TRItem(19, TRItemCategory.Pickup, gameStrings[54] + " 1"));
            _pickups.Add(Puzzle2 = new TRItem(20, TRItemCategory.Pickup, gameStrings[54] + " 2"));
            _pickups.Add(Puzzle3 = new TRItem(21, TRItemCategory.Pickup, gameStrings[54] + " 3"));
            _pickups.Add(Puzzle4 = new TRItem(22, TRItemCategory.Pickup, gameStrings[54] + " 4"));
            _pickups.Add(Key1 = new TRItem(23, TRItemCategory.Pickup, gameStrings[55] + " 1"));
            _pickups.Add(Key2 = new TRItem(24, TRItemCategory.Pickup, gameStrings[55] + " 2"));
            _pickups.Add(Key3 = new TRItem(25, TRItemCategory.Pickup, gameStrings[55] + " 3"));
            _pickups.Add(Key4 = new TRItem(26, TRItemCategory.Pickup, gameStrings[55] + " 4"));

            _allItems.AddRange(_weapons);
            _allItems.Add(PistolAmmo);
            _allItems.AddRange(_miscItems);
            _allItems.AddRange(_pickups);

            SortAllItems();
        }
    }
}