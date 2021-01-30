using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;
using TRGE.Coord;

namespace TRGE.Core.Test
{
    [TestClass]
    public class TR2PCManagedDirectoryIOTests : BaseTestCollection
    {
        private readonly string _dataDirectory = @"C:\Users\Lewis\Desktop\TR2_TRGE_TEST\data";
        private readonly string _bakDirectory = @"C:\Users\Lewis\Desktop\TR2_TRGE_TEST\data - Copy";

        //[TestMethod]
        protected void TestManagedIO()
        {
            //File.Copy(@"C:\Users\Lewis\Desktop\TR2_TRGE_TEST\data - Copy\TOMBPC.DAT", @"C:\Users\Lewis\Desktop\TR2_TRGE_TEST\data\TOMBPC.DAT", true);
            if (_dataDirectory == null || !Directory.Exists(_dataDirectory))
            {
                Assert.Fail("Test cannot proceed - data directory not set or does not exit.");
            }
            new DirectoryInfo(_bakDirectory).Copy(_dataDirectory, true);
            TREditor editor = TRCoord.Instance.Open(_dataDirectory);
            editor.LevelEditor.AllowSuccessiveEdits = true;

            Assert.IsTrue(editor.ScriptEditor.BackupFile.Exists);
            TR23ScriptEditor sm = editor.ScriptEditor as TR23ScriptEditor;
            sm.LevelSelectEnabled = true;
            sm.UnarmedLevelOrganisation = Organisation.Manual;
            List<MutableTuple<string, string, bool>> unarmedData = sm.UnarmedLevelData;
            //unarmedData[2].Item3 = true;
            //unarmedData[15].Item3 = true; //floater
            //unarmedData[16].Item3 = true; //lair 
            unarmedData[17].Item3 = false; //hsh
            sm.UnarmedLevelData = unarmedData;

            List<MutableTuple<string, string, bool>> ammolessData = sm.AmmolessLevelData;
            ammolessData[17].Item3 = false;
            sm.AmmolessLevelData = ammolessData;
            sm.LevelsHaveFMV = false;
            sm.LevelsHaveCutScenes = false;

            editor.Save();
            int j = 0;
        }
    }
}