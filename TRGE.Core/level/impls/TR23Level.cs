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
            set => SetOperationActive(TR23OpDefs.RemoveWeapons, value);
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
                    GetOperation(TR23OpDefs.Complete).TROpDef = TR23OpDefs.GameComplete;
                }
                else
                {
                    TROperation op = GetOperation(TR23OpDefs.GameComplete);
                    if (op != null)
                    {
                        op.TROpDef = TR23OpDefs.Complete;
                    }
                }
            }
        }

        protected override TROpDef GetOpDefFor(ushort scriptData)
        {
            return TR23OpDefs.Get(scriptData);
        }
    }
}