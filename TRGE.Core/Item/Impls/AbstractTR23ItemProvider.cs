using System;
using System.Collections.Generic;

namespace TRGE.Core
{
    internal abstract class AbstractTR23ItemProvider : AbstractTRItemProvider
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

        protected TREdition _edition;
        protected IReadOnlyList<string> _gameStrings;
        protected int[] _itemNameIndices;

        internal AbstractTR23ItemProvider(TREdition edition, IReadOnlyList<string> gameStrings)
            :base()
        {
            _edition = edition;
            _gameStrings = gameStrings;
            _itemNameIndices = GetGameStringIndices();

            _allItems.Add(Pistols = CreateItem(0, TRItemCategory.Weapon));

            _weapons.Add(Shotgun = CreateItem(1, TRItemCategory.Weapon));
            _weapons.Add(Uzis    = CreateItem(3, TRItemCategory.Weapon));
            _weapons.Add(Harpoon = CreateItem(4, TRItemCategory.Weapon));
        }

        protected abstract int[] GetGameStringIndices();

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
            List<TRItem> bonuses = new List<TRItem>();
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

            List<TRItem> bonuses = new List<TRItem>();
            bonuses.AddRange(_weapons.RandomSelection(rand, Convert.ToUInt32(broker.WeaponCount), false, exclusions[TRItemCategory.Weapon]));
            bonuses.AddRange(GetRandomItems(rand, broker.AmmoTypeCount, broker.MaxAmmoCount, _ammo, exclusions[TRItemCategory.Ammo]));
            bonuses.AddRange(GetRandomItems(rand, broker.MiscTypeCount, broker.MaxMiscCount, _miscItems, exclusions[TRItemCategory.Misc]));

            return bonuses;
        }

        protected virtual TRItemBrokerDealer<BaseTRItemBroker> GetBrokerDealer(Random rand)
        {
            return null;
        }

        protected Dictionary<TRItemCategory, ISet<TRItem>> VerifyBonusExclusions(Dictionary<TRItemCategory, ISet<TRItem>> exclusions)
        {
            if (exclusions == null)
            {
                exclusions = new Dictionary<TRItemCategory, ISet<TRItem>>();
            }
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

        protected List<TRItem> GetRandomItems(Random rand, int numItems, int maxItems, List<TRItem> itemList, ISet<TRItem> exclusions)
        {
            List<TRItem> items = new List<TRItem>();
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
}