using System;

namespace TRGE.Core
{
    internal class TRItem : IComparable<TRItem>
    {
        internal ushort ID { get; }
        internal TRItemCategory Category { get; }
        internal string Name { get; }

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