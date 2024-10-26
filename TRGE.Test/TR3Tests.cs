using Microsoft.VisualStudio.TestTools.UnitTesting;
using TRGE.Coord;
using TRGE.Core;

namespace TRGE.Test;

[TestClass]
public class TR3Tests
{
    [TestMethod]
    public void TestTR3OriginalVersion()
    {
        string dir = "TR3/Tomb3/Data";
        TREditor editor = TRCoord.Instance.Open(dir, TRScriptOpenOption.DiscardBackup);

        Assert.IsFalse(editor.ScriptEditor.Edition.IsCommunityPatch);
    }

    [TestMethod]
    public void TestTR3PatchVersion()
    {
        string dir = "TR3/Tomb3Main/Data";
        TREditor editor = TRCoord.Instance.Open(dir, TRScriptOpenOption.DiscardBackup);

        Assert.IsTrue(editor.ScriptEditor.Edition.IsCommunityPatch);
    }

    [TestMethod]
    public void TestWrite()
    {
        string dir = "TR3/Tomb3/Data";
        TREditor editor = TRCoord.Instance.Open(dir, TRScriptOpenOption.DiscardBackup);
        editor.Save();
    }

    [TestMethod]
    public void TestRestore()
    {
        string dir = "TR3/Tomb3/Data";
        TREditor editor = TRCoord.Instance.Open(dir, TRScriptOpenOption.DiscardBackup);
        editor.Restore();
    }
}