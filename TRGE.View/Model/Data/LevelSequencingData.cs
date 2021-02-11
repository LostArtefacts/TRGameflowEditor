using System;
using System.Collections.Generic;

namespace TRGE.View.Model.Data
{
    public class LevelSequencingData : List<SequencedLevel>
    {
        public LevelSequencingData(List<Tuple<string, string>> levelData)
        {
            for (int i = 0; i < levelData.Count; i++)
            {
                Tuple<string, string> data = levelData[i];
                Add(new SequencedLevel(data.Item1, data.Item2, i + 1));
            }
        }

        public List<Tuple<string, string>> ToTupleList()
        {
            List<Tuple<string, string>> result = new List<Tuple<string, string>>();
            foreach (SequencedLevel level in this)
            {
                result.Add(level.ToTuple());
            }
            return result;
        }
    }

    public class SequencedLevel : BaseLevelData, IComparable<SequencedLevel>
    {
        public int DisplaySequence { get; set; }

        public SequencedLevel(string levelID, string levelName, int sequence)
            : base(levelID, levelName)
        {
            DisplaySequence = sequence;
        }

        public Tuple<string, string> ToTuple()
        {
            return new Tuple<string, string>(LevelID, LevelName);
        }

        public int CompareTo(SequencedLevel other)
        {
            return DisplaySequence.CompareTo(other.DisplaySequence);
        }
    }
}