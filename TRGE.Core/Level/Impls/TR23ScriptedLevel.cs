using System;
using System.Collections.Generic;
using System.Linq;

namespace TRGE.Core
{
    internal class TR23ScriptedLevel : AbstractTRScriptedLevel
    {
        private static readonly string[] PistolInjectionLevels = new string[]
        {
            CreateID("FLOATING"), CreateID("XIAN"), CreateID("HOUSE")
        };

        internal override ushort Sequence
        {
            get => GetOperation(TR23OpDefs.Level).Operand;
            set => GetOperation(TR23OpDefs.Level).Operand = value;
        }

        internal override ushort TrackID
        {
            get => GetOperation(TR23OpDefs.Track).Operand;
            set => GetOperation(TR23OpDefs.Track).Operand = value;
        }

        internal override bool HasFMV
        {
            get => HasActiveOperation(TR23OpDefs.FMV);
            set
            {
                SetOperationActive(TR23OpDefs.FMV, value);
                SetOperationActive(TR23OpDefs.ListStart, value);
                SetOperationActive(TR23OpDefs.ListEnd, value);
            }
        }

        internal override bool SupportsFMVs => HasOperation(TR23OpDefs.FMV);

        internal override bool HasStartAnimation
        {
            get => HasActiveOperation(TR23OpDefs.StartAnimation);
            set => SetOperationActive(TR23OpDefs.StartAnimation, value);
        }

        internal override bool SupportsStartAnimations => HasOperation(TR23OpDefs.StartAnimation);

        internal override short StartAnimationID
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

        internal override bool HasCutScene
        {
            get => HasActiveOperation(TR23OpDefs.Cinematic);
            set
            {
                SetOperationActive(TR23OpDefs.Cinematic, value);
                SetOperationActive(TR23OpDefs.CutAngle, value);
            }
        }

        internal override bool SupportsCutScenes => HasOperation(TR23OpDefs.Cinematic);

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

        internal bool RequiresPistolTextureInjection
        {
            get
            {
                return PistolInjectionLevels.Contains(ID);
            }
        }

        internal override bool RemovesAmmo
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
    }
}