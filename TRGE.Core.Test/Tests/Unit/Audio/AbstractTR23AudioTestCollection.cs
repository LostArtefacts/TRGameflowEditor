﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using TRGE.Coord;

namespace TRGE.Core.Test;

public abstract class AbstractTR23AudioTestCollection : BaseTestCollection
{
    protected abstract int ScriptFileIndex { get; }
    protected abstract ushort SampleTrack { get; }
    internal abstract Dictionary<string, ushort> ExpectedLevelTracks { get; }
    internal abstract Dictionary<string, ushort> NewLevelTracks { get; }

    protected override void Setup()
    {
        base.Setup();
        //File.Copy(@"audio\tr2audio.wad", Path.Combine(TRCoord.Instance.ConfigDirectory, "tr2audio.wad"));
        //File.Copy(@"audio\tr3audio.wad", Path.Combine(TRCoord.Instance.ConfigDirectory, "tr3audio.wad"));
    }

    [TestMethod]
    [TestSequence(0)]
    protected void TestLoadTracks()
    {
        TR23ScriptEditor sm = TRCoord.Instance.Open(_validScripts[ScriptFileIndex]).ScriptEditor as TR23ScriptEditor;
        List<MutableTuple<string, string, ushort>> trackData = sm.GameTrackData;
        try
        {
            CompareTrackData(trackData, ExpectedLevelTracks);
        }
        catch
        {
            throw;
        }
    }

    //[TestMethod]
    [TestSequence(1)]
    protected void TestPlayTrack()
    {
        TR23ScriptEditor sm = TRCoord.Instance.Open(_validScripts[ScriptFileIndex]).ScriptEditor as TR23ScriptEditor;
        using MemoryStream ms = new(sm.GetTrackData(SampleTrack));
        //new SoundPlayer(ms).PlaySync();
    }

    [TestMethod]
    [TestSequence(2)]
    protected void TestSetTracks()
    {
        TR23ScriptEditor sm = TRCoord.Instance.Open(_validScripts[ScriptFileIndex]).ScriptEditor as TR23ScriptEditor;
        List<MutableTuple<string, string, ushort>> trackData = sm.GameTrackData;
        foreach (MutableTuple<string, string, ushort> levelData in trackData)
        {
            if (NewLevelTracks.ContainsKey(levelData.Item1))
            {
                levelData.Item3 = NewLevelTracks[levelData.Item1];
            }
            else
            {
                NewLevelTracks.Add(levelData.Item1, levelData.Item3);
            }
        }

        sm.GameTrackData = trackData;
        CompareTrackData(sm.GameTrackData, NewLevelTracks);
    }

    private static void CompareTrackData(List<MutableTuple<string, string, ushort>> trackData, Dictionary<string, ushort> expectedResults)
    {
        Dictionary<string, ushort> trackMap = new();
        foreach (MutableTuple<string, string, ushort> levelData in trackData)
        {
            trackMap.Add(levelData.Item1, levelData.Item3);
        }

        CollectionAssert.AreEquivalent(expectedResults, trackMap);
    }

    [TestMethod]
    [TestSequence(3)]
    protected void TestRandomiseTracks()
    {
        TREditor editor = TRCoord.Instance.Open(_validScripts[ScriptFileIndex]);
        TR23ScriptEditor sm = editor.ScriptEditor as TR23ScriptEditor;
        List<MutableTuple<string, string, ushort>> trackData = sm.GameTrackData;

        sm.GameTrackOrganisation = Organisation.Random;
        sm.GameTrackRNG = new RandomGenerator(RandomGenerator.Type.Date);

        sm.RandomiseGameTracks();

        CollectionAssert.AreNotEqual(sm.GameTrackData, trackData);
    }

    [TestMethod]
    [TestSequence(4)]
    protected void TestRandomiseTracksExcludingBlank()
    {
        TREditor editor = TRCoord.Instance.Open(_validScripts[ScriptFileIndex]);
        TR23ScriptEditor sm = editor.ScriptEditor as TR23ScriptEditor;
        List<MutableTuple<string, string, ushort>> trackData = sm.GameTrackData;

        sm.GameTrackOrganisation = Organisation.Random;
        sm.GameTrackRNG = new RandomGenerator(RandomGenerator.Type.Date);
        sm.RandomGameTracksIncludeBlank = false;

        sm.RandomiseGameTracks();

        CollectionAssert.AreNotEqual(sm.GameTrackData, trackData);

        ushort blankID = sm.LevelManager.AudioProvider.GetBlankTrack().ID;
        foreach (AbstractTRScriptedLevel level in sm.LevelManager.Levels)
        {
            Assert.AreNotEqual(level.TrackID, blankID);
        }
    }

    [TestMethod]
    [TestSequence(5)]
    protected void TestRandomiseTracksReload()
    {
        TREditor editor = TRCoord.Instance.Open(_validScripts[ScriptFileIndex]);
        TR23ScriptEditor sm = editor.ScriptEditor as TR23ScriptEditor;

        sm.GameTrackOrganisation = Organisation.Random;
        sm.GameTrackRNG = new RandomGenerator(RandomGenerator.Type.Date);
        editor.Save();

        List<MutableTuple<string, string, ushort>> randoTrackData = sm.GameTrackData;

        List<MutableTuple<string, string, ushort>> trackData = sm.GameTrackData;
        foreach (MutableTuple<string, string, ushort> levelData in trackData)
        {
            if (NewLevelTracks.ContainsKey(levelData.Item1))
            {
                levelData.Item3 = NewLevelTracks[levelData.Item1];
            }
        }

        sm.GameTrackOrganisation = Organisation.Manual;
        sm.GameTrackData = trackData;
        editor.Save();

        sm.LevelSequencingOrganisation = Organisation.Random;
        sm.LevelSequencingRNG = new RandomGenerator(RandomGenerator.Type.Date);
        editor.Save();

        sm.GameTrackOrganisation = Organisation.Random;
        sm.GameTrackRNG = new RandomGenerator(RandomGenerator.Type.Date);
        sm.Save();// new TRSaveMonitor(new TRSaveEventArgs()));

        CollectionAssert.AreNotEquivalent(sm.GameTrackData, randoTrackData);
    }
}