using System;
using System.Collections.Generic;

namespace TRGE.Core
{
    internal abstract class AbstractTRLevelManager
    {
        internal abstract AbstractTRAudioProvider AudioProvider { get; }
        protected abstract ushort TitleSoundID { get; set; }
        protected abstract ushort SecretSoundID { get; set; }
        internal abstract AbstractTRItemProvider ItemProvider { get; }
        internal abstract List<AbstractTRLevel> Levels { get; set; }
        internal abstract int LevelCount { get; }

        internal Organisation LevelOrganisation { get; set; }
        internal RandomGenerator LevelRNG { get; set; }
        internal TREdition Edition { get; private set; }

        internal AbstractTRLevelManager(TREdition edition)
        {
            Edition = edition;
        }

        internal abstract void Save();

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

        internal virtual List<Tuple<string, string>> GetLevelSequencing()
        {
            return GetLevelSequencing(Levels);
        }

        private List<Tuple<string, string>> GetLevelSequencing(List<AbstractTRLevel> levels)
        {
            List<Tuple<string, string>> data = new List<Tuple<string, string>>();
            foreach (AbstractTRLevel level in levels)
            {
                data.Add(new Tuple<string, string>(level.ID, level.Name));
            }
            return data;
        }

        internal virtual void SetLevelSequencing(List<Tuple<string, string>> data)
        {
            List<AbstractTRLevel> newLevels = new List<AbstractTRLevel>();
            //ushort newSeq = 1;
            foreach (Tuple<string, string> item in data)
            {
                AbstractTRLevel level = GetLevel(item.Item1);
                if (level == null)
                {
                    throw new ArgumentException(string.Format("{0} does not represent a valid level", item.Item1));
                }
                //level.Sequence = newSeq++;
                //level.IsFinalLevel = newSeq == LevelCount + 1;
                newLevels.Add(level);
            }

            Levels = newLevels;
            SetLevelSequencing();
        }

        internal virtual void RandomiseLevelSequencing(List<AbstractTRLevel> originalLevels)
        {
            List<AbstractTRLevel> shuffledLevels = new List<AbstractTRLevel>(originalLevels);
            shuffledLevels.Randomise(LevelRNG.Create());

            List<AbstractTRLevel> newLevels = new List<AbstractTRLevel>();
            //ushort newSeq = 1;
            foreach (AbstractTRLevel shfLevel in shuffledLevels)
            {
                AbstractTRLevel level = GetLevel(shfLevel.ID);
                if (level == null)
                {
                    throw new ArgumentException(string.Format("{0} does not represent a valid level", shfLevel.ID));
                }

                //level.Sequence = newSeq++;
                //level.IsFinalLevel = newSeq == (LevelCount - Edition.LevelCompleteOffset) + 1;
                newLevels.Add(level);
            }

            Levels = newLevels;
            SetLevelSequencing();
        }

        internal void SetLevelSequencing()
        {
            ushort newSeq = 1;
            foreach (AbstractTRLevel level in Levels)
            {
                level.Sequence = newSeq++;
                level.IsFinalLevel = newSeq == (LevelCount - Edition.LevelCompleteOffset) + 1;
            }
        }

        internal void RestoreLevelSequencing(List<AbstractTRLevel> originalLevels)
        {
            SetLevelSequencing(GetLevelSequencing(originalLevels));
        }

        internal virtual void RandomiseLevelsWithOperation(RandomGenerator rng, uint levelCount, List<AbstractTRLevel> originalLevels, TROperation operation)
        {
            List<AbstractTRLevel> levelSet = originalLevels.RandomSelection(rng.Create(), levelCount);
            foreach (AbstractTRLevel level in Levels)
            {
                if (levelSet.Contains(level))
                {
                    level.EnsureOperation(operation);
                }
                else
                {
                    level.RemoveOperation(operation.Definition);
                }
            }
        }

        internal List<AbstractTRLevel> GetLevelsWithOperation(TROpDef opDef, bool activeOnly)
        {
            List<AbstractTRLevel> levels = new List<AbstractTRLevel>();
            foreach (AbstractTRLevel level in Levels)
            {
                if (activeOnly)
                {
                    if (level.HasActiveOperation(opDef))
                    {
                        levels.Add(level);
                    }
                }
                else if (level.HasOperation(opDef))
                {
                    levels.Add(level);
                }
            }
            return levels;
        }

        internal List<MutableTuple<string, string, ushort>> GetLevelTrackData()
        {
            List<MutableTuple<string, string, ushort>> ret = new List<MutableTuple<string, string, ushort>>();

            TRAudioTrack track = AudioProvider.GetTrack(TitleSoundID);
            if (track != null)
            {
                ret.Add(new MutableTuple<string, string, ushort>("TITLE", "Title Screen", TitleSoundID));
            }
            track = AudioProvider.GetTrack(SecretSoundID);
            if (track != null)
            {
                ret.Add(new MutableTuple<string, string, ushort>("SECRET", "Secret Found", SecretSoundID));
            }

            foreach (AbstractTRLevel level in Levels)
            {
                track = AudioProvider.GetTrack(level.TrackID);
                if (track == null)
                {
                    track = AudioProvider.GetBlankTrack();
                }
                ret.Add(new MutableTuple<string, string, ushort>(level.ID, level.Name, track.ID));
            }
            return ret;
        }

        internal void SetLevelTrackData(List<MutableTuple<string, string, ushort>> data)
        {
            foreach (MutableTuple<string, string, ushort> item in data)
            {
                if (item.Item1.Equals("TITLE"))
                {
                    TitleSoundID = item.Item3;
                }
                else if (item.Item1.Equals("SECRET"))
                {
                    SecretSoundID = item.Item3;
                }
                else
                {
                    AbstractTRLevel level = GetLevel(item.Item1);
                    if (level == null)
                    {
                        throw new ArgumentException(string.Format("{0} does not represent a valid level", item.Item1));
                    }

                    level.TrackID = item.Item3;
                }
            }
        }
    }
}