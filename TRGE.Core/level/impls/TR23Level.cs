using System.Collections.Generic;

namespace TRGE.Core
{
    internal class TR23Level : AbstractTRLevel
    {
        internal override ushort Sequence
        {
            get => GetOperation(TR23OpDefs.Level).Operand;
            set => GetOperation(TR23OpDefs.Level).Operand = value;
        }

        internal override bool HasFMV
        {
            get => HasActiveOperation(TR23OpDefs.FMV);
            set => SetOperationActive(TR23OpDefs.FMV, value);
        }

        internal override bool HasStartAnimation
        {
            get => HasActiveOperation(TR23OpDefs.StartAnimation);
            set => SetOperationActive(TR23OpDefs.StartAnimation, value);
        }

        internal override bool HasCutScene
        {
            get => HasActiveOperation(TR23OpDefs.Cinematic);
            set => SetOperationActive(TR23OpDefs.Cinematic, value);
        }

        internal override bool HasSunset
        {
            get => HasActiveOperation(TR23OpDefs.Sunset);
            set => SetOperationActive(TR23OpDefs.Sunset, value);
        }

        internal override bool HasDeadlyWater
        {
            get => HasActiveOperation(TR23OpDefs.DeadlyWater);
            set => SetOperationActive(TR23OpDefs.DeadlyWater, value);
        }

        internal override bool RemovesWeapons
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

        internal override bool RemovesAmmo
        {
            get => HasActiveOperation(TR23OpDefs.RemoveAmmo);
            set => SetOperationActive(TR23OpDefs.RemoveAmmo, value);
        }

        internal override bool HasSecrets
        {
            get => !HasActiveOperation(TR23OpDefs.Secrets);
            set => SetOperationActive(TR23OpDefs.Secrets, !value);
        }

        internal override bool KillToComplete => HasOperation(TR23OpDefs.KillToComplete);

        internal override bool IsFinalLevel
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
                    //GetOperation(TR23OpDefs.Complete).Definition = TR23OpDefs.GameComplete;
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

        internal void SetBonuses(List<TRItem> items)
        {
            RemoveBonuses();
            foreach (TRItem item in items)
            {
                AddBonusItem(item.ID);
            }
        }

        private void AddBonusItem(ushort itemID)
        {
            int pos = GetLastOperationIndex(TR23OpDefs.StartInvBonus);
            if (pos == -1)
            {
                pos = GetOperationIndex(TR23OpDefs.StartInvBonus.Next);
            }
            _operations.Insert(pos, new TROperation(TR23OpDefs.StartInvBonus, itemID, true));
        }

        internal List<MutableTuple<ushort, TRItemCategory, string, int>> GetBonusItemData(AbstractTRItemProvider provider)
        {
            List<MutableTuple<ushort, TRItemCategory, string, int>> items = new List<MutableTuple<ushort, TRItemCategory, string, int>>();
            foreach (TRItem item in provider.BonusItems)
            {
                items.Add(new MutableTuple<ushort, TRItemCategory, string, int>(item.ID, item.Category, item.Name, GetBonusItemCount(item, provider)));
            }
            return items;
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
            return i == 0 ? -1 : i;
        }

        internal List<TRItem> GetBonusItems(AbstractTRItemProvider itemProvider, bool startInv = false)
        {
            List<TRItem> ret = new List<TRItem>();
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

        internal void SetBonusItemData(List<MutableTuple<ushort, TRItemCategory, string, int>> items)
        {
            RemoveBonuses();
            foreach (MutableTuple<ushort, TRItemCategory, string, int> item in items)
            {
                for (int i = 0; i < item.Item4; i++)
                {
                    AddBonusItem(item.Item1);
                }
            }
        }
    }
}