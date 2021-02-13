using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using TRGE.Coord;
using TRGE.Extension;

namespace TRGE.Core.Test
{
    [TestClass]
    public abstract class AbstractTRExtensionTests : BaseTestCollection
    {
        protected abstract string DataDirectory { get; }
        protected abstract TREdition Edition { get; }
        protected string WorkingDirectory => DataDirectory + @"\WorkingDir";

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
        protected void TestExtensionSupported()
        {
            PrepareDirectories();

            Assert.IsTrue(TRLevelEditorFactory.EditionSupportsLevelEditing(Edition));

            TREditor editor = TRCoord.Instance.Open(WorkingDirectory);
            Assert.IsNotNull(editor.LevelEditor);
            Assert.IsTrue(editor.LevelEditor is TRLevelEditorExtensionExample);
        }

        [TestMethod]
        [TestSequence(1)]
        protected void TestExtensionSaving()
        {
            TREditor editor = TRCoord.Instance.Open(WorkingDirectory);
            (editor.LevelEditor as TRLevelEditorExtensionExample).CustomBool = true;

            int expectedTarget = editor.ScriptEditor.GetSaveTargetCount() + editor.LevelEditor.GetSaveTargetCount();
            int progress = 0;
            editor.SaveProgressChanged += delegate(object sender, TRSaveEventArgs e)
            {
                progress = e.ProgressValue;
            };
            editor.Save();

            Assert.AreEqual(expectedTarget, progress);
        }

        [TestMethod]
        [TestSequence(2)]
        protected void TestExtensionConfig()
        {
            TREditor editor = TRCoord.Instance.Open(WorkingDirectory);
            (editor.LevelEditor as TRLevelEditorExtensionExample).CustomBool = true;
            (editor.LevelEditor as TRLevelEditorExtensionExample).CustomInt = 300;
            editor.Save();

            editor = TRCoord.Instance.Open(WorkingDirectory);
            Assert.IsTrue((editor.LevelEditor as TRLevelEditorExtensionExample).CustomBool);
            Assert.IsTrue((editor.LevelEditor as TRLevelEditorExtensionExample).CustomInt == 300);
        }

        protected override void TearDown()
        {
            if (Directory.Exists(WorkingDirectory))
            {
                Directory.Delete(WorkingDirectory, true);
            }
            base.TearDown();
        }
    }
}