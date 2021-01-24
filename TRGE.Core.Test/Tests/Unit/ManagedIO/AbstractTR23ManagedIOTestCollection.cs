using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using TRGE.Coord;

namespace TRGE.Core.Test
{
    public abstract class AbstractTR23ManagedIOTestCollection : BaseTestCollection
    {
        protected abstract int ScriptFileIndex { get; }

        [TestMethod]
        [TestSequence(0)]
        protected void TestManagedIO()
        {
            TREditor editor = TRCoord.Instance.Open(_validScripts[ScriptFileIndex]);
            TR23ScriptManager sm = editor.ScriptManager as TR23ScriptManager;
            sm.TitleScreenEnabled = false;
            editor.Save();;

            sm = TRCoord.Instance.Open(_validScripts[ScriptFileIndex]).ScriptManager as TR23ScriptManager;
            Assert.IsFalse(sm.TitleScreenEnabled);
        }

        [TestMethod]
        [TestSequence(1)]
        protected void TestChecksumHandlingDiscard()
        {
            TestChecksumHandling(TRScriptOpenOption.DiscardBackup);
        }

        [TestMethod]
        [TestSequence(2)]
        protected void TestChecksumHandlingRestore()
        {
            TestChecksumHandling(TRScriptOpenOption.RestoreBackup);
        }

        private void TestChecksumHandling(TRScriptOpenOption option)
        {
            File.Copy(_validScripts[ScriptFileIndex], _testOutputPath, true);
            try
            {
                TREditor editor = TRCoord.Instance.Open(_testOutputPath);
                TR23ScriptManager sm = editor.ScriptManager as TR23ScriptManager;
                sm.FrontEndHasFMV = !sm.FrontEndHasFMV;
                editor.Save();

                File.WriteAllBytes(_testOutputPath, File.ReadAllBytes(_validScripts[ScriptFileIndex]));

                try
                {
                    sm = TRCoord.Instance.Open(_testOutputPath).ScriptManager as TR23ScriptManager;
                    Assert.Fail();
                }
                catch (ChecksumMismatchException)
                {
                    try
                    {
                        sm = TRCoord.Instance.Open(_testOutputPath, option).ScriptManager as TR23ScriptManager;
                    }
                    catch (ChecksumMismatchException)
                    {
                        Assert.Fail();
                    }
                }
            }
            finally
            {
                File.Delete(_testOutputPath);
            }
        }

        [TestMethod]
        protected void TestManualLevelSequencing()
        {
            List<Tuple<string, string>> levelSequencingData;
            TREditor editor = TRCoord.Instance.Open(_validScripts[ScriptFileIndex]);
            TR23ScriptManager sm = editor.ScriptManager as TR23ScriptManager;
            levelSequencingData = sm.LevelSequencing;
            levelSequencingData.Reverse();
            sm.LevelOrganisation = Organisation.Manual;
            sm.LevelSequencing = levelSequencingData;

            editor.Save();
            
            sm = TRCoord.Instance.Open(_validScripts[ScriptFileIndex]).ScriptManager as TR23ScriptManager;
            CollectionAssert.AreEqual(levelSequencingData, sm.LevelSequencing);
        }

        [TestMethod]
        protected void TestManualUnarmedLevels()
        {
            List<MutableTuple<string, string, bool>> unarmedData;
            TREditor editor = TRCoord.Instance.Open(_validScripts[ScriptFileIndex]);
            TR23ScriptManager sm = editor.ScriptManager as TR23ScriptManager;
            unarmedData = sm.UnarmedLevelData;
            unarmedData[0].Item3 = !unarmedData[0].Item3;
            sm.UnarmedLevelOrganisation = Organisation.Manual;
            sm.UnarmedLevelData = unarmedData;

            editor.Save();
            
            sm = TRCoord.Instance.Open(_validScripts[ScriptFileIndex]).ScriptManager as TR23ScriptManager;
            CollectionAssert.AreEqual(unarmedData, sm.UnarmedLevelData);
        }

        [TestMethod]
        protected void TestManualAmmolessLevels()
        {
            List<MutableTuple<string, string, bool>> ammolessData;
            TREditor editor = TRCoord.Instance.Open(_validScripts[ScriptFileIndex]);
            TR23ScriptManager sm = editor.ScriptManager as TR23ScriptManager;
            ammolessData = sm.AmmolessLevelData;
            ammolessData[0].Item3 = !ammolessData[0].Item3;
            sm.AmmolessLevelOrganisation = Organisation.Manual;
            sm.AmmolessLevelData = ammolessData;

            editor.Save();

            sm = TRCoord.Instance.Open(_validScripts[ScriptFileIndex]).ScriptManager as TR23ScriptManager;
            CollectionAssert.AreEqual(ammolessData, sm.AmmolessLevelData);
        }

        [TestMethod]
        protected void TestAmmolessRandomisation()
        {
            List<MutableTuple<string, string, bool>> ammolessData;
            TREditor editor = TRCoord.Instance.Open(_validScripts[ScriptFileIndex]);
            TR23ScriptManager sm = editor.ScriptManager as TR23ScriptManager;
            ammolessData = sm.AmmolessLevelData;

            sm.AmmolessLevelOrganisation = Organisation.Random;
            sm.AmmolessLevelRNG = new RandomGenerator(RandomGenerator.Type.UnixTime);
            sm.RandomAmmolessLevelCount = 5;

            editor.Save();

            sm = TRCoord.Instance.Open(_validScripts[ScriptFileIndex]).ScriptManager as TR23ScriptManager;
            CollectionAssert.AreEqual(ammolessData, sm.AmmolessLevelData);
        }

        [TestMethod]
        protected void TestUnarmedRandomisation()
        {
            List<MutableTuple<string, string, bool>> unarmedData;
            TREditor editor = TRCoord.Instance.Open(_validScripts[ScriptFileIndex]);
            TR23ScriptManager sm = editor.ScriptManager as TR23ScriptManager;
            unarmedData = sm.UnarmedLevelData;

            sm.UnarmedLevelOrganisation = Organisation.Random;
            sm.UnarmedLevelRNG = new RandomGenerator(RandomGenerator.Type.UnixTime);
            sm.RandomUnarmedLevelCount = 3;

            editor.Save();

            sm = TRCoord.Instance.Open(_validScripts[ScriptFileIndex]).ScriptManager as TR23ScriptManager;
            CollectionAssert.AreEqual(unarmedData, sm.UnarmedLevelData);
        }

        [TestMethod]
        protected void TestAudioChange()
        {
            List<MutableTuple<string, string, ushort>> trackData;
            TREditor editor = TRCoord.Instance.Open(_validScripts[ScriptFileIndex]);
            TR23ScriptManager sm = editor.ScriptManager as TR23ScriptManager;
            trackData = sm.GameTrackData;
            trackData[0].Item3 = 47;
            sm.GameTrackData = trackData;

            editor.Save();

            sm = TRCoord.Instance.Open(_validScripts[ScriptFileIndex]).ScriptManager as TR23ScriptManager;
            CollectionAssert.AreEqual(trackData, sm.GameTrackData);
        }
    }
}