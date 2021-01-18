namespace TRGE.Core
{
    internal class TRAudioTrack
    {
        internal ushort ID;
        internal string Name;
        internal uint Length;
        internal uint Offset;

        public override string ToString()
        {
            return string.Format("{0} - {1}", ID.ToString().PadLeft(3, '0'), Name);
        }
    }
}