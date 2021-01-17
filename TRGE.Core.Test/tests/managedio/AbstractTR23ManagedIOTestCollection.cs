using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace TRGE.Core.Test
{
    public abstract class AbstractTR23ManagedIOTestCollection : BaseTestCollection
    {
        protected abstract int ScriptFileIndex { get; }

        [TestMethod]
        [TestSequence(0)]
        protected void TestManagedIO()
        {
            TR23ScriptManager sm = TRGameflowEditor.Instance.GetScriptManager(_validScripts[ScriptFileIndex]) as TR23ScriptManager;
            try
            {
                sm.TitleScreenEnabled = false;
                TRGameflowEditor.Instance.Save(sm, _testOutputPath);
            }
            finally
            {
                TRGameflowEditor.Instance.CloseScriptManager(sm);
            }
            
            sm = TRGameflowEditor.Instance.GetScriptManager(_validScripts[ScriptFileIndex]) as TR23ScriptManager;
            try
            {
                Assert.IsFalse(sm.TitleScreenEnabled);
            }
            finally
            {
                TRGameflowEditor.Instance.CloseScriptManager(sm);
            }
        }

        [TestMethod]
        protected void TestManualLevelSequencing()
        {
            List<Tuple<string, string>> levelSequencingData;
            TR23ScriptManager sm = TRGameflowEditor.Instance.GetScriptManager(_validScripts[ScriptFileIndex]) as TR23ScriptManager;
            try
            {
                levelSequencingData = sm.LevelSequencing;
                levelSequencingData.Reverse();
                sm.LevelOrganisation = Organisation.Manual;
                sm.LevelSequencing = levelSequencingData;

                TRGameflowEditor.Instance.Save(sm, _testOutputPath);
            }
            finally
            {
                TRGameflowEditor.Instance.CloseScriptManager(sm);
            }

            sm = TRGameflowEditor.Instance.GetScriptManager(_validScripts[ScriptFileIndex]) as TR23ScriptManager;
            try
            {
                CollectionAssert.AreEqual(levelSequencingData, sm.LevelSequencing);
            }
            finally
            {
                TRGameflowEditor.Instance.CloseScriptManager(sm);
            }
        }

        [TestMethod]
        protected void TestManualUnarmedLevels()
        {
            List<MutableTuple<string, string, bool>> unarmedData;
            TR23ScriptManager sm = TRGameflowEditor.Instance.GetScriptManager(_validScripts[ScriptFileIndex]) as TR23ScriptManager;
            try
            {
                unarmedData = sm.UnarmedLevelData;
                unarmedData[0].Item3 = !unarmedData[0].Item3;
                sm.UnarmedLevelOrganisation = Organisation.Manual;
                sm.UnarmedLevelData = unarmedData;

                TRGameflowEditor.Instance.Save(sm, _testOutputPath);
            }
            finally
            {
                TRGameflowEditor.Instance.CloseScriptManager(sm);
            }

            sm = TRGameflowEditor.Instance.GetScriptManager(_validScripts[ScriptFileIndex]) as TR23ScriptManager;
            try
            {
                CollectionAssert.AreEqual(unarmedData, sm.UnarmedLevelData);
            }
            finally
            {
                TRGameflowEditor.Instance.CloseScriptManager(sm);
            }
        }

        [TestMethod]
        protected void TestManualAmmolessLevels()
        {
            List<MutableTuple<string, string, bool>> ammolessData;
            TR23ScriptManager sm = TRGameflowEditor.Instance.GetScriptManager(_validScripts[ScriptFileIndex]) as TR23ScriptManager;
            try
            {
                ammolessData = sm.AmmolessLevelData;
                ammolessData[0].Item3 = !ammolessData[0].Item3;
                sm.AmmolessLevelOrganisation = Organisation.Manual;
                sm.AmmolessLevelData = ammolessData;

                TRGameflowEditor.Instance.Save(sm, _testOutputPath);

            }
            finally
            {
                TRGameflowEditor.Instance.CloseScriptManager(sm);
            }

            sm = TRGameflowEditor.Instance.GetScriptManager(_validScripts[ScriptFileIndex]) as TR23ScriptManager;
            try
            {
                CollectionAssert.AreEqual(ammolessData, sm.AmmolessLevelData);
            }
            finally
            {
                TRGameflowEditor.Instance.CloseScriptManager(sm);
            }
        }

        [TestMethod]
        protected void TestAmmolessRandomisation()
        {
            List<MutableTuple<string, string, bool>> ammolessData;
            TR23ScriptManager sm = TRGameflowEditor.Instance.GetScriptManager(_validScripts[ScriptFileIndex]) as TR23ScriptManager;
            try
            {
                ammolessData = sm.AmmolessLevelData;

                sm.AmmolessLevelOrganisation = Organisation.Random;
                sm.AmmolessLevelRNG = new RandomGenerator(RandomGenerator.Type.UnixTime);
                sm.RandomAmmolessLevelCount = 5;

                TRGameflowEditor.Instance.Save(sm, _testOutputPath);
            }
            finally
            {
                TRGameflowEditor.Instance.CloseScriptManager(sm);
            }

            sm = TRGameflowEditor.Instance.GetScriptManager(_validScripts[ScriptFileIndex]) as TR23ScriptManager;
            try
            {
                CollectionAssert.AreEqual(ammolessData, sm.AmmolessLevelData);
            }
            finally
            {
                TRGameflowEditor.Instance.CloseScriptManager(sm);
            }
        }

        [TestMethod]
        protected void TestUnarmedRandomisation()
        {
            List<MutableTuple<string, string, bool>> unarmedData;
            TR23ScriptManager sm = TRGameflowEditor.Instance.GetScriptManager(_validScripts[ScriptFileIndex]) as TR23ScriptManager;
            try
            {
                unarmedData = sm.UnarmedLevelData;

                sm.UnarmedLevelOrganisation = Organisation.Random;
                sm.UnarmedLevelRNG = new RandomGenerator(RandomGenerator.Type.UnixTime);
                sm.RandomUnarmedLevelCount = 3;

                TRGameflowEditor.Instance.Save(sm, _testOutputPath);
            }
            finally
            {
                TRGameflowEditor.Instance.CloseScriptManager(sm);
            }

            sm = TRGameflowEditor.Instance.GetScriptManager(_validScripts[ScriptFileIndex]) as TR23ScriptManager;
            try
            {
                CollectionAssert.AreEqual(unarmedData, sm.UnarmedLevelData);
            }
            finally
            {
                TRGameflowEditor.Instance.CloseScriptManager(sm);
            }
        }
    }
}