using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        internal AbstractTR23ItemProvider(IReadOnlyList<string> gameStrings)
            :base()
        {
            _allItems.Add(Pistols = new TRItem(0, TRItemCategory.Weapon, gameStrings[36]));

            _weapons.Add(Shotgun = new TRItem(1, TRItemCategory.Weapon, gameStrings[37]));
            _weapons.Add(Uzis = new TRItem(3, TRItemCategory.Weapon, gameStrings[39]));
            _weapons.Add(Harpoon = new TRItem(4, TRItemCategory.Weapon, gameStrings[40]));
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
            exclusions = VerifyBonusExclusions(exclusions);
            int weaponGenerosity = rand.Next(0, 2);

            Dictionary<TRItemCategory, int[]>[] generosity = new Dictionary<TRItemCategory, int[]>[]
            {
                new Dictionary<TRItemCategory, int[]>
                {
                    { TRItemCategory.Ammo, new int[] { rand.Next(1, 9), rand.Next(2, 5) } },
                    { TRItemCategory.Misc, new int[] { rand.Next(0, 3), rand.Next(2, 9) } }
                },
                new Dictionary<TRItemCategory, int[]>
                {
                    { TRItemCategory.Ammo, new int[] { rand.Next(0, 2), rand.Next(2, 4) } },
                    { TRItemCategory.Misc, new int[] { rand.Next(1, 4), rand.Next(2, 4) } }
                },
                new Dictionary<TRItemCategory, int[]>
                {
                    { TRItemCategory.Ammo, new int[] { rand.Next(1, 5), rand.Next(2, 5) } },
                    { TRItemCategory.Misc, new int[] { rand.Next(1, 4), rand.Next(3, 9) } }
                }
            };
                        
            Dictionary<TRItemCategory, int[]> generosityMap = generosity[rand.Next(0, generosity.Length)];

            List<TRItem> bonuses = new List<TRItem>();
            bonuses.AddRange(_weapons.RandomSelection(rand, Convert.ToUInt32(weaponGenerosity), false, exclusions[TRItemCategory.Weapon]));
            bonuses.AddRange(GetRandomItems(rand, generosityMap[TRItemCategory.Ammo], _ammo));
            bonuses.AddRange(GetRandomItems(rand, generosityMap[TRItemCategory.Misc], _miscItems));
            return bonuses;
        }

        private Dictionary<TRItemCategory, ISet<TRItem>> VerifyBonusExclusions(Dictionary<TRItemCategory, ISet<TRItem>> exclusions)
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

        private List<TRItem> GetRandomItems(Random rand, int[] qtyDeterminer, List<TRItem> itemList)
        {
            int numItems = qtyDeterminer[0];
            int maxItems = qtyDeterminer[1];

            List<TRItem> items = new List<TRItem>();
            for (int i = 0; i < numItems; i++)
            {
                int qty = rand.Next(1, maxItems);
                for (int j = 0; j < qty; j++)
                {
                    items.Add(itemList[rand.Next(0, itemList.Count)]);
                }
            }
            return items;
        }
    }
}