using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TRGE.Coord;

namespace TRGE.Core.Test
{
    [TestClass]
    public abstract class AbstractTR23ImportExportTestCollection : BaseTestCollection
    {
        protected abstract string DataDirectory { get; }
        protected string WorkingDirectory => DataDirectory + @"\WorkingDir";
        protected string TestSettingsPath => DataDirectory + @"\settings.trge";

        private void PrepareDirectories()
        {
            if (DataDirectory == null || !Directory.Exists(DataDirectory))
            {
                Assert.Fail("Test cannot proceed - data directory not set or does not exit.");
            }
            new DirectoryInfo(DataDirectory + @"\Original").Copy(WorkingDirectory, true);
        }

        [TestMethod]
        [TestSequence(0)]
        protected void TestExportInitialLoad()
        {
            PrepareDirectories();

            TREditor editor = TRCoord.Instance.Open(WorkingDirectory);
            Assert.IsFalse(editor.IsExportPossible);

            try
            {
                editor.ExportSettings(TestSettingsPath);
                Assert.Fail();
            }
            catch (InvalidOperationException) { }
        }

        [TestMethod]
        [TestSequence(1)]
        protected void TestExportPostSave()
        {
            PrepareDirectories();

            TREditor editor = TRCoord.Instance.Open(WorkingDirectory);
            editor.ScriptEditor.FrontEndHasFMV = false;
            editor.Save();

            Assert.IsTrue(editor.IsExportPossible);

            try
            {
                editor.ExportSettings(TestSettingsPath);

                Assert.IsTrue(File.Exists(TestSettingsPath));
            }
            catch (InvalidOperationException)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        [TestSequence(2)]
        protected void TestExportReload()
        {
            TREditor editor = TRCoord.Instance.Open(WorkingDirectory);
            Assert.IsTrue(editor.IsExportPossible);
        }

        [TestMethod]
        [TestSequence(3)]
        protected void TestImport()
        {
            TREditor editor = TRCoord.Instance.Open(WorkingDirectory);
            editor.ScriptEditor.FrontEndHasFMV = true;
            editor.Save();

            editor.ImportSettings(TestSettingsPath);
            Assert.IsFalse(editor.ScriptEditor.FrontEndHasFMV);
        }

        [ClassCleanup]
        protected override void TearDown()
        {
            base.TearDown();
            if (Directory.Exists(WorkingDirectory))
            {
                Directory.Delete(WorkingDirectory, true);
            }
            if (File.Exists(TestSettingsPath))
            {
                File.Delete(TestSettingsPath);
            }
        }
    }
}