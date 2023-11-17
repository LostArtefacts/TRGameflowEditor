using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;

namespace TRGE.Core.Test
{
    [TestClass]
    public class TR3PSXScriptIOTests : AbstractTestCollection
    {
        private string _validFilePath, _invalidFilePath;
        private TR23Script _script;

        [ClassInitialize]
        protected override void Setup()
        {
            _invalidFilePath = @"scripts\INVALID.dat";
            _validFilePath = @"scripts\TOMBPSX_TR3.dat";
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
                Assert.IsTrue(script.Edition.Equals(TREdition.TR3PSX));
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
            List<string> expectedCutScenes = new()
            {
                @"cuts\cut6.PSX", @"cuts\cut9.PSX", @"cuts\cut1.PSX", @"cuts\cut4.PSX",  @"cuts\cut2.PSX", @"cuts\cut5.PSX",
                @"cuts\cut11.PSX", @"cuts\cut7.PSX", @"cuts\cut8.PSX", @"cuts\cut3.PSX", @"cuts\cut12.PSX"
            };

            Assert.IsTrue(_script.NumCutScenes == expectedCutScenes.Count);
            CompareStrings(expectedCutScenes, _script.CutSceneFileNames);
        }

        [TestMethod]
        protected void TestDemoData()
        {
            List<ushort> expectedDemoData = new()
            {
                21, 22, 23
            };

            Assert.IsTrue(_script.DeathDemoMode == 1280);
            CompareUShorts(expectedDemoData, _script.DemoData);
            Assert.IsTrue(_script.DemoEnd == 1280);
            Assert.IsTrue(_script.DemoInterrupt == 1280);
            Assert.IsTrue(_script.DemoTime == 500);
            Assert.IsTrue(_script.NumDemoLevels == expectedDemoData.Count);
        }

        [TestMethod]
        protected void TestFMVData()
        {
            List<uint[]> excpectedFMVData = new()
            {
                new uint[] { 1, 817 },
                new uint[] { 1, 4510 },
                new uint[] { 1, 7729 },
                new uint[] { 1, 2033 },
                new uint[] { 1, 1662 }
            };

            CompareUIntArrays(excpectedFMVData, _script.PSXFMVData);
        }

        [TestMethod]
        protected void TestGameStringData()
        {
            List<string> expectedStrings1 = new()
            {
                "INVENTORY","OPTION","ITEMS","GAME OVER","Load Game","Save Game","New Game","Restart Level","Exit to Title","Exit Demo",
                "Exit Game","Select Level","Save Position","Select Detail","High","Medium","Low","Walk","Roll","Run",
                "Left","Right","Back","Step Left","?","Step Right","?","Look","Jump","Action",
                "Draw Weapon","?","Inventory","Duck","Dash","Statistics","Pistols","Shotgun","Desert Eagle","Uzis",
                "Harpoon Gun","MP5","Rocket Launcher","Grenade Launcher","Flare","Pistol Clips","Shotgun Shells","Desert Eagle Clips","Uzi Clips","Harpoons","MP5 Clips",
                "Rockets","Grenades","Small Medi Pack","Large Medi Pack","Pickup","Puzzle","Key","Game","Lara's Home","LOADING","Time Taken",
                "Secrets Found","Location","Kills","Ammo Used","Hits","Saves Performed","Distance Travelled","Health Packs Used","Release Version 1.0","None",
                "Finish","BEST TIMES","No Times Set","N/A","Current Position","Final Statistics","of","Story so far...","Infada Stone","Element 115",
                "The Eye Of Isis","Ora Dagger","Savegame Crystal","London","Nevada","South Pacific Islands","Antarctica","Peru","NEXT ADVENTURE", "s"
            };

            List<string> expectedStrings2 = new()
            {
                "Screen Adjust","DEMO MODE","Sound","Controls","Gamma","Set Volumes","Control Method","The file could not be saved!","Try Again?","YES","NO",
                "Save Complete!","No save games!","None valid","Save Game?","- EMPTY SLOT -","Press any button to quit","Use directional buttons","to adjust screen position",
                "> Select","; Go Back","> Continue","PAUSED","Controller Removed","Save game to","Overwrite game on","Load game from","the Memory card in","Memory card slot 1",
                "Are you sure ?","Yes","No","has insufficient free blocks","There are no","Tomb Raider III games on","There is a problem with","There is no Memory card in",
                "is unformatted","Would you like to","format it ?","Saving game to","Loading game from","Formatting","Overwrite game","Save game","Load game","Format Memory card",
                "Exit","Continue","You will not be able","to save any games unless","you insert a formatted","Memory card with at least","two free blocks","The Memory card in",
                "Exit without saving","Exit without loading","Start game","UNFORMATTED MEMORY CARD","Insert a formatted","or press > to continue","without saving.",
                "bu00:BESLES-01649TOMB3","bu00:BASLUS-00691TOMB3","Load O.K.","Load failed","Saved O.K.","Save failed","Format O.K.","Format failed","Vibration Off","Vibration On",
                "Toggle Vibration","spare","spare","spare","spare","spare","spare","Select Adventure"
            };

            Assert.IsTrue(_script.NumGameStrings1 == expectedStrings1.Count);
            CompareStrings(expectedStrings1, _script.GameStrings1);

            Assert.IsTrue(_script.NumGameStrings2 == expectedStrings2.Count);
            CompareStrings(expectedStrings2, _script.GameStrings2);
        }

        [TestMethod]
        protected void TestKeyStringData()
        {
            List<string> expectedKeys1 = new()
            {
                "Racetrack Key","K1","Key Of Ganesha","Gate Key","K1","Smuggler's Key","Commander Bishop's Key","K1","K1","Flue Room Key","Maintenance Key",
                "Boiler Room Key","K1","Generator Access","Keycard Type A","Launch Code Pass","Hut Key","K1","Uli Key","K1","Vault Key","K1","K1","K1"
            };

            List<string> expectedKeys2 = new()
            {
                "K2","K2","Key Of Ganesha","K2","K2","K2","Lt. Tuckerman's Key","K2","K2","Cathedral Key","Solomon's Key","K2","K2","Detonator Switch",
                "Keycard Type B","K2","K2","K2","K2","K2","K2","K2","K2","K2"
            };

            List<string> expectedKeys3 = new()
            {
                "K3","K3","Key Of Ganesha","K3","K3","K3","K3","K3","K3","K3","Solomon's Key","K3","K3","K3","K3","K3","K3","K3","K3","K3","K3","K3","K3","K3"
            };

            List<string> expectedKeys4 = new()
            {
                "K4","Indra Key","Key Of Ganesha","K4","K4","K4","K4","K4","K4","K4","Solomon's Key","K4","K4","K4","K4","K4","K4","K4","K4","K4","K4","K4","K4","K4"
            };

            CompareStrings(expectedKeys1, _script.KeyNames1);
            CompareStrings(expectedKeys2, _script.KeyNames2);
            CompareStrings(expectedKeys3, _script.KeyNames3);
            CompareStrings(expectedKeys4, _script.KeyNames4);
        }

        [TestMethod]
        protected void TestLevelData()
        {
            List<string> expectedLevelNames = new()
            {
                "Lara's House",
                "Jungle", "Temple Ruins", "The River Ganges", "Caves Of Kaliya",
                "Coastal Village", "Crash Site", "Madubu Gorge", "Temple Of Puna",
                "Thames Wharf", "Aldwych", "Lud's Gate", "City",
                "Nevada Desert", "High Security Compound", "Area 51",
                "Antarctica", "RX-Tech Mines", "Lost City Of Tinnos","Meteorite Cavern",
                "All Hallows",
                "1", "2", "3" //demos
            };

            List<string> expectedLevelFileNames = new()
            {
                @"data\house.PSX",
                @"data\jungle.PSX", @"data\temple.PSX", @"data\quadchas.PSX", @"data\tonyboss.PSX",
                @"data\shore.PSX", @"data\crash.PSX", @"data\rapids.PSX", @"data\triboss.PSX",
                @"data\roofs.PSX", @"data\sewer.PSX", @"data\tower.PSX", @"data\office.PSX",
                @"data\nevada.PSX", @"data\compound.PSX", @"data\area51.PSX",
                @"data\antarc.PSX", @"data\mines.PSX", @"data\city.PSX", @"data\chamber.PSX",
                @"data\stpaul.PSX",
                @"data\demo1.psx",@"data\demo2.psx",@"data\demo3.psx" //demos
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
            List<string> expectedPickups1 = new()
            {
                "P1","P1","P1","P1","P1","P1","Swamp Map","P1","P1","P1","P1",
                "P1","P1","P1","P1","P1","P1","P1","P1","P1","P1","P1","P1","P1"
            };

            List<string> expectedPickups2 = new()
            {
                "P2","P2","P2","P2","P2","P2","P2","P2","P2","P2","P2","P2",
                "P2","P2","P2","P2","P2","P2","P2","P2","P2","P2","P2","P2"
            };

            CompareStrings(expectedPickups1, _script.PickupNames1);
            CompareStrings(expectedPickups2, _script.PickupNames2);
        }

        [TestMethod]
        protected void TestPictureData()
        {
            List<string> expectedPictures = new()
            {
                @"pix\house.raw",
                @"pix\india.raw",@"pix\india.raw",@"pix\india.raw",@"pix\india.raw",
                @"pix\southpac.raw",@"pix\southpac.raw",@"pix\southpac.raw",@"pix\southpac.raw",
                @"pix\london.raw",@"pix\london.raw",@"pix\london.raw",@"pix\london.raw",
                @"pix\nevada.raw",@"pix\nevada.raw",@"pix\nevada.raw",
                @"pix\antarc.raw",@"pix\antarc.raw",@"pix\antarc.raw",@"pix\antarc.raw",
                @"pix\london.raw",
                @"pix\london.raw",@"pix\india.raw",@"pix\nevada.raw"
            };

            Assert.IsTrue(_script.NumPictures == expectedPictures.Count);
            CompareStrings(expectedPictures, _script.PictureNames);
        }

        [TestMethod]
        protected void TestPuzzleStringData()
        {
            List<string> expectedPuzzles1 = new()
            {
                "P1","P1","Scimitar","P1","P1","Serpent Stone","P1","P1","P1","P1","Old Penny","Embalming Fluid","P1","P1",
                "Blue Security Pass","Tower Access Key","Crowbar","Crowbar","Oceanic Mask","P1","P1","P1","P1","P1"
            };

            List<string> expectedPuzzles2 = new()
            {
                "P2","P2","Scimitar","P2","P2","P2","P2","P2","P2","P2","Ticket","P2","P2","P2","Yellow Security Pass",
                "Code Clearance Disk","Gate Control Key","Lead Acid Battery","P2","P2","P2","P2","P2","P2"
            };

            List<string> expectedPuzzles3 = new()
            {
                "P3","P3","P3","P3","P3","P3","P3","P3","P3","P3","Masonic Mallet","P3","P3","P3","P3",
                "Code Clearance Disk","P3","Winch Starter","P3","P3","P3","P3","P3","P3"
            };

            List<string> expectedPuzzles4 = new()
            {
                "P4","P4","P4","P4","P4","P4","P4","P4","P4","P4","Ornate Star","P4","P4","P4","P4",
                "Hangar Access Key","P4","P4","P4","P4","P4","P4","P4","P4"
            };

            CompareStrings(expectedPuzzles1, _script.PuzzleNames1);
            CompareStrings(expectedPuzzles2, _script.PuzzleNames2);
            CompareStrings(expectedPuzzles3, _script.PuzzleNames3);
            CompareStrings(expectedPuzzles4, _script.PuzzleNames4);
        }

        [TestMethod]
        protected void TestRPLData()
        {
            List<string> expectedRPLs = new()
            {
                @"\FMV\LOGO.FMV", @"\FMV\INTRO.FMV", @"\FMV\LAGOON.FMV", @"\FMV\HUEY.FMV", @"\FMV\END.FMV"
            };

            Assert.IsTrue(_script.NumRPLs == expectedRPLs.Count);
            CompareStrings(expectedRPLs, _script.RPLFileNames);
        }

        [TestMethod]
        protected void TestScriptData()
        {
            List<ushort[]> expectedScriptData = new()
            {
                new ushort[] { 3, 0, 3, 1, 9 },
                new ushort[] { 12, 0, 10, 2, 4, 0, 18, 1029, 9 },
                new ushort[] { 10, 34, 12, 1, 4, 1, 10, 64, 16, 16384, 5, 0, 6, 9 },
                new ushort[] { 10, 34, 12, 2, 4, 2, 10, 69, 16, 16384, 5, 1, 6, 9 },
                new ushort[] { 10, 34, 12, 3, 4, 3, 6, 9 },
                new ushort[] { 10, 30, 12, 4, 4, 4, 3, 2, 6, 9 },
                new ushort[] { 10, 32, 12, 5, 4, 5, 10, 68, 16, 16384, 5, 2, 6, 9 },
                new ushort[] { 10, 33, 12, 6, 18, 1019, 4, 6, 10, 65, 16, 16384, 5, 3, 6, 9 },
                new ushort[] { 10, 36, 12, 7, 4, 7, 6, 9 },
                new ushort[] { 10, 30, 12, 8, 4, 8, 6, 9 },
                new ushort[] { 10, 73, 12, 9, 17, 1792, 4, 9, 10, 67, 16, 49152, 5, 4, 6, 9 },
                new ushort[] { 10, 74, 12, 10, 4, 10, 10, 63, 16, 16384, 5, 5, 6, 9 },
                new ushort[] { 10, 31, 12, 11, 4, 11, 10, 71, 16, 16384, 5, 6, 6, 9 },
                new ushort[] { 10, 78, 17, 5120, 12, 12, 4, 12, 6, 9 },
                new ushort[] { 10, 33, 12, 13, 4, 13, 10, 72, 16, 16384, 5, 7, 6, 9 },
                new ushort[] { 10, 27, 12, 14, 14, 22, 4, 14, 10, 70, 16, 16384, 5, 8, 6, 9 },
                new ushort[] { 10, 27, 12, 15, 4, 15, 6, 9 },
                new ushort[] { 3, 3, 10, 28, 12, 16, 4, 16, 10, 62, 16, 16384, 5, 9, 6, 9 },
                new ushort[] { 10, 30, 12, 17, 4, 17, 6, 9 },
                new ushort[] { 10, 26, 12, 18, 4, 18, 10, 66, 16, 16384, 5, 10, 6, 9 },
                new ushort[] { 10, 26, 12, 19, 4, 19, 3, 4, 15, 9 },
                new ushort[] { 10, 30, 12, 20, 17, 10000, 4, 20, 6, 9 },
                new ushort[] { 18, 1016, 18, 1016, 10, 74, 12, 21, 7, 21, 9 },
                new ushort[] { 10, 34, 12, 22, 7, 22, 9 },
                new ushort[] { 10, 34, 12, 23, 7, 23, 9 }
            };

            CompareUShortArrays(expectedScriptData, _script.ScriptData);
        }

        [TestMethod]
        protected void TestTitleData()
        {
            List<string> expectedTitles = new()
            {
                @"data\title.PSX", @"pixUK\titleuk.raw", @"pixUK\legaluk.raw", @"pixUS\titleUS.raw",
                @"pixUS\legalUS.raw", @"pixJAP\titleJAP.raw", @"pixJAP\legalJAP.raw"
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