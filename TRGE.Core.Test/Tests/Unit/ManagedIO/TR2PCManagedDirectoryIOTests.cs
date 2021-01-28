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

        [TestMethod]
        protected void TestManagedIO()
        {
            File.Copy(@"C:\Users\Lewis\Desktop\TR2_TRGE_TEST\data - Copy\TOMBPC.DAT", @"C:\Users\Lewis\Desktop\TR2_TRGE_TEST\data\TOMBPC.DAT", true);
            if (_dataDirectory == null || !Directory.Exists(_dataDirectory))
            {
                Assert.Fail("Test cannot proceed - data directory not set or does not exit.");
            }
            TREditor editor = TRCoord.Instance.Open(_dataDirectory);
            Assert.IsTrue(editor.ScriptEditor.BackupFile.Exists);
            TR23ScriptEditor sm = editor.ScriptEditor as TR23ScriptEditor;
            sm.LevelSelectEnabled = true;
            sm.UnarmedLevelOrganisation = Organisation.Manual;
            //sm.UnarmedLevelOrganisation = Organisation.Random;
            //sm.UnarmedLevelRNG = new RandomGenerator(RandomGenerator.Type.Date);
            List<MutableTuple<string, string, bool>> unarmedData = sm.UnarmedLevelData;
            //unarmedData[15].Item3 = true; //floating islands texture bug
            //unarmedData[16].Item3 = true; //floating islands texture bug
            unarmedData[17].Item3 = false;
            sm.UnarmedLevelData = unarmedData;

            List<MutableTuple<string, string, bool>> ammolessData = sm.AmmolessLevelData;
            ammolessData[17].Item3 = false;
            sm.AmmolessLevelData = ammolessData;

            editor.Save();
            int j = 0;
        }
    }
}