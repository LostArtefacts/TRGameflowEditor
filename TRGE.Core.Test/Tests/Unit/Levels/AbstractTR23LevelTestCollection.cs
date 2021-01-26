using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using TRGE.Coord;

namespace TRGE.Core.Test
{
    public abstract class AbstractTR23LevelTestCollection : BaseTestCollection
    {
        protected abstract int ScriptFileIndex { get; }
        List<AbstractTRScriptedLevel> _expectedLevels;
        protected abstract string[] LevelNames { get; }
        protected abstract string[] LevelFileNames { get; }
        protected abstract TREdition Edition { get; }

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
        protected virtual void TestLoadLevels()
        {
            InitialiseLevels();
            TR23ScriptEditor sm = TRCoord.Instance.Open(_validScripts[ScriptFileIndex]).ScriptEditor as TR23ScriptEditor;
            CollectionAssert.AreEqual(sm.LevelManager.Levels, _expectedLevels);
        }

        [TestMethod]
        protected void TestRandomiseLevels()
        {
            InitialiseLevels();
            RandomGenerator rng = new RandomGenerator(RandomGenerator.Type.Date);
            _expectedLevels.Randomise(rng.Create());

            TR23ScriptEditor sm = TRCoord.Instance.Open(_validScripts[ScriptFileIndex]).ScriptEditor as TR23ScriptEditor;
            string[] newLevelNames = new string[LevelNames.Length];
            sm.LevelManager.LevelModified += delegate (object sender, TRScriptedLevelEventArgs e)
            {
                newLevelNames[e.LevelSequence - 1] = e.LevelName;
            };
            sm.LevelOrganisation = Organisation.Random;
            sm.LevelRNG = rng;
            sm.RandomiseLevels();
            
            List<AbstractTRScriptedLevel> levels = sm.LevelManager.Levels;
            CollectionAssert.AreEqual(levels, _expectedLevels);
            TestForFinalLevel(levels, sm.Edition);

            CollectionAssert.AreNotEqual(newLevelNames, LevelNames);
        }

        [TestMethod]
        protected void TestReorganiseLevels()
        {
            InitialiseLevels();
            _expectedLevels.Reverse();
            TR23ScriptEditor sm = TRCoord.Instance.Open(_validScripts[ScriptFileIndex]).ScriptEditor as TR23ScriptEditor;
            List<Tuple<string, string>> levelSequencingData = sm.LevelSequencing;
            levelSequencingData.Reverse();
            sm.LevelSequencing = levelSequencingData;
            List<AbstractTRScriptedLevel> levels = sm.LevelManager.Levels;
            CollectionAssert.AreEqual(levels, _expectedLevels);
            TestForFinalLevel(levels, sm.Edition);
        }

        private void TestForFinalLevel(List<AbstractTRScriptedLevel> levels, TREdition edition)
        {
            int expectedIndex = levels.Count - edition.LevelCompleteOffset - 1;
            for (int i = 0; i < levels.Count; i++)
            {
                if (i == expectedIndex)
                {
                    Assert.IsTrue(levels[i].IsFinalLevel, string.Format("Level at index {0} is not marked as the final level", i));
                }
                else
                {
                    Assert.IsFalse(levels[i].IsFinalLevel, string.Format("Level at index {0} is marked as the final level", i));
                }
            }
        }
    }
}