using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using TRGE.Coord;

namespace TRGE.Core.Test
{
    public abstract class AbstractTestCollection
    {
        protected string[] _validScripts = new string[]
        {
            @"scripts\TOMBPC_TR2.dat", @"scripts\TOMBPC_TR2G.dat", @"scripts\TOMBPC_TR3.dat", @"scripts\TOMBPSX_BETA_TR2.dat",
            @"scripts\TOMBPSX_TR2.dat", @"scripts\TOMBPSX_TR3.dat", @"scripts\TRTLA_TR3G.dat"
        };

        public Dictionary<string, Exception> Run()
        {
            RootSetup();

            List<MethodInfo> methods = new(GetType().GetMethods(BindingFlags.FlattenHierarchy | BindingFlags.NonPublic | BindingFlags.Instance));
            methods.Sort(delegate (MethodInfo m1, MethodInfo m2)
            {
                TestSequenceAttribute tseq1 = (TestSequenceAttribute)m1.GetCustomAttribute(typeof(TestSequenceAttribute));
                TestSequenceAttribute tseq2 = (TestSequenceAttribute)m2.GetCustomAttribute(typeof(TestSequenceAttribute));

                int seq1 = tseq1 == null ? int.MaxValue : tseq1.Sequence;
                int seq2 = tseq2 == null ? int.MaxValue : tseq2.Sequence;

                if (seq1 != seq2)
                {
                    return seq1.CompareTo(seq2);
                }
                return m1.Name.CompareTo(m2.Name);
            });

            Dictionary<string, Exception> result = new();
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

            RootTearDown();

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

        private void RootSetup()
        {
            TRCoord.Instance.RootConfigDirectory = Directory.GetCurrentDirectory();
            RestoreScripts();
            Setup();
        }

        private void RootTearDown()
        {
            Directory.Delete(TRCoord.Instance.ConfigDirectory, true);
            RestoreScripts();
            TearDown();
        }

        private void RestoreScripts()
        {
            foreach (string script in _validScripts)
            {
                File.Copy(@"..\..\" + script, script, true);
            }
        }

        [ClassInitialize]
        protected abstract void Setup();
        [ClassCleanup]
        protected abstract void TearDown();
    }
}