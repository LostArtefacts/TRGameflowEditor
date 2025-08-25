using TRGE.Core.Item.Enums;

namespace TRGE.Core;

internal class TRXLevelManager : AbstractTRLevelManager
{
    private readonly AbstractTRAudioProvider _audioProvider;
    private readonly TRXScript _script;
    private readonly TRXScriptedLevel _assaultLevel;
    private List<TRXScriptedLevel> _levels;

    internal override int LevelCount => _levels.Count;
    public override AbstractTRAudioProvider AudioProvider => _audioProvider;
    internal override AbstractTRItemProvider ItemProvider => throw new NotSupportedException();

    internal override AbstractTRScriptedLevel AssaultLevel => _assaultLevel;
    internal override List<AbstractTRScriptedLevel> Levels
    {
        get => [.. _levels.Cast<AbstractTRScriptedLevel>()];
        set => _levels = [.. value.Cast<TRXScriptedLevel>()];
    }

    public override void ImportLevels(IEnumerable<AbstractTRScriptedLevel> levels)
    {
        _levels.AddRange(levels.Cast<TRXScriptedLevel>());
    }

    public override void RemoveLevels(Func<AbstractTRScriptedLevel, bool> predicate)
    {
        _levels.RemoveAll(l => predicate(l));
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

    internal Organisation MedilessLevelOrganisation { get; set; }
    internal RandomGenerator MedilessLevelRNG { get; set; }
    internal uint RandomMedilessLevelCount { get; set; }

    internal TRXLevelManager(TRXScript script)
        : base(script.Edition)
    {
        Levels = (_script = script).Levels;
        _assaultLevel = _script.AssaultLevel as TRXScriptedLevel;
        _audioProvider = TRAudioFactory.GetAudioProvider(script.Edition);
    }

    internal override void Save()
    {
        _script.Levels = Levels;
    }

    internal override void UpdateScript()
    {
        _script.Levels = Levels.FindAll(l => l.Enabled);
    }

    protected override void FirstLevelChanged(AbstractTRScriptedLevel oldFinalLevel, AbstractTRScriptedLevel newFinalLevel)
    {
        if (_script.Edition.Version != TRVersion.TR2)
        {
            return;
        }

        var oldLevel = oldFinalLevel as TRXScriptedLevel;
        var newLevel = newFinalLevel as TRXScriptedLevel;

        int loopIndex = newLevel.Sequences.FindIndex(s => s.Type == LevelSequenceType.Loop_Game);
        if (loopIndex == -1)
        {
            return;
        }

        var giveSequences = oldLevel.Sequences.FindAll(s => s.Type == LevelSequenceType.Give_Item);
        foreach (var sequence in giveSequences)
        {
            newLevel.Sequences.Insert(loopIndex, sequence);
            oldLevel.Sequences.Remove(sequence);
        }
    }

    protected override void FinalLevelChanged(AbstractTRScriptedLevel oldFinalLevel, AbstractTRScriptedLevel newFinalLevel)
    {
        var oldLevel = oldFinalLevel as TRXScriptedLevel;
        var newLevel = newFinalLevel as TRXScriptedLevel;

        List<BaseLevelSequence> endSequences = [];
        int statsIndex = oldLevel.Sequences.FindIndex(s => s.Type == LevelSequenceType.Level_Stats);
        for (int i = oldLevel.Sequences.Count - 1; i > statsIndex; i--)
        {
            BaseLevelSequence sequence = oldLevel.Sequences[i];
            endSequences.Insert(0, sequence);
            oldLevel.Sequences.Remove(sequence);
        }

        if (endSequences.Any(s => s.Type == LevelSequenceType.Level_Complete))
        {
            newLevel.Sequences.RemoveAll(s => s.Type == LevelSequenceType.Level_Complete);
        }

        if (_script.Edition.Version == TRVersion.TR2)
        {
            if (oldLevel.Sequences.Last(s => s.Type == LevelSequenceType.Play_Music) is PlayMusicLevelSequence oldOutroSeq
                && newLevel.Sequences.Last(s => s.Type == LevelSequenceType.Play_Music) is PlayMusicLevelSequence newOutroSeq)
            {
                (newOutroSeq.MusicTrack, oldOutroSeq.MusicTrack) = (oldOutroSeq.MusicTrack, newOutroSeq.MusicTrack);
            }

            var loopSeq = newLevel.Sequences.FindIndex(l => l.Type == LevelSequenceType.Loop_Game);
            var fmvSeq = newLevel.Sequences.FindIndex(loopSeq, l => l.Type == LevelSequenceType.Play_FMV);
            if (fmvSeq > loopSeq)
            {
                var fmv = newLevel.Sequences[fmvSeq];
                newLevel.Sequences.Remove(fmv);
                newLevel.Sequences.Insert(loopSeq + 1, fmv);
            }
        }

        newLevel.Sequences.AddRange(endSequences);
    }

    protected override TRScriptedLevelModification OpDefToModification(TROpDef opDef)
    {
        return opDef == TR3OpDefs.Sunset ? TRScriptedLevelModification.SunsetChanged
            : throw new NotImplementedException();
    }

    internal List<TRXScriptedLevel> GetLevelsWithSequence(LevelSequenceType type)
    {
        return _levels.FindAll(l => l.HasSequence(type));
    }

    internal List<TRXScriptedLevel> GetAmmolessLevels()
    {
        return _levels.FindAll(l => l.RemovesAmmo);
    }

    internal uint GetAmmolessLevelCount()
    {
        return (uint)GetAmmolessLevels().Count;
    }

    public List<MutableTuple<string, string, bool>> GetAmmolessLevelData(List<AbstractTRScriptedLevel> originalLevels)
    {
        List<MutableTuple<string, string, bool>> data = new();
        foreach (AbstractTRScriptedLevel originalLevel in originalLevels)
        {
            TRXScriptedLevel level = GetLevel(originalLevel.ID) as TRXScriptedLevel;
            data.Add(new MutableTuple<string, string, bool>(level.ID, level.Name, level.RemovesAmmo));
        }
        return data;
    }

    internal virtual void SetAmmolessLevelData(List<MutableTuple<string, string, bool>> data)
    {
        foreach (MutableTuple<string, string, bool> item in data)
        {
            TRXScriptedLevel level = (TRXScriptedLevel)GetLevel(item.Item1);
            if (level != null && level.RemovesAmmo != item.Item3)
            {
                level.RemovesAmmo = item.Item3;
                FireLevelModificationEvent(level, TRScriptedLevelModification.AmmolessStateChanged);
            }
        }
    }

    internal void RandomiseAmmolessLevels(List<AbstractTRScriptedLevel> basisLevels)
    {
        List<AbstractTRScriptedLevel> enabledLevels = new();
        foreach (AbstractTRScriptedLevel originalLevel in basisLevels)
        {
            AbstractTRScriptedLevel lvl = GetLevel(originalLevel.ID);
            if (lvl.Enabled)
            {
                enabledLevels.Add(originalLevel);
            }
        }

        var levelSet = enabledLevels.RandomSelection(AmmolessLevelRNG.Create(), RandomAmmolessLevelCount);
        foreach (var level in Levels.Cast<TRXScriptedLevel>())
        {
            level.RemovesAmmo = levelSet.Contains(level);
            if (_script.Edition.Version == TRVersion.TR2)
            {
                level.RemovesFlares = levelSet.Contains(level);
                level.RemovesMedis = levelSet.Contains(level);
            }
            FireLevelModificationEvent(level, TRScriptedLevelModification.AmmolessStateChanged);
        }
    }

    internal void RestoreAmmolessLevels(List<AbstractTRScriptedLevel> originalLevels)
    {
        SetAmmolessLevelData(GetAmmolessLevelData(originalLevels));
    }

    internal List<TRXScriptedLevel> GetMedilessLevels()
    {
        return _levels.FindAll(l => l.RemovesMedis);
    }

    internal uint GetMedilessLevelCount()
    {
        return (uint)GetMedilessLevels().Count;
    }

    public List<MutableTuple<string, string, bool>> GetMedilessLevelData(List<AbstractTRScriptedLevel> originalLevels)
    {
        List<MutableTuple<string, string, bool>> data = new();
        foreach (AbstractTRScriptedLevel originalLevel in originalLevels)
        {
            TRXScriptedLevel level = GetLevel(originalLevel.ID) as TRXScriptedLevel;
            data.Add(new MutableTuple<string, string, bool>(level.ID, level.Name, level.RemovesMedis));
        }
        return data;
    }

    internal virtual void SetMedilessLevelData(List<MutableTuple<string, string, bool>> data)
    {
        foreach (MutableTuple<string, string, bool> item in data)
        {
            TRXScriptedLevel level = (TRXScriptedLevel)GetLevel(item.Item1);
            if (level != null && level.RemovesAmmo != item.Item3)
            {
                level.RemovesMedis = item.Item3;
                FireLevelModificationEvent(level, TRScriptedLevelModification.MedilessStateChanged);
            }
        }
    }

    internal void RandomiseMedilessLevels(List<AbstractTRScriptedLevel> basisLevels)
    {
        List<AbstractTRScriptedLevel> enabledLevels = new();
        foreach (AbstractTRScriptedLevel originalLevel in basisLevels)
        {
            AbstractTRScriptedLevel lvl = GetLevel(originalLevel.ID);
            if (lvl.Enabled)
            {
                enabledLevels.Add(originalLevel);
            }
        }

        List<AbstractTRScriptedLevel> levelSet = enabledLevels.RandomSelection(MedilessLevelRNG.Create(), RandomMedilessLevelCount);
        foreach (var level in Levels.Cast<TRXScriptedLevel>())
        {
            level.RemovesMedis = levelSet.Contains(level);
            FireLevelModificationEvent(level, TRScriptedLevelModification.MedilessStateChanged);
        }
    }

    internal void RandomiseUnarmedLevels(List<AbstractTRScriptedLevel> basisLevels)
    {
        List<AbstractTRScriptedLevel> enabledLevels = new();
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

        SetUnarmedDependencies();
    }

    internal List<TRXScriptedLevel> GetUnarmedLevels()
    {
        return _levels.FindAll(l => l.RemovesWeapons);
    }

    internal uint GetUnarmedLevelCount()
    {
        return (uint)GetUnarmedLevels().Count;
    }

    public List<MutableTuple<string, string, bool>> GetUnarmedLevelData(List<AbstractTRScriptedLevel> originalLevels)
    {
        List<MutableTuple<string, string, bool>> data = new();
        foreach (AbstractTRScriptedLevel originalLevel in originalLevels)
        {
            TRXScriptedLevel level = GetLevel(originalLevel.ID) as TRXScriptedLevel;
            data.Add(new MutableTuple<string, string, bool>(level.ID, level.Name, level.RemovesWeapons));
        }
        return data;
    }

    internal virtual void SetUnarmedLevelData(List<MutableTuple<string, string, bool>> data)
    {
        foreach (MutableTuple<string, string, bool> item in data)
        {
            TRXScriptedLevel level = (TRXScriptedLevel)GetLevel(item.Item1);
            if (level != null)
            {
                level.RemovesWeapons = item.Item3;
                FireLevelModificationEvent(level, TRScriptedLevelModification.WeaponlessStateChanged);
            }
        }

        SetUnarmedDependencies();
    }

    internal void RestoreUnarmedLevels(List<AbstractTRScriptedLevel> originalLevels)
    {
        SetUnarmedLevelData(GetUnarmedLevelData(originalLevels));
    }

    private void SetUnarmedDependencies()
    {
        // Add pistols to each level that follows one that's unarmed and remove
        // them if they're present and following a level that isn't uanarmed.
        foreach (TRXScriptedLevel level in Levels.Cast<TRXScriptedLevel>())
        {
            if (!level.Enabled || level.IsFinalLevel)
            {
                continue;
            }

            var pistols = (Enum)(_script.Edition.Version == TRVersion.TR1 ? TR1Items.Pistols_S_P : TR2Items.Pistols_S_P);
            TRXScriptedLevel nextLevel = Levels.Find(l => l.Sequence == level.Sequence + 1 && l.Enabled) as TRXScriptedLevel;
            if (level.RemovesWeapons && !nextLevel.RemovesWeapons)
            {
                if (nextLevel.GetStartInventoryItem(pistols) == null)
                {
                    nextLevel.AddStartInventoryItem(pistols);
                }
            }
            else
            {
                nextLevel.RemoveStartInventoryItem(pistols);
            }
        }
    }

    internal bool GetLevelsHaveCutScenes()
    {
        foreach (TRXScriptedLevel level in _levels)
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
        foreach (TRXScriptedLevel level in _levels)
        {
            level.HasCutScene = haveCutScenes;
        }
    }

    internal bool GetLevelsSupportCutScenes()
    {
        foreach (TRXScriptedLevel level in _levels)
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
        foreach (TRXScriptedLevel level in _levels)
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
        foreach (TRXScriptedLevel level in _levels)
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
        foreach (TRXScriptedLevel level in _levels)
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
            TRXScriptedLevel origLevel = originalLevel as TRXScriptedLevel;
            TRXScriptedLevel level = GetLevel(originalLevel.ID) as TRXScriptedLevel;
            level.Demo = value && origLevel.Demo.HasValue && origLevel.Demo.Value;
        }
    }

    internal override void RandomiseGameTracksImpl(Random rand)
    {
        IReadOnlyDictionary<TRAudioCategory, List<TRAudioTrack>> tracks = AudioProvider.GetCategorisedTracks();
        HashSet<TRAudioTrack> exclusions = new()
        {
            AudioProvider.GetBlankTrack()
        };
        if (tracks[TRAudioCategory.Credits].Count > 0)
        {
            foreach (TRXScriptedLevel level in Levels.Cast<TRXScriptedLevel>())
            {
                if (level.IsFinalLevel && level.Sequences.Find(s => s is PlayMusicLevelSequence) is PlayMusicLevelSequence sequence)
                {
                    sequence.MusicTrack = tracks[TRAudioCategory.Credits].RandomSelection(rand, 1, exclusions: exclusions)[0].ID;
                    break;
                }
            }
        }
    }

    protected override ushort ConvertTrackID(ushort trackID)
    {
        if (_script.Edition.Version != TRVersion.TR2)
        {
            return trackID;
        }

        short[] skippedIDs = { 2, 19, 20, 26, -1 };
        short idx = 0;
        ushort result = 2;

        for (int i = 2; i < trackID; i++)
        {
            if ((skippedIDs[idx] >= 0) && (i == skippedIDs[idx]))
            {
                idx++;
            }
            else
            {
                result++;
            }
        }
        return result;
    }
}
