namespace TRGE.Core;

internal class TRRLevelManager : AbstractTRLevelManager
{
    private readonly AbstractTRAudioProvider _audioProvider;
    private readonly TRRScript _script;
    private readonly TRRScriptedLevel _assaultLevel;

    internal override int LevelCount => _script.Levels.Count;
    public override AbstractTRAudioProvider AudioProvider => _audioProvider;
    internal override AbstractTRItemProvider ItemProvider => throw new NotSupportedException();

    internal override AbstractTRScriptedLevel AssaultLevel => _assaultLevel;
    internal override List<AbstractTRScriptedLevel> Levels
    {
        get => _script.Levels;
        set => _script.Levels = value;
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

    internal TRRLevelManager(TRRScript script)
        : base(script.Edition)
    {
        _script = script;
        _assaultLevel = _script.AssaultLevel as TRRScriptedLevel;
        _audioProvider = TRAudioFactory.GetAudioProvider(script.Edition);
    }

    protected override TRScriptedLevelModification OpDefToModification(TROpDef opDef)
    {
        throw new NotImplementedException();
    }

    internal override void Save() { }

    internal override void UpdateScript() { }

    internal override void RandomiseSequencing(List<AbstractTRScriptedLevel> originalLevels)
    {
        List<AbstractTRScriptedLevel> shuffledLevels = new(originalLevels);
        shuffledLevels.Randomise(SequencingRNG.Create());

        if (_script.Edition.Remastered && _script.Edition.Version == TRVersion.TR1)
        {
            int cisternIndex = shuffledLevels.FindIndex(l => l.Is("LEVEL7A.PHD"));
            (shuffledLevels[cisternIndex], shuffledLevels[7]) = (shuffledLevels[7], shuffledLevels[cisternIndex]);
        }

        List<AbstractTRScriptedLevel> newLevels = new();
        foreach (AbstractTRScriptedLevel shfLevel in shuffledLevels)
        {
            AbstractTRScriptedLevel level = GetLevel(shfLevel.ID) ?? throw new ArgumentException(string.Format("{0} does not represent a valid level", shfLevel.ID));
            newLevels.Add(level);
        }

        Levels = newLevels;
        SetLevelSequencing();
    }
}
