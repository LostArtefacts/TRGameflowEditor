namespace TRGE.Core;

internal static class CollectionExtensions
{
    internal static void Randomise<T>(this List<T> list, Random rand)
    {
        SortedDictionary<int, T> map = new();
        foreach (T item in list)
        {
            int r;
            do
            {
                r = rand.Next();
            }
            while (map.ContainsKey(r));
            map.Add(r, item);
        }

        list.Clear();
        list.AddRange(map.Values);
    }

    internal static List<T> RandomSelection<T>(this List<T> list, Random rand, uint count, bool allowDuplicates = false, ISet<T> exclusions = null)
    {
        if (count > list.Count && !allowDuplicates)
        {
            throw new ArgumentException(string.Format("The given count ({0}) is larger than that of the provided list {1}.", count, list.Count));
        }

        if (count == list.Count)
        {
            return list;
        }

        List<T> iterList = new(list);
        if (exclusions != null && exclusions.Count > 0)
        {
            foreach (T excludeItem in exclusions)
            {
                iterList.Remove(excludeItem);
            }
        }

        List<T> resultSet = new();
        if (iterList.Count > 0)
        {
            int icount = Convert.ToInt32(count);
            int maxIter = allowDuplicates ? icount : Math.Min(icount, iterList.Count);
            for (int i = 0; i < maxIter; i++)
            {
                T item;
                do
                {
                    item = iterList[rand.Next(0, iterList.Count)];
                }
                while (!allowDuplicates && resultSet.Contains(item));
                resultSet.Add(item);
            }
        }            

        return resultSet;
    }

    public static void Shuffle<T>(this List<T> list, Random rand, int count = 5)
    {
        for (int i = 0; i < count; i++)
        {
            ShuffleImpl(list, rand);
        }
    }

    private static void ShuffleImpl<T>(List<T> list, Random rand)
    {
        List<T> iterList = new(list);
        list.Clear();

        int count = iterList.Count;
        while (list.Count < count)
        {
            T item = iterList[rand.Next(0, iterList.Count)];
            list.Add(item);
            iterList.Remove(item);
        }
    }

    internal static void Sort<T1, T2>(this Dictionary<T1, T2> dict, Comparison<T1> comp)
    {
        List<T1> keys = new(dict.Keys);
        keys.Sort(comp);
        Dictionary<T1, T2> result = new();
        foreach (T1 key in keys)
        {
            result.Add(key, dict[key]);
        }

        dict.Clear();
        foreach (T1 key in result.Keys)
        {
            dict.Add(key, result[key]);
        }
    }

    internal static void AddOrIgnore<T1, T2>(this Dictionary<T1, T2> dict, T1 key, T2 val)
    {
        if (!dict.ContainsKey(key))
        {
            dict.Add(key, val);
        }
    }

    internal static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dict)
    {
        return dict.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
    }
}