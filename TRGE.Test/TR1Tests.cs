using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TRGE.Coord;
using TRGE.Core;

namespace TRGE.Test;

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
    public void TestTomb1MainLevelCount()
    {
        string dir = @"TR1\Tomb1Main\Data";
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
        scriptEd.EnableCheats = true;

        editor.Save();
    }

    [TestMethod]
    public void TestTomb1MainMediless()
    {
        string dir = @"TR1\Tomb1Main\Data";
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
        const string cfEmpty = @"TR1\cfgonly\Tomb1Main_empty.json5";
        const string cfFuture = @"TR1\cfgonly\Tomb1Main_future.json5";
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

        script = TRScriptFactory.OpenScript(gf) as TR1Script;
        script.ReadConfig(cfEmpty);

        script.Write(gfTest);
        script.WriteConfig(cfTest);

        savedGameflow = JObject.Parse(File.ReadAllText(gfTest));
        savedSerializedGameflow = JsonConvert.SerializeObject(savedGameflow, Formatting.Indented);

        File.WriteAllText("gf1_empty.json", defaultSerializedGameflow);
        File.WriteAllText("gf2_empty.json", savedSerializedGameflow);
        Assert.AreEqual(defaultSerializedGameflow, savedSerializedGameflow);

        savedConfig = JObject.Parse(File.ReadAllText(cfTest));
        savedSerializedConfig = JsonConvert.SerializeObject(savedConfig, Formatting.Indented);

        File.WriteAllText("cf1_empty.json", defaultSerializedConfig);
        File.WriteAllText("cf2_empty.json", savedSerializedConfig);
        Assert.AreEqual(defaultSerializedConfig, savedSerializedConfig);

        defaultConfig = JObject.Parse(File.ReadAllText(cfFuture));
        defaultSerializedConfig = JsonConvert.SerializeObject(defaultConfig, Formatting.Indented);

        script = TRScriptFactory.OpenScript(gf) as TR1Script;
        script.ReadConfig(cfFuture);

        script.Write(gfTest);
        script.WriteConfig(cfTest);

        savedGameflow = JObject.Parse(File.ReadAllText(gfTest));
        savedSerializedGameflow = JsonConvert.SerializeObject(savedGameflow, Formatting.Indented);

        File.WriteAllText("gf1_future.json", defaultSerializedGameflow);
        File.WriteAllText("gf2_future.json", savedSerializedGameflow);
        Assert.AreEqual(defaultSerializedGameflow, savedSerializedGameflow);

        savedConfig = JObject.Parse(File.ReadAllText(cfTest));
        savedSerializedConfig = JsonConvert.SerializeObject(savedConfig, Formatting.Indented);

        File.WriteAllText("cf1_future.json", defaultSerializedConfig);
        File.WriteAllText("cf2_future.json", savedSerializedConfig);
        Assert.AreEqual(defaultSerializedConfig, savedSerializedConfig);
    }
}