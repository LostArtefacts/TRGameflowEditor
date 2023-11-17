namespace TRGE.Core;

internal class TR3ItemProvider : AbstractTR23ItemProvider
{
    public TRItem DEagle { get; protected set; }
    public TRItem MP5 { get; protected set; }
    public TRItem RLauncher { get; protected set; }
    public TRItem DEagleAmmo { get; protected set; }
    public TRItem MP5Ammo { get; protected set; }
    public TRItem Rockets { get; protected set; }
    public TRItem SaveCrystal { get; protected set; }

    internal TR3ItemProvider(TREdition edition, IReadOnlyList<string> gameStrings)
        : base(edition, gameStrings)
    {
        _weapons.Add(DEagle    = CreateItem(2, TRItemCategory.Weapon));
        _weapons.Add(MP5       = CreateItem(5, TRItemCategory.Weapon));
        _weapons.Add(RLauncher = CreateItem(6, TRItemCategory.Weapon));
        _weapons.Add(GLauncher = CreateItem(7, TRItemCategory.Weapon));
        _allItems.AddRange(_weapons);

        _allItems.Add(PistolAmmo = CreateItem(8, TRItemCategory.Ammo));

        _ammo.Add(ShotgunAmmo = CreateItem(9,  TRItemCategory.Ammo));
        _ammo.Add(DEagleAmmo  = CreateItem(10, TRItemCategory.Ammo));
        _ammo.Add(UziAmmo     = CreateItem(11, TRItemCategory.Ammo));
        _ammo.Add(HarpoonAmmo = CreateItem(12, TRItemCategory.Ammo));
        _ammo.Add(MP5Ammo     = CreateItem(13, TRItemCategory.Ammo));
        _ammo.Add(Rockets     = CreateItem(14, TRItemCategory.Ammo));
        _ammo.Add(Grenades    = CreateItem(15, TRItemCategory.Ammo));

        _miscItems.Add(Flare       = CreateItem(16, TRItemCategory.Misc));
        _miscItems.Add(SmallMedi   = CreateItem(17, TRItemCategory.Health));
        _miscItems.Add(LargeMedi   = CreateItem(18, TRItemCategory.Health));
        _miscItems.Add(SaveCrystal = CreateItem(29, TRItemCategory.Health));
        _allItems.AddRange(_miscItems);

        _pickups.Add(Pickup1 = CreateItem(19, TRItemCategory.Pickup, " 1"));
        _pickups.Add(Pickup2 = CreateItem(20, TRItemCategory.Pickup, " 2"));
        _pickups.Add(Puzzle1 = CreateItem(21, TRItemCategory.Pickup, " 1"));
        _pickups.Add(Puzzle2 = CreateItem(22, TRItemCategory.Pickup, " 2"));
        _pickups.Add(Puzzle3 = CreateItem(23, TRItemCategory.Pickup, " 3"));
        _pickups.Add(Puzzle4 = CreateItem(24, TRItemCategory.Pickup, " 4"));
        _pickups.Add(Key1    = CreateItem(25, TRItemCategory.Pickup, " 1"));
        _pickups.Add(Key2    = CreateItem(26, TRItemCategory.Pickup, " 2"));
        _pickups.Add(Key3    = CreateItem(27, TRItemCategory.Pickup, " 3"));
        _pickups.Add(Key4    = CreateItem(28, TRItemCategory.Pickup, " 4"));
        _allItems.AddRange(_pickups);

        SortAllItems();
    }

    protected override int[] GetGameStringIndices()
    {
        return new int[]
        {
            36, //Pistols
            37, //Shotgun
            38, //Desert Eagle
            39, //Uzis
            40, //Harpoon Gun
            41, //MP5
            42, //Rocket Launcher
            43, //Grenade Launcher
            45, //Pistol Clips
            46, //Shotgun Shells
            47, //Desert Eagle Clips
            48, //Uzi Clips
            49, //Harpoons
            50, //MP5 Clips
            51, //Rockets
            52, //Grenades
            44, //Flare
            53, //Small Medi
            54, //Large Medi
            55, 55, //Pickup
            56, 56, 56, 56, //Puzzle
            57, 57, 57, 57, //Key
            84 //Savegame Crystal
        };
    }
}