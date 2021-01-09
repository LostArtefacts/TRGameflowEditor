using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TRGE.Core.Test
{
    public abstract class AbstractTestCollection
    {
        public Dictionary<string, Exception> Run()
        {
            Setup();

            Dictionary<string, Exception> result = new Dictionary<string, Exception>();
            MethodInfo[] methods = GetType().GetMethods(BindingFlags.FlattenHierarchy | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (MethodInfo mi in methods)
            {
                if (mi.GetCustomAttribute(typeof(TestMethodAttribute)) != null)
                {
                    result.Add(mi.Name, null);
                    try
                    {
                        ((Action)Delegate.CreateDelegate(typeof(Action), this, mi))();
                    }
                    catch (Exception e)
                    {
                        result[mi.Name] = e;
                    }
                }
            }

            TearDown();

            return result;
        }

        protected void CompareUShorts(List<ushort> list, IReadOnlyList<ushort> compareWith)
        {
            CollectionAssert.AreEqual(list, new List<ushort>(compareWith));
        }

        protected void CompareUShorts(List<ushort> list, IReadOnlyList<ushort> compareWith, string message)
        {
            CollectionAssert.AreEqual(list, new List<ushort>(compareWith), message);
        }

        protected void CompareUShortArrays(List<ushort[]> list, IReadOnlyList<ushort[]> compareWith)
        {
            if (list.Count != compareWith.Count)
            {
                Assert.Fail("Array lengths do not match");
            }

            for (int i = 0; i < list.Count; i++)
            {
                ushort[] arr1 = list[i];
                ushort[] arr2 = compareWith[i];
                if (arr1.Length != arr2.Length)
                {
                    Assert.Fail(string.Format("Elements at index {0} are not the same length", i));
                }

                for (int j = 0; j < arr1.Length; j++)
                {
                    if (arr1[j] != arr2[j])
                    {
                        Assert.Fail(string.Format("Elements at position ({0},{1}) do not match", i, j));
                    }
                }
            }
        }

        protected void CompareUIntArrays(List<uint[]> list, IReadOnlyList<uint[]> compareWith)
        {
            if (list.Count != compareWith.Count)
            {
                Assert.Fail("Array lengths do not match");
            }

            for (int i = 0; i < list.Count; i++)
            {
                uint[] arr1 = list[i];
                uint[] arr2 = compareWith[i];
                if (arr1.Length != arr2.Length)
                {
                    Assert.Fail(string.Format("Elements at index {0} are not the same length", i));
                }

                for (int j = 0; j < arr1.Length; j++)
                {
                    if (arr1[j] != arr2[j])
                    {
                        Assert.Fail(string.Format("Elements at position ({0},{1}) do not match", i, j));
                    }
                }
            }
        }

        protected void CompareStrings(List<string> list, IReadOnlyList<string> compareWith)
        {
            CollectionAssert.AreEqual(list, new List<string>(compareWith));
        }

        protected void CompareStrings(List<string> list, IReadOnlyList<string> compareWith, string message)
        {
            CollectionAssert.AreEqual(list, new List<string>(compareWith), message);
        }

        [ClassInitialize]
        protected abstract void Setup();
        [ClassCleanup]
        protected abstract void TearDown();
    }
}