using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TRGE.Core;

namespace TRGE.View.Model.Data;

public class AudioData : List<AudioLevelData>
{
    private List<AudioTrack> _allTracks;
    public IReadOnlyList<AudioTrack> AllTracks => _allTracks;

    public AudioData(List<MutableTuple<string, string, ushort>> levelTrackData, IReadOnlyList<Tuple<ushort, string>> allTrackdata)
    {
        BuildAlLTrackData(allTrackdata);
        BuildLevelTrackData(levelTrackData);
    }

    private void BuildAlLTrackData(IReadOnlyList<Tuple<ushort, string>> allTrackdata)
    {
        _allTracks = new List<AudioTrack>(allTrackdata.Count);
        foreach (Tuple<ushort, string> data in allTrackdata)
        {
            _allTracks.Add(new AudioTrack(data.Item1, data.Item2));
        }
        _allTracks.Sort();
    }

    private void BuildLevelTrackData(List<MutableTuple<string, string, ushort>> levelTrackData)
    {
        foreach (MutableTuple<string, string, ushort> data in levelTrackData)
        {
            Add(new AudioLevelData(data.Item1, data.Item2, GetTrack(data.Item3)));
        }
    }

    public AudioTrack GetTrack(ushort id)
    {
        foreach (AudioTrack track in _allTracks)
        {
            if (track.ID == id)
            {
                return track;
            }
        }
        return null;
    }

    public List<MutableTuple<string, string, ushort>> ToTupleList()
    {
        List<MutableTuple<string, string, ushort>> result = new();
        foreach (AudioLevelData levelData in this)
        {
            result.Add(levelData.ToTuple());
        }
        return result;
    }
}

public class AudioLevelData : BaseLevelData
{
    public AudioTrack Track { get; set; }

    public AudioLevelData(string levelID, string levelName, AudioTrack track)
        :base(levelID, levelName)
    {
        Track = track;
    }

    public MutableTuple<string, string, ushort> ToTuple()
    {
        return new MutableTuple<string, string, ushort>(LevelID, LevelName, Track.ID);
    }
}

public class AudioTrack : IComparable<AudioTrack>
{
    private static readonly Regex _compRegex = new(@"(\d+)$", RegexOptions.Compiled | RegexOptions.CultureInvariant);

    public ushort ID { get; private set; }
    public string Name { get; private set; }

    public AudioTrack(ushort id, string name)
    {
        ID = id;
        Name = name;
    }

    public override bool Equals(object obj)
    {
        return obj is AudioTrack track && ID == track.ID;
    }

    public override int GetHashCode()
    {
        return 1213502048 + ID.GetHashCode();
    }

    public int CompareTo(AudioTrack other)
    {
        string name1 = Name.ToLower();
        string name2 = other.Name.ToLower();
                    
        Match m1 = _compRegex.Match(name1);
        Match m2 = _compRegex.Match(name2);
        if (m1.Success && m2.Success)
        {
            string name3 = name1.Substring(0, name1.LastIndexOf(m1.Value)).Trim();
            string name4 = name2.Substring(0, name2.LastIndexOf(m2.Value)).Trim();
            if (name3.Equals(name4))
            {
                return int.Parse(m1.Value).CompareTo(int.Parse(m2.Value));
            }
        }

        return name1.CompareTo(name2);
    }
}