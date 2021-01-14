using System;
using System.Collections.Generic;

namespace TRGE.Core
{
    internal abstract class AbstractTRLevelManager
    {
        internal virtual AbstractTRLevel GetLevel(string id)
        {
            foreach (AbstractTRLevel level in Levels)
            {
                if (level.ID.Equals(id))
                {
                    return level;
                }
            }
            return null;
        }

        internal abstract int LevelCount { get; }
        internal abstract List<AbstractTRLevel> Levels { get; set; }
        internal abstract void Save();

        internal virtual List<Tuple<string, string>> GetLevelSequencing()
        {
            List<Tuple<string, string>> data = new List<Tuple<string, string>>();
            foreach (AbstractTRLevel level in Levels)
            {
                data.Add(new Tuple<string, string>(level.ID, level.Name));
            }
            return data;
        }

        internal virtual void SetLevelSequencing(List<Tuple<string, string>> data)
        {
            List<AbstractTRLevel> newLevels = new List<AbstractTRLevel>();
            foreach (Tuple<string, string> item in data)
            {
                AbstractTRLevel level = GetLevel(item.Item1);
                if (level == null)
                {
                    throw new ArgumentException(string.Format("{0} does not represent a valid level", item.Item1));
                }
                newLevels.Add(level);
            }

            Levels = newLevels;
        }

        internal virtual void RandomiseLevels(RandomGenerator rng, List<AbstractTRLevel> originalLevels)
        {
            List<AbstractTRLevel> shuffledLevels = new List<AbstractTRLevel>(originalLevels);
            shuffledLevels.Randomise(rng.Create());

            List<AbstractTRLevel> newLevels = new List<AbstractTRLevel>();
            ushort newSeq = 1;
            foreach (AbstractTRLevel shfLevel in shuffledLevels)
            {
                AbstractTRLevel level = GetLevel(shfLevel.ID);
                if (level == null)
                {
                    throw new ArgumentException(string.Format("{0} does not represent a valid level", shfLevel.ID));
                }

                level.Sequence = newSeq++;
                level.IsFinalLevel = newSeq == LevelCount + 1;
                newLevels.Add(level);
            }

            Levels = newLevels;
        }
    }
}