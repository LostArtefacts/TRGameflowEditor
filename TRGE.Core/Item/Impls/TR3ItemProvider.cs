namespace TRGE.Core;

internal class TR3ItemProvider : AbstractTRItemProvider
{
    public TRItem Pistols { get; protected set; }
    public TRItem Shotgun { get; protected set; }
    public TRItem Uzis { get; protected set; }
    public TRItem Harpoon { get; protected set; }
    public TRItem GLauncher { get; protected set; }
    public TRItem PistolAmmo { get; protected set; }
    public TRItem ShotgunAmmo { get; protected set; }
    public TRItem UziAmmo { get; protected set; }
    public TRItem HarpoonAmmo { get; protected set; }
    public TRItem Grenades { get; protected set; }
    public TRItem Flare { get; protected set; }
    public TRItem SmallMedi { get; protected set; }
    public TRItem LargeMedi { get; protected set; }
    public TRItem Pickup1 { get; protected set; }
    public TRItem Pickup2 { get; protected set; }
    public TRItem Puzzle1 { get; protected set; }
    public TRItem Puzzle2 { get; protected set; }
    public TRItem Puzzle3 { get; protected set; }
    public TRItem Puzzle4 { get; protected set; }
    public TRItem Key1 { get; protected set; }
    public TRItem Key2 { get; protected set; }
    public TRItem Key3 { get; protected set; }
    public TRItem Key4 { get; protected set; }

    public TRItem DEagle { get; protected set; }
    public TRItem MP5 { get; protected set; }
    public TRItem RLauncher { get; protected set; }
    public TRItem DEagleAmmo { get; protected set; }
    public TRItem MP5Ammo { get; protected set; }
    public TRItem Rockets { get; protected set; }
    public TRItem SaveCrystal { get; protected set; }

    protected TREdition _edition;
    protected IReadOnlyList<string> _gameStrings;
    protected int[] _itemNameIndices;

    internal TR3ItemProvider(TREdition edition, IReadOnlyList<string> gameStrings)
    {
        _edition = edition;
        _gameStrings = gameStrings;
        _itemNameIndices = GetGameStringIndices();

        _allItems.Add(Pistols = CreateItem(0, TRItemCategory.Weapon));

        _weapons.Add(Shotgun = CreateItem(1, TRItemCategory.Weapon));
        _weapons.Add(Uzis = CreateItem(3, TRItemCategory.Weapon));
        _weapons.Add(Harpoon = CreateItem(4, TRItemCategory.Weapon));

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

    protected int[] GetGameStringIndices()
    {
        return
        [
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
        ];
    }

    protected TRItem CreateItem(ushort id, TRItemCategory category, string append = null)
    {
        string name = _gameStrings[_itemNameIndices[id]];
        if (append != null)
        {
            name += append;
        }
        return new TRItem(id, category, name);
    }

    protected override List<TRItem> GetBonusItems()
    {
        List<TRItem> bonuses = new();
        bonuses.AddRange(_weapons);
        bonuses.AddRange(_ammo);
        bonuses.AddRange(_miscItems);
        return bonuses;
    }

    internal override List<TRItem> GetRandomBonusItems(Random rand, Dictionary<TRItemCategory, ISet<TRItem>> exclusions = null)
    {
        TRItemBrokerDealer<BaseTRItemBroker> dealer = GetBrokerDealer(rand);
        if (dealer == null)
        {
            return null;
        }

        exclusions = VerifyBonusExclusions(exclusions);
        BaseTRItemBroker broker = dealer.Get(rand);

        List<TRItem> bonuses = new();
        bonuses.AddRange(_weapons.RandomSelection(rand, Convert.ToUInt32(broker.WeaponCount), false, exclusions[TRItemCategory.Weapon]));
        bonuses.AddRange(GetRandomItems(rand, broker.AmmoTypeCount, broker.MaxAmmoCount, _ammo, exclusions[TRItemCategory.Ammo]));
        bonuses.AddRange(GetRandomItems(rand, broker.MiscTypeCount, broker.MaxMiscCount, _miscItems, exclusions[TRItemCategory.Misc]));

        return bonuses;
    }

    protected virtual TRItemBrokerDealer<BaseTRItemBroker> GetBrokerDealer(Random rand)
    {
        return null;
    }

    protected static Dictionary<TRItemCategory, ISet<TRItem>> VerifyBonusExclusions(Dictionary<TRItemCategory, ISet<TRItem>> exclusions)
    {
        exclusions ??= new Dictionary<TRItemCategory, ISet<TRItem>>();
        TRItemCategory[] cats = new TRItemCategory[] { TRItemCategory.Weapon, TRItemCategory.Ammo, TRItemCategory.Misc };
        foreach (TRItemCategory cat in cats)
        {
            if (!exclusions.ContainsKey(cat))
            {
                exclusions[cat] = new HashSet<TRItem>();
            }
        }
        return exclusions;
    }

    protected static List<TRItem> GetRandomItems(Random rand, int numItems, int maxItems, List<TRItem> itemList, ISet<TRItem> exclusions)
    {
        List<TRItem> items = new();
        if (maxItems > 0)
        {
            for (int i = 0; i < numItems; i++)
            {
                // #73 removed random max qty, instead use the broker definitions.
                int qty = maxItems;// rand.Next(1, maxItems);
                for (int j = 0; j < qty; j++)
                {
                    TRItem item = itemList[rand.Next(0, itemList.Count)];
                    if (!exclusions.Contains(item))
                    {
                        items.Add(item);
                    }
                }
            }
        }
        return items;
    }

    internal virtual void SortBonusItems(List<TRItem> items) { }
}
