using TRGE.Core.Item.Enums;

namespace TRGE.Core
{
    public class TR2ScriptedLevel : AbstractTRScriptedLevel
    {
        private static readonly string[] _pistolInjectionLevels = new string[]
        {
            CreateID("FLOATING"), CreateID("XIAN")
        };

        public override ushort Sequence
        {
            get => GetOperation(TR23OpDefs.Level).Operand;
            set => GetOperation(TR23OpDefs.Level).Operand = value;
        }

        public override ushort TrackID
        {
            get => GetOperation(TR23OpDefs.Track).Operand;
            set => GetOperation(TR23OpDefs.Track).Operand = value;
        }

        public override bool HasFMV
        {
            get => HasActiveOperation(TR23OpDefs.FMV);
            set
            {
                SetOperationActive(TR23OpDefs.FMV, value);
                SetOperationActive(TR23OpDefs.ListStart, value);
                SetOperationActive(TR23OpDefs.ListEnd, value);
            }
        }

        public override bool SupportsFMVs => HasOperation(TR23OpDefs.FMV);

        public override bool HasStartAnimation
        {
            get => HasActiveOperation(TR23OpDefs.StartAnimation);
            set => SetOperationActive(TR23OpDefs.StartAnimation, value);
        }

        public override bool SupportsStartAnimations => HasOperation(TR23OpDefs.StartAnimation);

        public override short StartAnimationID
        {
            get
            {
                if (HasStartAnimation)
                {
                    return Convert.ToInt16(GetOperation(TR23OpDefs.StartAnimation).Operand);
                }
                return -1;
            }
            set
            {
                if (value == -1)
                {
                    HasStartAnimation = false;
                }
                else if (HasStartAnimation)
                {
                    GetOperation(TR23OpDefs.StartAnimation).Operand = Convert.ToUInt16(value);
                }
                else
                {
                    InsertOperation(TR23OpDefs.StartAnimation, Convert.ToUInt16(value), TR23OpDefs.StartAnimation.Next);
                }
            }
        }

        public override bool HasCutScene
        {
            get => HasActiveOperation(TR23OpDefs.Cinematic);
            set
            {
                SetOperationActive(TR23OpDefs.Cinematic, value);
                SetOperationActive(TR23OpDefs.CutAngle, value);
            }
        }

        public override bool SupportsCutScenes => HasOperation(TR23OpDefs.Cinematic);

        private TR2ScriptedLevel _cutSceneLevel;
        public override AbstractTRScriptedLevel CutSceneLevel
        {
            get => _cutSceneLevel;
            set => _cutSceneLevel = value as TR2ScriptedLevel;
        }

        public override bool HasSunset
        {
            get => HasActiveOperation(TR23OpDefs.Sunset);
            set
            {
                if (value)
                {
                    EnsureOperation(new TROperation(TR23OpDefs.Sunset, ushort.MaxValue, true));
                }
                else
                {
                    SetOperationActive(TR23OpDefs.Sunset, value);
                }
            }
        }

        public override bool HasDeadlyWater
        {
            get => HasActiveOperation(TR23OpDefs.DeadlyWater);
            set => SetOperationActive(TR23OpDefs.DeadlyWater, value);
        }

        public override bool RemovesWeapons
        {
            get => HasActiveOperation(TR23OpDefs.RemoveWeapons);
            set
            {
                if (value)
                {
                    EnsureOperation(new TROperation(TR23OpDefs.RemoveWeapons, ushort.MaxValue, true));
                }
                else
                {
                    SetOperationActive(TR23OpDefs.RemoveWeapons, value);
                }
            }
        }

        public bool RequiresWeaponTextureInjection
        {
            get
            {
                return _pistolInjectionLevels.Contains(ID);
            }
        }

        public override bool RemovesAmmo
        {
            get => HasActiveOperation(TR23OpDefs.RemoveAmmo);
            set
            {
                if (value)
                {
                    EnsureOperation(new TROperation(TR23OpDefs.RemoveAmmo, ushort.MaxValue, true));
                }
                else
                {
                    SetOperationActive(TR23OpDefs.RemoveAmmo, value);
                }
            }
        }

        /// <summary>
        /// In the script, the operand is either 0 for no secrets, or any non-zero value
        /// means secrets will be counted. If it's left out, the default is 3 - TODO: test
        /// what happens with anything other than 3.
        /// </summary>
        public override bool HasSecrets
        {
            get => !HasActiveOperation(TR23OpDefs.Secrets);
            set
            {
                if (value)
                {
                    SetOperationActive(TR23OpDefs.Secrets, false);
                }
                else
                {
                    EnsureOperation(new TROperation(TR23OpDefs.Secrets, 0, true));
                }
            }
        }

        /// <summary>
        /// Although the script is defined to specify the number of secrets, it's all hard-coded in the game.
        /// TR2 can have a maximum of 3 per level.
        /// </summary>
        public override ushort NumSecrets
        {
            get => (ushort)(HasSecrets ? 3 : 0);
            set { }
        }        

        /// <summary>
        /// This is set in HSH, although the game is hard-coded to complete after killing
        /// all enemies so this flag actually has no purpose. Remains untested in other
        /// levels.
        /// </summary>
        public override bool KillToComplete
        {
            get => HasOperation(TR23OpDefs.KillToComplete);
            set
            {
                if (value)
                {
                    EnsureOperation(new TROperation(TR23OpDefs.KillToComplete, ushort.MaxValue, true));
                }
                else
                {
                    SetOperationActive(TR23OpDefs.KillToComplete, value);
                }
            }
        }

        public override bool IsFinalLevel
        {
            get => HasActiveOperation(TR23OpDefs.GameComplete);
            set
            {
                if (value)
                {
                    TROperation gcOp = GetOperation(TR23OpDefs.GameComplete);
                    if (gcOp == null)
                    {
                        gcOp = GetOperation(TR23OpDefs.Complete);
                        if (gcOp == null)
                        {
                            gcOp = AddOperation(TR23OpDefs.GameComplete);
                        }
                    }
                    gcOp.Definition = TR23OpDefs.GameComplete;
                }
                else
                {
                    TROperation op = GetOperation(TR23OpDefs.GameComplete);
                    if (op != null)
                    {
                        op.Definition = TR23OpDefs.Complete;
                    }
                }
            }
        }

        protected override TROpDef GetOpDefFor(ushort scriptData)
        {
            return TR23OpDefs.Get(scriptData);
        }

        private void RemoveBonuses()
        {
            for (int i = _operations.Count - 1; i >= 0; i--)
            {
                if (_operations[i].Definition == TR23OpDefs.StartInvBonus && _operations[i].Operand < 1000)
                {
                    _operations.RemoveAt(i);
                }
            }
        }

        public void SetBonuses(List<TRItem> items)
        {
            RemoveBonuses();
            foreach (TRItem item in items)
            {
                AddBonusItem(item.ID);
            }
        }

        public void AddBonusItem(ushort itemID)
        {
            _operations.Insert(GetLastOperationIndex(TR23OpDefs.Level), new TROperation(TR23OpDefs.StartInvBonus, itemID, true));
        }

        internal int GetBonusItemCount(TRItem item, AbstractTRItemProvider itemProvider)
        {
            int i = 0;
            foreach (TRItem bonusItem in GetBonusItems(itemProvider))
            {
                if (bonusItem == item)
                {
                    i++;
                }
            }
            return i;
        }

        internal List<TRItem> GetBonusItems(AbstractTRItemProvider itemProvider, bool startInv = false)
        {
            List<TRItem> ret = new();
            foreach (TROperation opcmd in _operations)
            {
                if (opcmd.Definition == TR23OpDefs.StartInvBonus)
                {
                    ushort itemID = opcmd.Operand;
                    if (startInv && itemID > 999)
                    {
                        itemID -= 1000;
                        ret.Add(itemProvider.GetItem(itemID));
                    }
                    else if (!startInv && itemID < 1000)
                    {
                        ret.Add(itemProvider.GetItem(itemID));
                    }
                }
            }
            return ret;
        }

        public List<ushort> GetBonusItemIDs()
        {
            ISet<ushort> items = new SortedSet<ushort>();
            foreach (TROperation opcmd in _operations)
            {
                if (opcmd.Definition == TR23OpDefs.StartInvBonus)
                {
                    ushort itemID = opcmd.Operand;
                    if (itemID < 1000)
                    {
                        items.Add(itemID);
                    }
                }
            }
            return items.ToList();
        }

        public List<ushort> GetStartInventoryItemIDs()
        {
            ISet<ushort> items = new SortedSet<ushort>();
            foreach (TROperation opcmd in _operations)
            {
                if (opcmd.Definition == TR23OpDefs.StartInvBonus)
                {
                    ushort itemID = opcmd.Operand;
                    if (itemID > 999)
                    {
                        items.Add((ushort)(itemID - 1000));
                    }
                }
            }
            return items.ToList();
        }

        public int GetStartInventoryItemCount()
        {
            int count = 0;
            foreach (TROperation op in _operations)
            {
                if (op.Definition == TR23OpDefs.StartInvBonus && op.Operand > 999)
                {
                    count++;
                }
            }
            return count;
        }

        public Dictionary<ushort, int> GetStartInventoryItems()
        {
            Dictionary<ushort, int> items = new();

            foreach (TROperation opcmd in _operations)
            {
                if (opcmd.Definition == TR23OpDefs.StartInvBonus)
                {
                    ushort itemID = opcmd.Operand;
                    if (itemID > 999)
                    {
                        ushort itemType = (ushort)(itemID - 1000);
                        if (!items.ContainsKey(itemType))
                        {
                            items[itemType] = 0;
                        }
                        items[itemType]++;
                    }
                }
            }

            return items;
        }

        public void SetStartInventoryItems(Dictionary<TR2Items, int> items)
        {
            SetStartInventoryItems(items.ToDictionary(item => (ushort)item.Key, item => item.Value));
        }

        public void SetStartInventoryItems(Dictionary<ushort, int> items)
        {
            ClearStartInventoryItems();

            foreach (ushort item in items.Keys)
            {
                AddStartInventoryItem(item, (uint)items[item]);
            }
        }

        public void AddStartInventoryItem(TR2Items item, uint count = 1)
        {
            AddStartInventoryItem((ushort)item, count);
        }

        public void RemoveStartInventoryItem(TR2Items item, bool removeAll = false)
        {
            RemoveStartInventoryItem((ushort)item, removeAll);
        }

        public void AddStartInventoryItem(ushort item, uint count = 1)
        {
            ushort itemID = (ushort)(1000 + item);
            for (int i = 0; i < count; i++)
            {
                _operations.Insert(GetLastOperationIndex(TR23OpDefs.Level), new TROperation(TR23OpDefs.StartInvBonus, itemID, true));
            }
        }

        public void RemoveStartInventoryItem(ushort item, bool removeAll = false)
        {
            ushort itemID = (ushort)(1000 + item);
            for (int i = _operations.Count - 1; i >= 0; i--)
            {
                TROperation op = _operations[i];
                if (op.Definition == TR23OpDefs.StartInvBonus && op.Operand == itemID)
                {
                    _operations.RemoveAt(i);

                    if (!removeAll)
                    {
                        break;
                    }
                }
            }
        }

        public void ClearStartInventoryItems()
        {
            for (int i = _operations.Count - 1; i >= 0; i--)
            {
                TROperation op = _operations[i];
                if (op.Definition == TR23OpDefs.StartInvBonus && op.Operand > 999)
                {
                    _operations.RemoveAt(i);
                }
            }
        }
    }
}