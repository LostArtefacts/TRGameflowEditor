using TRGE.Core.Item.Enums;

namespace TRGE.Core;

public class TR3ScriptedLevel : AbstractTRScriptedLevel
{
    // The number of secrets is hardcoded in TR3 based on the level sequence unfortunately.
    // The sequencing here starts from Lara's Home.
    private static readonly ushort[] _levelSecrets = new ushort[21]
    {
        0, 6, 4, 5, 0, 3, 3, 3, 1, 5, 5, 6, 1, 3, 2, 3, 3, 3, 3, 0, 0
    };

    private static readonly List<ushort> _ogRainLevels = new() { 1, 3, 9, 12, 20 };
    private static readonly List<ushort> _ogSnowLevels = new() { 16, 19 };
    private static readonly List<ushort> _ogColdLevels = new() { 16, 17 };

    public override bool HasSecrets
    {
        get => _levelSecrets[Sequence] > 0;
        set { }
    }

    public override ushort NumSecrets
    {
        get => _levelSecrets[Sequence];
        set { }
    }

    public override ushort Sequence
    {
        get => GetOperation(TR3OpDefs.Level).Operand;
        set => GetOperation(TR3OpDefs.Level).Operand = value;
    }

    public override ushort TrackID
    {
        get => GetOperation(TR3OpDefs.Track).Operand;
        set => GetOperation(TR3OpDefs.Track).Operand = value;
    }

    public override bool HasFMV
    {
        get => HasActiveOperation(TR3OpDefs.FMV);
        set
        {
            SetOperationActive(TR3OpDefs.FMV, value);
            SetOperationActive(TR3OpDefs.ListStart, value);
            SetOperationActive(TR3OpDefs.ListEnd, value);
        }
    }

    public override bool SupportsFMVs => HasOperation(TR3OpDefs.FMV);

    public override bool HasStartAnimation
    {
        get => HasActiveOperation(TR3OpDefs.StartAnimation);
        set => SetOperationActive(TR3OpDefs.StartAnimation, value);
    }

    public override bool SupportsStartAnimations => HasOperation(TR3OpDefs.StartAnimation);

    public override short StartAnimationID
    {
        get
        {
            if (HasStartAnimation)
            {
                return Convert.ToInt16(GetOperation(TR3OpDefs.StartAnimation).Operand);
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
                GetOperation(TR3OpDefs.StartAnimation).Operand = Convert.ToUInt16(value);
            }
            else
            {
                InsertOperation(TR3OpDefs.StartAnimation, Convert.ToUInt16(value), TR3OpDefs.StartAnimation.Next);
            }
        }
    }

    public override bool HasCutScene
    {
        get => HasActiveOperation(TR3OpDefs.Cinematic);
        set
        {
            SetOperationActive(TR3OpDefs.Cinematic, value);
            SetOperationActive(TR3OpDefs.CutAngle, value);
        }
    }

    public override bool SupportsCutScenes => HasOperation(TR3OpDefs.Cinematic);

    private TR3ScriptedLevel _cutSceneLevel;
    public override AbstractTRScriptedLevel CutSceneLevel
    {
        get => _cutSceneLevel;
        set => _cutSceneLevel = value as TR3ScriptedLevel;
    }

    public override bool HasSunset
    {
        get => HasActiveOperation(TR3OpDefs.Sunset);
        set
        {
            if (value)
            {
                EnsureOperation(new TROperation(TR3OpDefs.Sunset, ushort.MaxValue, true));
            }
            else
            {
                SetOperationActive(TR3OpDefs.Sunset, value);
            }
        }
    }

    public override bool HasDeadlyWater
    {
        get => HasActiveOperation(TR3OpDefs.DeadlyWater);
        set => SetOperationActive(TR3OpDefs.DeadlyWater, value);
    }

    public override bool RemovesWeapons
    {
        get => HasActiveOperation(TR3OpDefs.RemoveWeapons);
        set
        {
            if (value)
            {
                EnsureOperation(new TROperation(TR3OpDefs.RemoveWeapons, ushort.MaxValue, true));
            }
            else
            {
                SetOperationActive(TR3OpDefs.RemoveWeapons, value);
            }
        }
    }

    public override bool RemovesAmmo
    {
        get => HasActiveOperation(TR3OpDefs.RemoveAmmo);
        set
        {
            if (value)
            {
                EnsureOperation(new TROperation(TR3OpDefs.RemoveAmmo, ushort.MaxValue, true));
            }
            else
            {
                SetOperationActive(TR3OpDefs.RemoveAmmo, value);
            }
        }
    }       

    /// <summary>
    /// This is set in HSH, although the game is hard-coded to complete after killing
    /// all enemies so this flag actually has no purpose. Remains untested in other
    /// levels.
    /// </summary>
    public override bool KillToComplete
    {
        get => HasOperation(TR3OpDefs.KillToComplete);
        set
        {
            if (value)
            {
                EnsureOperation(new TROperation(TR3OpDefs.KillToComplete, ushort.MaxValue, true));
            }
            else
            {
                SetOperationActive(TR3OpDefs.KillToComplete, value);
            }
        }
    }

    public override bool IsFinalLevel
    {
        get => HasActiveOperation(TR3OpDefs.GameComplete);
        set
        {
            if (value)
            {
                TROperation gcOp = GetOperation(TR3OpDefs.GameComplete);
                if (gcOp == null)
                {
                    gcOp = GetOperation(TR3OpDefs.Complete);
                    gcOp ??= AddOperation(TR3OpDefs.GameComplete);
                }
                gcOp.Definition = TR3OpDefs.GameComplete;
            }
            else
            {
                TROperation op = GetOperation(TR3OpDefs.GameComplete);
                if (op != null)
                {
                    op.Definition = TR3OpDefs.Complete;
                }
            }
        }
    }

    protected override TROpDef GetOpDefFor(ushort scriptData)
    {
        return TR3OpDefs.Get(scriptData);
    }

    private void RemoveBonuses()
    {
        for (int i = _operations.Count - 1; i >= 0; i--)
        {
            if (_operations[i].Definition == TR3OpDefs.StartInvBonus && _operations[i].Operand < 1000)
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
        _operations.Insert(GetLastOperationIndex(TR3OpDefs.Level), new TROperation(TR3OpDefs.StartInvBonus, itemID, true));
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
            if (opcmd.Definition == TR3OpDefs.StartInvBonus)
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
            if (opcmd.Definition == TR3OpDefs.StartInvBonus)
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
            if (opcmd.Definition == TR3OpDefs.StartInvBonus)
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
            if (op.Definition == TR3OpDefs.StartInvBonus && op.Operand > 999)
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
            if (opcmd.Definition == TR3OpDefs.StartInvBonus)
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

    public void SetStartInventoryItems(Dictionary<ushort, int> items)
    {
        ClearStartInventoryItems();

        foreach (ushort item in items.Keys)
        {
            AddStartInventoryItem(item, (uint)items[item]);
        }
    }

    public void AddStartInventoryItem(ushort item, uint count = 1)
    {
        ushort itemID = (ushort)(1000 + item);
        for (int i = 0; i < count; i++)
        {
            _operations.Insert(GetLastOperationIndex(TR3OpDefs.Level), new TROperation(TR3OpDefs.StartInvBonus, itemID, true));
        }
    }

    public void RemoveStartInventoryItem(ushort item, bool removeAll = false)
    {
        ushort itemID = (ushort)(1000 + item);
        for (int i = _operations.Count - 1; i >= 0; i--)
        {
            TROperation op = _operations[i];
            if (op.Definition == TR3OpDefs.StartInvBonus && op.Operand == itemID)
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
            if (op.Definition == TR3OpDefs.StartInvBonus && op.Operand > 999)
            {
                _operations.RemoveAt(i);
            }
        }
    }

    public void AddStartInventoryItem(TR3Items item, uint count = 1)
    {
        AddStartInventoryItem((ushort)item, count);
    }

    public void RemoveStartInventoryItem(TR3Items item, bool removeAll = false)
    {
        RemoveStartInventoryItem((ushort)item, removeAll);
    }

    public void SetStartInventoryItems(Dictionary<TR3Items, int> items)
    {
        SetStartInventoryItems(items.ToDictionary(item => (ushort)item.Key, item => item.Value));
    }

    public virtual bool HasRain
    {
        get => _ogRainLevels.Contains(Sequence);
        set { }
    }

    public virtual bool HasSnow
    {
        get => _ogSnowLevels.Contains(Sequence);
        set { }
    }

    public virtual bool HasColdWater
    {
        get => _ogColdLevels.Contains(Sequence);
        set { }
    }

    public override void SerializeToMain(BinaryWriter writer)
    {
        base.SerializeToMain(writer);

        writer.Write((byte)(HasRain ? 1 : 0));
        writer.Write((byte)(HasSnow ? 1 : 0));
        writer.Write((byte)(HasColdWater ? 1 : 0));
    }
}