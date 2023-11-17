using Newtonsoft.Json;
using System.Runtime.CompilerServices;

namespace TRGE.Core
{
    public class MutableTuple<T1, T2> : ITuple
    {
        public T1 Item1 { get; set; }
        public T2 Item2 { get; set; }
        [JsonIgnore]
        public int Length => 2;

        public object this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return Item1;
                    case 1:
                        return Item2;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
            set
            {
                switch (index)
                {
                    case 0:
                        Item1 = (T1)value;
                        break;
                    case 1:
                        Item2 = (T2)value;
                        break;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
        }

        public MutableTuple(T1 item1, T2 item2)
        {
            Item1 = item1;
            Item2 = item2;
        }

        public override bool Equals(object obj)
        {
            return obj is MutableTuple<T1, T2> tuple &&
                   EqualityComparer<T1>.Default.Equals(Item1, tuple.Item1) &&
                   EqualityComparer<T2>.Default.Equals(Item2, tuple.Item2);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Item1, Item2);
        }

        public MutableTuple<T1, T2> DeepCopy()
        {
            return new MutableTuple<T1, T2>(Item1, Item2);
        }
    }

    public class MutableTuple<T1, T2, T3> : ITuple
    {
        public T1 Item1 { get; set; }
        public T2 Item2 { get; set; }
        public T3 Item3 { get; set; }
        [JsonIgnore]
        public int Length => 3;

        public object this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return Item1;
                    case 1:
                        return Item2;
                    case 2:
                        return Item3;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
            set
            {
                switch (index)
                {
                    case 0:
                        Item1 = (T1)value;
                        break;
                    case 1:
                        Item2 = (T2)value;
                        break;
                    case 3:
                        Item3 = (T3)value;
                        break;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
        }

        public MutableTuple(T1 item1, T2 item2, T3 item3)
        {
            Item1 = item1;
            Item2 = item2;
            Item3 = item3;
        }

        public override bool Equals(object obj)
        {
            return obj is MutableTuple<T1, T2, T3> tuple &&
                   EqualityComparer<T1>.Default.Equals(Item1, tuple.Item1) &&
                   EqualityComparer<T2>.Default.Equals(Item2, tuple.Item2) &&
                   EqualityComparer<T3>.Default.Equals(Item3, tuple.Item3);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Item1, Item2, Item3);
        }

        public MutableTuple<T1, T2, T3> DeepCopy()
        {
            return new MutableTuple<T1, T2, T3>(Item1, Item2, Item3);
        }
    }

    public class MutableTuple<T1, T2, T3, T4> : ITuple
    {
        public T1 Item1 { get; set; }
        public T2 Item2 { get; set; }
        public T3 Item3 { get; set; }
        public T4 Item4 { get; set; }
        [JsonIgnore]
        public int Length => 4;

        public object this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return Item1;
                    case 1:
                        return Item2;
                    case 2:
                        return Item3;
                    case 3:
                        return Item4;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
            set
            {
                switch (index)
                {
                    case 0:
                        Item1 = (T1)value;
                        break;
                    case 1:
                        Item2 = (T2)value;
                        break;
                    case 3:
                        Item3 = (T3)value;
                        break;
                    case 4:
                        Item4 = (T4)value;
                        break;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
        }

        public MutableTuple(T1 item1, T2 item2, T3 item3, T4 item4)
        {
            Item1 = item1;
            Item2 = item2;
            Item3 = item3;
            Item4 = item4;
        }

        public override bool Equals(object obj)
        {
            return obj is MutableTuple<T1, T2, T3, T4> tuple &&
                   EqualityComparer<T1>.Default.Equals(Item1, tuple.Item1) &&
                   EqualityComparer<T2>.Default.Equals(Item2, tuple.Item2) &&
                   EqualityComparer<T3>.Default.Equals(Item3, tuple.Item3) &&
                   EqualityComparer<T4>.Default.Equals(Item4, tuple.Item4);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Item1, Item2, Item3, Item4);
        }

        public MutableTuple<T1, T2, T3, T4> DeepCopy()
        {
            return new MutableTuple<T1, T2, T3, T4>(Item1, Item2, Item3, Item4);
        }
    }
}