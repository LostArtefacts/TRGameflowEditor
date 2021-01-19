using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;

namespace TRGE.Core.Test
{
    [TestClass]
    public class TR23ScriptManagementTests : BaseTestCollection
    {
        [TestMethod]
        [TestSequence(0)]
        protected void TestCreateScriptManager()
        {
            foreach (string scriptFile in _validScripts)
            {
                TestCreateScriptManager(new FileInfo(scriptFile).FullName);
            }
        }

        private void TestCreateScriptManager(string filePath)
        {
            AbstractTRScriptManager scriptMan = TRGameflowEditor.Instance.GetScriptManager(filePath);
            Assert.IsFalse(scriptMan == null);
            Assert.IsTrue(scriptMan is TR23ScriptManager);
            Assert.IsTrue(scriptMan.OriginalFilePath.Equals(filePath));
        }

        [TestMethod]
        protected void TestBackup()
        {
            TR23ScriptManager sm = TRGameflowEditor.Instance.GetScriptManager(_validScripts[0]) as TR23ScriptManager;
            Assert.IsFalse(sm.BackupFilePath == null);
            Assert.IsTrue(File.Exists(sm.BackupFilePath));
            try
            {
                AbstractTRScript script = TRScriptFactory.OpenScript(sm.BackupFilePath);
                Assert.IsTrue(script is TR23Script);
                Assert.IsTrue(script.Edition == sm.Script.Edition);
            }
            catch (UnsupportedScriptException)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        protected void TestHistory()
        {
            TRGameflowEditor.Instance.CloseAllScriptManagers();
            TRGameflowEditor.Instance.ClearFileHistory();

            List<string> openedScripts = new List<string>();
            TRGameflowEditor.Instance.FileHistoryAdded += delegate (object sender, TRFileEventArgs e)
            {
                openedScripts.Add(e.FilePath);
            };

            foreach (string scriptFile in _validScripts)
            {
                TRGameflowEditor.Instance.GetScriptManager(scriptFile);
            }

            Assert.IsTrue(openedScripts.Count == _validScripts.Length);
            for (int i = 0; i < _validScripts.Length; i++)
            {
                Assert.IsTrue(new FileInfo(_validScripts[i]).FullName.Equals(openedScripts[i]));
            }
        }

        [TestMethod]
        protected void TestRestore()
        {
            byte[] originalData = File.ReadAllBytes(_validScripts[0]);
            TR23ScriptManager sm = TRGameflowEditor.Instance.GetScriptManager(_validScripts[0]) as TR23ScriptManager;
            File.Move(_validScripts[0], _validScripts[0] + ".bak");
            try
            {
                sm.Restore();
                Assert.IsTrue(File.Exists(_validScripts[0]));
                CollectionAssert.AreEqual(originalData, File.ReadAllBytes(_validScripts[0]));
            }
            finally
            {
                File.Delete(_validScripts[0]);
                File.Move(_validScripts[0] + ".bak", _validScripts[0]);
            }
        }
    }
}