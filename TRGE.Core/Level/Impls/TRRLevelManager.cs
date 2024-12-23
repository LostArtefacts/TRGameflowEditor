﻿using Newtonsoft.Json;

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
        string path = $"Resources/{_script.Edition.Version}/Restrictions/trr_sequencing.json";
        if (_script.Edition.Version == TRVersion.TR3 || !File.Exists(path))
        {
            return;
        }

        TRRSequencing sequencing = JsonConvert.DeserializeObject<TRRSequencing>(File.ReadAllText(path));
        List<AbstractTRScriptedLevel> shuffledLevels = new(originalLevels);
        Random generator = SequencingRNG.Create();
        do
        {
            shuffledLevels.Randomise(generator);
            foreach (int sequence in sequencing.Fixed)
            {
                int index = shuffledLevels.FindIndex(l => l.OriginalSequence == sequence);
                (shuffledLevels[index], shuffledLevels[sequence - 1]) = (shuffledLevels[sequence - 1], shuffledLevels[index]);
            }
        }
        while (shuffledLevels.Any(l => sequencing.Invalid.ContainsKey(l.OriginalSequence)
            && sequencing.Invalid[l.OriginalSequence].Contains(shuffledLevels.IndexOf(l) + 1)));

        List<AbstractTRScriptedLevel> newLevels = new();
        foreach (AbstractTRScriptedLevel shfLevel in shuffledLevels)
        {
            AbstractTRScriptedLevel level = GetLevel(shfLevel.ID)
                ?? throw new ArgumentException(string.Format("{0} does not represent a valid level", shfLevel.ID));
            newLevels.Add(level);
        }

        Levels = newLevels;
        SetLevelSequencing();
    }

    internal override void RandomiseGameTracks(TRScriptIOArgs io, List<AbstractTRScriptedLevel> originalLevels)
    {
        IReadOnlyDictionary<TRAudioCategory, List<TRAudioTrack>> tracks = AudioProvider.GetCategorisedTracks();
        Random rand = GameTrackRNG.Create();

        HashSet<TRAudioTrack> exclusions = new()
        {
            AudioProvider.GetBlankTrack()
        };

        if (tracks[TRAudioCategory.Title].Count > 0)
        {
            List<TRAudioTrack> titleTracks = new(tracks[TRAudioCategory.Title]);
            if (Edition.Version == TRVersion.TR3)
            {
                titleTracks.Add(new()
                {
                    ID = 123,
                });
            }
            ushort currentTrack = (_script.FrontEnd as TRRFrontEnd).TrackID;
            ushort titleSound;
            do
            {
                titleSound = titleTracks.RandomSelection(rand, 1, exclusions: exclusions)[0].ID;
            }
            while (titleSound == currentTrack);

            currentTrack -= _script.TrackOffset;
            titleSound -= _script.TrackOffset;

            string originalFile = Path.Combine(io.WIPOutputDirectory.FullName, currentTrack + ".OGG");
            string newFile = Path.Combine(io.WIPOutputDirectory.FullName, titleSound + ".OGG");
            if (File.Exists(originalFile) && File.Exists(newFile))
            {
                File.Copy(newFile, originalFile, true);
            }
        }

        List<ushort> ambientTracks = originalLevels.Select(l => l.TrackID).Distinct().ToList();
        ambientTracks.RemoveAll(t => t == 0 || t == ushort.MaxValue || !File.Exists(Path.Combine(io.BackupDirectory.FullName, (t - _script.TrackOffset) + ".OGG")));
        if (ambientTracks.Count == 0)
        {
            return;
        }

        List<ushort> mixedTracks = new(ambientTracks);
        do
        {
            mixedTracks.Shuffle(rand);
        }
        while (mixedTracks.Any(t => ambientTracks.IndexOf(t) == mixedTracks.IndexOf(t)));

        for (int i = 0; i < mixedTracks.Count; i++)
        {
            int trackID = ambientTracks[i] - _script.TrackOffset;
            int nextID = mixedTracks[i] - _script.TrackOffset;

            string backupFile = Path.Combine(io.BackupDirectory.FullName, nextID + ".OGG");
            string targetFile = Path.Combine(io.WIPOutputDirectory.FullName, trackID + ".OGG");
            if (File.Exists(backupFile))
            {
                File.WriteAllBytes(targetFile, File.ReadAllBytes(backupFile));
            }
        }
    }

    private class TRRSequencing
    {
        public List<int> Fixed { get; set; }
        public Dictionary<int, List<int>> Invalid { get; set; }
    }
}
