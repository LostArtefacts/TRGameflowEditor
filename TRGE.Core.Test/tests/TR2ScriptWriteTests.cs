using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TRGE.Core.Test
{
    [TestClass]
    public class TR2ScriptWriteTests : AbstractTestCollection
    {
        private string _filePath;
        private TR23Script _script;

        [ClassInitialize]
        protected override void Setup()
        {
            _filePath = @"scripts\TOMBPC_TR2.dat";
        }

        [ClassCleanup]
        protected override void TearDown() { }

        [TestMethod]
        protected void TestUntouchedWrite()
        {
            byte[] originalData = File.ReadAllBytes(_filePath);
            _script = ScriptFactory.OpenScript(_filePath) as TR23Script;
            CollectionAssert.AreEqual(originalData, _script.Serialise());
        }
    }
}