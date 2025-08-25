namespace TRGE.Core;

public class Tomb3ScriptedLevel : TR3ScriptedLevel
{
    public override bool HasRain
    {
        get => HasActiveOperation(TR3OpDefs.HasRain);
        set
        {
            if (value)
            {
                EnsureOperation(new TROperation(TR3OpDefs.HasRain, ushort.MaxValue, true));
            }
            else
            {
                SetOperationActive(TR3OpDefs.HasRain, value);
            }
        }
    }

    public override bool HasSnow
    {
        get => HasActiveOperation(TR3OpDefs.HasSnow);
        set
        {
            if (value)
            {
                EnsureOperation(new TROperation(TR3OpDefs.HasSnow, ushort.MaxValue, true));
            }
            else
            {
                SetOperationActive(TR3OpDefs.HasSnow, value);
            }
        }
    }

    public override bool HasColdWater
    {
        get => HasActiveOperation(TR3OpDefs.IsCold);
        set
        {
            if (value)
            {
                EnsureOperation(new TROperation(TR3OpDefs.IsCold, ushort.MaxValue, true));
            }
            else
            {
                SetOperationActive(TR3OpDefs.IsCold, value);
            }
        }
    }
}
