using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;
using TRGE.Coord;

namespace TRGE.Core.Test
{
    [TestClass]
    public class TR2PCManagedDirectoryIOTests : BaseTestCollection
    {
        private readonly string _dataDirectory = @"DirectoryIOTest\WorkingDir";
        private readonly string _bakDirectory = @"DirectoryIOTest\Original";

        private void PrepareDirectories()
        {
            if (_dataDirectory == null || !Directory.Exists(_dataDirectory))
            {
                Assert.Fail("Test cannot proceed - data directory not set or does not exit.");
            }
            new DirectoryInfo(_bakDirectory).Copy(_dataDirectory, true);
        }

        [TestMethod]
        protected void TestManagedIO()
        {
            PrepareDirectories();

            TREditor editor = TRCoord.Instance.Open(_dataDirectory);
            editor.AllowSuccessiveEdits = true;

            Assert.IsTrue(editor.ScriptEditor.BackupFile.Exists);
            TR23ScriptEditor sm = editor.ScriptEditor as TR23ScriptEditor;
            sm.LevelSelectEnabled = true;
            sm.UnarmedLevelOrganisation = Organisation.Manual;
            List<MutableTuple<string, string, bool>> unarmedData = sm.UnarmedLevelData;
            //unarmedData[2].Item3 = true;
            //unarmedData[15].Item3 = true; //floater
            unarmedData[16].Item3 = true; //lair 
            unarmedData[17].Item3 = false; //hsh
            sm.UnarmedLevelData = unarmedData;

            List<MutableTuple<string, string, bool>> ammolessData = sm.AmmolessLevelData;
            ammolessData[17].Item3 = false;
            sm.AmmolessLevelData = ammolessData;
            sm.LevelsHaveFMV = false;
            sm.LevelsHaveCutScenes = false;
            sm.LevelsHaveStartAnimation = false;

            List<MutableTuple<string, string, bool>> sunsetData = sm.LevelSunsetData;
            sunsetData[0].Item3 = true;
            sm.LevelSunsetOrganisation = Organisation.Manual;
            sm.LevelSunsetData = sunsetData;

            editor.Save();
        }

        //[TestMethod]
        protected void TestManagedRestore()
        {
            PrepareDirectories();

            TREditor editor = TRCoord.Instance.Open(_dataDirectory, TRScriptOpenOption.DiscardBackup);
            editor.AllowSuccessiveEdits = true;

            Dictionary<string, string> originalChecksums = GetChecksums();

            TR23ScriptEditor sm = editor.ScriptEditor as TR23ScriptEditor;
            sm.LevelSequencingOrganisation = Organisation.Random;
            sm.LevelSequencingRNG = new RandomGenerator(RandomGenerator.Type.UnixTime);
            editor.Save();

            Dictionary<string, string> modifiedChecksums = GetChecksums();
            CollectionAssert.AreNotEqual(originalChecksums, modifiedChecksums);

            editor.Restore();
            modifiedChecksums = GetChecksums();
            CollectionAssert.AreEqual(originalChecksums, modifiedChecksums);
        }

        private Dictionary<string, string> GetChecksums()
        {
            DirectoryInfo dataDir = new DirectoryInfo(_dataDirectory);
            Dictionary<string, string> checksums = new Dictionary<string, string>();
            foreach (FileInfo fi in dataDir.GetFiles())
            {
                checksums.Add(fi.Name, fi.Checksum());
            }
            return checksums;
        }
    }
}