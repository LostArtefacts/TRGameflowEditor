using System;
using System.Collections.Generic;
using System.Linq;

namespace TRGE.Core
{
    internal class TR23LevelManager : AbstractTRLevelManager
    {
        private readonly AbstractTR23AudioProvider _audioProvider;
        private readonly AbstractTR23ItemProvider _itemProvider;
        private readonly TR23Script _script;
        private List<TR23ScriptedLevel> _levels;

        internal override int LevelCount => _levels.Count;
        internal override AbstractTRAudioProvider AudioProvider => _audioProvider;
        internal override AbstractTRItemProvider ItemProvider => _itemProvider;
        internal bool CanOrganiseBonuses => Edition.SecretBonusesSupported;

        internal override List<AbstractTRScriptedLevel> Levels
        {
            get => _levels.Cast<AbstractTRScriptedLevel>().ToList();
            set
            {
                _levels = value.Cast<TR23ScriptedLevel>().ToList();
            }
        }

        protected override ushort TitleSoundID
        {
            get => _script.TitleSoundID;
            set => _script.TitleSoundID = value;
        }

        protected override ushort SecretSoundID
        {
            get => _script.SecretSoundID;
            set => _script.SecretSoundID = value;
        }

        internal Organisation BonusOrganisation { get; set; }
        internal RandomGenerator BonusRNG { get; set; }

        internal Organisation UnarmedLevelOrganisation { get; set; }
        internal RandomGenerator UnarmedLevelRNG { get; set; }
        internal uint RandomUnarmedLevelCount { get; set; }

        internal Organisation AmmolessLevelOrganisation { get; set; }
        internal RandomGenerator AmmolessLevelRNG { get; set; }
        internal uint RandomAmmolessLevelCount { get; set; }

        internal TR23LevelManager(TR23Script script)
            : base(script.Edition)
        {
            Levels = (_script = script).Levels;
            _audioProvider = TRAudioFactory.GetAudioProvider(script.Edition) as AbstractTR23AudioProvider;
            _itemProvider = TRItemFactory.GetProvider(script.Edition, script.GameStrings1) as AbstractTR23ItemProvider;
        }

        internal List<TR23ScriptedLevel> GetAmmolessLevels()
        {
            return GetLevelsWithOperation(TR23OpDefs.RemoveAmmo, true).Cast<TR23ScriptedLevel>().ToList();
        }

        internal uint GetAmmolessLevelCount()
        {
            return Convert.ToUInt32(GetAmmolessLevels().Count);
        }

        internal virtual List<MutableTuple<string, string, bool>> GetAmmolessLevelData()
        {
            return GetAmmolessLevelData(Levels);
        }

        private List<MutableTuple<string, string, bool>> GetAmmolessLevelData(List<AbstractTRScriptedLevel> levels)
        {
            List<MutableTuple<string, string, bool>> data = new List<MutableTuple<string, string, bool>>();
            foreach (TR23ScriptedLevel level in levels)
            {
                data.Add(new MutableTuple<string, string, bool>(level.ID, level.Name, level.RemovesAmmo));
            }
            return data;
        }

        internal virtual void SetAmmolessLevelData(List<MutableTuple<string, string, bool>> data)
        {
            foreach (MutableTuple<string, string, bool> item in data)
            {
                TR23ScriptedLevel level = (TR23ScriptedLevel)GetLevel(item.Item1);
                if (level != null && level.RemovesAmmo != item.Item3)
                {
                    level.RemovesAmmo = item.Item3;
                    FireLevelModificationEvent(level);
                }
            }
        }

        internal void RandomiseAmmolessLevels(List<AbstractTRScriptedLevel> originalLevels)
        {
            RandomiseLevelsWithOperation
            (
                AmmolessLevelRNG,
                RandomAmmolessLevelCount,
                originalLevels,
                new TROperation(TR23OpDefs.RemoveAmmo, ushort.MaxValue, true)
            );
        }

        internal void RestoreAmmolessLevels(List<AbstractTRScriptedLevel> originalLevels)
        {
            SetAmmolessLevelData(GetAmmolessLevelData(originalLevels));
        }

        internal void RandomiseUnarmedLevels(List<AbstractTRScriptedLevel> originalLevels)
        {
            RandomiseLevelsWithOperation
            (
                UnarmedLevelRNG,
                RandomUnarmedLevelCount, 
                originalLevels, 
                new TROperation(TR23OpDefs.RemoveWeapons, ushort.MaxValue, true)
            );
        }

        internal List<TR23ScriptedLevel> GetUnarmedLevels()
        {
            return GetLevelsWithOperation(TR23OpDefs.RemoveWeapons, true).Cast<TR23ScriptedLevel>().ToList();
        }

        internal uint GetUnarmedLevelCount()
        {
            return Convert.ToUInt32(GetUnarmedLevels().Count);
        }

        internal virtual List<MutableTuple<string, string, bool>> GetUnarmedLevelData()
        {
            return GetUnarmedLevelData(Levels);
        }

        private List<MutableTuple<string, string, bool>> GetUnarmedLevelData(List<AbstractTRScriptedLevel> levels)
        {
            List<MutableTuple<string, string, bool>> data = new List<MutableTuple<string, string, bool>>();
            foreach (TR23ScriptedLevel level in levels)
            {
                data.Add(new MutableTuple<string, string, bool>(level.ID, level.Name, level.RemovesWeapons));
            }
            return data;
        }

        internal virtual void SetUnarmedLevelData(List<MutableTuple<string, string, bool>> data)
        {
            foreach (MutableTuple<string, string, bool> item in data)
            {
                TR23ScriptedLevel level = (TR23ScriptedLevel)GetLevel(item.Item1);
                if (level != null)
                {
                    level.RemovesWeapons = item.Item3;
                }
            }
        }

        internal void RestoreUnarmedLevels(List<AbstractTRScriptedLevel> originalLevels)
        {
            SetUnarmedLevelData(GetUnarmedLevelData(originalLevels));
        }

        internal void RandomiseBonuses()
        {
            TRItem shotgun = (ItemProvider as TR2ItemProvider).Shotgun;
            bool unarmedLevelSeen = false;
            Dictionary<TRItemCategory, ISet<TRItem>> exclusions = new Dictionary<TRItemCategory, ISet<TRItem>>
            {
                { TRItemCategory.Weapon, new HashSet<TRItem> { shotgun } }
            };
            Random rand = BonusRNG.Create();
            foreach (TR23ScriptedLevel level in _levels)
            {
                if (level.HasSecrets)
                {
                    if (!unarmedLevelSeen && level.RemovesWeapons)
                    {
                        //shotgun will only be given if Lara has lost her weapons
                        exclusions[TRItemCategory.Weapon].Remove(shotgun);
                        unarmedLevelSeen = true;
                    }

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

        internal List<MutableTuple<string, string, List<MutableTuple<ushort, TRItemCategory, string, int>>>> GetLevelBonusData()
        {
            return GetLevelBonusData(Levels);
        }

        private List<MutableTuple<string, string, List<MutableTuple<ushort, TRItemCategory, string, int>>>> GetLevelBonusData(List<AbstractTRScriptedLevel> levels)
        {
            List<MutableTuple<string, string, List<MutableTuple<ushort, TRItemCategory, string, int>>>> data = new List<MutableTuple<string, string, List<MutableTuple<ushort, TRItemCategory, string, int>>>>();
            foreach (TR23ScriptedLevel level in levels)
            {
                if (level.HasSecrets)
                {
                    List<MutableTuple<ushort, TRItemCategory, string, int>> items = new List<MutableTuple<ushort, TRItemCategory, string, int>>();
                    foreach (TRItem item in ItemProvider.BonusItems)
                    {
                        int count = level.GetBonusItemCount(item, ItemProvider);
                        items.Add(new MutableTuple<ushort, TRItemCategory, string, int>(item.ID, item.Category, item.Name, count == 0 ? -1 : count));
                    }

                    data.Add(new MutableTuple<string, string, List<MutableTuple<ushort, TRItemCategory, string, int>>>
                    (
                        level.ID,
                        level.Name,
                        items
                    ));
                }
            }
            return data;
        }

        internal void SetLevelBonusData(List<MutableTuple<string, string, List<MutableTuple<ushort, TRItemCategory, string, int>>>> data)
        {
            foreach (MutableTuple<string, string, List<MutableTuple<ushort, TRItemCategory, string, int>>> levelData in data)
            {
                TR23ScriptedLevel level = (TR23ScriptedLevel)GetLevel(levelData.Item1);
                if (level != null)
                {
                    List<TRItem> bonuses = new List<TRItem>();
                    foreach (var n in levelData.Item3)
                    {
                        TRItem item = ItemProvider.GetItem(n.Item1);
                        for (int i = 0; i < n.Item4; i++)
                        {
                            bonuses.Add(item);
                        }
                    }
                    _itemProvider.SortBonusItems(bonuses);
                    level.SetBonuses(bonuses);
                }
            }
        }

        internal Dictionary<string, List<TRItem>> GetLevelBonusItems()
        {
            Dictionary<string, List<TRItem>> ret = new Dictionary<string, List<TRItem>>();
            foreach (TR23ScriptedLevel level in _levels)
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

        internal void RestoreBonuses(List<AbstractTRScriptedLevel> orignalLevels)
        {
            SetLevelBonusData(GetLevelBonusData(orignalLevels));
        }

        internal bool GetLevelsHaveStartAnimation()
        {
            foreach (TR23ScriptedLevel level in _levels)
            {
                if (level.HasStartAnimation)
                {
                    return true;
                }
            }
            return false;
        }

        internal bool GetLevelsSupportStartAnimations()
        {
            foreach (TR23ScriptedLevel level in _levels)
            {
                if (level.SupportsStartAnimations)
                {
                    return true;
                }
            }
            return false;
        }

        internal void SetLevelsHaveStartAnimation(bool haveStartAnimations)
        {
            foreach (TR23ScriptedLevel level in _levels)
            {
                level.HasStartAnimation = haveStartAnimations;
            }
        }

        internal bool GetLevelsHaveCutScenes()
        {
            foreach (TR23ScriptedLevel level in _levels)
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
            foreach (TR23ScriptedLevel level in _levels)
            {
                level.HasCutScene = haveCutScenes;
            }
        }

        internal bool GetLevelsSupportCutScenes()
        {
            foreach (TR23ScriptedLevel level in _levels)
            {
                if (level.SupportsCutScenes)
                {
                    return true;
                }
            }
            return false;
        }

        internal bool GetLevelsHaveFMV()
        {
            foreach (TR23ScriptedLevel level in _levels)
            {
                if (level.HasFMV)
                {
                    return true;
                }
            }
            return false;
        }

        internal bool GetLevelsSupportFMVs()
        {
            foreach (TR23ScriptedLevel level in _levels)
            {
                if (level.SupportsFMVs)
                {
                    return true;
                }
            }
            return false;
        }

        internal void SetLevelsHaveFMV(bool haveFMVs)
        {
            foreach (TR23ScriptedLevel level in _levels)
            {
                level.HasFMV = haveFMVs;
            }
        }

        internal override void Save()
        {
            _script.Levels = Levels;
        }
    }
}