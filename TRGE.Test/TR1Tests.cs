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
        public void TestRead()
        {
            string dir = @"Defaults\TestGame\Data";
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
        public void TestConfig()
        {
            const string gf = @"Defaults\cfg\Tomb1Main_gameflow.json5";
            const string cf = @"Defaults\cfg\Tomb1Main.json5";
            const string gfTest = @"Defaults\cfg\Tomb1Main_gameflow_TEST.json5";
            const string cfTest = @"Defaults\cfg\Tomb1Main_TEST.json5";

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