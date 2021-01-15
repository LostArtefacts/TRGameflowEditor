using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TRGE.Core
{
    internal abstract class AbstractTRItemProvider
    {
        protected readonly List<TRItem> _allItems, _weapons, _ammo, _miscItems, _pickups;

        internal IReadOnlyList<TRItem> Weapons => _weapons;
        internal IReadOnlyList<TRItem> Ammo => _ammo;
        internal IReadOnlyList<TRItem> MiscItems => _miscItems;
        internal IReadOnlyList<TRItem> Pickups => _pickups;
        internal IReadOnlyList<TRItem> BonusItems => GetBonusItems();

        internal AbstractTRItemProvider()
        {
            _allItems = new List<TRItem>();
            _weapons = new List<TRItem>();
            _ammo = new List<TRItem>();
            _miscItems = new List<TRItem>();
            _pickups = new List<TRItem>();
        }

        protected void SortAllItems()
        {
            _weapons.Sort();
            _ammo.Sort();
            _miscItems.Sort();
            _pickups.Sort();
        }

        internal TRItem GetItem(ushort itemID)
        {
            List<TRItem>[] searchBase = new List<TRItem>[] { _allItems, _weapons, _ammo, _miscItems, _pickups };
            TRItem result = null;
            for (int i = 0; i < searchBase.Length && result == null; i++)
            {
                foreach (TRItem item in searchBase[i])
                {
                    if (item.ID == itemID)
                    {
                        result = item;
                        break;
                    }
                }
            }

            return result;
        }

        protected abstract List<TRItem> GetBonusItems();
        internal abstract List<TRItem> GetRandomBonusItems(Random rand, Dictionary<TRItemCategory, ISet<TRItem>> exclusions = null);
    }
}