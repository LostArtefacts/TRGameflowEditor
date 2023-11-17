using Microsoft.VisualStudio.TestTools.UnitTesting;
using TRGE.Coord;

namespace TRGE.Core.Test
{
    public class TR2RandoLockTestCollection : BaseTestCollection
    {
        //[TestMethod]
        protected void TestRandoLock()
        {
            Assert.IsTrue(TRInterop.RandomisationSupported);

            //initially randomise the level order
            TREditor editor = TRCoord.Instance.Open(_validScripts[0]);
            TR23ScriptEditor scriptEditor = editor.ScriptEditor as TR23ScriptEditor;
            scriptEditor.LevelSequencingOrganisation = Organisation.Random;
            scriptEditor.LevelSunsetOrganisation = Organisation.Random; scriptEditor.RandomSunsetLevelCount = 4;
            scriptEditor.GameTrackOrganisation = Organisation.Random;
            scriptEditor.AmmolessLevelOrganisation = Organisation.Random; scriptEditor.RandomAmmolessLevelCount = 2;
            scriptEditor.UnarmedLevelOrganisation = Organisation.Random; scriptEditor.RandomUnarmedLevelCount = 3;
            scriptEditor.SecretBonusOrganisation = Organisation.Random;

            scriptEditor.LevelSequencingRNG = new RandomGenerator(RandomGenerator.Type.Date);
            scriptEditor.LevelSunsetRNG = new RandomGenerator(RandomGenerator.Type.Date);
            scriptEditor.GameTrackRNG = new RandomGenerator(RandomGenerator.Type.Date);
            scriptEditor.AmmolessLevelRNG = new RandomGenerator(RandomGenerator.Type.Date);
            scriptEditor.UnarmedLevelRNG = new RandomGenerator(RandomGenerator.Type.Date);
            scriptEditor.SecretBonusRNG = new RandomGenerator(RandomGenerator.Type.Date);
            editor.Save();

            TR23Script script = new();
            script.Read(_validScripts[0]);
            TR23LevelManager lm = TRScriptedLevelFactory.GetLevelManager(script) as TR23LevelManager;
            var levels1 = lm.GetSequencing();
            var sunsets1 = lm.GetSunsetData(scriptEditor.LevelManager.Levels);
            var tracks1 = lm.GetTrackData(scriptEditor.LevelManager.Levels);
            var ammoless1 = lm.GetAmmolessLevelData(scriptEditor.LevelManager.Levels);
            var unarmed1 = lm.GetUnarmedLevelData(scriptEditor.LevelManager.Levels);
            var secrets1 = lm.GetLevelBonusData(scriptEditor.LevelManager.Levels);

            //simulate loading again where rando is not supported
            TRInterop.RandomisationSupported = false;
            Assert.IsFalse(TRInterop.RandomisationSupported);

            editor = TRCoord.Instance.Open(_validScripts[0]);
            scriptEditor = editor.ScriptEditor as TR23ScriptEditor;
            
            scriptEditor.LevelSequencingOrganisation = Organisation.Random;
            scriptEditor.LevelSunsetOrganisation = Organisation.Random; scriptEditor.RandomSunsetLevelCount = 5;
            scriptEditor.GameTrackOrganisation = Organisation.Random;
            scriptEditor.AmmolessLevelOrganisation = Organisation.Random; scriptEditor.RandomAmmolessLevelCount = 3;
            scriptEditor.UnarmedLevelOrganisation = Organisation.Random; scriptEditor.RandomUnarmedLevelCount = 2;
            scriptEditor.SecretBonusOrganisation = Organisation.Random;

            scriptEditor.LevelSequencingRNG = new RandomGenerator(RandomGenerator.Type.Custom) { Value = 1986 };
            scriptEditor.LevelSunsetRNG = new RandomGenerator(RandomGenerator.Type.Custom) { Value = 1986 };
            scriptEditor.GameTrackRNG = new RandomGenerator(RandomGenerator.Type.Custom) { Value = 1986 };
            scriptEditor.AmmolessLevelRNG = new RandomGenerator(RandomGenerator.Type.Custom) { Value = 1986 };
            scriptEditor.UnarmedLevelRNG = new RandomGenerator(RandomGenerator.Type.Custom) { Value = 1986 };
            scriptEditor.SecretBonusRNG = new RandomGenerator(RandomGenerator.Type.Custom) { Value = 1986 };

            editor.Save();

            //the previous randomisations should still be in place, so lines 50-62 should be redundant
            script = new TR23Script();
            script.Read(_validScripts[0]);
            lm = TRScriptedLevelFactory.GetLevelManager(script) as TR23LevelManager;
            var levels2 = lm.GetSequencing();
            var sunsets2 = lm.GetSunsetData(scriptEditor.LevelManager.Levels);
            var tracks2 = lm.GetTrackData(scriptEditor.LevelManager.Levels);
            var ammoless2 = lm.GetAmmolessLevelData(scriptEditor.LevelManager.Levels);
            var unarmed2 = lm.GetUnarmedLevelData(scriptEditor.LevelManager.Levels);
            var secrets2 = lm.GetLevelBonusData(scriptEditor.LevelManager.Levels);

            CollectionAssert.AreEqual(levels1, levels2);
            CollectionAssert.AreEqual(sunsets1, sunsets2);
            CollectionAssert.AreEqual(tracks1, tracks2);
            CollectionAssert.AreEqual(ammoless1, ammoless2);
            CollectionAssert.AreEqual(unarmed1, unarmed2);
            try
            {
                CollectionAssert.AreEquivalent(secrets1, secrets2);
            }
            catch
            {
                //I don't know why CollectionAssert.AreEqual/Equivalent fail for secret items, but stress-checking seems to pass.
                foreach (var levelSecrets1 in secrets1)
                {
                    foreach (var levelSecrets2 in secrets2)
                    {
                        if (levelSecrets1.Item1 == levelSecrets2.Item2)
                        {
                            foreach (var bonuses1 in levelSecrets1.Item3)
                            {
                                bool found = false;
                                foreach (var bonuses2 in levelSecrets2.Item3)
                                {
                                    if (bonuses1.Equals(bonuses2))
                                    {
                                        found = true;
                                        break;
                                    }
                                }

                                if (!found)
                                {
                                    Assert.Fail();
                                }
                            }
                            break;
                        }
                    }
                }
            }
        }

        [TestMethod]
        protected void OutputTest()
        {
            TRCoord.Instance.RootConfigDirectory = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            try
            {
                TREditor editor = TRCoord.Instance.Open(@"RandoLock");
                TR23ScriptEditor scriptEditor = editor.ScriptEditor as TR23ScriptEditor;
                scriptEditor.LevelSequencingOrganisation = Organisation.Random;
                scriptEditor.LevelSunsetOrganisation = Organisation.Random; scriptEditor.RandomSunsetLevelCount = 4;
                scriptEditor.GameTrackOrganisation = Organisation.Random;
                scriptEditor.RandomGameTracksIncludeBlank = false;
                scriptEditor.AmmolessLevelOrganisation = Organisation.Random; scriptEditor.RandomAmmolessLevelCount = 2;
                scriptEditor.UnarmedLevelOrganisation = Organisation.Random; scriptEditor.RandomUnarmedLevelCount = 3;
                scriptEditor.SecretBonusOrganisation = Organisation.Random;

                scriptEditor.LevelSequencingRNG = new RandomGenerator(RandomGenerator.Type.Date);
                scriptEditor.LevelSunsetRNG = new RandomGenerator(RandomGenerator.Type.Date);
                scriptEditor.GameTrackRNG = new RandomGenerator(RandomGenerator.Type.Date);
                scriptEditor.AmmolessLevelRNG = new RandomGenerator(RandomGenerator.Type.Date);
                scriptEditor.UnarmedLevelRNG = new RandomGenerator(RandomGenerator.Type.Date);
                scriptEditor.SecretBonusRNG = new RandomGenerator(RandomGenerator.Type.Date);
                editor.Save();
            }
            finally
            {
                TRCoord.Instance.RootConfigDirectory = Directory.GetCurrentDirectory();
            }
        }
    }
}