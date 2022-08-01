using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;

namespace TRGE.Core.Test
{
    [TestClass]
    public class TR2GPCScriptIOTests : AbstractTestCollection
    {
        private string _validFilePath, _invalidFilePath;
        private TR23Script _script;

        [ClassInitialize]
        protected override void Setup()
        {
            _invalidFilePath = @"scripts\INVALID.dat";
            _validFilePath = @"scripts\TOMBPC_TR2G.dat";
        }

        [ClassCleanup]
        protected override void TearDown() { }

        [TestMethod]
        [TestSequence(0)]
        protected void TestOpenInvalidScript()
        {
            try
            {
                TRScriptFactory.OpenScript(_invalidFilePath);
                Assert.Fail();
            }
            catch (UnsupportedScriptException)
            {
                Assert.IsTrue(true);
            }
        }

        [TestMethod]
        [TestSequence(1)]
        protected void TestOpenValidScript()
        {
            try
            {
                AbstractTRScript script = TRScriptFactory.OpenScript(_validFilePath);
                Assert.IsTrue(script is TR23Script);
                Assert.IsTrue(script.Edition.Equals(TREdition.TR2G));
                
                _script = script as TR23Script;
            }
            catch (UnsupportedScriptException)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        protected void TestCutSceneData()
        {
            Assert.IsTrue(_script.NumCutScenes == 0);
        }

        [TestMethod]
        protected void TestDemoData()
        {
            Assert.IsTrue(_script.DeathDemoMode == 1280);
            Assert.IsTrue(_script.DemoEnd == 1280);
            Assert.IsTrue(_script.DemoInterrupt == 1280);
            Assert.IsTrue(_script.DemoTime == 900);
            Assert.IsTrue(_script.NumDemoLevels == 0);
        }

        [TestMethod]
        protected void TestGameStringData()
        {
            List<string> expectedStrings1 = new List<string>
            {
                "INVENTORY","OPTION","ITEMS","GAME OVER","Load Game","Save Game","New Game","Restart Level","Exit to Title","Exit Demo",
                "Exit Game","Select Level","Save Position","Select Detail","High","Medium","Low","Walk","Roll","Run",
                "Left","Right","Back","Step Left","?","Step Right","?","Look","Jump","Action",
                "Draw Weapon","?","Inventory","Flare","Step","Statistics","Pistols","Shotgun","Automatic Pistols","Uzis",
                "Harpoon Gun","M16","Grenade Launcher","Flare","Pistol Clips","Shotgun Shells","Automatic Pistol Clips","Uzi Clips","Harpoons","M16 Clips",
                "Grenades","Small Medi Pack","Large Medi Pack","Pickup","Puzzle","Key","Game","Lara's Home","LOADING","Time Taken",
                "Secrets Found","Location","Kills","Ammo Used","Hits","Saves Performed","Distance Travelled","Health Packs Used","Release Version 1.1","None",
                "Finish","BEST TIMES","No Times Set","N/A","Current Position","Final Statistics","of","Story so far...","spare","spare",
                "spare","spare","spare","spare","spare","spare","spare","spare","spare"
            };

            List<string> expectedStrings2 = new List<string>
            {
                "Detail Levels","Demo Mode","Sound","Controls","Gamma","Set Volumes","User Keys","The file could not be saved!","Try Again?","YES",
                "NO","Save Complete!","No save games!","None valid","Save Game?","- EMPTY SLOT -","OFF","ON","Setup Sound Card","Default Keys",
                "DOZY","spare","spare","spare","spare","spare","spare","spare","spare","spare","spare","spare","spare","spare","spare","spare",
                "spare","spare","spare","spare","spare"
            };

            Assert.IsTrue(_script.NumGameStrings1 == expectedStrings1.Count);
            CompareStrings(expectedStrings1, _script.GameStrings1);

            Assert.IsTrue(_script.NumGameStrings2 == expectedStrings2.Count);
            CompareStrings(expectedStrings2, _script.GameStrings2);
        }

        [TestMethod]
        protected void TestKeyStringData()
        {
            List<string> expectedKeys1 = new List<string>
            {
                "K1","Guardroom Key","CardKey 1","K1","K1","Hotel Key"
            };

            List<string> expectedKeys2 = new List<string>
            {
                "K2","Shaft 'B' Key","K2","K2","K2","K2"
            };

            List<string> expectedKeys3 = new List<string>
            {
                "K3","K3","K3","K3","K3","K3"
            };

            List<string> expectedKeys4 = new List<string>
            {
                "K4","K4","CardKey 2","K4","K4","K4"
            };

            CompareStrings(expectedKeys1, _script.KeyNames1);
            CompareStrings(expectedKeys2, _script.KeyNames2);
            CompareStrings(expectedKeys3, _script.KeyNames3);
            CompareStrings(expectedKeys4, _script.KeyNames4);
        }

        [TestMethod]
        protected void TestLevelData()
        {
            List<string> expectedLevelNames = new List<string>
            {
                "Lara's Home",
                "The Cold War", "Fool's Gold", "Furnace of the Gods", "Kingdom", "Nightmare In Vegas"
            };

            List<string> expectedLevelFileNames = new List<string>
            {
                @"data\assault.TR2",
                @"data\level1.TR2", @"data\level2.TR2", @"data\level3.TR2", @"data\level4.TR2", @"data\level5.TR2"
            };

            Assert.IsTrue(_script.NumLevels == expectedLevelNames.Count);
            CompareStrings(expectedLevelNames, _script.LevelNames);

            Assert.IsTrue(_script.NumLevels == expectedLevelFileNames.Count);
            CompareStrings(expectedLevelFileNames, _script.LevelFileNames);
        }

        [TestMethod]
        protected void TestMiscDefaultData()
        {
            Assert.IsTrue(_script.DeathInGame == 0);
            Assert.IsTrue(_script.Description.Equals("Tomb Raider II Script. Final Release Version 1.1 (c) Core Design Ltd 1997"));
            Assert.IsTrue(_script.FirstOption == 1280);
            Assert.IsTrue(_script.Flags == 768);
            Assert.IsTrue(_script.GameflowSize == 128);
            Assert.IsTrue(_script.Language == 0);
            Assert.IsTrue(_script.SecretSound == 47);
            Assert.IsTrue(_script.SingleLevel == 65535);
            Assert.IsTrue(_script.TitleReplace == -1);
            Assert.IsTrue(_script.TitleSoundID == 64);
            Assert.IsTrue(_script.Xor == 166);
        }

        [TestMethod]
        protected void TestPickupStringData()
        {
            List<string> expectedPickups1 = new List<string>
            {
                "P1","P1","P1","P1","P1","P1"
            };

            List<string> expectedPickups2 = new List<string>
            {
                "P2","P2","P2","P2","P2","P2"
            };

            CompareStrings(expectedPickups1, _script.PickupNames1);
            CompareStrings(expectedPickups2, _script.PickupNames2);
        }

        [TestMethod]
        protected void TestPictureData()
        {
            List<string> expectedPictures = new List<string>();
            Assert.IsTrue(_script.NumPictures == 0);
            CompareStrings(expectedPictures, _script.PictureNames);
        }

        [TestMethod]
        protected void TestPuzzleStringData()
        {
            List<string> expectedPuzzles1 = new List<string>
            {
                "P1","P1","Circuit Board","Mask Of Tornarsuk","Mask Of Tornarsuk","Elevator Junction"
            };

            List<string> expectedPuzzles2 = new List<string>
            {
                "P2","P2","P2","Gold Nugget","P2","Door Circuit"
            };

            List<string> expectedPuzzles3 = new List<string>
            {
                "P3","P3","P3","P3","P3","P3"
            };

            List<string> expectedPuzzles4 = new List<string>
            {
                "P4","P4","P4","P4","P4","P4"
            };

            CompareStrings(expectedPuzzles1, _script.PuzzleNames1);
            CompareStrings(expectedPuzzles2, _script.PuzzleNames2);
            CompareStrings(expectedPuzzles3, _script.PuzzleNames3);
            CompareStrings(expectedPuzzles4, _script.PuzzleNames4);
        }

        [TestMethod]
        protected void TestRPLData()
        {
            List<string> expectedRPLs = new List<string>
            {
                @"FMV\LOGO.RPL"
            };

            Assert.IsTrue(_script.NumRPLs == expectedRPLs.Count);
            CompareStrings(expectedRPLs, _script.RPLFileNames);
        }

        [TestMethod]
        protected void TestScriptData()
        {
            List<ushort[]> expectedScriptData = new List<ushort[]>
            {
                new ushort[] { 3, 0, 9 },
                new ushort[] { 20, 0, 10, 0, 4, 0, 9 },
                new ushort[] { 10, 33, 4, 1, 6, 9 },
                new ushort[] { 10, 58, 4, 2, 6, 9 },
                new ushort[] { 10, 59, 4, 3, 6, 9 },
                new ushort[] { 10, 31, 18, 1019, 4, 4, 15, 9 },
                new ushort[] { 10, 34, 4, 5, 6, 9 }
            };

            CompareUShortArrays(expectedScriptData, _script.ScriptData);
        }

        [TestMethod]
        protected void TestTitleData()
        {
            List<string> expectedTitles = new List<string>
            {
                @"data\title.TR2", @"data\title.pcx", @"data\legal.pcx", @"data\titleUS.pcx",
                @"data\legalUS.pcx", @"data\titleJAP.pcx", @"data\legalJAP.pcx"
            };

            Assert.IsTrue(_script.NumTitles == expectedTitles.Count);
            CompareStrings(expectedTitles, _script.TitleFileNames);
        }

        [TestMethod]
        protected void TestUntouchedWrite()
        {
            byte[] originalData = File.ReadAllBytes(_validFilePath);
            CollectionAssert.AreEqual(originalData, _script.SerialiseScriptToBin());
        }
    }
}