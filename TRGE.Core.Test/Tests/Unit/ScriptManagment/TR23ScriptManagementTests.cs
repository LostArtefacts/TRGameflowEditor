using Microsoft.VisualStudio.TestTools.UnitTesting;
using TRGE.Coord;

namespace TRGE.Core.Test;

[TestClass]
public class TR23ScriptManagementTests : BaseTestCollection
{
    [TestMethod]
    [TestSequence(0)]
    protected void TestCreateScriptManager()
    {
        foreach (string scriptFile in _validScripts)
        {
            TestCreateScriptManager(new FileInfo(scriptFile));
        }
    }

    private static void TestCreateScriptManager(FileInfo file)
    {
        AbstractTRScriptEditor scriptMan = TRCoord.Instance.Open(file).ScriptEditor;
        Assert.IsFalse(scriptMan == null);
        Assert.IsTrue(scriptMan is TR23ScriptEditor);
        Assert.IsTrue(scriptMan.OriginalFile.FullName.Equals(file.FullName));
    }

    [TestMethod]
    protected void TestBackup()
    {
        TR23ScriptEditor sm = TRCoord.Instance.Open(_validScripts[0]).ScriptEditor as TR23ScriptEditor;
        Assert.IsFalse(sm.BackupFile == null);
        Assert.IsTrue(sm.BackupFile.Exists);
        try
        {
            AbstractTRScript script = TRScriptFactory.OpenScript(sm.BackupFile);
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
        TRCoord.Instance.ClearHistory();

        List<FileInfo> openedScripts = new();
        TRCoord.Instance.HistoryAdded += delegate (object sender, TRHistoryEventArgs e)
        {
            openedScripts.Add(new FileInfo(e.Path));
        };

        foreach (string scriptFile in _validScripts)
        {
            TRCoord.Instance.Open(scriptFile);
        }

        Assert.IsTrue(openedScripts.Count == _validScripts.Length);
        for (int i = 0; i < _validScripts.Length; i++)
        {
            Assert.IsTrue(new FileInfo(_validScripts[i]).FullName.Equals(openedScripts[i].FullName));
        }
    }

    [TestMethod]
    protected void TestRestore()
    {
        byte[] originalData = File.ReadAllBytes(_validScripts[0]);
        TREditor editor = TRCoord.Instance.Open(_validScripts[0]);
        //TR23ScriptManager sm = editor.ScriptManager as TR23ScriptManager;
        File.Move(_validScripts[0], _validScripts[0] + ".bak");
        try
        {
            editor.Restore();
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