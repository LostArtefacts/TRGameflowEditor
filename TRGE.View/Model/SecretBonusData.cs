using System.Collections.Generic;
using System.Linq;
using TRGE.Core;

namespace TRGE.View.Model
{
    public class GlobalSecretBonusData : List<LevelSecretBonusData>
    {
        public GlobalSecretBonusData(List<MutableTuple<string, string, List<MutableTuple<ushort, TRItemCategory, string, int>>>> globalData)
        {
            foreach (MutableTuple<string, string, List<MutableTuple<ushort, TRItemCategory, string, int>>> data in globalData)
            {
                Add(new LevelSecretBonusData(data.Item1, data.Item2, data.Item3));
            }
        }

        public List<MutableTuple<string, string, List<MutableTuple<ushort, TRItemCategory, string, int>>>> ToTuple()
        {
            List<MutableTuple<string, string, List<MutableTuple<ushort, TRItemCategory, string, int>>>> result = new List<MutableTuple<string, string, List<MutableTuple<ushort, TRItemCategory, string, int>>>>();
            foreach (LevelSecretBonusData data in this)
            {
                result.Add(data.ToTuple());
            }
            return result;
        }
    }

    public class LevelSecretBonusData
    {
        public string LevelID { get; private set; }
        public string LevelName { get; private set; }
        public SecretBonusData BonusData { get; private set; }

        public LevelSecretBonusData(string levelID, string levelName, List<MutableTuple<ushort, TRItemCategory, string, int>> bonusItems)
        {
            LevelID = levelID;
            LevelName = levelName;
            BonusData = new SecretBonusData(bonusItems);
        }

        public MutableTuple<string, string, List<MutableTuple<ushort, TRItemCategory, string, int>>> ToTuple()
        {
            return new MutableTuple<string, string, List<MutableTuple<ushort, TRItemCategory, string, int>>>
            (
                LevelID, LevelName, BonusData.ToTuple()
            );
        }
    }

    public class SecretBonusData : List<SecretBonusItem>
    {
        public SecretBonusData(List<MutableTuple<ushort, TRItemCategory, string, int>> bonusItems)
        {
            foreach (MutableTuple<ushort, TRItemCategory, string, int> item in bonusItems)
            {
                Add(SecretBonusItem.FromTuple(item));
            }
        }

        public List<SecretBonusItem> WeaponItems => GetCategorisedItems(TRItemCategory.Weapon);
        public List<SecretBonusItem> AmmoItems => GetCategorisedItems(TRItemCategory.Ammo);
        public List<SecretBonusItem> OtherItems => GetCategorisedItems(TRItemCategory.Health).Union(GetCategorisedItems(TRItemCategory.Misc)).ToList();

        private List<SecretBonusItem> GetCategorisedItems(TRItemCategory category)
        {
            return this.Where(e => e.Category == category).ToList();
        }

        public List<MutableTuple<ushort, TRItemCategory, string, int>> ToTuple()
        {
            List<MutableTuple<ushort, TRItemCategory, string, int>> result = new List<MutableTuple<ushort, TRItemCategory, string, int>>();
            foreach (SecretBonusItem item in this)
            {
                result.Add(item.ToTuple());
            }
            return result;
        }
    }

    public class SecretBonusItem
    {
        public ushort ID { get; private set; }
        public TRItemCategory Category { get; private set; }
        public string Name { get; private set; }
        public bool Enabled { get; set; }
        public int Quantity { get; set; }

        public static SecretBonusItem FromTuple(MutableTuple<ushort, TRItemCategory, string, int> item)
        {
            return new SecretBonusItem
            {
                ID = item.Item1,
                Category = item.Item2,
                Name = item.Item3,
                Enabled = item.Item4 != -1,
                Quantity = item.Item4 == -1 ? 1 : item.Item4
            };
        }

        public MutableTuple<ushort, TRItemCategory, string, int> ToTuple()
        {
            return new MutableTuple<ushort, TRItemCategory, string, int>
            (
                ID, Category, Name, Enabled ? (Category == TRItemCategory.Weapon ? 1 : Quantity) : -1
            );
        }
    }
}