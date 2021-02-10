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

        internal Organisation SequencingOrganisation { get; set; }
        internal RandomGenerator SequencingRNG { get; set; }
        internal Organisation GameTrackOrganisation { get; set; }
        internal RandomGenerator GameTrackRNG { get; set; }
        internal Organisation SecretSupportOrganisation { get; set; }
        internal Organisation SunsetOrganisation { get; set; }
        internal RandomGenerator SunsetRNG { get; set; }
        internal uint RandomSunsetCount { get; set; }

        internal TREdition Edition { get; private set; }

        internal bool CanSetSunsets => Edition.SunsetsSupported;

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

        internal virtual List<Tuple<string, string>> GetSequencing()
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

        internal virtual void SetSequencing(List<Tuple<string, string>> data)
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

        internal virtual void RandomiseSequencing(List<AbstractTRScriptedLevel> originalLevels)
        {
            List<AbstractTRScriptedLevel> shuffledLevels = new List<AbstractTRScriptedLevel>(originalLevels);
            shuffledLevels.Randomise(SequencingRNG.Create());

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
                    FireLevelModificationEvent(level, TRScriptedLevelModification.SequenceChanged);
                }
            }
        }

        internal void RestoreSequencing(List<AbstractTRScriptedLevel> originalLevels)
        {
            SetSequencing(GetLevelSequencing(originalLevels));
        }

        protected virtual void FireLevelModificationEvent(AbstractTRScriptedLevel level, TROpDef opDef)
        {
            FireLevelModificationEvent(level, OpDefToModification(opDef));
        }

        protected virtual void FireLevelModificationEvent(AbstractTRScriptedLevel level, TRScriptedLevelModification modification)
        {
            LevelModified?.Invoke(this, TRScriptedLevelEventArgs.Create(level, modification));
        }

        protected abstract TRScriptedLevelModification OpDefToModification(TROpDef opDef);

        internal virtual void RandomiseLevelsWithOperation(RandomGenerator rng, uint levelCount, List<AbstractTRScriptedLevel> basisLevels, TROperation operation)
        {
            List<AbstractTRScriptedLevel> levelSet = basisLevels.RandomSelection(rng.Create(), levelCount);
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
                    FireLevelModificationEvent(level, operation.Definition);
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

        internal List<Tuple<ushort, string>> GetAllGameTracks()
        {
            List<Tuple<ushort, string>> tracks = new List<Tuple<ushort, string>>();
            foreach (TRAudioTrack track in AudioProvider.Tracks)
            {
                tracks.Add(new Tuple<ushort, string>(track.ID, track.Name));
            }
            return tracks;
        }

        internal List<MutableTuple<string, string, ushort>> GetTrackData(List<AbstractTRScriptedLevel> originalLevels)
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

            foreach (AbstractTRScriptedLevel originalLevel in originalLevels)
            {
                AbstractTRScriptedLevel level = GetLevel(originalLevel.ID);
                track = AudioProvider.GetTrack(level.TrackID);
                if (track == null)
                {
                    track = AudioProvider.GetBlankTrack();
                }
                ret.Add(new MutableTuple<string, string, ushort>(level.ID, level.Name, track.ID));
            }
            return ret;
        }

        internal void SetTrackData(List<MutableTuple<string, string, ushort>> data)
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

        internal List<MutableTuple<string, string, bool>> GetSecretSupport(List<AbstractTRScriptedLevel> originalLevels)
        {
            List<MutableTuple<string, string, bool>> support = new List<MutableTuple<string, string, bool>>();
            foreach (AbstractTRScriptedLevel originalLevel in originalLevels)
            {
                AbstractTRScriptedLevel level = GetLevel(originalLevel.ID);
                support.Add(new MutableTuple<string, string, bool>(level.ID, level.Name, level.HasSecrets));
            }
            return support;
        }

        internal void SetSecretSupport(List<MutableTuple<string, string, bool>> secretSupport)
        {
            foreach (MutableTuple<string, string, bool> item in secretSupport)
            {
                AbstractTRScriptedLevel level = GetLevel(item.Item1);
                if (level == null)
                {
                    throw new ArgumentException(string.Format("{0} does not represent a valid level", item.Item1));
                }

                level.HasSecrets = item.Item3;
            }
        }

        internal void RestoreSecretSupport(List<AbstractTRScriptedLevel> originalLevels)
        {
            foreach (AbstractTRScriptedLevel originalLevel in originalLevels)
            {
                AbstractTRScriptedLevel level = GetLevel(originalLevel.ID);
                if (level == null)
                {
                    throw new ArgumentException(string.Format("{0} does not represent a valid level", originalLevel.ID));
                }
                level.HasSecrets = originalLevel.HasSecrets;
            }
        }

        internal List<MutableTuple<string, string, bool>> GetSunsetData(List<AbstractTRScriptedLevel> originalLevels)
        {
            List<MutableTuple<string, string, bool>> data = new List<MutableTuple<string, string, bool>>();
            foreach (AbstractTRScriptedLevel originalLevel in originalLevels)
            {
                AbstractTRScriptedLevel level = GetLevel(originalLevel.ID);
                data.Add(new MutableTuple<string, string, bool>(level.ID, level.Name, level.HasSunset));
            }
            return data;
        }

        internal void SetSunsetData(List<MutableTuple<string, string, bool>> data)
        {
            foreach (MutableTuple<string, string, bool> item in data)
            {
                AbstractTRScriptedLevel level = GetLevel(item.Item1);
                if (level == null)
                {
                    throw new ArgumentException(string.Format("{0} does not represent a valid level", item.Item1));
                }

                level.HasSunset = item.Item3;
                FireLevelModificationEvent(level, TRScriptedLevelModification.SunsetChanged);
            }
        }

        internal void RestoreSunsetData(List<AbstractTRScriptedLevel> originalLevels)
        {
            foreach (AbstractTRScriptedLevel originalLevel in originalLevels)
            {
                AbstractTRScriptedLevel level = GetLevel(originalLevel.ID);
                if (level == null)
                {
                    throw new ArgumentException(string.Format("{0} does not represent a valid level", originalLevel.ID));
                }

                level.HasSunset = originalLevel.HasSunset;
            }
        }

        internal void RandomiseSunsets(List<AbstractTRScriptedLevel> basisLevels)
        {
            RandomiseLevelsWithOperation
            (
                SunsetRNG,
                RandomSunsetCount,
                basisLevels,
                new TROperation(TR23OpDefs.Sunset, ushort.MaxValue, true)
            );
        }

        internal List<AbstractTRScriptedLevel> GetSunsetLevels()
        {
            return GetLevelsWithOperation(TR23OpDefs.Sunset, true);
        }

        internal uint GetSunsetLevelCount()
        {
            return Convert.ToUInt32(GetSunsetLevels().Count);
        }
    }
}