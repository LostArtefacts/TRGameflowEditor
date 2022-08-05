using System;
using System.Collections.Generic;
using System.Linq;
using TRGE.Core.Item.Enums;

namespace TRGE.Core
{
    internal class TR1LevelManager : AbstractTRLevelManager
    {
        private readonly TR1AudioProvider _audioProvider;
        private readonly TR1Script _script;
        private readonly TR1ScriptedLevel _assaultLevel;
        private List<TR1ScriptedLevel> _levels;

        internal override int LevelCount => _levels.Count;
        public override AbstractTRAudioProvider AudioProvider => _audioProvider;
        internal override AbstractTRItemProvider ItemProvider => throw new NotSupportedException();

        internal override AbstractTRScriptedLevel AssaultLevel => _assaultLevel;
        internal override List<AbstractTRScriptedLevel> Levels
        {
            get => _levels.Cast<AbstractTRScriptedLevel>().ToList();
            set
            {
                _levels = value.Cast<TR1ScriptedLevel>().ToList();
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

        internal Organisation UnarmedLevelOrganisation { get; set; }
        internal RandomGenerator UnarmedLevelRNG { get; set; }
        internal uint RandomUnarmedLevelCount { get; set; }

        internal Organisation AmmolessLevelOrganisation { get; set; }
        internal RandomGenerator AmmolessLevelRNG { get; set; }
        internal uint RandomAmmolessLevelCount { get; set; }

        internal TR1LevelManager(TR1Script script)
            : base(script.Edition)
        {
            Levels = (_script = script).Levels;
            _assaultLevel = _script.AssaultLevel as TR1ScriptedLevel;
            _audioProvider = TRAudioFactory.GetAudioProvider(script.Edition) as TR1AudioProvider;
        }

        internal override void Save()
        {
            _script.Levels = Levels;
        }

        internal override void UpdateScript()
        {
            _script.Levels = Levels.FindAll(l => l.Enabled);
        }

        protected override void FinalLevelChanged(AbstractTRScriptedLevel oldFinalLevel, AbstractTRScriptedLevel newFinalLevel)
        {
            TR1ScriptedLevel oldLevel = (oldFinalLevel.HasCutScene ? oldFinalLevel.CutSceneLevel : oldFinalLevel) as TR1ScriptedLevel;
            TR1ScriptedLevel newLevel = (newFinalLevel.HasCutScene ? newFinalLevel.CutSceneLevel : newFinalLevel) as TR1ScriptedLevel;

            List<BaseLevelSequence> endSequences = new List<BaseLevelSequence>();
            int statsIndex = oldLevel.Sequences.FindIndex(s => s.Type == LevelSequenceType.Level_Stats);
            for (int i = oldLevel.Sequences.Count - 1; i > statsIndex; i--)
            {
                BaseLevelSequence sequence = oldLevel.Sequences[i];
                endSequences.Insert(0, sequence);
                oldLevel.Sequences.Remove(sequence);
            }

            oldLevel.Sequences.Add(new LevelExitLevelSequence
            {
                Type = LevelSequenceType.Exit_To_Level,
                LevelId = oldFinalLevel.Sequence + 1
            });

            newLevel.Sequences.RemoveAll(s => s.Type == LevelSequenceType.Exit_To_Level);
            newLevel.Sequences.AddRange(endSequences);
        }

        protected override TRScriptedLevelModification OpDefToModification(TROpDef opDef)
        {
            throw new NotImplementedException();
        }

        internal List<TR1ScriptedLevel> GetLevelsWithSequence(LevelSequenceType type)
        {
            return _levels.FindAll(l => l.HasSequence(type));
        }

        internal List<TR1ScriptedLevel> GetAmmolessLevels()
        {
            return _levels.FindAll(l => l.RemovesAmmo);
        }

        internal uint GetAmmolessLevelCount()
        {
            return (uint)GetAmmolessLevels().Count;
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
                TR1ScriptedLevel level = GetLevel(originalLevel.ID) as TR1ScriptedLevel;
                data.Add(new MutableTuple<string, string, bool>(level.ID, level.Name, level.RemovesAmmo));
            }
            return data;
        }

        internal virtual void SetAmmolessLevelData(List<MutableTuple<string, string, bool>> data)
        {
            foreach (MutableTuple<string, string, bool> item in data)
            {
                TR1ScriptedLevel level = (TR1ScriptedLevel)GetLevel(item.Item1);
                if (level != null && level.RemovesAmmo != item.Item3)
                {
                    level.RemovesAmmo = item.Item3;
                    FireLevelModificationEvent(level, TRScriptedLevelModification.AmmolessStateChanged);
                }
            }
        }

        internal void RandomiseAmmolessLevels(List<AbstractTRScriptedLevel> basisLevels)
        {
            List<AbstractTRScriptedLevel> enabledLevels = new List<AbstractTRScriptedLevel>();
            foreach (AbstractTRScriptedLevel originalLevel in basisLevels)
            {
                AbstractTRScriptedLevel lvl = GetLevel(originalLevel.ID);
                if (lvl.Enabled)
                {
                    enabledLevels.Add(originalLevel);
                }
            }

            List<AbstractTRScriptedLevel> levelSet = enabledLevels.RandomSelection(AmmolessLevelRNG.Create(), RandomAmmolessLevelCount);
            foreach (AbstractTRScriptedLevel level in Levels)
            {
                level.RemovesAmmo = levelSet.Contains(level);
                FireLevelModificationEvent(level, TRScriptedLevelModification.AmmolessStateChanged);
            }
        }

        internal void RestoreAmmolessLevels(List<AbstractTRScriptedLevel> originalLevels)
        {
            SetAmmolessLevelData(GetAmmolessLevelData(originalLevels));
        }

        internal void RandomiseUnarmedLevels(List<AbstractTRScriptedLevel> basisLevels)
        {
            RemoveUnarmedDependencies();

            List<AbstractTRScriptedLevel> enabledLevels = new List<AbstractTRScriptedLevel>();
            foreach (AbstractTRScriptedLevel originalLevel in basisLevels)
            {
                AbstractTRScriptedLevel lvl = GetLevel(originalLevel.ID);
                if (lvl.Enabled)
                {
                    enabledLevels.Add(originalLevel);
                }
            }

            List<AbstractTRScriptedLevel> levelSet = enabledLevels.RandomSelection(UnarmedLevelRNG.Create(), RandomUnarmedLevelCount);
            foreach (AbstractTRScriptedLevel level in Levels)
            {
                level.RemovesWeapons = levelSet.Contains(level);
                FireLevelModificationEvent(level, TRScriptedLevelModification.WeaponlessStateChanged);
            }

            AddUnarmedDependencies();
        }

        internal List<TR1ScriptedLevel> GetUnarmedLevels()
        {
            return _levels.FindAll(l => l.RemovesWeapons);
        }

        internal uint GetUnarmedLevelCount()
        {
            return (uint)GetUnarmedLevels().Count;
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
                TR1ScriptedLevel level = GetLevel(originalLevel.ID) as TR1ScriptedLevel;
                data.Add(new MutableTuple<string, string, bool>(level.ID, level.Name, level.RemovesWeapons));
            }
            return data;
        }

        internal virtual void SetUnarmedLevelData(List<MutableTuple<string, string, bool>> data)
        {
            RemoveUnarmedDependencies();

            foreach (MutableTuple<string, string, bool> item in data)
            {
                TR1ScriptedLevel level = (TR1ScriptedLevel)GetLevel(item.Item1);
                if (level != null)
                {
                    level.RemovesWeapons = item.Item3;
                    FireLevelModificationEvent(level, TRScriptedLevelModification.WeaponlessStateChanged);
                }
            }

            AddUnarmedDependencies();
        }

        internal void RestoreUnarmedLevels(List<AbstractTRScriptedLevel> originalLevels)
        {
            SetUnarmedLevelData(GetUnarmedLevelData(originalLevels));
        }

        private void RemoveUnarmedDependencies()
        {
            // Remove pistols from the level following one that was unarmed
            foreach (TR1ScriptedLevel level in Levels)
            {
                if (level.RemovesWeapons)
                {
                    if (Levels.Find(l => l.Sequence == level.Sequence + 1 && l.Enabled) is TR1ScriptedLevel nextLevel)
                    {
                        nextLevel.RemoveStartInventoryItem(TR1Items.Pistols);
                    }
                }
            }
        }

        private void AddUnarmedDependencies()
        {
            // Add pistols to each level that follows one that's unarmed
            foreach (TR1ScriptedLevel level in Levels)
            {
                if (level.RemovesWeapons)
                {
                    if (Levels.Find(l => l.Sequence == level.Sequence + 1 && l.Enabled) is TR1ScriptedLevel nextLevel && !nextLevel.RemovesWeapons)
                    {
                        if (nextLevel.GetStartInventoryItem(TR1Items.Pistols) == null)
                        {
                            nextLevel.AddStartInventoryItem(TR1Items.Pistols);
                        }
                    }
                }
            }
        }

        internal bool GetLevelsHaveCutScenes()
        {
            foreach (TR1ScriptedLevel level in _levels)
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
            foreach (TR1ScriptedLevel level in _levels)
            {
                level.HasCutScene = haveCutScenes;
            }
        }

        internal bool GetLevelsSupportCutScenes()
        {
            foreach (TR1ScriptedLevel level in _levels)
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
            foreach (TR1ScriptedLevel level in _levels)
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
            foreach (TR1ScriptedLevel level in _levels)
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
            foreach (TR1ScriptedLevel level in _levels)
            {
                level.HasFMV = haveFMVs;
            }
        }

        internal bool GetLevelsHaveDemos()
        {
            return _levels.FindAll(l => l.Demo.HasValue && l.Demo.Value).Count > 0;
        }

        internal void SetLevelsHaveDemos(bool value, List<AbstractTRScriptedLevel> originalLevels)
        {
            foreach (AbstractTRScriptedLevel originalLevel in originalLevels)
            {
                TR1ScriptedLevel origLevel = originalLevel as TR1ScriptedLevel;
                TR1ScriptedLevel level = GetLevel(originalLevel.ID) as TR1ScriptedLevel;
                level.Demo = value && origLevel.Demo.HasValue && origLevel.Demo.Value;
            }
        }
    }
}