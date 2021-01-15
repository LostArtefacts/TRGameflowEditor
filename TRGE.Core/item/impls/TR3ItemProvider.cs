using System.Collections.Generic;

namespace TRGE.Core
{
    internal class TR3ItemProvider : AbstractTR23ItemProvider
    {
        public TRItem DEagle { get; protected set; }
        public TRItem MP5 { get; protected set; }
        public TRItem RLauncher { get; protected set; }
        public TRItem DEagleAmmo { get; protected set; }
        public TRItem MP5Ammo { get; protected set; }
        public TRItem Rockets { get; protected set; }
        public TRItem SaveCrystal { get; protected set; }

        internal TR3ItemProvider(IReadOnlyList<string> gameStrings)
            : base(gameStrings)
        {
            _weapons.Add(DEagle = new TRItem(2, TRItemCategory.Weapon, gameStrings[38]));
            _weapons.Add(MP5 = new TRItem(5, TRItemCategory.Weapon, gameStrings[41]));
            _weapons.Add(RLauncher = new TRItem(6, TRItemCategory.Weapon, gameStrings[42]));
            _weapons.Add(GLauncher = new TRItem(7, TRItemCategory.Weapon, gameStrings[43]));

            PistolAmmo = new TRItem(8, TRItemCategory.Ammo, gameStrings[45]);

            _ammo.Add(ShotgunAmmo = new TRItem(9, TRItemCategory.Ammo, gameStrings[46]));
            _ammo.Add(DEagleAmmo = new TRItem(10, TRItemCategory.Ammo, gameStrings[47]));
            _ammo.Add(UziAmmo = new TRItem(11, TRItemCategory.Ammo, gameStrings[48]));
            _ammo.Add(HarpoonAmmo = new TRItem(12, TRItemCategory.Ammo, gameStrings[49]));
            _ammo.Add(MP5Ammo = new TRItem(13, TRItemCategory.Ammo, gameStrings[50]));
            _ammo.Add(Rockets = new TRItem(14, TRItemCategory.Ammo, gameStrings[51]));
            _ammo.Add(Grenades = new TRItem(15, TRItemCategory.Ammo, gameStrings[52]));

            _miscItems.Add(Flare = new TRItem(16, TRItemCategory.Misc, gameStrings[44]));

            _miscItems.Add(SmallMedi = new TRItem(17, TRItemCategory.Health, gameStrings[53]));
            _miscItems.Add(LargeMedi = new TRItem(18, TRItemCategory.Health, gameStrings[54]));

            _pickups.Add(Pickup1 = new TRItem(19, TRItemCategory.Pickup, gameStrings[55] + " 1"));
            _pickups.Add(Pickup2 = new TRItem(20, TRItemCategory.Pickup, gameStrings[55] + " 2"));
            _pickups.Add(Puzzle1 = new TRItem(21, TRItemCategory.Pickup, gameStrings[56] + " 1"));
            _pickups.Add(Puzzle2 = new TRItem(22, TRItemCategory.Pickup, gameStrings[56] + " 2"));
            _pickups.Add(Puzzle3 = new TRItem(23, TRItemCategory.Pickup, gameStrings[56] + " 3"));
            _pickups.Add(Puzzle4 = new TRItem(24, TRItemCategory.Pickup, gameStrings[56] + " 4"));
            _pickups.Add(Key1 = new TRItem(25, TRItemCategory.Pickup, gameStrings[57] + " 1"));
            _pickups.Add(Key2 = new TRItem(26, TRItemCategory.Pickup, gameStrings[57] + " 2"));
            _pickups.Add(Key3 = new TRItem(27, TRItemCategory.Pickup, gameStrings[57] + " 3"));
            _pickups.Add(Key4 = new TRItem(28, TRItemCategory.Pickup, gameStrings[57] + " 4"));

            _miscItems.Add(SaveCrystal = new TRItem(29, TRItemCategory.Health, gameStrings[84]));

            _allItems.AddRange(_weapons);
            _allItems.Add(PistolAmmo);
            _allItems.AddRange(_miscItems);
            _allItems.AddRange(_pickups);

            SortAllItems();
        }
    }
}