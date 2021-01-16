using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

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
            TR23ScriptManager sm = TRGameflowEditor.Instance.GetScriptManager(_validScripts[ScriptFileIndex]) as TR23ScriptManager;
            try
            {
                Assert.AreEqual(sm.LevelsHaveCutScenes, ExpectedCutScenes);
            }
            finally
            {
                TRGameflowEditor.Instance.CloseScriptManager(sm);
            }
        }

        [TestMethod]
        [TestSequence(1)]
        protected void TestSetCutScenes()
        {
            TR23ScriptManager sm = TRGameflowEditor.Instance.GetScriptManager(_validScripts[ScriptFileIndex]) as TR23ScriptManager;
            try
            {
                sm.LevelsHaveCutScenes = !ExpectedCutScenes;
                Assert.AreNotEqual(sm.LevelsHaveCutScenes, ExpectedCutScenes);
            }
            finally
            {
                TRGameflowEditor.Instance.CloseScriptManager(sm);
            }
        }

        [TestMethod]
        [TestSequence(2)]
        protected void TestLoadFrontEndFMV()
        {
            TR23ScriptManager sm = TRGameflowEditor.Instance.GetScriptManager(_validScripts[ScriptFileIndex]) as TR23ScriptManager;
            try
            {
                Assert.AreEqual(sm.FrontEndHasFMV, ExpectedFrontEndFMV);
            }
            finally
            {
                TRGameflowEditor.Instance.CloseScriptManager(sm);
            }
        }

        [TestMethod]
        [TestSequence(3)]
        protected void TestSetFrontEndFMV()
        {
            TR23ScriptManager sm = TRGameflowEditor.Instance.GetScriptManager(_validScripts[ScriptFileIndex]) as TR23ScriptManager;
            try
            {
                sm.FrontEndHasFMV = !ExpectedFrontEndFMV;
                Assert.AreNotEqual(sm.FrontEndHasFMV, ExpectedFrontEndFMV);
            }
            finally
            {
                TRGameflowEditor.Instance.CloseScriptManager(sm);
            }
        }

        [TestMethod]
        [TestSequence(4)]
        protected void TestLoadLevelsFMV()
        {
            TR23ScriptManager sm = TRGameflowEditor.Instance.GetScriptManager(_validScripts[ScriptFileIndex]) as TR23ScriptManager;
            try
            {
                Assert.AreEqual(sm.LevelsHaveFMV, ExpectedLevelsFMV);
            }
            finally
            {
                TRGameflowEditor.Instance.CloseScriptManager(sm);
            }
        }

        [TestMethod]
        [TestSequence(5)]
        protected void TestSetLevelsFMV()
        {
            TR23ScriptManager sm = TRGameflowEditor.Instance.GetScriptManager(_validScripts[ScriptFileIndex]) as TR23ScriptManager;
            try
            {
                sm.LevelsHaveFMV = !ExpectedLevelsFMV;
                Assert.AreNotEqual(sm.LevelsHaveFMV, ExpectedLevelsFMV);
            }
            finally
            {
                TRGameflowEditor.Instance.CloseScriptManager(sm);
            }
        }

        [TestMethod]
        [TestSequence(6)]
        protected void TestLoadLevelsStartAnimation()
        {
            TR23ScriptManager sm = TRGameflowEditor.Instance.GetScriptManager(_validScripts[ScriptFileIndex]) as TR23ScriptManager;
            try
            {
                Assert.AreEqual(sm.LevelsHaveStartAnimation, ExpectedLevelsStartAnimation);
            }
            finally
            {
                TRGameflowEditor.Instance.CloseScriptManager(sm);
            }
        }

        [TestMethod]
        [TestSequence(7)]
        protected void TestSetLevelsStartAnimation()
        {
            TR23ScriptManager sm = TRGameflowEditor.Instance.GetScriptManager(_validScripts[ScriptFileIndex]) as TR23ScriptManager;
            try
            {
                sm.LevelsHaveStartAnimation = !ExpectedLevelsStartAnimation;
                Assert.AreNotEqual(sm.LevelsHaveStartAnimation, ExpectedLevelsStartAnimation);
            }
            finally
            {
                TRGameflowEditor.Instance.CloseScriptManager(sm);
            }
        }
    }
}