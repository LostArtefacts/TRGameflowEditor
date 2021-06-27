using System;
using System.Collections.Generic;
using TRGE.Core;

namespace TRGE.View.Model.Data
{
    public class LevelSequencingData : List<SequencedLevel>
    {
        public LevelSequencingData(List<Tuple<string, string>> levelData, List<MutableTuple<string, string, bool>> enabledStatus)
        {
            if (enabledStatus.Count != levelData.Count)
            {
                throw new ArgumentException("Level sequence data and enabled status data do not match.");
            }

            for (int i = 0; i < levelData.Count; i++)
            {
                Tuple<string, string> data = levelData[i];
                MutableTuple<string, string, bool> enabledData = enabledStatus.Find(m => m.Item1 == data.Item1);
                Add(new SequencedLevel(enabledData.Item3, data.Item1, data.Item2, i + 1));
            }
        }

        public List<Tuple<string, string>> ToSequenceTupleList()
        {
            List<Tuple<string, string>> result = new List<Tuple<string, string>>();
            foreach (SequencedLevel level in this)
            {
                result.Add(level.ToSequenceTuple());
            }
            return result;
        }

        public List<MutableTuple<string, string, bool>> ToEnabledTupleList()
        {
            List<MutableTuple<string, string, bool>> result = new List<MutableTuple<string, string, bool>>();
            foreach (SequencedLevel level in this)
            {
                result.Add(level.ToEnabledTuple());
            }
            return result;
        }
    }

    public class SequencedLevel : FlaggedLevel, IComparable<SequencedLevel>
    {
        public int DisplaySequence { get; set; }

        public SequencedLevel(bool levelEnabled, string levelID, string levelName, int sequence)
            : base(levelID, levelName, levelEnabled)
        {
            DisplaySequence = sequence;
        }

        public Tuple<string, string> ToSequenceTuple()
        {
            return new Tuple<string, string>(LevelID, LevelName);
        }

        public MutableTuple<string, string, bool> ToEnabledTuple()
        {
            return new MutableTuple<string, string, bool>(LevelID, LevelName, Flag);
        }

        public int CompareTo(SequencedLevel other)
        {
            return DisplaySequence.CompareTo(other.DisplaySequence);
        }
    }
}