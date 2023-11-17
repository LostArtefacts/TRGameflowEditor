using Microsoft.VisualStudio.TestTools.UnitTesting;
using TRGE.Coord;

namespace TRGE.Core.Test
{
    [TestClass]
    public class TR23MiscTests : BaseTestCollection
    {
        [TestMethod]
        protected void TestDemoTime()
        {
            foreach (string scriptFile in _validScripts)
            {
                TestDemoTime(TRScriptFactory.OpenScript(scriptFile) as TR23Script);
            }
        }

        private void TestDemoTime(TR23Script script)
        {
            uint newDemoTime = script.DemoTimeSeconds + 300;
            script.DemoTimeSeconds = newDemoTime;
            Assert.IsTrue(script.DemoTimeSeconds == newDemoTime);
            Assert.IsTrue(SaveAndReload(script).DemoTimeSeconds == newDemoTime);
        }

        [TestMethod]
        protected void TestDemoLevels()
        {
            foreach (string scriptFile in _validScripts)
            {
                TestDemoLevels(TRScriptFactory.OpenScript(scriptFile) as TR23Script);
                TestDisableDemos(scriptFile);
            }
        }

        private void TestDemoLevels(TR23Script script)
        {
            List<AbstractTRScriptedLevel> demos = script.DemoLevels;
            List<AbstractTRScriptedLevel> levels = script.Levels;
            levels.Insert(0, script.AssaultLevel);

            script.DemoLevels = null;
            Assert.AreEqual(script.NumDemoLevels, 0);
            Assert.AreEqual(script.NumPlayableLevels, levels.Count);

            TR23Script reloadedScript = SaveAndReload(script);
            Assert.AreEqual(reloadedScript.NumDemoLevels, 0);
            Assert.AreEqual(reloadedScript.NumPlayableLevels, levels.Count);
        }

        private void TestDisableDemos(string scriptFile)
        {
            TREditor editor = TRCoord.Instance.Open(scriptFile);
            TR23ScriptEditor scriptEditor = editor.ScriptEditor as TR23ScriptEditor;
            scriptEditor.DemosEnabled = false;
            editor.Save();

            scriptEditor = TRCoord.Instance.Open(scriptFile).ScriptEditor as TR23ScriptEditor;
            Assert.IsFalse(scriptEditor.DemosEnabled);
            Assert.AreEqual((scriptEditor.Script as TR23Script).NumDemoLevels, 0);
        }

        [TestMethod]
        protected void TestLevelIDs()
        {
            string id = AbstractTRScriptedLevel.CreateID("wall.TR2");
            Assert.AreEqual(id, AbstractTRScriptedLevel.CreateID("wall.PSX"));
            Assert.AreEqual(id, AbstractTRScriptedLevel.CreateID("WALL.psx"));
            Assert.AreEqual(id, AbstractTRScriptedLevel.CreateID(@"data\wall.TR2"));
            Assert.AreEqual(id, AbstractTRScriptedLevel.CreateID(@"C:\Program Files (x86)\etc\data\wall.TR2"));
            Assert.AreEqual(id, AbstractTRScriptedLevel.CreateID(@"..\data\wall.TR2"));
            Assert.AreEqual(id, AbstractTRScriptedLevel.CreateID("data/wall.TR2"));
            Assert.AreNotEqual(id, AbstractTRScriptedLevel.CreateID("boat.TR2"));
        }
    }
}