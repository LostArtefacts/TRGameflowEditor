using System;
using System.Collections.Generic;
using System.Linq;

namespace TRGE.Core
{
    internal class TR23LevelManager : AbstractTRLevelManager
    {
        private readonly TR23Script _script;
        private List<TR23Level> _levels;
        private readonly AbstractTR23ItemProvider _itemProvider;
        private readonly bool _canRandomiseBonuses;

        internal override int LevelCount => _levels.Count;
        internal override AbstractTRItemProvider ItemProvider => _itemProvider;
        internal override bool CanRandomiseBonuses => _canRandomiseBonuses;
        internal override List<AbstractTRLevel> Levels
        {
            get => _levels.Cast<AbstractTRLevel>().ToList();
            set
            {
                _levels = value.Cast<TR23Level>().ToList();
            }
        }

        internal Organisation UnarmedLevelOrganisation { get; set; }
        internal RandomGenerator UnarmedLevelRNG { get; set; }
        internal uint RandomUnarmedLevelCount { get; set; }

        internal Organisation AmmolessLevelOrganisation { get; set; }
        internal RandomGenerator AmmolessLevelRNG { get; set; }
        internal uint RandomAmmolessLevelCount { get; set; }

        internal TR23LevelManager(TR23Script script)
        {
            _script = script;
            Levels = _script.Levels;
            _itemProvider = TRItemFactory.GetProvider(script.Edition, script.GameStrings1) as AbstractTR23ItemProvider;
            _canRandomiseBonuses = script.Edition.Version == TRVersion.TR2 || script.Edition.Version == TRVersion.TR2G;
        }

        internal List<TR23Level> GetAmmolessLevels()
        {
            return GetLevelsWithOperation(TR23OpDefs.RemoveAmmo, true).Cast<TR23Level>().ToList();
        }

        internal virtual List<MutableTuple<string, string, bool>> GetAmmolessLevelData()
        {
            List<MutableTuple<string, string, bool>> data = new List<MutableTuple<string, string, bool>>();
            foreach (TR23Level level in _levels)
            {
                data.Add(new MutableTuple<string, string, bool>(level.ID, level.Name, level.RemovesAmmo));
            }
            return data;
        }

        internal virtual void SetAmmolessLevelData(List<MutableTuple<string, string, bool>> data)
        {
            foreach (MutableTuple<string, string, bool> item in data)
            {
                TR23Level level = (TR23Level)GetLevel(item.Item1);
                if (level != null)
                {
                    level.RemovesAmmo = item.Item3;
                }
            }
        }

        internal void RandomiseAmmolessLevels(List<AbstractTRLevel> originalLevels)
        {
            RandomiseLevelsWithOperation
            (
                AmmolessLevelRNG,
                RandomAmmolessLevelCount,
                originalLevels,
                new TROperation(TR23OpDefs.RemoveAmmo, ushort.MaxValue, true)
            );
        }

        internal void RandomiseUnarmedLevels(List<AbstractTRLevel> originalLevels)
        {
            RandomiseLevelsWithOperation
            (
                UnarmedLevelRNG,
                RandomUnarmedLevelCount, 
                originalLevels, 
                new TROperation(TR23OpDefs.RemoveWeapons, ushort.MaxValue, true)
            );
        }

        internal List<TR23Level> GetUnarmedLevels()
        {
            return GetLevelsWithOperation(TR23OpDefs.RemoveWeapons, true).Cast<TR23Level>().ToList();
        }

        internal virtual List<MutableTuple<string, string, bool>> GetUnarmedLevelData()
        {
            List<MutableTuple<string, string, bool>> data = new List<MutableTuple<string, string, bool>>();
            foreach (TR23Level level in _levels)
            {
                data.Add(new MutableTuple<string, string, bool>(level.ID, level.Name, level.RemovesWeapons));
            }
            return data;
        }

        internal virtual void SetUnarmedLevelData(List<MutableTuple<string, string, bool>> data)
        {
            foreach (MutableTuple<string, string, bool> item in data)
            {
                TR23Level level = (TR23Level)GetLevel(item.Item1);
                if (level != null)
                {
                    level.RemovesWeapons = item.Item3;
                }
            }
        }

        internal override void RandomiseBonuses()
        {
            if (!CanRandomiseBonuses)
            {
                base.RandomiseBonuses();
                return;
            }

            Dictionary<TRItemCategory, ISet<TRItem>> exclusions = new Dictionary<TRItemCategory, ISet<TRItem>>
            {
                { TRItemCategory.Weapon, new HashSet<TRItem>() }
            };
            Random rand = BonusRNG.Create();
            foreach (TR23Level level in _levels)
            {
                if (level.HasSecrets)
                {
                    List<TRItem> bonuses = ItemProvider.GetRandomBonusItems(rand, exclusions);
                    foreach (TRItem bonus in bonuses)
                    {
                        if (bonus.Category == TRItemCategory.Weapon)
                        {
                            exclusions[TRItemCategory.Weapon].Add(bonus);
                        }
                    }
                    level.SetBonuses(bonuses);
                }
            }
        }

        internal override List<MutableTuple<string, string, List<MutableTuple<ushort, TRItemCategory, string, int>>>> GetLevelBonusData()
        {
            List<MutableTuple<string, string, List<MutableTuple<ushort, TRItemCategory, string, int>>>> data = new List<MutableTuple<string, string, List<MutableTuple<ushort, TRItemCategory, string, int>>>>();
            foreach (TR23Level level in _levels)
            {
                if (level.HasSecrets)
                {
                    data.Add(new MutableTuple<string, string, List<MutableTuple<ushort, TRItemCategory, string, int>>>
                    (
                        level.ID,
                        level.Name,
                        level.GetBonusItemData(ItemProvider)
                    ));
                }
            }
            return data;
        }

        internal override void SetLevelBonusData(List<MutableTuple<string, string, List<MutableTuple<ushort, TRItemCategory, string, int>>>> data)
        {
            foreach (MutableTuple<string, string, List<MutableTuple<ushort, TRItemCategory, string, int>>> levelData in data)
            {
                TR23Level level = (TR23Level)GetLevel(levelData.Item1);
                if (level != null)
                {
                    level.SetBonusItemData(levelData.Item3);
                }
            }
        }

        internal Dictionary<string, List<TRItem>> GetLevelBonusItems()
        {
            Dictionary<string, List<TRItem>> ret = new Dictionary<string, List<TRItem>>();
            foreach (TR23Level level in _levels)
            {
                if (level.HasSecrets)
                {
                    List<TRItem> items = level.GetBonusItems(ItemProvider);
                    if (items.Count > 0)
                    {
                        ret.Add(level.ID, items);
                    }
                }
            }
            return ret;
        }

        internal bool GetLevelsHaveStartAnimation()
        {
            foreach (TR23Level level in _levels)
            {
                if (level.HasStartAnimation)
                {
                    return true;
                }
            }
            return false;
        }

        internal void SetLevelsHaveStartAnimation(bool haveStartAnimations)
        {
            foreach (TR23Level level in _levels)
            {
                level.HasStartAnimation = haveStartAnimations;
            }
        }

        internal bool GetLevelsHaveCutScenes()
        {
            foreach (TR23Level level in _levels)
            {
                if (level.HasCutScene)
                {
                    return true;
                }
            }
            return false;
        }

        internal void SetLevelsHaveCutScenes(bool haveCutScenes)
        {
            foreach (TR23Level level in _levels)
            {
                level.HasCutScene = haveCutScenes;
            }
        }

        internal bool GetLevelsHaveFMV()
        {
            foreach (TR23Level level in _levels)
            {
                if (level.HasFMV)
                {
                    return true;
                }
            }
            return false;
        }

        internal void SetLevelsHaveFMV(bool haveFMVs)
        {
            foreach (TR23Level level in _levels)
            {
                level.HasFMV = haveFMVs;
            }
        }

        internal override void Save()
        {
            throw new NotImplementedException();
        }
    }
}