using Microsoft.VisualStudio.TestTools.UnitTesting;
using TRGE.Coord;
using TRGE.Core;

namespace TRGE.Test;

[TestClass]
public class TR1Tests
{
    [TestMethod]
    public void TestDetectTomb1Main()
    {
        string dir = "TR1/Tomb1MainTest/Data";
        TREditor editor = TRCoord.Instance.Open(dir, TRScriptOpenOption.DiscardBackup);
        Assert.IsTrue(editor.ScriptEditor.Edition.IsCommunityPatch);
    }

    [TestMethod]
    public void TestDetectTR1ATI()
    {
        string dir = "TR1/TR1ATITest/Data";
        TREditor editor = TRCoord.Instance.Open(dir, TRScriptOpenOption.DiscardBackup);
        Assert.IsFalse(editor.ScriptEditor.Edition.IsCommunityPatch);
    }

    [TestMethod]
    public void TestATIConversionToTomb1Main()
    {
        string dir = "TR1/TR1ATITest/Data";
        TREditor editor = TRCoord.Instance.Open(dir, TRScriptOpenOption.DiscardBackup);
        editor.Save();
        Assert.IsTrue(editor.ScriptEditor.Edition.IsCommunityPatch);
        
    }

    [TestMethod]
    public void TestTomb1MainRead()
    {
        string dir = "TR1/Tomb1Main/Data";
        TREditor editor = TRCoord.Instance.Open(dir, TRScriptOpenOption.DiscardBackup);

        editor.ScriptEditor.LevelSequencingOrganisation = Organisation.Random;
        editor.ScriptEditor.LevelSequencingRNG = new RandomGenerator(20220731);

        editor.ScriptEditor.GameTrackOrganisation = Organisation.Random;
        editor.ScriptEditor.GameTrackRNG = new RandomGenerator(20220731);

        TR1ScriptEditor scriptEd = editor.ScriptEditor as TR1ScriptEditor;
        scriptEd.UnarmedLevelOrganisation = Organisation.Random;
        scriptEd.RandomUnarmedLevelCount = 2;
        scriptEd.UnarmedLevelRNG = new RandomGenerator(20220731);

        scriptEd.DemosEnabled = false;

        editor.Save();
    }

    [TestMethod]
    public void TestTomb1MainLevelCount()
    {
        string dir = "TR1/Tomb1Main/Data";
        TREditor editor = TRCoord.Instance.Open(dir, TRScriptOpenOption.DiscardBackup);
        TRInterop.RandomisationSupported = true;

        editor.ScriptEditor.EnabledLevelOrganisation = Organisation.Random;
        editor.ScriptEditor.EnabledLevelRNG = new RandomGenerator(20220731);
        editor.ScriptEditor.RandomEnabledLevelCount = 5;

        TR1ScriptEditor scriptEd = editor.ScriptEditor as TR1ScriptEditor;
        scriptEd.UnarmedLevelOrganisation = Organisation.Random;
        scriptEd.RandomUnarmedLevelCount = 2;
        scriptEd.UnarmedLevelRNG = new RandomGenerator(20220731);

        scriptEd.DemosEnabled = false;

        editor.Save();
    }

    [TestMethod]
    public void TestTomb1MainMediless()
    {
        string dir = "TR1/Tomb1Main/Data";
        TREditor editor = TRCoord.Instance.Open(dir, TRScriptOpenOption.DiscardBackup);
        TRInterop.RandomisationSupported = true;

        editor.ScriptEditor.EnabledLevelOrganisation = Organisation.Random;
        editor.ScriptEditor.EnabledLevelRNG = new RandomGenerator(20220731);
        editor.ScriptEditor.RandomEnabledLevelCount = 5;

        TR1ScriptEditor scriptEd = editor.ScriptEditor as TR1ScriptEditor;
        scriptEd.MedilessLevelOrganisation = Organisation.Random;
        scriptEd.RandomMedilessLevelCount = 3;
        scriptEd.MedilessLevelRNG = new RandomGenerator(20220731);

        scriptEd.AmmolessLevelOrganisation = Organisation.Random;
        scriptEd.RandomAmmolessLevelCount = 3;
        scriptEd.AmmolessLevelRNG = new RandomGenerator(20220805);

        scriptEd.UnarmedLevelOrganisation = Organisation.Random;
        scriptEd.RandomUnarmedLevelCount = 2;
        scriptEd.UnarmedLevelRNG = new RandomGenerator(20220731);

        scriptEd.DemosEnabled = false;

        editor.Save();
    }

    [TestMethod]
    public void TestTomb1MainRestore()
    {
        string dir = "TR1/Tomb1Main/Data";
        TREditor editor = TRCoord.Instance.Open(dir, TRScriptOpenOption.DiscardBackup);
        editor.Restore();
    }

    [TestMethod]
    public void TestATIRead()
    {
        string dir = "TR1/ATI/Data";
        TREditor editor = TRCoord.Instance.Open(dir, TRScriptOpenOption.DiscardBackup);
        editor.Save();
    }

    [TestMethod]
    public void TestATIRestore()
    {
        string dir = "TR1/ATI/Data";
        TREditor editor = TRCoord.Instance.Open(dir, TRScriptOpenOption.DiscardBackup);
        editor.Restore();
    }
}