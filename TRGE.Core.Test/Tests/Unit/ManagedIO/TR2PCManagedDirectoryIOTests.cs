using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using TRGE.Coord;

namespace TRGE.Core.Test
{
    [TestClass]
    public class TR2PCManagedDirectoryIOTests : BaseTestCollection
    {
        private readonly string _dataDirectory = "tr2datatest";

        [TestMethod]
        protected void TestManagedIO()
        {
            if (_dataDirectory == null || !Directory.Exists(_dataDirectory))
            {
                Assert.Fail("Test cannot proceed - data directory not set or does not exit.");
            }
            TREditor editor = TRCoord.Instance.Open(_dataDirectory);
            Assert.IsTrue(editor.ScriptManager.BackupFile.Exists);
        }
    }
}