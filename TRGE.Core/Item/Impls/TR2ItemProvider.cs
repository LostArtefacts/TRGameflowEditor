namespace TRGE.Core;

internal class TR2ItemProvider : AbstractTR23ItemProvider
{
    public TRItem Autos { get; protected set; }
    public TRItem M16 { get; protected set; }
    public TRItem AutoAmmo { get; protected set; }
    public TRItem M16Ammo { get; protected set; }

    internal TR2ItemProvider(TREdition edition, IReadOnlyList<string> gameStrings)
        :base(edition, gameStrings)
    {
        _weapons.Add(Autos     = CreateItem(2, TRItemCategory.Weapon));
        _weapons.Add(M16       = CreateItem(5, TRItemCategory.Weapon));
        _weapons.Add(GLauncher = CreateItem(6, TRItemCategory.Weapon));
        _allItems.AddRange(_weapons);

        _allItems.Add(PistolAmmo = CreateItem(7, TRItemCategory.Ammo));

        _ammo.Add(ShotgunAmmo = CreateItem(8,  TRItemCategory.Ammo));
        _ammo.Add(AutoAmmo    = CreateItem(9,  TRItemCategory.Ammo));
        _ammo.Add(UziAmmo     = CreateItem(10, TRItemCategory.Ammo));
        _ammo.Add(HarpoonAmmo = CreateItem(11, TRItemCategory.Ammo));
        _ammo.Add(M16Ammo     = CreateItem(12, TRItemCategory.Ammo));
        _ammo.Add(Grenades    = CreateItem(13, TRItemCategory.Ammo));

        _miscItems.Add(Flare     = CreateItem(14, TRItemCategory.Misc));
        _miscItems.Add(SmallMedi = CreateItem(15, TRItemCategory.Health));
        _miscItems.Add(LargeMedi = CreateItem(16, TRItemCategory.Health));
        _allItems.AddRange(_miscItems);

        _pickups.Add(Pickup1 = CreateItem(17, TRItemCategory.Pickup, " 1"));
        _pickups.Add(Pickup2 = CreateItem(18, TRItemCategory.Pickup, " 2"));
        _pickups.Add(Puzzle1 = CreateItem(19, TRItemCategory.Pickup, " 1"));
        _pickups.Add(Puzzle2 = CreateItem(20, TRItemCategory.Pickup, " 2"));
        _pickups.Add(Puzzle3 = CreateItem(21, TRItemCategory.Pickup, " 3"));
        _pickups.Add(Puzzle4 = CreateItem(22, TRItemCategory.Pickup, " 4"));
        _pickups.Add(Key1    = CreateItem(23, TRItemCategory.Pickup, " 1"));
        _pickups.Add(Key2    = CreateItem(24, TRItemCategory.Pickup, " 2"));
        _pickups.Add(Key3    = CreateItem(25, TRItemCategory.Pickup, " 3"));
        _pickups.Add(Key4    = CreateItem(26, TRItemCategory.Pickup, " 4"));
        _allItems.AddRange(_pickups);

        SortAllItems();
    }

    protected override int[] GetGameStringIndices()
    {
        if (_edition.Equals(TREdition.TR2PC) || _edition.Equals(TREdition.TR2G) || _edition.Equals(TREdition.TR2PSX))
        {
            return new int[]
            {
                36, //Pistols
                37, //Shotgun
                38, //Autos
                39, //Uzis
                40, //Harpoon Gun
                41, //M16
                42, //Grenade Launcher
                44, //Pistol Clips
                45, //Shotgun Shells
                46, //Auto Clips
                47, //Uzi Clips
                48, //Harpoons
                49, //M16 Clips
                50, //Grenades
                43, //Flare
                51, //Small Medi
                52, //Large Medi
                53, 53, //Pickup
                54, 54, 54, 54, //Puzzle
                55, 55, 55, 55  //Key
            };
        }
        if (_edition.Equals(TREdition.TR2PSXBeta))
        {
            return new int[]
            {
                33, //Pistols
                34, //Shotgun
                35, //Autos
                36, //Uzis
                37, //Harpoon Gun
                38, //M16
                39, //Grenade Launcher
                41, //Pistol Clips
                42, //Shotgun Shells
                43, //Auto Clips
                44, //Uzi Clips
                45, //Harpoons
                46, //M16 Clips
                47, //Grenades
                30, //Flare
                48, //Small Medi
                49, //Large Medi
                50, 50, //Pickup
                51, 51, 51, 51, //Puzzle
                52, 52, 52, 52  //Key
            };
        }
        return null;
    }

    protected override TRItemBrokerDealer<BaseTRItemBroker> GetBrokerDealer(Random rand)
    {
        return new TRItemBrokerDealer<BaseTRItemBroker>
        {
            //tightest
            new() {
                WeaponCount = 0,
                AmmoTypeCount = 1,
                MaxAmmoCount = rand.Next(1, 3),
                MiscTypeCount = 0,
                MaxMiscCount = 0,
                Weight = 8
            },
            //tight
            new() {
                WeaponCount = 0,
                AmmoTypeCount = rand.Next(1, 2),
                MaxAmmoCount = rand.Next(1, 3),
                MiscTypeCount = 1,
                MaxMiscCount = rand.Next(1, 3),
                Weight = 12 // #73 reduce weight to make more fair
            },
            //default...ish?
            new() {
                WeaponCount = rand.Next(0, 2),
                AmmoTypeCount = 1,
                MaxAmmoCount = 2 * rand.Next(2, 5), //4, 6, 8
                MiscTypeCount = rand.Next(0, 2),
                MaxMiscCount = rand.Next(2, 4),
                Weight = 50
            },
            //generous
            new() {
                WeaponCount = rand.Next(1, 3),
                AmmoTypeCount = rand.Next(1, 5),
                MaxAmmoCount = rand.Next(0, 9),
                MiscTypeCount = rand.Next(1, 5),
                MaxMiscCount = rand.Next(0, 9),
                Weight = 22 // #73 increase weight to make more fair
            },
            //generous++
            new() {
                WeaponCount = rand.Next(1, 4),
                AmmoTypeCount = rand.Next(2, 5),
                MaxAmmoCount = rand.Next(2, 5),
                MiscTypeCount = rand.Next(2, 5),
                MaxMiscCount = rand.Next(2, 5),
                Weight = 8
            }
        };
    }

    internal override void SortBonusItems(List<TRItem> items)
    {
        //this is very specific for Catacombs of the Talion,
        //where to ensure the output script matches the original
        //grenades are listed in the script before M16, whereas
        //in all other cases, the item ID determines the sequence
        items.Sort(delegate(TRItem item1, TRItem item2)
        {
            if (item2 == M16Ammo && item1 == Grenades)
            {
                return item2.CompareTo(item1);
            }

            return item1.CompareTo(item2);
        });
    }
}