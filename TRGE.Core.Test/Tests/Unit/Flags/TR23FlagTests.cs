using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TRGE.Core.Test;

[TestClass]
public class TR23FlagTests : BaseTestCollection
{
    [TestMethod]
    protected void TestCheatFlag()
    {
        foreach (string scriptFile in _validScripts)
        {
            TestCheatFlag(TRScriptFactory.OpenScript(scriptFile) as TR23Script);
        }
    }

    private void TestCheatFlag(TR23Script script)
    {
        Assert.IsFalse(script.CheatsIgnored);
        script.CheatsIgnored = true;
        Assert.IsTrue(script.CheatsIgnored);
        Assert.IsTrue(SaveAndReload(script).CheatsIgnored);
    }

    [TestMethod]
    protected void TestDemosFlag()
    {
        foreach (string scriptFile in _validScripts)
        {
            TestDemosFlag(TRScriptFactory.OpenScript(scriptFile) as TR23Script);
        }
    }

    private void TestDemosFlag(TR23Script script)
    {
        Assert.IsFalse(script.DemosDisabled);
        script.DemosDisabled = true;
        Assert.IsTrue(script.DemosDisabled);
        Assert.IsTrue(SaveAndReload(script).DemosDisabled);
    }

    [TestMethod]
    protected void TestDemoModeFlag()
    {
        foreach (string scriptFile in _validScripts)
        {
            TestDemoModeFlag(TRScriptFactory.OpenScript(scriptFile) as TR23Script);
        }
    }

    private void TestDemoModeFlag(TR23Script script)
    {
        Assert.IsFalse(script.DemoVersion);
        script.DemoVersion = true;
        Assert.IsTrue(script.DemoVersion);
        Assert.IsTrue(SaveAndReload(script).DemoVersion);
    }

    [TestMethod]
    protected void TestDozyFlag()
    {
        foreach (string scriptFile in _validScripts)
        {
            TestDozyFlag(TRScriptFactory.OpenScript(scriptFile) as TR23Script);
        }
    }

    private void TestDozyFlag(TR23Script script)
    {
        Assert.IsFalse(script.DozyEnabled);
        script.DozyEnabled = true;
        if (script.DozyViable)
        {
            Assert.IsTrue(script.DozyEnabled);
            Assert.IsTrue(SaveAndReload(script).DozyEnabled);
        }
        else
        {
            Assert.IsFalse(script.DozyEnabled);
            Assert.IsFalse(SaveAndReload(script).DozyEnabled);
        }
    }

    [TestMethod]
    protected void TestGymFlag()
    {
        foreach (string scriptFile in _validScripts)
        {
            TestGymFlag(TRScriptFactory.OpenScript(scriptFile) as TR23Script);
        }
    }

    private void TestGymFlag(TR23Script script)
    {
        Assert.IsTrue(script.GymEnabled);
        script.GymEnabled = false;
        Assert.IsFalse(script.GymEnabled);
        Assert.IsFalse(SaveAndReload(script).GymEnabled);
    }

    [TestMethod]
    protected void TestLevelSelectionFlag()
    {
        foreach (string scriptFile in _validScripts)
        {
            TestLevelSelectionFlag(TRScriptFactory.OpenScript(scriptFile) as TR23Script);
        }
    }

    private void TestLevelSelectionFlag(TR23Script script)
    {
        Assert.IsFalse(script.LevelSelectEnabled);
        script.LevelSelectEnabled = true;
        Assert.IsTrue(script.LevelSelectEnabled);
        Assert.IsTrue(SaveAndReload(script).LevelSelectEnabled);
    }

    [TestMethod]
    protected void TestOptionRingFlag()
    {
        foreach (string scriptFile in _validScripts)
        {
            TestOptionRingFlag(TRScriptFactory.OpenScript(scriptFile) as TR23Script);
        }
    }

    private void TestOptionRingFlag(TR23Script script)
    {
        Assert.IsFalse(script.OptionRingDisabled);
        script.OptionRingDisabled = true;
        Assert.IsTrue(script.OptionRingDisabled);
        Assert.IsTrue(SaveAndReload(script).OptionRingDisabled);
    }

    [TestMethod]
    protected void TestSaveLoadFlag()
    {
        foreach (string scriptFile in _validScripts)
        {
            TestSaveLoadFlag(TRScriptFactory.OpenScript(scriptFile) as TR23Script);
        }
    }

    private void TestSaveLoadFlag(TR23Script script)
    {
        Assert.IsFalse(script.SaveLoadDisabled);
        script.SaveLoadDisabled = true;
        Assert.IsTrue(script.SaveLoadDisabled);
        Assert.IsTrue(SaveAndReload(script).SaveLoadDisabled);
    }

    [TestMethod]
    protected void TestScreensizingFlag()
    {
        foreach (string scriptFile in _validScripts)
        {
            TestScreensizingFlag(TRScriptFactory.OpenScript(scriptFile) as TR23Script);
        }
    }

    private void TestScreensizingFlag(TR23Script script)
    {
        Assert.IsFalse(script.ScreensizingDisabled);
        script.ScreensizingDisabled = true;
        Assert.IsTrue(script.ScreensizingDisabled);
        Assert.IsTrue(SaveAndReload(script).ScreensizingDisabled);
    }

    [TestMethod]
    protected void TestTitleScreenFlag()
    {
        foreach (string scriptFile in _validScripts)
        {
            TestTitleScreenFlag(TRScriptFactory.OpenScript(scriptFile) as TR23Script);
        }
    }

    private void TestTitleScreenFlag(TR23Script script)
    {
        string exitToTitle = script.GameStrings1[8];
        Assert.IsFalse(script.TitleDisabled);
        script.TitleDisabled = true;
        Assert.IsTrue(script.TitleDisabled);
        Assert.AreNotEqual(script.GameStrings1[8], exitToTitle);
        script = SaveAndReload(script);
        Assert.IsTrue(script.TitleDisabled);
        Assert.AreNotEqual(script.GameStrings1[8], exitToTitle);
        script.TitleDisabled = false;
        Assert.AreEqual(script.GameStrings1[8], exitToTitle);
    }
}