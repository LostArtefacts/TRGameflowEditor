using System;
using System.Collections.Generic;
using System.Linq;
using TRGE.Core.Item.Enums;

namespace TRGE.Core
{
    public class TR1ScriptedLevel : AbstractTRScriptedLevel
    {
        private static readonly Dictionary<ushort, ushort> _levelSecrets = new Dictionary<ushort, ushort>
        {
            // Gym
            [0] = 0,
            // Peru
            [1] = 3,
            [2] = 3,
            [3] = 5,
            [4] = 3,
            // Greece
            [5] = 4,
            [6] = 3,
            [7] = 3,
            [8] = 3,
            [9] = 2,
            // Egypt
            [10] = 3,
            [11] = 3,
            [12] = 1,
            // Atlantis
            [13] = 3,
            [14] = 3,
            [15] = 3
        };

        private ushort _sequence;
        public override ushort Sequence
        {
            get => _sequence;
            set
            {
                _sequence = value;
                AdjustSequenceDependencies();
            }
        }

        private void AdjustSequenceDependencies()
        {
            AdjustSequenceDependencies(this);
            if (HasCutScene)
            {
                AdjustSequenceDependencies(CutSceneLevel as TR1ScriptedLevel);
            }
        }

        private void AdjustSequenceDependencies(TR1ScriptedLevel level)
        {
            if (level.Sequences == null)
            {
                return;
            }

            BaseLevelSequence sequence = level.Sequences.Find(s => s.Type == LevelSequenceType.Level_Stats);
            if (sequence is LevelExitLevelSequence levelStatsSequence)
            {
                levelStatsSequence.LevelId = _sequence;
            }

            sequence = level.Sequences.Find(s => s.Type == LevelSequenceType.Exit_To_Level);
            if (sequence is LevelExitLevelSequence levelExitSequence)
            {
                levelExitSequence.LevelId = _sequence + 1;
            }
        }

        public int Music { get; set; }
        public override ushort TrackID
        {
            get => (ushort)Music;
            set => Music = value;
        }
        public override bool HasFMV { get; set; }

        public override bool SupportsFMVs => HasSequence(LevelSequenceType.Play_FMV);

        public override bool HasCutScene
        {
            get => CutSceneLevel != null;
            set { }
        }

        public override bool SupportsCutScenes => HasSequence(LevelSequenceType.Exit_To_Cine);

        public override AbstractTRScriptedLevel CutSceneLevel { get; set; }

        public override bool RemovesWeapons
        {
            get => HasSequence(LevelSequenceType.Remove_Guns);
            set
            {
                if (value)
                {
                    AddSequenceBefore(LevelSequenceType.Start_Game, new BaseLevelSequence { Type = LevelSequenceType.Remove_Guns }, false);
                }
                else
                {
                    RemoveSequence(LevelSequenceType.Remove_Guns);
                }
            }
        }

        public override bool RemovesAmmo
        {
            get => HasSequence(LevelSequenceType.Remove_Ammo);
            set
            {
                if (value)
                {
                    AddSequenceBefore(LevelSequenceType.Start_Game, new BaseLevelSequence { Type = LevelSequenceType.Remove_Ammo }, false);
                }
                else
                {
                    RemoveSequence(LevelSequenceType.Remove_Ammo);
                }
            }
        }

        public bool RemovesMedis
        {
            get => HasSequence(LevelSequenceType.Remove_Medipacks);
            set
            {
                if (value)
                {
                    AddSequenceBefore(LevelSequenceType.Start_Game, new BaseLevelSequence { Type = LevelSequenceType.Remove_Medipacks }, false);
                }
                else
                {
                    RemoveSequence(LevelSequenceType.Remove_Medipacks);
                }
            }
        }

        public override bool HasSecrets
        {
            get => NumSecrets > 0;
            set { }
        }

        private ushort? _numSecrets;
        public override ushort NumSecrets
        {
            get => _numSecrets ?? _levelSecrets[OriginalSequence];
            set => _numSecrets = value;
        }

        public override bool IsFinalLevel { get; set; }

        public LevelType Type { get; set; }

        public List<BaseLevelSequence> Sequences { get; set; }

        public string[] Injections { get; set; }
        public bool? InheritInjections { get; set; }
        public uint? LaraType { get; set; }
        public bool? Demo { get; set; }
        public double[] WaterColor { get; set; }
        public double? DrawDistanceFade { get; set; }
        public double? DrawDistanceMax { get; set; }
        public int? UnobtainablePickups { get; set; }
        public int? UnobtainableKills { get; set; }
        public List<TR1ItemDrop> ItemDrops { get; set; }

        public void ResetInjections()
        {
            Injections = null;
            InheritInjections = null;
            if (HasCutScene)
            {
                (CutSceneLevel as TR1ScriptedLevel).ResetInjections();
            }
        }

        public void AddItemDrops(int enemyNum, params TR1Items[] items)
        {
            AddItemDrops(enemyNum, (IEnumerable<TR1Items>)items);
        }

        public void AddItemDrops(int enemyNum, IEnumerable<TR1Items> items)
        {
            ItemDrops ??= new();
            TR1ItemDrop drop = ItemDrops.Find(i => i.EnemyNum == enemyNum);
            if (drop == null)
            {
                ItemDrops.Add(drop = new()
                {
                    EnemyNum = enemyNum,
                    ObjectIds = new()
                });
            }

            drop.ObjectIds.AddRange(items);
        }

        public bool HasSequence(LevelSequenceType type)
        {
            return Sequences.Any(s => s.Type == type);
        }

        public List<GiveItemLevelSequence> GetStartInventoryItems()
        {
            List<GiveItemLevelSequence> items = new List<GiveItemLevelSequence>();
            foreach (BaseLevelSequence sequence in Sequences)
            {
                if (sequence is GiveItemLevelSequence giver)
                {
                    items.Add(giver);
                }
            }

            return items;
        }

        public int GetStartInventoryItemCount()
        {
            int count = 0;
            foreach (BaseLevelSequence sequence in Sequences)
            {
                if (sequence is GiveItemLevelSequence giver)
                {
                    count += giver.Quantity;
                }
            }
            return count;
        }

        public void SetStartInventoryItems(Dictionary<TR1Items, int> items)
        {
            ClearStartInventoryItems();

            foreach (TR1Items item in items.Keys)
            {
                AddStartInventoryItem(item, (uint)items[item]);
            }
        }

        public void AddStartInventoryItem(TR1Items item, uint count = 1)
        {
            if (count > 0)
            {
                AddSequenceAfter(LevelSequenceType.Start_Game, new GiveItemLevelSequence
                {
                    Type = LevelSequenceType.Give_Item,
                    ObjectId = item,
                    Quantity = (int)count
                });
            }
        }

        public GiveItemLevelSequence GetStartInventoryItem(TR1Items item)
        {
            return Sequences.Find(s => s is GiveItemLevelSequence giver && giver.ObjectId == item) as GiveItemLevelSequence;
        }

        public void RemoveStartInventoryItem(TR1Items item)
        {
            Sequences.RemoveAll(s => s is GiveItemLevelSequence giver && giver.ObjectId == item);
        }

        public void ClearStartInventoryItems()
        {
            Sequences.RemoveAll(s => s.Type == LevelSequenceType.Give_Item);
        }

        public void AddSequenceBefore(LevelSequenceType beforeType, BaseLevelSequence sequence, bool allowDuplicates = true)
        {
            if (!allowDuplicates && Sequences.Find(s => s.Type == sequence.Type) != null)
            {
                return;
            }

            int insertIndex = Sequences.FindIndex(s => s.Type == beforeType);
            Sequences.Insert(insertIndex, sequence);
        }

        public void AddSequenceAfter(LevelSequenceType afterType, BaseLevelSequence sequence, bool allowDuplicates = true)
        {
            if (!allowDuplicates && Sequences.Find(s => s.Type == sequence.Type) != null)
            {
                return;
            }

            int insertIndex = Sequences.FindIndex(s => s.Type == afterType) + 1;
            Sequences.Insert(insertIndex, sequence);
        }

        public void RemoveSequence(LevelSequenceType type)
        {
            Sequences.RemoveAll(s => s.Type == type);
        }

        #region Obsolete
        public override bool HasStartAnimation
        {
            get => false;
            set { }
        }

        public override bool SupportsStartAnimations => false;

        public override short StartAnimationID
        {
            get => -1;
            set { }
        }

        public override bool HasSunset
        {
            get => false;
            set { }
        }

        public override bool HasDeadlyWater
        {
            get => false;
            set { }
        }

        public override bool KillToComplete
        {
            get => false;
            set { }
        }

        protected override TROpDef GetOpDefFor(ushort scriptData)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}