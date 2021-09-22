using System;
using System.Collections.Generic;

namespace TRGE.Core
{
    public abstract class AbstractTRAudioProvider
    {
        protected readonly List<TRAudioTrack> _tracks;
        public IReadOnlyList<TRAudioTrack> Tracks => _tracks;

        public abstract TRAudioType AudioType { get; }

        public AbstractTRAudioProvider()
        {
            _tracks = new List<TRAudioTrack>();
        }

        public abstract TRAudioTrack GetBlankTrack();
        public abstract byte[] GetTrackData(TRAudioTrack track);

        public byte[] GetTrackData(uint id)
        {
            TRAudioTrack track = GetTrack(id);
            if (track != null)
            {
                return GetTrackData(track);
            }

            return null;
        }

        public TRAudioTrack GetTrack(uint id)
        {
            foreach (TRAudioTrack track in _tracks)
            {
                if (track.ID == id)
                {
                    return track;
                }
            }

            TRAudioTrack blankTrack = GetBlankTrack();
            if (id == blankTrack.ID)
            {
                return blankTrack;
            }
            return null;
        }

        public IReadOnlyDictionary<TRAudioCategory, List<TRAudioTrack>> GetCategorisedTracks()
        {
            Dictionary<TRAudioCategory, List<TRAudioTrack>> data = new Dictionary<TRAudioCategory, List<TRAudioTrack>>();
            foreach (TRAudioCategory category in (TRAudioCategory[])Enum.GetValues(typeof(TRAudioCategory)))
            {
                data.Add(category, new List<TRAudioTrack>());
            }

            foreach (TRAudioTrack track in Tracks)
            {
                foreach (TRAudioCategory category in track.Categories)
                {
                    data[category].Add(track);
                }
            }

            return data;
        }
    }
}