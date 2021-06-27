using System.Collections.Generic;
using TRGE.Core;

namespace TRGE.View.Model.Data
{
    public class FlaggedLevelData : List<FlaggedLevel>
    {
        public FlaggedLevelData(List<MutableTuple<string, string, bool>> unarmedLevels)
        {
            foreach (MutableTuple<string, string, bool> data in unarmedLevels)
            {
                Add(new FlaggedLevel(data.Item1, data.Item2, data.Item3));
            }
        }

        public List<MutableTuple<string, string, bool>> ToTupleList()
        {
            List<MutableTuple<string, string, bool>> result = new List<MutableTuple<string, string, bool>>();
            foreach (FlaggedLevel level in this)
            {
                result.Add(level.ToTuple());
            }
            return result;
        }
    }

    public class FlaggedLevel : BaseLevelData
    {
        public bool Flag { get; set; }

        public FlaggedLevel(string levelID, string levelName, bool flag)
            :base(levelID, levelName)
        {
            Flag = flag;
        }

        public MutableTuple<string, string, bool> ToTuple()
        {
            return new MutableTuple<string, string, bool>(LevelID, LevelName, Flag);
        }
    }
}