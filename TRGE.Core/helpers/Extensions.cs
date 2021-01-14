using System;
using System.Collections.Generic;
using System.Linq;

namespace TRGE.Core
{
    internal static class Extensions
    {
        internal static void Randomise<T>(this List<T> list, Random rand)
        {
            SortedDictionary<int, T> map = new SortedDictionary<int, T>();
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

        internal static List<T> RandomSelection<T>(this List<T> list, Random rand, uint count)
        {
            if (count > list.Count)
            {
                throw new ArgumentException("Given count is larer than provided list");
            }

            if (count == list.Count)
            {
                return list;
            }

            HashSet<T> resultSet = new HashSet<T>();
            while (resultSet.Count < count)
            {
                resultSet.Add(list[rand.Next(0, list.Count)]);
            }

            return resultSet.ToList();
        }
    }
}