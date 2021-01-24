using Microsoft.VisualStudio.TestTools.UnitTesting;
using TRGE.Coord;

namespace TRGE.Core.Test
{
    [TestClass]
    public abstract class AbstractTR23InterruptionTestCollection : BaseTestCollection
    {
        protected abstract int ScriptFileIndex { get; }
        protected abstract bool ExpectedCutScenes { get; }
        protected abstract bool ExpectedFrontEndFMV { get; }
        protected abstract bool ExpectedLevelsFMV { get; }
        protected abstract bool ExpectedLevelsStartAnimation { get; }

        [TestMethod]
        [TestSequence(0)]
        protected void TestLoadCutScenes()
        {
            TR23ScriptManager sm = TRCoord.Instance.Open(_validScripts[ScriptFileIndex]).ScriptManager as TR23ScriptManager;
            Assert.AreEqual(sm.LevelsSupportCutScenes, ExpectedCutScenes);
        }

        [TestMethod]
        [TestSequence(1)]
        protected void TestSetCutScenes()
        {
            TR23ScriptManager sm = TRCoord.Instance.Open(_validScripts[ScriptFileIndex]).ScriptManager as TR23ScriptManager;
            if (sm.LevelsSupportCutScenes)
            {
                sm.LevelsHaveCutScenes = !ExpectedCutScenes;
                Assert.AreNotEqual(sm.LevelsHaveCutScenes, ExpectedCutScenes);
            }
        }

        [TestMethod]
        [TestSequence(2)]
        protected void TestLoadFrontEndFMV()
        {
            TR23ScriptManager sm = TRCoord.Instance.Open(_validScripts[ScriptFileIndex]).ScriptManager as TR23ScriptManager;
            Assert.AreEqual(sm.FrontEndHasFMV, ExpectedFrontEndFMV);
        }

        [TestMethod]
        [TestSequence(3)]
        protected void TestSetFrontEndFMV()
        {
            TR23ScriptManager sm = TRCoord.Instance.Open(_validScripts[ScriptFileIndex]).ScriptManager as TR23ScriptManager;
            sm.FrontEndHasFMV = !ExpectedFrontEndFMV;
            Assert.AreNotEqual(sm.FrontEndHasFMV, ExpectedFrontEndFMV);
        }

        [TestMethod]
        [TestSequence(4)]
        protected void TestLoadLevelsFMV()
        {
            TR23ScriptManager sm = TRCoord.Instance.Open(_validScripts[ScriptFileIndex]).ScriptManager as TR23ScriptManager;
            Assert.AreEqual(sm.LevelsSupportFMVs, ExpectedLevelsFMV);
        }

        [TestMethod]
        [TestSequence(5)]
        protected void TestSetLevelsFMV()
        {
            TR23ScriptManager sm = TRCoord.Instance.Open(_validScripts[ScriptFileIndex]).ScriptManager as TR23ScriptManager;
            if (sm.LevelsSupportFMVs)
            {
                sm.LevelsHaveFMV = !ExpectedLevelsFMV;
                Assert.AreNotEqual(sm.LevelsHaveFMV, ExpectedLevelsFMV);
            }
        }

        [TestMethod]
        [TestSequence(6)]
        protected void TestLoadLevelsStartAnimation()
        {
            TR23ScriptManager sm = TRCoord.Instance.Open(_validScripts[ScriptFileIndex]).ScriptManager as TR23ScriptManager;
            Assert.AreEqual(sm.LevelsSupportStartAnimations, ExpectedLevelsStartAnimation);
        }

        [TestMethod]
        [TestSequence(7)]
        protected void TestSetLevelsStartAnimation()
        {
            TR23ScriptManager sm = TRCoord.Instance.Open(_validScripts[ScriptFileIndex]).ScriptManager as TR23ScriptManager;
            if (sm.LevelsSupportStartAnimations)
            {
                sm.LevelsHaveStartAnimation = !ExpectedLevelsStartAnimation;
                Assert.AreNotEqual(sm.LevelsHaveStartAnimation, ExpectedLevelsStartAnimation);
            }
        }
    }
}