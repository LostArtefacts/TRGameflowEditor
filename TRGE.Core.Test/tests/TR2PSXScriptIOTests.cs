using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;

namespace TRGE.Core.Test
{
    [TestClass]
    public class TR2PSXScriptIOTests : AbstractTestCollection
    {
        private string _validFilePath, _invalidFilePath;
        private TR23Script _script;

        [ClassInitialize]
        protected override void Setup()
        {
            _invalidFilePath = @"scripts\INVALID.dat";
            _validFilePath = @"scripts\TOMBPSX_TR2.dat";
        }

        [ClassCleanup]
        protected override void TearDown() { }

        [TestMethod]
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
        protected void TestOpenValidScript()
        {
            try
            {
                AbstractTRScript script = ScriptFactory.OpenScript(_validFilePath);
                Assert.IsTrue(script is TR23Script);
                Assert.IsTrue(script.Edition == TREdition.TR2PSX);
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
                @"data\cut1.PSX", @"data\cut2.PSX", @"data\cut3.PSX", @"data\cut4.PSX"
            };

            Assert.IsTrue(_script.NumCutScenes == expectedCutScenes.Count);
            CompareStrings(expectedCutScenes, _script.CutSceneFileNames);
        }

        [TestMethod]
        protected void TestDemoData()
        {
            List<ushort> expectedDemoData = new List<ushort>
            {
                19, 20, 21
            };

            Assert.IsTrue(_script.DeathDemoMode == 1280);
            CompareUShorts(expectedDemoData, _script.DemoData);
            Assert.IsTrue(_script.DemoEnd == 1280);
            Assert.IsTrue(_script.DemoInterrupt == 1280);
            Assert.IsTrue(_script.DemoTime == 900);
            Assert.IsTrue(_script.NumDemoLevels == 3);
        }

        [TestMethod]
        protected void TestFMVData()
        {
            List<uint[]> excpectedFMVData = new List<uint[]>
            {
                new uint[] { 1, 864 },
                new uint[] { 1, 4355 },
                new uint[] { 1, 300 },
                new uint[] { 1, 350 },
                new uint[] { 1, 975 },
                new uint[] { 1, 1515 },
                new uint[] { 1, 1841 },
                new uint[] { 1, 603 }
            };

            CompareUIntArrays(excpectedFMVData, _script.PSXFMVData);
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
                "Screen Adjust","DEMO MODE","Sound","Controls","Gamma","Set Volumes","Control Method","The file could not be saved!",
                "Try Again?","YES","NO","Save Complete!","No save games!","None valid","Save Game?","- EMPTY SLOT -",
                "Press any button to quit","Use directional buttons","to adjust screen position","> Select","; Go Back","> Continue",
                "Paused","Controller Removed","Save game to","Overwrite game on","Load game from","the Memory card in","Memory card slot 1",
                "Are you sure ?","Yes","No","is full","There are no","Tomb Raider II games on","There is a problem with","There is no Memory card in",
                "is unformatted","Would you like to","format it ?","Saving game to","Loading game from","Formatting","Overwrite game","Save game",
                "Load game","Format Memory card","Exit","Continue","You will not be able","to save any games unless","you insert a formatted",
                "Memory card with at least","one free block","The Memory card in","Exit without saving","Exit without loading","Start game",
                "UNFORMATTED MEMORY CARD","Insert a formatted","or press > to continue","without saving.","bu00:BESLES-00718TOMB2","bu00:BASLUS-00437TOMB2",
                "Load O.K.","Load failed","Saved O.K.","Save failed","spare","spare","spare","spare","spare","spare","spare","spare","spare","spare","spare","spare"
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
                "K1","Guardhouse Key","Boathouse Key","Library Key","Ornate Key","Red Pass Card","Red Pass Card","K1","Rest Room Key","Theatre Key",
                "K1","Drawbridge Key","Strongroom Key","K1","K1","K1","K1","K1","Gun Cupboard Key","Boathouse Key","Rest Room Key","Drawbridge Key"
            };

            List<string> expectedKeys2 = new List<string>
            {
                "K2","Rusty Key","Steel Key","Detonator Key","K2","Yellow Pass Card","K2","K2","Rusty Key","Rusty Key","Stern Key","Hut Key",
                "Trapdoor Key","K2","Gong Hammer","Gold Key","K2","K2","K2","Steel Key","Rusty Key","Hut Key"
            };

            List<string> expectedKeys3 = new List<string>
            {
                "K3","K3","Iron Key","K3","K3","Green Pass Card","K3","K3","Cabin Key","K3","Storage Key","K3","Rooftops Key","K3","K3",
                "Silver Key","K3","K3","K3","Iron Key","Cabin Key","K3"
            };

            List<string> expectedKeys4 = new List<string>
            {
                "K4","K4","K4","K4","K4","K4","Blue Pass Card","K4","K4","K4","Cabin Key","K4","Main Hall Key","K4","K4","Main Chamber Key",
                "K4","K4","K4","K4","K4","K4"
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
                "The Great Wall", "Venice", "Bartoli's Hideout", "Opera House", "Offshore Rig",
                "Diving Area", "40 Fathoms", "Wreck of the Maria Doria", "Living Quarters",
                "The Deck", "Tibetan Foothills", "Barkhang Monastery", "Catacombs of the Talion",
                "Ice Palace", "Temple of Xian", "Floating Islands", "The Dragon's Lair", "Home Sweet Home",
                "Venice", "Wreck of the Maria Doria", "Tibetan Foothills" //demos
            };

            List<string> expectedLevelFileNames = new List<string>
            {
                @"data\assault.PSX",
                @"data\wall.PSX", @"data\boat.PSX", @"data\venice.PSX", @"data\opera.PSX", @"data\rig.PSX",
                @"data\platform.PSX", @"data\unwater.PSX", @"data\keel.PSX", @"data\living.PSX",
                @"data\deck.PSX", @"data\skidoo.PSX", @"data\monastry.PSX", @"data\catacomb.PSX",
                @"data\icecave.PSX", @"data\emprtomb.PSX", @"data\floating.PSX", @"data\xian.PSX", @"data\house.PSX",
                @"data\boat.psx", @"data\keel.psx", @"data\skidoo.psx" //demos
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
            Assert.IsTrue(_script.TitleSound == 64);
            Assert.IsTrue(_script.Xor == 166);
        }

        [TestMethod]
        protected void TestPickupStringData()
        {
            List<string> expectedPickups1 = new List<string>
            {
                "P1","P1","P1","P1","P1","P1","P1","P1","P1","P1","P1","P1",
                "P1","Gong Hammer","P1","P1","P1","P1","P1","P1","P1","P1"
            };

            List<string> expectedPickups2 = new List<string>
            {
                "P2","P2","P2","P2","P2","P2","P2","P2","P2","P2","P2","P2",
                "P2","P2","Talion","P2","P2","P2","P2","P2","P2","P2"
            };

            CompareStrings(expectedPickups1, _script.PickupNames1);
            CompareStrings(expectedPickups2, _script.PickupNames2);
        }

        [TestMethod]
        protected void TestPictureData()
        {
            List<string> expectedPictures = new List<string>
            {
                @"pix\mansion.raw",@"pix\china.raw",@"pix\venice.raw",@"pix\venice.raw",@"pix\venice.raw",@"pix\rig.raw",
                @"pix\rig.raw",@"pix\titan.raw",@"pix\titan.raw",@"pix\titan.raw",@"pix\titan.raw",@"pix\tibet.raw",
                @"pix\tibet.raw",@"pix\tibet.raw",@"pix\tibet.raw",@"pix\china.raw",@"pix\china.raw",@"pix\china.raw",
                @"pix\mansion.raw",@"pix\venice.raw",@"pix\titan.raw",@"pix\tibet.raw"
            };

            Assert.IsTrue(_script.NumPictures == expectedPictures.Count);
            CompareStrings(expectedPictures, _script.PictureNames);
        }

        [TestMethod]
        protected void TestPuzzleStringData()
        {
            List<string> expectedPuzzles1 = new List<string>
            {
                "P1","P1","P1","P1","Relay Box","P1","Machine Chip","P1","Circuit Breaker","P1","P1","P1","Prayer Wheels","Tibetan Mask",
                "Tibetan Mask","The Dragon Seal","Mystic Plaque","Mystic Plaque","Dagger of Xian","P1","Circuit Breaker","P1"
            };

            List<string> expectedPuzzles2 = new List<string>
            {
                "P2","P2","P2","P2","Circuit Board","P2","P2","P2","P2","P2","P2","P2","Gemstones","P2","P2","P2","Mystic Plaque",
                "Dagger of Xian","P2","P2","P2","P2"
            };

            List<string> expectedPuzzles3 = new List<string>
            {
                "P3","P3","P3","P3","P3","P3","P3","P3","P3","P3","P3",
                "P3","P3","P3","P3","P3","P3","P3","P3","P3","P3","P3"
            };

            List<string> expectedPuzzles4 = new List<string>
            {
                "P4","P4","P4","P4","P4","P4","P4","P4","P4","P4","The Seraph","The Seraph",
                "The Seraph","P4","P4","P4","P4","P4","P4","P4","P4","The Seraph"
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
                @"\FMV\LOGO.FMV", @"\FMV\ANCIENT.FMV", @"\FMV\MODERN.FMV", @"\FMV\LANDING.FMV",
                @"\FMV\MS.FMV", @"\FMV\CRASH.FMV", @"\FMV\JEEP.FMV", @"\FMV\END.FMV"
            };

            Assert.IsTrue(_script.NumRPLs == expectedRPLs.Count);
            CompareStrings(expectedRPLs, _script.RPLFileNames);
        }

        [TestMethod]
        protected void TestScriptData()
        {
            List<ushort[]> expectedScriptData = new List<ushort[]>
            {
                new ushort[] { 3, 0, 3, 1, 9 },
                new ushort[] { 20, 0, 12, 0, 10, 0, 4, 0, 9 },
                new ushort[] { 3, 2, 10, 33, 12, 1, 18, 6, 18, 13, 18, 13, 18, 15, 4, 1, 10, 3, 16, 0, 5, 0, 6, 9 },
                new ushort[] { 10, 0, 12, 2, 18, 9, 18, 9, 18, 9, 18, 9, 4, 2, 6, 9 },
                new ushort[] { 10, 0, 11, 12, 3, 18, 8, 18, 8, 18, 8, 18, 8, 4, 3, 6, 9 },
                new ushort[] { 10, 31, 12, 4, 18, 3, 18, 10, 18, 10, 18, 10, 18, 10, 4, 4, 10, 4, 5, 1, 6, 9 },
                new ushort[] { 3, 3, 10, 58, 19, 8, 14, 12, 5, 18, 3, 18, 10, 18, 10, 4, 5, 6, 9 },
                new ushort[] { 10, 58, 12, 6, 18, 10, 18, 10, 18, 10, 18, 10, 4, 6, 10, 5, 5, 2, 6, 9 },
                new ushort[] { 3, 4, 10, 34, 12, 7, 18, 11, 18, 11, 18, 11, 18, 11, 4, 7, 6, 9 },
                new ushort[] { 10, 31, 12, 8, 18, 6, 18, 13, 18, 13, 4, 8, 6, 9 },
                new ushort[] { 10, 34, 12, 9, 18, 12, 18, 12, 18, 12, 18, 12, 4, 9, 6, 9 },
                new ushort[] { 10, 31, 12, 10, 18, 13, 18, 13, 18, 13, 18, 13, 4, 10, 6, 9 },
                new ushort[] { 3, 5, 10, 33, 18, 1022, 12, 11, 18, 10, 18, 10, 18, 10, 18, 10, 4, 11, 6, 9 },
                new ushort[] { 10, 0, 18, 1022, 12, 12, 18, 12, 18, 12, 18, 12, 18, 12, 4, 12, 6, 9 },
                new ushort[] { 10, 31, 12, 13, 18, 13, 18, 13, 18, 12, 18, 12, 4, 13, 6, 9 },
                new ushort[] { 10, 31, 13, 12, 14, 18, 13, 18, 13, 18, 13, 18, 13, 4, 14, 6, 9 },
                new ushort[] { 3, 6, 10, 59, 12, 15, 18, 10, 18, 10, 18, 10, 18, 10, 18, 10, 18, 10, 18, 10, 18, 10, 4, 15, 10, 30, 16, 0, 5, 3, 6, 9 },
                new ushort[] { 10, 59, 17, 9728, 12, 16, 18, 13, 18, 13, 18, 13, 18, 13, 18, 13, 18, 13, 18, 13, 18, 13, 4, 16, 6, 9 },
                new ushort[] { 20, 0, 10, 59, 12, 17, 4, 17, 6, 3, 7, 9 },
                new ushort[] { 20, 0, 18, 1023, 19, 9, 21, 10, 0, 12, 18, 14, 22, 4, 18, 10, 52, 15, 9 },
                new ushort[] { 10, 0, 12, 19, 7, 19, 9 },
                new ushort[] { 10, 32, 12, 20, 7, 20, 9 },
                new ushort[] { 10, 33, 12, 21, 18, 1022, 7, 21, 9 }
            };

            CompareUShortArrays(expectedScriptData, _script.ScriptData);
        }

        [TestMethod]
        protected void TestTitleData()
        {
            List<string> expectedTitles = new List<string>
            {
                @"data\title.PSX", @"pixUK\title.raw", @"pixUK\legal.raw", @"pixUS\titleUS.raw",
                @"pixUS\legalUS.raw", @"pixJAP\titleJAP.raw", @"pixJAP\legalJAP.raw"
            };

            Assert.IsTrue(_script.NumTitles == expectedTitles.Count);
            CompareStrings(expectedTitles, _script.TitleFileNames);
        }

        [TestMethod]
        protected void TestUntouchedWrite()
        {
            byte[] originalData = File.ReadAllBytes(_validFilePath);
            CollectionAssert.AreEqual(originalData, _script.Serialise());
        }
    }
}