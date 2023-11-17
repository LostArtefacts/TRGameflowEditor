using Newtonsoft.Json;

namespace TRGE.Core
{
    public class TRAudioTrack
    {
        [JsonProperty]
        public ushort ID;
        [JsonProperty]
        public string Name;
        [JsonProperty]
        public uint Offset;
        [JsonProperty]
        public uint Length;
        [JsonProperty]
        public List<TRAudioCategory> Categories;
        [JsonProperty]
        public TRAudioCategory PrimaryCategory;

        public TRAudioTrack()
        {
            Categories = new List<TRAudioCategory>();
            PrimaryCategory = TRAudioCategory.General;
        }

        public override bool Equals(object obj)
        {
            return obj is TRAudioTrack track &&
                   ID == track.ID;
        }

        public override int GetHashCode()
        {
            return 1213502048 + ID.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("{0} - {1}", ID.ToString().PadLeft(3, '0'), Name);
        }
    }
}