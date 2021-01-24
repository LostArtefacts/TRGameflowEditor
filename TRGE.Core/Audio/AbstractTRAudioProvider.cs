using System;
using System.Collections.Generic;

namespace TRGE.Core
{
    internal abstract class AbstractTRAudioProvider
    {
        protected readonly List<TRAudioTrack> _tracks;
        internal IReadOnlyList<TRAudioTrack> Tracks => _tracks;

        internal abstract TRAudioType AudioType { get; }

        internal AbstractTRAudioProvider()
        {
            _tracks = new List<TRAudioTrack>();
        }

        internal abstract TRAudioTrack GetBlankTrack();
        internal abstract byte[] GetTrackData(TRAudioTrack track);

        internal byte[] GetTrackData(uint id)
        {
            TRAudioTrack track = GetTrack(id);
            if (track != null)
            {
                return GetTrackData(track);
            }

            return null;
        }

        internal TRAudioTrack GetTrack(uint id)
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

        internal IReadOnlyDictionary<TRAudioCategory, List<TRAudioTrack>> GetCategorisedTracks()
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