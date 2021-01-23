using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
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
                _expectedLevels.Add(new TR23ScriptedLevel
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
            RandomGenerator rng = new RandomGenerator(RandomGenerator.Type.Date);
            List<AbstractTRScriptedLevel> expectedLevels = _expectedLevels.RandomSelection(rng.Create(), 2);

            TR23ScriptManager sm = TRCoord.Instance.OpenScript(_validScripts[ScriptFileIndex]) as TR23ScriptManager;
            sm.AmmolessLevelOrganisation = Organisation.Random;
            sm.AmmolessLevelRNG = rng;
            sm.RandomAmmolessLevelCount = 2;
            sm.RandomiseAmmolessLevels();
            List<TR23ScriptedLevel> levels = sm.GetAmmolessLevels();
            CollectionAssert.AreEquivalent(levels, expectedLevels);
        }

        [TestMethod]
        protected void TestRandomiseUnarmedLevels()
        {
            InitialiseLevels();
            RandomGenerator rng = new RandomGenerator(RandomGenerator.Type.Date);
            List<AbstractTRScriptedLevel> expectedLevels = _expectedLevels.RandomSelection(rng.Create(), 2);

            TR23ScriptManager sm = TRCoord.Instance.OpenScript(_validScripts[ScriptFileIndex]) as TR23ScriptManager;
            sm.UnarmedLevelOrganisation = Organisation.Random;
            sm.UnarmedLevelRNG = rng;
            sm.RandomUnarmedLevelCount = 2;
            sm.RandomiseUnarmedLevels();
            List<TR23ScriptedLevel> levels = sm.GetUnarmedLevels();
            CollectionAssert.AreEquivalent(levels, expectedLevels);
        }

        [TestMethod]
        protected void TestReorganiseAmmolessLevels()
        {
            InitialiseLevels();
            List<AbstractTRScriptedLevel> expectedLevels = new List<AbstractTRScriptedLevel>
            {
                _expectedLevels[0],
                _expectedLevels[1]
            };

            TR23ScriptManager sm = TRCoord.Instance.OpenScript(_validScripts[ScriptFileIndex]) as TR23ScriptManager;
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
            List<AbstractTRScriptedLevel> expectedLevels = new List<AbstractTRScriptedLevel>
            {
                _expectedLevels[0],
                _expectedLevels[1]
            };

            TR23ScriptManager sm = TRCoord.Instance.OpenScript(_validScripts[ScriptFileIndex]) as TR23ScriptManager;
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