using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using TRGE.Coord;
using TRGE.Core;

namespace TRGE.Test
{
    [TestClass]
    public class TR1Tests
    {
        [TestMethod]
        public void TestDetectTomb1Main()
        {
            string dir = @"TR1\Tomb1MainTest\Data";
            TREditor editor = TRCoord.Instance.Open(dir, TRScriptOpenOption.DiscardBackup);
            Assert.IsTrue(editor.ScriptEditor.Edition.IsCommunityPatch);
        }

        [TestMethod]
        public void TestDetectTR1ATI()
        {
            string dir = @"TR1\TR1ATITest\Data";
            TREditor editor = TRCoord.Instance.Open(dir, TRScriptOpenOption.DiscardBackup);
            Assert.IsFalse(editor.ScriptEditor.Edition.IsCommunityPatch);
        }

        [TestMethod]
        public void TestATIConversionToTomb1Main()
        {
            string dir = @"TR1\TR1ATITest\Data";
            TREditor editor = TRCoord.Instance.Open(dir, TRScriptOpenOption.DiscardBackup);
            editor.Save();
            Assert.IsTrue(editor.ScriptEditor.Edition.IsCommunityPatch);
            
        }

        [TestMethod]
        public void TestTomb1MainRead()
        {
            string dir = @"TR1\Tomb1Main\Data";
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
            scriptEd.EnableCheats = true;

            editor.Save();
        }

        [TestMethod]
        public void TestTomb1MainRestore()
        {
            string dir = @"TR1\Tomb1Main\Data";
            TREditor editor = TRCoord.Instance.Open(dir, TRScriptOpenOption.DiscardBackup);
            editor.Restore();
        }

        [TestMethod]
        public void TestATIRead()
        {
            string dir = @"TR1\ATI\Data";
            TREditor editor = TRCoord.Instance.Open(dir, TRScriptOpenOption.DiscardBackup);
            editor.Save();
        }

        [TestMethod]
        public void TestATIRestore()
        {
            string dir = @"TR1\ATI\Data";
            TREditor editor = TRCoord.Instance.Open(dir, TRScriptOpenOption.DiscardBackup);
            editor.Restore();
        }

        [TestMethod]
        public void TestConfig()
        {
            const string gf = @"TR1\cfgonly\Tomb1Main_gameflow.json5";
            const string cf = @"TR1\cfgonly\Tomb1Main.json5";
            const string gfTest = @"TR1\cfgonly\Tomb1Main_gameflow_TEST.json5";
            const string cfTest = @"TR1\cfgonly\Tomb1Main_TEST.json5";

            JObject defaultGameflow = JObject.Parse(File.ReadAllText(gf));
            string defaultSerializedGameflow = JsonConvert.SerializeObject(defaultGameflow, Formatting.Indented);

            JObject defaultConfig = JObject.Parse(File.ReadAllText(cf));
            string defaultSerializedConfig = JsonConvert.SerializeObject(defaultConfig, Formatting.Indented);

            TR1Script script = TRScriptFactory.OpenScript(gf) as TR1Script;
            script.ReadConfig(cf);

            script.Write(gfTest);
            script.WriteConfig(cfTest);

            JObject savedGameflow = JObject.Parse(File.ReadAllText(gfTest));
            string savedSerializedGameflow = JsonConvert.SerializeObject(savedGameflow, Formatting.Indented);

            File.WriteAllText("gf1.json", defaultSerializedGameflow);
            File.WriteAllText("gf2.json", savedSerializedGameflow);
            Assert.AreEqual(defaultSerializedGameflow, savedSerializedGameflow);

            JObject savedConfig = JObject.Parse(File.ReadAllText(cfTest));
            string savedSerializedConfig = JsonConvert.SerializeObject(savedConfig, Formatting.Indented);

            File.WriteAllText("cf1.json", defaultSerializedConfig);
            File.WriteAllText("cf2.json", savedSerializedConfig);
            Assert.AreEqual(defaultSerializedConfig, savedSerializedConfig);
        }
    }
}