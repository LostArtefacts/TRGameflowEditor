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

        internal static List<T> RandomSelection<T>(this List<T> list, Random rand, uint count, bool allowDuplicates = false, ISet<T> exclusions = null)
        {
            if (count > list.Count)
            {
                throw new ArgumentException("Given count is larger than provided list");
            }

            if (count == list.Count)
            {
                return list;
            }

            List<T> iterList = new List<T>(list);
            if (exclusions != null && exclusions.Count > 0)
            {
                foreach (T excludeItem in exclusions)
                {
                    iterList.Remove(excludeItem);
                }
            }

            List<T> resultSet = new List<T>();
            if (iterList.Count > 0)
            {
                for (int i = 0; i < count; i++)
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
    }
}