namespace TRGE.Core
{
    public class TRItem : IComparable<TRItem>
    {
        public ushort ID { get; set; }
        public TRItemCategory Category { get; set; }
        public string Name { get; set; }

        internal TRItem(ushort id, TRItemCategory category, string name)
        {
            ID = id;
            Category = category;
            Name = name;
        }

        public int CompareTo(TRItem other)
        {
            return ID.CompareTo(other.ID);
        }

        public override bool Equals(object obj)
        {
            return obj is TRItem item && ID == item.ID;
        }

        public override int GetHashCode()
        {
            return 1213502048 + ID.GetHashCode();
        }
    }
}