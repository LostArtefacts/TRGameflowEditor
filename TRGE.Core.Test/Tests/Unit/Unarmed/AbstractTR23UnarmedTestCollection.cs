using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace TRGE.Core.Test
{
    public abstract class AbstractTR23UnarmedTestCollection : BaseTestCollection
    {
        protected abstract int ScriptFileIndex { get; }
        protected abstract TREdition Edition { get; }
        List<AbstractTRLevel> _expectedLevels;
        protected abstract string[] LevelNames { get; }
        protected abstract string[] LevelFileNames { get; }

        protected void InitialiseLevels()
        {
            _expectedLevels = new List<AbstractTRLevel>();
            for (int i = 0; i < LevelNames.Length; i++)
            {
                _expectedLevels.Add(new TR23Level
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
            List<AbstractTRLevel> expectedLevels = _expectedLevels.RandomSelection(rng.Create(), 2);

            TR23ScriptManager sm = TRGameflowEditor.Instance.GetScriptManager(_validScripts[ScriptFileIndex]) as TR23ScriptManager;
            try
            {
                sm.AmmolessLevelOrganisation = Organisation.Random;
                sm.AmmolessLevelRNG = rng;
                sm.RandomAmmolessLevelCount = 2;
                sm.RandomiseAmmolessLevels();
                List<TR23Level> levels = sm.GetAmmolessLevels();
                CollectionAssert.AreEquivalent(levels, expectedLevels);
            }
            finally
            {
                TRGameflowEditor.Instance.CloseScriptManager(sm);
            }
        }

        [TestMethod]
        protected void TestRandomiseUnarmedLevels()
        {
            InitialiseLevels();
            RandomGenerator rng = new RandomGenerator(RandomGenerator.Type.Date);
            List<AbstractTRLevel> expectedLevels = _expectedLevels.RandomSelection(rng.Create(), 2);

            TR23ScriptManager sm = TRGameflowEditor.Instance.GetScriptManager(_validScripts[ScriptFileIndex]) as TR23ScriptManager;
            try
            {
                sm.UnarmedLevelOrganisation = Organisation.Random;
                sm.UnarmedLevelRNG = rng;
                sm.RandomUnarmedLevelCount = 2;
                sm.RandomiseUnarmedLevels();
                List<TR23Level> levels = sm.GetUnarmedLevels();
                CollectionAssert.AreEquivalent(levels, expectedLevels);
            }
            finally
            {
                TRGameflowEditor.Instance.CloseScriptManager(sm);
            }
        }

        [TestMethod]
        protected void TestReorganiseAmmolessLevels()
        {
            InitialiseLevels();
            List<AbstractTRLevel> expectedLevels = new List<AbstractTRLevel>
            {
                _expectedLevels[0],
                _expectedLevels[1]
            };

            TR23ScriptManager sm = TRGameflowEditor.Instance.GetScriptManager(_validScripts[ScriptFileIndex]) as TR23ScriptManager;
            try
            {
                List<MutableTuple<string, string, bool>> ammolessLevelData = sm.AmmolessLevelData;
                for (int i = 0; i < ammolessLevelData.Count; i++)
                {
                    ammolessLevelData[i].Item3 = i < 2;
                }

                sm.AmmolessLevelData = ammolessLevelData;
                CollectionAssert.AreEquivalent(sm.GetAmmolessLevels(), expectedLevels);
            }
            finally
            {
                TRGameflowEditor.Instance.CloseScriptManager(sm);
            }
        }

        [TestMethod]
        protected void TestReorganiseUnarmedLevels()
        {
            InitialiseLevels();
            List<AbstractTRLevel> expectedLevels = new List<AbstractTRLevel>
            {
                _expectedLevels[0],
                _expectedLevels[1]
            };

            TR23ScriptManager sm = TRGameflowEditor.Instance.GetScriptManager(_validScripts[ScriptFileIndex]) as TR23ScriptManager;
            try
            {
                List<MutableTuple<string, string, bool>> unarmedLevelData = sm.UnarmedLevelData;
                for (int i = 0; i < unarmedLevelData.Count; i++)
                {
                    unarmedLevelData[i].Item3 = i < 2;
                }

                sm.UnarmedLevelData = unarmedLevelData;
                CollectionAssert.AreEquivalent(sm.GetUnarmedLevels(), expectedLevels);
            }
            finally
            {
                TRGameflowEditor.Instance.CloseScriptManager(sm);
            }
        }
    }
}