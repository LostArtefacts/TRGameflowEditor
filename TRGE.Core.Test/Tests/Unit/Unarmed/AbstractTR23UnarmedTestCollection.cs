using Microsoft.VisualStudio.TestTools.UnitTesting;
using TRGE.Coord;

namespace TRGE.Core.Test
{
    public abstract class AbstractTR23UnarmedTestCollection : BaseTestCollection
    {
        protected abstract int ScriptFileIndex { get; }
        protected abstract TREdition Edition { get; }
        List<AbstractTRScriptedLevel> _expectedLevels;
        protected abstract string[] LevelNames { get; }
        protected abstract string[] LevelFileNames { get; }

        protected void InitialiseLevels()
        {
            _expectedLevels = new List<AbstractTRScriptedLevel>();
            for (int i = 0; i < LevelNames.Length; i++)
            {
                _expectedLevels.Add(new TR2ScriptedLevel
                {
                    Name = LevelNames[i],
                    LevelFile = LevelFileNames[i]
                });
            }
        }

        [TestMethod]
        protected void TestRandomiseAmmolessLevels()
        {
            InitialiseLevels();
            RandomGenerator rng = new(RandomGenerator.Type.Date);
            List<AbstractTRScriptedLevel> expectedLevels = _expectedLevels.RandomSelection(rng.Create(), 2);

            TR23ScriptEditor sm = TRCoord.Instance.Open(_validScripts[ScriptFileIndex]).ScriptEditor as TR23ScriptEditor;
            sm.AmmolessLevelOrganisation = Organisation.Random;
            sm.AmmolessLevelRNG = rng;
            sm.RandomAmmolessLevelCount = 2;
            sm.RandomiseAmmolessLevels();
            List<TR2ScriptedLevel> levels = sm.GetAmmolessLevels();
            CollectionAssert.AreEquivalent(levels, expectedLevels);
        }

        [TestMethod]
        protected void TestRandomiseUnarmedLevels()
        {
            InitialiseLevels();
            RandomGenerator rng = new(RandomGenerator.Type.Date);
            List<AbstractTRScriptedLevel> expectedLevels = _expectedLevels.RandomSelection(rng.Create(), 2);

            TR23ScriptEditor sm = TRCoord.Instance.Open(_validScripts[ScriptFileIndex]).ScriptEditor as TR23ScriptEditor;
            sm.UnarmedLevelOrganisation = Organisation.Random;
            sm.UnarmedLevelRNG = rng;
            sm.RandomUnarmedLevelCount = 2;
            sm.RandomiseUnarmedLevels();
            List<TR2ScriptedLevel> levels = sm.GetUnarmedLevels();
            CollectionAssert.AreEquivalent(levels, expectedLevels);
        }

        [TestMethod]
        protected void TestReorganiseAmmolessLevels()
        {
            InitialiseLevels();
            List<AbstractTRScriptedLevel> expectedLevels = new()
            {
                _expectedLevels[0],
                _expectedLevels[1]
            };

            TR23ScriptEditor sm = TRCoord.Instance.Open(_validScripts[ScriptFileIndex]).ScriptEditor as TR23ScriptEditor;
            List<MutableTuple<string, string, bool>> ammolessLevelData = sm.AmmolessLevelData;
            for (int i = 0; i < ammolessLevelData.Count; i++)
            {
                ammolessLevelData[i].Item3 = i < 2;
            }
            
            sm.AmmolessLevelData = ammolessLevelData;
            CollectionAssert.AreEquivalent(sm.GetAmmolessLevels(), expectedLevels);
        }

        [TestMethod]
        protected void TestReorganiseUnarmedLevels()
        {
            InitialiseLevels();
            List<AbstractTRScriptedLevel> expectedLevels = new()
            {
                _expectedLevels[0],
                _expectedLevels[1]
            };

            TR23ScriptEditor sm = TRCoord.Instance.Open(_validScripts[ScriptFileIndex]).ScriptEditor as TR23ScriptEditor;
            List<MutableTuple<string, string, bool>> unarmedLevelData = sm.UnarmedLevelData;
            for (int i = 0; i < unarmedLevelData.Count; i++)
            {
                unarmedLevelData[i].Item3 = i < 2;
            }

            sm.UnarmedLevelData = unarmedLevelData;
            CollectionAssert.AreEquivalent(sm.GetUnarmedLevels(), expectedLevels);
        }
    }
}