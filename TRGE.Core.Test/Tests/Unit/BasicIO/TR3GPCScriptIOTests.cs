using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;

namespace TRGE.Core.Test
{
    [TestClass]
    public class TR3GPCScriptIOTests : AbstractTestCollection
    {
        private string _validFilePath, _invalidFilePath;
        private TR23Script _script;

        [ClassInitialize]
        protected override void Setup()
        {
            _invalidFilePath = @"scripts\INVALID.dat";
            _validFilePath = @"scripts\TRTLA_TR3G.dat";
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
                Assert.IsTrue(script.Edition.Equals(TREdition.TR3G));
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
            Assert.IsTrue(_script.DemoTime == 9000);
            Assert.IsTrue(_script.NumDemoLevels == 0);
        }

        [TestMethod]
        protected void TestGameStringData()
        {
            List<string> expectedStrings1 = new List<string>
            {
                "INVENTORY","OPTION","ITEMS","GAME OVER","Load Game","Save Game","New Game","Restart Level","Exit to Title","Exit Demo",
                "Exit Game","Select Level","Save Position","Select Detail","High","Medium","Low","Walk","Roll","Run",
                "Left","Right","Back","Duck","?","Dash","?","Look","Jump","Action",
                "Arm","?","Inventory","Flare","Duck, Dash","Statistics","Pistols","Shotgun","Desert Eagle","Uzis",
                "Harpoon Gun","MP5","Rocket Launcher","Grenade Launcher","Flare","Pistol Clips","Shotgun Shells","Desert Eagle Clips","Uzi Clips","Harpoons","MP5 Clips",
                "Rockets","Grenades","Small Medi Pack","Large Medi Pack","Pickup","Puzzle","Key","Game","Lara's Home","LOADING","Time Taken",
                "Secrets Found","Location","Kills","Ammo Used","Hits","Saves Performed","Distance Travelled","Health Packs Used","Release Version 1.0","None",
                "Finish","BEST TIMES","No Times Set","N/A","Current Position","Final Statistics","of","Story so far...","Hand of Rathmore","Hand of Rathmore",
                "Hand of Rathmore","Hand of Rathmore","Savegame Crystal","London","Nevada","South Pacific Islands","Antarctica","Peru","Adventure", "s"
            };

            List<string> expectedStrings2 = new List<string>
            {
                "Detail Levels","Demo Mode","Sound","Controls","Gamma","Set Volumes","User Keys","The file could not be saved!","Try Again?","YES",
                "NO","Save Complete!","No save games!","None valid","Save Game?","- Empty Slot -","OFF","ON","Setup Sound Card","Default Keys",
                "DOZY","Detail Options","Resolution","ZBuffer","Filter","Dither","True Alpha","Gamma","NA","spare","spare","spare","spare","spare","spare","spare",
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
                "Racetrack Key","K1","Cairn Key","Drill Activator Card","K1","Zoo Key","K1"
            };

            List<string> expectedKeys2 = new List<string>
            {
                "K2","K2","K2","K2","K2","K2","K2"
            };

            List<string> expectedKeys3 = new List<string>
            {
                "K3","K3","K3","K3","K3","K3","K3"
            };

            List<string> expectedKeys4 = new List<string>
            {
                "K4","K4","K4","K4","K4","Aviary Key","K4"
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
                "Lara's House",
                "Highland Fling", "Willard's Lair", "Shakespeare Cliff", "Sleeping with the Fishes",
                "It's a Madhouse!", "Reunion"
            };

            List<string> expectedLevelFileNames = new List<string>
            {
                @"data\house.TR2",
                @"data\scotland.TR2", @"data\willsden.TR2", @"data\chunnel.TR2", @"data\undersea.TR2",
                @"data\zoo.TR2", @"data\slinc.TR2"
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
            Assert.IsTrue(_script.Description.Equals("Tomb Raider III Script. Final Release (c) Core Design Ltd 1998"));
            Assert.IsTrue(_script.FirstOption == 1280);
            Assert.IsTrue(_script.Flags == 768);
            Assert.IsTrue(_script.GameflowSize == 128);
            Assert.IsTrue(_script.Language == 0);
            Assert.IsTrue(_script.SecretSound == 2);
            Assert.IsTrue(_script.SingleLevel == 65535);
            Assert.IsTrue(_script.TitleReplace == -1);
            Assert.IsTrue(_script.TitleSoundID == 5);
            Assert.IsTrue(_script.Xor == 166);
        }

        [TestMethod]
        protected void TestPickupStringData()
        {
            List<string> expectedPickups1 = new List<string>
            {
                "P1","P1","P1","P1","The Hand Of Rathmore","P1","P1"
            };

            List<string> expectedPickups2 = new List<string>
            {
                "P2","P2","P2","P2","P2","P2","P2"
            };

            CompareStrings(expectedPickups1, _script.PickupNames1);
            CompareStrings(expectedPickups2, _script.PickupNames2);
        }

        [TestMethod]
        protected void TestPictureData()
        {
            List<string> expectedPictures = new List<string>
            {
                @"pix\house.bmp",
                @"pix\highland.bmp",@"pix\willard.bmp",@"pix\chunnel.bmp",@"pix\undersea.bmp",
                @"pix\zoo.bmp",@"pix\slinc.bmp"
            };

            Assert.IsTrue(_script.NumPictures == expectedPictures.Count);
            CompareStrings(expectedPictures, _script.PictureNames);
        }

        [TestMethod]
        protected void TestPuzzleStringData()
        {
            List<string> expectedPuzzles1 = new List<string>
            {
                "P1","Crowbar","Crowbar","Pump Access Disk","Circuit Bulb","The Hand Of Rathmore","The Hand Of Rathmore"
            };

            List<string> expectedPuzzles2 = new List<string>
            {
                "P2","Thistle Stone","P2","P2","Mutant Sample","P2","P2"
            };

            List<string> expectedPuzzles3 = new List<string>
            {
                "P3","P3","P3","P3","Mutant Sample","P3","P3"
            };

            List<string> expectedPuzzles4 = new List<string>
            {
                "P4","P4","P4","P4","Circuit Bulb","P4","P4"
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
                new ushort[] { 12, 0, 10, 2, 4, 0, 9 },
                new ushort[] { 10, 36, 12, 1, 4, 1, 6, 9 },
                new ushort[] { 10, 30, 12, 2, 4, 2, 6, 9 },
                new ushort[] { 10, 74, 12, 3, 4, 3, 6, 9 },
                new ushort[] { 10, 27, 12, 4, 4, 4, 6, 9 },
                new ushort[] { 10, 34, 12, 5, 4, 5, 6, 9 },
                new ushort[] { 10, 26, 12, 6, 4, 6, 15, 9 }
            };

            CompareUShortArrays(expectedScriptData, _script.ScriptData);
        }

        [TestMethod]
        protected void TestTitleData()
        {
            List<string> expectedTitles = new List<string>
            {
                @"data\title.TR2", @"pix\titleuk.bmp", @"pix\copyrus.bmp", @"data\titleUS.pcx",
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