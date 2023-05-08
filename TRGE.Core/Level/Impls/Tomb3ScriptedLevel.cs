namespace TRGE.Core
{
    public class Tomb3ScriptedLevel : TR3ScriptedLevel
    {
        public override bool HasRain
        {
            get => HasActiveOperation(TR23OpDefs.HasRain);
            set
            {
                if (value)
                {
                    EnsureOperation(new TROperation(TR23OpDefs.HasRain, ushort.MaxValue, true));
                }
                else
                {
                    SetOperationActive(TR23OpDefs.HasRain, value);
                }
            }
        }

        public override bool HasSnow
        {
            get => HasActiveOperation(TR23OpDefs.HasSnow);
            set
            {
                if (value)
                {
                    EnsureOperation(new TROperation(TR23OpDefs.HasSnow, ushort.MaxValue, true));
                }
                else
                {
                    SetOperationActive(TR23OpDefs.HasSnow, value);
                }
            }
        }

        public override bool HasColdWater
        {
            get => HasActiveOperation(TR23OpDefs.IsCold);
            set
            {
                if (value)
                {
                    EnsureOperation(new TROperation(TR23OpDefs.IsCold, ushort.MaxValue, true));
                }
                else
                {
                    SetOperationActive(TR23OpDefs.IsCold, value);
                }
            }
        }
    }
}
