using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;

namespace TRGE.Core.Test
{
    [TestClass]
    public class TR3PCScriptIOTests : AbstractTestCollection
    {
        private string _validFilePath, _invalidFilePath;
        private TR23Script _script;

        [ClassInitialize]
        protected override void Setup()
        {
            _invalidFilePath = @"scripts\INVALID.dat";
            _validFilePath = @"scripts\TOMBPC_TR3.dat";
        }

        [ClassCleanup]
        protected override void TearDown() { }

        [TestMethod]
        [TestSequence(0)]
        protected void TestOpenInvalidScript()
        {
            try
            {
                ScriptFactory.OpenScript(_invalidFilePath);
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
                AbstractTRScript script = ScriptFactory.OpenScript(_validFilePath);
                Assert.IsTrue(script is TR23Script);
                Assert.IsTrue(script.Edition == TREdition.TR3PC);
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
            List<string> expectedCutScenes = new List<string>
            {
                @"cuts\cut6.TR2", @"cuts\cut9.TR2", @"cuts\cut1.TR2", @"cuts\cut4.TR2",  @"cuts\cut2.TR2", @"cuts\cut5.TR2",
                @"cuts\cut11.TR2", @"cuts\cut7.TR2", @"cuts\cut8.TR2", @"cuts\cut3.TR2", @"cuts\cut12.TR2"
            };

            Assert.IsTrue(_script.NumCutScenes == expectedCutScenes.Count);
            CompareStrings(expectedCutScenes, _script.CutSceneFileNames);
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
                "Finish","BEST TIMES","No Times Set","N/A","Current Position","Final Statistics","of","Story so far...","Infada Stone","Ora Dagger",
                "The Eye Of Isis","Element 115","Savegame Crystal","London","Nevada","South Pacific Islands","Antarctica","Peru","Adventure", "s"
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
                "Racetrack Key","K1","Key Of Ganesha","Gate Key","K1","Smuggler's Key","Commander Bishop's Key","K1","K1",
                "Flue Room Key","Maintenance Key","Boiler Room Key","K1","Generator Access","Keycard Type A","Launch Code Pass",
                "Hut Key","K1","Uli Key","K1","Vault Key"
            };

            List<string> expectedKeys2 = new List<string>
            {
                "K2","K2","Key Of Ganesha","K2","K2","K2","Lt. Tuckerman's Key","K2","K2","Cathedral Key","Solomon's Key","K2",
                "K2","Detonator Switch","Keycard Type B","K2","K2","K2","K2","K2","K2"
            };

            List<string> expectedKeys3 = new List<string>
            {
                "K3","K3","Key Of Ganesha","K3","K3","K3","K3","K3","K3","K3","Solomon's Key","K3","K3","K3","K3","K3","K3","K3","K3","K3","K3"
            };

            List<string> expectedKeys4 = new List<string>
            {
                "K4","Indra Key","Key Of Ganesha","K4","K4","K4","K4","K4","K4","K4","Solomon's Key","K4","K4","K4","K4","K4","K4","K4","K4","K4","K4",
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
                "Jungle", "Temple Ruins", "The River Ganges", "Caves Of Kaliya",
                "Coastal Village", "Crash Site", "Madubu Gorge", "Temple Of Puna",
                "Thames Wharf", "Aldwych", "Lud's Gate", "City",
                "Nevada Desert", "High Security Compound", "Area 51",
                "Antarctica", "RX-Tech Mines", "Lost City Of Tinnos","Meteorite Cavern",
                "All Hallows"
            };

            List<string> expectedLevelFileNames = new List<string>
            {
                @"data\house.TR2",
                @"data\jungle.TR2", @"data\temple.TR2", @"data\quadchas.TR2", @"data\tonyboss.TR2",
                @"data\shore.TR2", @"data\crash.TR2", @"data\rapids.TR2", @"data\triboss.TR2",
                @"data\roofs.TR2", @"data\sewer.TR2", @"data\tower.TR2", @"data\office.TR2",
                @"data\nevada.TR2", @"data\compound.TR2", @"data\area51.TR2", 
                @"data\antarc.TR2", @"data\mines.TR2", @"data\city.TR2", @"data\chamber.TR2", 
                @"data\stpaul.TR2"
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
            Assert.IsTrue(_script.Description.Equals("Tomb Raider III Script. E3 Release (c) Core Design Ltd 1998"));
            Assert.IsTrue(_script.FirstOption == 1280);
            Assert.IsTrue(_script.Flags == 768);
            Assert.IsTrue(_script.GameflowSize == 128);
            Assert.IsTrue(_script.Language == 0);
            Assert.IsTrue(_script.SecretSound == 0);
            Assert.IsTrue(_script.SingleLevel == 65535);
            Assert.IsTrue(_script.TitleReplace == -1);
            Assert.IsTrue(_script.TitleSound == 5);
            Assert.IsTrue(_script.Xor == 166);
        }

        [TestMethod]
        protected void TestPickupStringData()
        {
            List<string> expectedPickups1 = new List<string>
            {
                "P1","P1","P1","P1","P1","P1","Swamp Map","P1","P1","P1",
                "P1","P1","P1","P1","P1","P1","P1","P1","P1","P1","P1"
            };

            List<string> expectedPickups2 = new List<string>
            {
                "P2","P2","P2","P2","P2","P2","P2","P2","P2","P2","P2",
                "P2","P2","P2","P2","P2","P2","P2","P2","P2","P2",
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
                @"pix\india.bmp",@"pix\india.bmp",@"pix\india.bmp",@"pix\india.bmp",
                @"pix\southpac.bmp",@"pix\southpac.bmp",@"pix\southpac.bmp",@"pix\southpac.bmp",
                @"pix\london.bmp",@"pix\london.bmp",@"pix\london.bmp",@"pix\london.bmp",
                @"pix\nevada.bmp",@"pix\nevada.bmp",@"pix\nevada.bmp",
                @"pix\antarc.bmp",@"pix\antarc.bmp",@"pix\antarc.bmp",@"pix\antarc.bmp",
                @"pix\london.bmp"
            };

            Assert.IsTrue(_script.NumPictures == expectedPictures.Count);
            CompareStrings(expectedPictures, _script.PictureNames);
        }

        [TestMethod]
        protected void TestPuzzleStringData()
        {
            List<string> expectedPuzzles1 = new List<string>
            {
                "P1","P1","Scimitar","P1","P1","Serpent Stone","P1","P1","P1","P1","Old Penny","Embalming Fluid","P1","P1",
                "Blue Security Pass","Tower Access Key","Crowbar","Crowbar","Oceanic Mask","P1","P1"
            };

            List<string> expectedPuzzles2 = new List<string>
            {
                "P2","P2","Scimitar","P2","P2","P2","P2","P2","P2","P2","Ticket","P2","P2","P2","Yellow Security Pass",
                "Code Clearance Disk","Gate Control Key","Lead Acid Battery","P2","P2","P2"
            };

            List<string> expectedPuzzles3 = new List<string>
            {
                "P3","P3","P3","P3","P3","P3","P3","P3","P3","P3","Masonic Mallet","P3","P3","P3","P3",
                "Code Clearance Disk","P3","Winch Starter","P3","P3","P3"
            };

            List<string> expectedPuzzles4 = new List<string>
            {
                "P4","P4","P4","P4","P4","P4","P4","P4","P4","P4","Ornate Star","P4","P4","P4",
                "P4","Hanger Access Key","P4","P4","P4","P4","P4"
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
                @"FMV\LOGO.RPL", @"FMV\INTR_ENG.RPL", @"FMV\LOGO.RPL", @"FMV\INTR_ENG.RPL",
                @"FMV\SAIL_ENG.RPL", @"FMV\CRSH_ENG.RPL", @"FMV\ENDGAME.RPL"
            };

            Assert.IsTrue(_script.NumRPLs == expectedRPLs.Count);
            CompareStrings(expectedRPLs, _script.RPLFileNames);
        }

        [TestMethod]
        protected void TestScriptData()
        {
            List<ushort[]> expectedScriptData = new List<ushort[]>
            {
                new ushort[] { 3, 2, 3, 3, 9 },
                new ushort[] { 12, 0, 10, 2, 4, 0, 9 },
                new ushort[] { 10, 34, 12, 1, 4, 1, 10, 64, 16, 16384, 5, 0, 6, 9 },
                new ushort[] { 10, 34, 12, 2, 4, 2, 10, 69, 16, 16384, 5, 1, 6, 9 },
                new ushort[] { 10, 34, 12, 3, 4, 3, 6, 9 },
                new ushort[] { 10, 30, 12, 4, 4, 4, 3, 4, 6, 9 },
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
                new ushort[] { 3, 5, 10, 28, 12, 16, 4, 16, 10, 62, 16, 16384, 5, 9, 6, 9 },
                new ushort[] { 10, 30, 12, 17, 4, 17, 6, 9 },
                new ushort[] { 10, 26, 12, 18, 4, 18, 10, 66, 16, 16384, 5, 10, 6, 9 },
                new ushort[] { 10, 26, 12, 19, 4, 19, 3, 6, 15, 9 },
                new ushort[] { 10, 30, 12, 20, 17, 10000, 4, 20, 6, 9 }
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
            //There seems to be an issue in the original TR3 script. I think the devs forgot to null-terminate the description properly as it contains the end of the TR2 description.
            //If we set the description to match here, it won't impact the test results.
            _script.Description += "0gn Ltd 1997";
            byte[] serialData = _script.Serialise();
            serialData[63] = 0; //see above
            CollectionAssert.AreEqual(originalData, serialData);
        }
    }
}