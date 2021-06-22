using System;
using System.Collections.Generic;
using System.Linq;
using TRGE.Core.Item.Enums;

namespace TRGE.Core
{
    internal class TR23LevelManager : AbstractTRLevelManager
    {
        private readonly AbstractTR23AudioProvider _audioProvider;
        private readonly AbstractTR23ItemProvider _itemProvider;
        private readonly TR23Script _script;
        private readonly TR23ScriptedLevel _assaultLevel;
        private List<TR23ScriptedLevel> _levels;

        internal override int LevelCount => _levels.Count;
        internal override AbstractTRAudioProvider AudioProvider => _audioProvider;
        internal override AbstractTRItemProvider ItemProvider => _itemProvider;
        internal bool CanOrganiseBonuses => Edition.SecretBonusesSupported;

        internal override AbstractTRScriptedLevel AssaultLevel => _assaultLevel;
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
            _assaultLevel = _script.AssaultLevel as TR23ScriptedLevel;
            _audioProvider = TRAudioFactory.GetAudioProvider(script.Edition) as AbstractTR23AudioProvider;
            _itemProvider = TRItemFactory.GetProvider(script.Edition, script.GameStrings1) as AbstractTR23ItemProvider;
        }

        protected override TRScriptedLevelModification OpDefToModification(TROpDef opDef)
        {
            if (opDef == TR23OpDefs.RemoveAmmo)
            {
                return TRScriptedLevelModification.AmmolessStateChanged;
            }
            if (opDef == TR23OpDefs.RemoveWeapons)
            {
                return TRScriptedLevelModification.WeaponlessStateChanged;
            }
            if (opDef == TR23OpDefs.Sunset)
            {
                return TRScriptedLevelModification.SunsetChanged;
            }
            return TRScriptedLevelModification.Generic;
        }

        internal List<TR23ScriptedLevel> GetAmmolessLevels()
        {
            return GetLevelsWithOperation(TR23OpDefs.RemoveAmmo, true).Cast<TR23ScriptedLevel>().ToList();
        }

        internal uint GetAmmolessLevelCount()
        {
            return Convert.ToUInt32(GetAmmolessLevels().Count);
        }

        internal virtual List<MutableTuple<string, string, bool>> GetAmmolessLevelData(List<AbstractTRScriptedLevel> originalLevels)
        {
            return GetAmmolessLevelData(Levels, originalLevels);
        }

        private List<MutableTuple<string, string, bool>> GetAmmolessLevelData(List<AbstractTRScriptedLevel> levels, List<AbstractTRScriptedLevel> originalLevels)
        {
            List<MutableTuple<string, string, bool>> data = new List<MutableTuple<string, string, bool>>();
            foreach (AbstractTRScriptedLevel originalLevel in originalLevels)
            {
                TR23ScriptedLevel level = GetLevel(originalLevel.ID) as TR23ScriptedLevel;
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
                    FireLevelModificationEvent(level, TRScriptedLevelModification.AmmolessStateChanged);
                }
            }
        }

        internal void RandomiseAmmolessLevels(List<AbstractTRScriptedLevel> basisLevels)
        {
            RandomiseLevelsWithOperation
            (
                AmmolessLevelRNG,
                RandomAmmolessLevelCount,
                basisLevels,
                new TROperation(TR23OpDefs.RemoveAmmo, ushort.MaxValue, true)
            );
        }

        internal void RestoreAmmolessLevels(List<AbstractTRScriptedLevel> originalLevels)
        {
            SetAmmolessLevelData(GetAmmolessLevelData(originalLevels));
        }

        internal void RandomiseUnarmedLevels(List<AbstractTRScriptedLevel> basisLevels)
        {
            RandomiseLevelsWithOperation
            (
                UnarmedLevelRNG,
                RandomUnarmedLevelCount, 
                basisLevels, 
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

        internal virtual List<MutableTuple<string, string, bool>> GetUnarmedLevelData(List<AbstractTRScriptedLevel> originalLevels)
        {
            return GetUnarmedLevelData(Levels, originalLevels);
        }

        private List<MutableTuple<string, string, bool>> GetUnarmedLevelData(List<AbstractTRScriptedLevel> levels, List<AbstractTRScriptedLevel> originalLevels)
        {
            List<MutableTuple<string, string, bool>> data = new List<MutableTuple<string, string, bool>>();
            foreach (AbstractTRScriptedLevel originalLevel in originalLevels)
            {
                TR23ScriptedLevel level = GetLevel(originalLevel.ID) as TR23ScriptedLevel;
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
                    FireLevelModificationEvent(level, TRScriptedLevelModification.WeaponlessStateChanged);
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
            TRItem flare = (ItemProvider as TR2ItemProvider).Flare;
            Dictionary<TRItemCategory, ISet<TRItem>> exclusions = new Dictionary<TRItemCategory, ISet<TRItem>>
            {
                { TRItemCategory.Weapon, new HashSet<TRItem> { shotgun } },
                { TRItemCategory.Misc, new HashSet<TRItem> { flare } }
            };
            Random rand = BonusRNG.Create();

            string hshID = AbstractTRScriptedLevel.CreateID("HOUSE");
            foreach (TR23ScriptedLevel level in _levels)
            {
                if (level.HasSecrets)
                {
                    if (level.RemovesWeapons && !level.ID.Equals(hshID))
                    {
                        //shotgun will only be given if Lara has lost her weapons and it's not Home Sweet Home
                        exclusions[TRItemCategory.Weapon].Remove(shotgun);
                    }
                    else if (level.ID.Equals(hshID))
                    {
                        exclusions[TRItemCategory.Weapon].Add(shotgun); //TODO: there should perhaps be a map of sorts to exclude specific weapons for each level
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

        internal List<MutableTuple<string, string, List<MutableTuple<ushort, TRItemCategory, string, int>>>> GetLevelBonusData(List<AbstractTRScriptedLevel> originalLevels)
        {
            return GetLevelBonusData(Levels, originalLevels);
        }

        private List<MutableTuple<string, string, List<MutableTuple<ushort, TRItemCategory, string, int>>>> GetLevelBonusData(List<AbstractTRScriptedLevel> levels, List<AbstractTRScriptedLevel> originalLevels)
        {
            List<MutableTuple<string, string, List<MutableTuple<ushort, TRItemCategory, string, int>>>> data = new List<MutableTuple<string, string, List<MutableTuple<ushort, TRItemCategory, string, int>>>>();
            foreach (AbstractTRScriptedLevel originalLevel in originalLevels)
            {
                TR23ScriptedLevel level = GetLevel(originalLevel.ID) as TR23ScriptedLevel;
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

        internal void MakeStartingWeaponsAvailable(TR23ScriptedLevel level, bool available)
        {
            if (available)
            {
                Dictionary<TRItems, int> weapons = null;
                switch (Edition.Version)
                {
                    case TRVersion.TR2:
                        weapons = new Dictionary<TRItems, int>
                        {
                            [TRItems.Pistols] = 1,
                            [TRItems.Shotgun] = 1,
                            [TRItems.ShotgunShells] = 4,
                            [TRItems.AutoPistols] = 1,
                            [TRItems.AutoClips] = 4,
                            [TRItems.Uzis] = 1,
                            [TRItems.UziClips] = 4,
                            [TRItems.HarpoonGun] = 1,
                            [TRItems.Harpoons] = 4,
                            [TRItems.M16] = 1,
                            [TRItems.M16Clips] = 4,
                        };
                        break;
                }

                if (weapons != null)
                {
                    level.SetStartInventoryItems(weapons);
                    FireLevelModificationEvent(level, TRScriptedLevelModification.StartingWeaponsAdded);
                }
            }
            else
            {
                FireLevelModificationEvent(level, TRScriptedLevelModification.StartingWeaponsRemoved);
            }
        }

        internal void MakeSkidooAvailable(TR23ScriptedLevel level, bool available)
        {
            FireLevelModificationEvent(level, available ? TRScriptedLevelModification.SkidooAdded : TRScriptedLevelModification.SkidooRemoved);
        }

        internal override void Save()
        {
            if (Edition.AssaultCourseSupported)
            {
                _script.AssaultLevel = AssaultLevel;
            }
            _script.Levels = Levels;
        }
    }
}