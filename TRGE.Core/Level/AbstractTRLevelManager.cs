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
        internal abstract List<AbstractTRScriptedLevel> Levels { get; set; }
        internal abstract int LevelCount { get; }

        internal Organisation LevelOrganisation { get; set; }
        internal RandomGenerator LevelRNG { get; set; }
        internal Organisation GameTrackOrganisation { get; set; }
        internal RandomGenerator GameTrackRNG { get; set; }
        internal TREdition Edition { get; private set; }

        internal event EventHandler<TRScriptedLevelEventArgs> LevelModified;

        internal AbstractTRLevelManager(TREdition edition)
        {
            Edition = edition;
        }

        internal abstract void Save();

        internal virtual AbstractTRScriptedLevel GetLevel(string id)
        {
            foreach (AbstractTRScriptedLevel level in Levels)
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

        private List<Tuple<string, string>> GetLevelSequencing(List<AbstractTRScriptedLevel> levels)
        {
            List<Tuple<string, string>> data = new List<Tuple<string, string>>();
            foreach (AbstractTRScriptedLevel level in levels)
            {
                data.Add(new Tuple<string, string>(level.ID, level.Name));
            }
            return data;
        }

        internal virtual void SetLevelSequencing(List<Tuple<string, string>> data)
        {
            List<AbstractTRScriptedLevel> newLevels = new List<AbstractTRScriptedLevel>();
            foreach (Tuple<string, string> item in data)
            {
                AbstractTRScriptedLevel level = GetLevel(item.Item1);
                if (level == null)
                {
                    throw new ArgumentException(string.Format("{0} does not represent a valid level", item.Item1));
                }
                newLevels.Add(level);
            }

            Levels = newLevels;
            SetLevelSequencing();
        }

        internal virtual void RandomiseLevelSequencing(List<AbstractTRScriptedLevel> originalLevels)
        {
            List<AbstractTRScriptedLevel> shuffledLevels = new List<AbstractTRScriptedLevel>(originalLevels);
            shuffledLevels.Randomise(LevelRNG.Create());

            List<AbstractTRScriptedLevel> newLevels = new List<AbstractTRScriptedLevel>();
            foreach (AbstractTRScriptedLevel shfLevel in shuffledLevels)
            {
                AbstractTRScriptedLevel level = GetLevel(shfLevel.ID);
                if (level == null)
                {
                    throw new ArgumentException(string.Format("{0} does not represent a valid level", shfLevel.ID));
                }
                newLevels.Add(level);
            }

            Levels = newLevels;
            SetLevelSequencing();
        }

        internal void SetLevelSequencing()
        {
            ushort newSeq = 1;
            foreach (AbstractTRScriptedLevel level in Levels)
            {
                bool modified = false;
                if (level.Sequence != newSeq)
                {
                    level.Sequence = newSeq;
                    modified = true;
                }
                newSeq++;

                bool isFinalLevel = newSeq == (LevelCount - Edition.LevelCompleteOffset) + 1;
                if (isFinalLevel != level.IsFinalLevel)
                {
                    level.IsFinalLevel = isFinalLevel;
                    modified = true;
                }

                if (modified)
                {
                    FireLevelModificationEvent(level);
                }
            }
        }

        internal void RestoreLevelSequencing(List<AbstractTRScriptedLevel> originalLevels)
        {
            SetLevelSequencing(GetLevelSequencing(originalLevels));
        }

        protected virtual void FireLevelModificationEvent(AbstractTRScriptedLevel level)
        {
            LevelModified?.Invoke(this, TRScriptedLevelEventArgs.Create(level));
        }

        internal virtual void RandomiseLevelsWithOperation(RandomGenerator rng, uint levelCount, List<AbstractTRScriptedLevel> originalLevels, TROperation operation)
        {
            List<AbstractTRScriptedLevel> levelSet = originalLevels.RandomSelection(rng.Create(), levelCount);
            foreach (AbstractTRScriptedLevel level in Levels)
            {
                bool modified;
                if (levelSet.Contains(level))
                {
                    modified = level.EnsureOperation(operation);
                }
                else
                {
                    modified = level.RemoveOperation(operation.Definition);
                }

                if (modified)
                {
                    FireLevelModificationEvent(level);
                }
            }
        }

        internal List<AbstractTRScriptedLevel> GetLevelsWithOperation(TROpDef opDef, bool activeOnly)
        {
            List<AbstractTRScriptedLevel> levels = new List<AbstractTRScriptedLevel>();
            foreach (AbstractTRScriptedLevel level in Levels)
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

            foreach (AbstractTRScriptedLevel level in Levels)
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
                    AbstractTRScriptedLevel level = GetLevel(item.Item1);
                    if (level == null)
                    {
                        throw new ArgumentException(string.Format("{0} does not represent a valid level", item.Item1));
                    }

                    level.TrackID = item.Item3;
                }
            }
        }

        internal void RandomiseGameTracks(List<AbstractTRScriptedLevel> originalLevels)
        {
            IReadOnlyDictionary<TRAudioCategory, List<TRAudioTrack>> tracks = AudioProvider.GetCategorisedTracks();
            Random rand = GameTrackRNG.Create();

            if (tracks[TRAudioCategory.Title].Count > 0)
            {
                TitleSoundID = tracks[TRAudioCategory.Title].RandomSelection(rand, 1)[0].ID;
            }
            if (tracks[TRAudioCategory.Secret].Count > 0)
            {
                SecretSoundID = tracks[TRAudioCategory.Secret].RandomSelection(rand, 1)[0].ID;
            }
            if (tracks[TRAudioCategory.Ambient].Count > 0)
            {
                List<TRAudioTrack> levelTracks = tracks[TRAudioCategory.Ambient].RandomSelection(rand, Convert.ToUInt32(Levels.Count), true);
                for (int i = 0; i < originalLevels.Count; i++)
                {
                    AbstractTRScriptedLevel level = GetLevel(originalLevels[i].ID);
                    if (level == null)
                    {
                        throw new ArgumentException(string.Format("{0} does not represent a valid level", originalLevels[i].ID));
                    }

                    level.TrackID = levelTracks[i].ID;
                }
            }
        }

        internal void RestoreGameTracks(AbstractTRScript originalScript)
        {
            TitleSoundID = originalScript.TitleSoundID;
            SecretSoundID = originalScript.SecretSoundID;

            foreach (AbstractTRScriptedLevel originalLevel in originalScript.Levels)
            {
                AbstractTRScriptedLevel level = GetLevel(originalLevel.ID);
                if (level == null)
                {
                    throw new ArgumentException(string.Format("{0} does not represent a valid level", originalLevel.ID));
                }
                level.TrackID = originalLevel.TrackID;
            }
        }
    }
}