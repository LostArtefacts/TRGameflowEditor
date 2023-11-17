using Microsoft.VisualStudio.TestTools.UnitTesting;
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
                _expectedLevels.Add(new TR2ScriptedLevel
                {
                    Name = LevelNames[i],
                    LevelFile = LevelFileNames[i]
                });
            }
        }

        [TestMethod]
        [TestSequence(0)]
        protected virtual void TestLoadLevels()
        {
            InitialiseLevels();
            TR23ScriptEditor sm = TRCoord.Instance.Open(_validScripts[ScriptFileIndex]).ScriptEditor as TR23ScriptEditor;
            CollectionAssert.AreEqual(sm.LevelManager.Levels, _expectedLevels);
        }

        [TestMethod]
        [TestSequence(1)]
        protected void TestRandomiseLevels()
        {
            InitialiseLevels();
            RandomGenerator rng = new(RandomGenerator.Type.Date);
            _expectedLevels.Randomise(rng.Create());

            TR23ScriptEditor sm = TRCoord.Instance.Open(_validScripts[ScriptFileIndex]).ScriptEditor as TR23ScriptEditor;
            string[] newLevelNames = new string[LevelNames.Length];
            sm.LevelManager.LevelModified += delegate (object sender, TRScriptedLevelEventArgs e)
            {
                newLevelNames[e.LevelSequence - 1] = e.LevelName;
            };
            sm.LevelSequencingOrganisation = Organisation.Random;
            sm.LevelSequencingRNG = rng;
            sm.RandomiseLevels();
            
            List<AbstractTRScriptedLevel> levels = sm.LevelManager.Levels;
            CollectionAssert.AreEqual(levels, _expectedLevels);
            TestForFinalLevel(levels, sm.Edition);

            CollectionAssert.AreNotEqual(newLevelNames, LevelNames);
        }

        [TestMethod]
        [TestSequence(2)]
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

        [TestMethod]
        [TestSequence(3)]
        protected void TestLevelCountChange()
        {
            TREditor editor = TRCoord.Instance.Open(_validScripts[ScriptFileIndex]);
            TR23ScriptEditor sm = editor.ScriptEditor as TR23ScriptEditor;
            sm.EnabledLevelOrganisation = Organisation.Manual;
            List<MutableTuple<string, string, bool>> status = sm.EnabledLevelStatus;
            status[1].Item3 = false;
            status[4].Item3 = false;
            //status[17].Item3 = false;
            sm.EnabledLevelStatus = status;
            editor.Save();

            editor = TRCoord.Instance.Open(_validScripts[ScriptFileIndex]);
            sm = editor.ScriptEditor as TR23ScriptEditor;
            List<MutableTuple<string, string, bool>> reloadedStatus = sm.EnabledLevelStatus;

            Assert.AreEqual(status.Count, reloadedStatus.Count);
            for (int i = 0; i < status.Count; i++)
            {
                Assert.AreEqual(status[i].Item3, reloadedStatus[i].Item3);
            }

            TestForFinalLevel(new List<AbstractTRScriptedLevel>(sm.EnabledScriptedLevels), sm.Edition);
        }

        [TestMethod]
        [TestSequence(4)]
        protected void TestLevelCountRandomisation()
        {
            TREditor editor = TRCoord.Instance.Open(_validScripts[ScriptFileIndex]);
            TR23ScriptEditor sm = editor.ScriptEditor as TR23ScriptEditor;
            sm.EnabledLevelOrganisation = Organisation.Random;
            sm.EnabledLevelRNG = new RandomGenerator(1986);
            sm.RandomEnabledLevelCount = 5;

            sm.DemosEnabled = false;
            sm.LevelSelectEnabled = true;
            
            editor.Save();

            TestForFinalLevel(new List<AbstractTRScriptedLevel>(sm.EnabledScriptedLevels), sm.Edition);
        }
    }
}