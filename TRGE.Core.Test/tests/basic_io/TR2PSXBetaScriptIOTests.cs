using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;

namespace TRGE.Core.Test
{
    [TestClass]
    public class TR2PSXBetaScriptIOTests : AbstractTestCollection
    {
        private string _validFilePath, _invalidFilePath;
        private TR23Script _script;

        [ClassInitialize]
        protected override void Setup()
        {
            _invalidFilePath = @"scripts\INVALID.dat";
            _validFilePath = @"scripts\TOMBPSX_BETA_TR2.dat";
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
                Assert.IsTrue(script.Edition == TREdition.TR2PSXBeta);
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
            Assert.IsTrue(_script.DeathDemoMode == 1280);
            Assert.IsTrue(_script.DemoEnd == 1280);
            Assert.IsTrue(_script.DemoInterrupt == 1280);
            Assert.IsTrue(_script.DemoTime == 900);
            Assert.IsTrue(_script.NumDemoLevels == 0);
        }

        [TestMethod]
        protected void TestFMVData()
        {
            List<uint[]> excpectedFMVData = new List<uint[]>
            {
                new uint[] { 1, 880 },
                new uint[] { 1, 4370 },
                new uint[] { 1, 315 },
                new uint[] { 1, 365 },
                new uint[] { 1, 990 },
                new uint[] { 1, 1856 }
            };

            CompareUIntArrays(excpectedFMVData, _script.PSXFMVData);
        }

        [TestMethod]
        protected void TestGameStringData()
        {
            List<string> expectedStrings1 = new List<string>
            {
                "INVENTORY","OPTION","ITEMS","GAME OVER","Load Game","Save Game","New Game","Restart Level","Exit to Title","Exit Demo",
                "Exit Game","Select Level","Save Position","Select Detail","High","Medium","Low","Walk","Roll","Run","Left","Right","Back",
                "Step Left","Step Right","Look","Jump","Action","Draw Weapon","Inventory","Flare","Step","Statistics","Pistols","Shotgun","Automatic Pistols",
                "Uzis","Harpoon Gun","M16","Grenade Launcher","Flare","Pistol Clips","Shotgun Shells","Automatic Pistol Clips","Uzi Clips","Harpoons","M16 Clips",
                "Grenades","Small Medi Pack","Large Medi Pack","Pickup","Puzzle","Key","Game","Lara's Home","LOADING","Time Taken",
                "Secrets Found","Location","Kills","Ammo Used","Hit/Miss Ratio","Saves Performed","Distance Travelled","Health Packs Used","Magazine Preview","spare",
                "spare","spare","spare","spare","spare","spare","spare","spare","spare","spare","spare","spare","spare","spare","spare","spare","spare","spare","spare"
            };

            List<string> expectedStrings2 = new List<string>
            {
                "Screen Adjust","DEMO MODE","Sound","Controls","Gamma","Set Volumes","Control Method","The file could not be saved!",
                "Try Again?","YES","NO","Save Complete!","No save games!","None valid","Save Game?","- EMPTY SLOT -",
                "Press any button to quit","Use directional buttons","to adjust screen position","> Select","; Go Back","> Continue",
                "Paused","Controller Removed","Save game to","Overwrite game on","Load game from","the Memory card in","Memory card slot 1",
                "Are you sure ?","Yes","No","is full","There are no games on","There is a problem with","There is no Memory card in",
                "is unformatted","Would you like to","format it ?","Saving game to","Loading game from","Formatting","Overwrite game","Save game",
                "Load game","Format Memory card","Exit","Continue","You will not be able","to save any games unless","you insert a formatted",
                "Memory card with at least","one free block","The Memory card in","Exit without saving","Exit without loading","Start game",
                "UNFORMATTED MEMORY CARD","Insert a formatted","or press > to continue","without saving.","bu00:BESLES-00718TOMB2","bu00:BASLUS-00152TOMB2",
                "spare","spare","spare","spare","spare","spare","spare","spare","spare","spare","spare","spare","spare","spare","spare","spare"
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
                "Key1 0","Guardhouse Key","Boathouse Key","Library Key","Ornate Key","Red Pass Card","Red Pass Card","Key1 7","Rest Room Key",
                "Theatre Key","Key1 10","Drawbridge Key","Strongroom Key","Key1 13","Key1 14","Key1 15","Key1 16","Key1 17","Gun Cupboard Key"
            };

            List<string> expectedKeys2 = new List<string>
            {
                "Key2 0","Rusty Key","Steel Key","Detonator Key","Key2 4","Yellow Pass Card","Key2 6","Key2 7","Rusty Key","Key2 9","Stern Key",
                "Hut Key","Trapdoor Key","Key2 13","Gong Hammer","Gold Key","Key2 16","Key2 17","Key2 18"
            };

            List<string> expectedKeys3 = new List<string>
            {
                "Key3 0","Key3 1","Iron Key","Key3 3","Key3 4","Green Pass Card","Key3 6","Key3 7","The Bridge Key","Key3 9","Storage Key","Key3 11",
                "Rooftops Key","Key3 13","Key3 14","Silver Key","Key3 16","Key3 17","Key3 18"
            };

            List<string> expectedKeys4 = new List<string>
            {
                "Key4 0","Key4 1","Key4 2","Key4 3","Key4 4","Key4 5","Blue Pass Card","Key4 7","Key4 8","Key4 9","Cabin Key","Key4 11","Main Hall Key",
                "Key4 13","Key4 14","Main Chamber Key","Key4 16","Key4 17","Key4 18"
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
                "Lara's Home A",
                "The Great Wall W", "Venice B", "Bartoli's Hideout V", "Opera House O", "Offshore Rig R",
                "Diving Area P", "40 Fathoms U", "Wreck of the Maria Doria K", "Living Quarters L",
                "The Deck D", "Tibetan Foothills S", "Barkhang Monastery M", "Catacombs of the Talion C",
                "Ice Palace I", "Temple of Xian E", "Floating Islands F", "Dragon's Lair X", "Home Sweet Home H"
            };

            List<string> expectedLevelFileNames = new List<string>
            {
                @"data\assault.PSX",
                @"data\wall.PSX", @"data\boat.PSX", @"data\venice.PSX", @"data\opera.PSX", @"data\rig.PSX",
                @"data\platform.PSX", @"data\unwater.PSX", @"data\keel.PSX", @"data\living.PSX",
                @"data\deck.PSX", @"data\skidoo.PSX", @"data\monastry.PSX", @"data\catacomb.PSX",
                @"data\icecave.PSX", @"data\emprtomb.PSX", @"data\floating.PSX", @"data\xian.PSX", @"data\house.PSX"
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
            Assert.IsTrue(_script.Description.Equals("Tomb Raider II Script. Release Version 1.0 (c) Core Design Ltd 1997"));
            Assert.IsTrue(_script.FirstOption == 1280);
            Assert.IsTrue(_script.Flags == 768);
            Assert.IsTrue(_script.GameflowSize == 128);
            Assert.IsTrue(_script.Language == 0);
            Assert.IsTrue(_script.SecretSound == 47);
            Assert.IsTrue(_script.SingleLevel == 65535);
            Assert.IsTrue(_script.TitleReplace == -1);
            Assert.IsTrue(_script.TitleSound == 2);
            Assert.IsTrue(_script.Xor == 0);
        }

        [TestMethod]
        protected void TestPickupStringData()
        {
            List<string> expectedPickups1 = new List<string>
            {
                "Pick1 0","Pick1 1","Pick1 2","Pick1 3","Pick1 4","Pick1 5","Pick1 6","Pick1 7","Pick1 8","Pick1 9",
                "Pick1 10","Pick1 11","Pick1 12","Pick1 13","Pick1 14","Pick1 15","Pick1 16","Pick1 17","Pick1 18"
            };

            List<string> expectedPickups2 = new List<string>
            {
                "Pick2 0","Pick2 1","Pick2 2","Pick2 3","Pick2 4","Pick2 5","Pick2 6","Pick2 7","Pick2 8","Pick2 9",
                "Pick2 10","Pick2 11","Pick2 12","Pick2 13","Talion","Pick2 15","Pick2 16","Pick2 17","Pick2 18"
            };

            CompareStrings(expectedPickups1, _script.PickupNames1);
            CompareStrings(expectedPickups2, _script.PickupNames2);
        }

        [TestMethod]
        protected void TestPictureData()
        {
            List<string> expectedPictures = new List<string>
            {
                @"pix\assault.raw",@"pix\wall.raw",@"pix\boat.raw",@"pix\venice.raw",@"pix\opera.raw",@"pix\rig.raw",@"pix\platform.raw",
                @"pix\unwater.raw",@"pix\keel.raw",@"pix\living.raw",@"pix\deck.raw",@"pix\skidoo.raw",@"pix\monastry.raw",@"pix\catacomb.raw",
                @"pix\icecave.raw",@"pix\emprtomb.raw",@"pix\floating.raw",@"pix\xian.raw",@"pix\house.raw"
            };

            Assert.IsTrue(_script.NumPictures == expectedPictures.Count);
            CompareStrings(expectedPictures, _script.PictureNames);
        }

        [TestMethod]
        protected void TestPuzzleStringData()
        {
            List<string> expectedPuzzles1 = new List<string>
            {
                "Puz1 0","Puz1 1","Puz1 2","Puz1 3","Relay Box","Puz1 5","Machine Chip","Puz1 7","Circuit Breaker","Puz1 9","Puz1 10","Puz1 11",
                "Prayer Wheels","Tibetan Mask","Tibetan Mask","The Dragon Seal","Mystic Plaque","Puz1 17","Puz1 18"
            };

            List<string> expectedPuzzles2 = new List<string>
            {
                "Puz2 0","Puz2 1","Puz2 2","Puz2 3","Circuit Board","Puz2 5","Puz2 6","Puz2 7","Puz2 8","Puz2 9","Puz2 10","Puz2 11","Gemstones",
                "Puz2 13","Puz2 14","Puz2 15","Mystic Plaque","Puz2 17","Puz2 18"
            };

            List<string> expectedPuzzles3 = new List<string>
            {
                "Puz3 0","Puz3 1","Puz3 2","Puz3 3","Puz3 4","Puz3 5","Puz3 6","Puz3 7","Puz3 8","Puz3 9","Puz3 10",
                "Puz3 11","Puz3 12","Puz3 13","Puz3 14","Puz3 15","Puz3 16","Puz3 17","Puz3 18"
            };

            List<string> expectedPuzzles4 = new List<string>
            {
                "Puz4 0","Puz4 1","Puz4 2","Puz4 3","Puz4 4","Puz4 5","Puz4 6","Puz4 7","Puz4 8","Puz4 9","Puz4 10","The Seraph",
                "The Seraph","Puz4 13","Puz4 14","Puz4 15","Puz4 16","Puz4 17","Puz4 18"
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
                @"\FMV\MS.FMV", @"\FMV\JEEP.FMV"
            };

            Assert.IsTrue(_script.NumRPLs == expectedRPLs.Count);
            CompareStrings(expectedRPLs, _script.RPLFileNames);
        }

        [TestMethod]
        protected void TestSecretStringData()
        {
            List<string> expectedSecrets1 = new List<string>
            {
                "Sec1 0","Sec1 1","Sec1 2","Sec1 3","Sec1 4","Sec1 5","Sec1 6","Sec1 7","Sec1 8","Sec1 9",
                "Sec1 10","Sec1 11","Sec1 12","Sec1 13","Sec1 14","Sec1 15","Sec1 16","Sec1 17","Sec1 18"
            };

            List<string> expectedSecrets2 = new List<string>
            {
                "Sec2 0","Sec2 1","Sec2 2","Sec2 3","Sec2 4","Sec2 5","Sec2 6","Sec2 7","Sec2 8","Sec2 9",
                "Sec2 10","Sec2 11","Sec2 12","Sec2 13","Sec2 14","Sec2 15","Sec2 16","Sec2 17","Sec2 18"
            };

            List<string> expectedSecrets3 = new List<string>
            {
                "Sec3 0","Sec3 1","Sec3 2","Sec3 3","Sec3 4","Sec3 5","Sec3 6","Sec3 7","Sec3 8","Sec3 9",
                "Sec3 10","Sec3 11","Sec3 12","Sec3 13","Sec3 14","Sec3 15","Sec3 16","Sec3 17","Sec3 18"
            };

            List<string> expectedSecrets4 = new List<string>
            {
                "Sec4 0","Sec4 1","Sec4 2","Sec4 3","Sec4 4","Sec4 5","Sec4 6","Sec4 7","Sec4 8",
                "Sec4 9","Sec4 10","Sec4 11","Sec4 12","Sec4 13","Sec4 14","Sec4 15","Sec4 16","Sec4 17","Sec4 18"
            };

            CompareStrings(expectedSecrets1, _script.SecretNames1);
            CompareStrings(expectedSecrets2, _script.SecretNames2);
            CompareStrings(expectedSecrets3, _script.SecretNames3);
            CompareStrings(expectedSecrets4, _script.SecretNames4);
        }

        [TestMethod]
        protected void TestScriptData()
        {
            List<ushort[]> expectedScriptData = new List<ushort[]>
            {
                new ushort[] { 3, 0, 3, 1, 9 },
                new ushort[] { 20, 0, 12, 0, 10, 33, 4, 0, 9 },
                new ushort[] { 3, 2, 10, 33, 12, 1, 4, 1, 10, 3, 16, 0, 5, 0, 18, 6, 18, 13, 18, 13, 18, 15, 6, 9 },
                new ushort[] { 10, 0, 10, 32, 12, 2, 4, 2, 18, 9, 18, 9, 18, 9, 18, 9, 6, 9 },
                new ushort[] { 10, 0, 11, 12, 3, 4, 3, 18, 8, 18, 8, 18, 8, 18, 8, 6, 9 },
                new ushort[] { 10, 31, 12, 4, 4, 4, 10, 4, 5, 1, 18, 3, 18, 10, 18, 10, 18, 10, 18, 10, 6, 9 },
                new ushort[] { 3, 3, 10, 58, 19, 8, 14, 12, 5, 4, 5, 18, 3, 18, 10, 18, 10, 6, 9 },
                new ushort[] { 10, 58, 12, 6, 4, 6, 10, 5, 5, 2, 18, 10, 18, 10, 18, 10, 18, 10, 6, 9 },
                new ushort[] { 3, 4, 10, 34, 12, 7, 4, 7, 18, 11, 18, 11, 18, 11, 18, 11, 6, 9 },
                new ushort[] { 10, 31, 12, 8, 4, 8, 18, 6, 18, 13, 18, 13, 6, 9 },
                new ushort[] { 10, 32, 12, 9, 4, 9, 18, 12, 18, 12, 18, 12, 18, 12, 6, 9 },
                new ushort[] { 10, 32, 12, 10, 4, 10, 18, 13, 18, 13, 18, 13, 18, 13, 6, 9 },
                new ushort[] { 10, 33, 10, 52, 10, 53, 18, 22, 12, 11, 4, 11, 18, 10, 18, 10, 18, 10, 18, 10, 6, 9 },
                new ushort[] { 10, 0, 18, 22, 12, 12, 4, 12, 18, 12, 18, 12, 18, 12, 18, 12, 6, 9 },
                new ushort[] { 10, 31, 12, 13, 4, 13, 18, 13, 18, 13, 18, 12, 18, 12, 6, 9 },
                new ushort[] { 10, 32, 13, 12, 14, 4, 14, 18, 13, 18, 13, 18, 13, 18, 13, 6, 9 },
                new ushort[] { 3, 5, 10, 59, 12, 15, 4, 15, 10, 30, 16, 0, 5, 3, 18, 10, 18, 10, 18, 10, 18, 10, 18, 10, 18, 10, 18, 10, 18, 10, 6, 9 },
                new ushort[] { 10, 59, 17, 4608, 12, 16, 4, 16, 18, 13, 18, 13, 18, 13, 18, 13, 18, 13, 18, 13, 18, 13, 18, 13, 6, 9 },
                new ushort[] { 20, 0, 10, 59, 12, 17, 4, 17, 6, 9 },
                new ushort[] { 20, 0, 18, 23, 19, 8, 21, 10, 18, 12, 18, 14, 4, 18, 6, 15, 9 }
            };

            CompareUShortArrays(expectedScriptData, _script.ScriptData);
        }

        [TestMethod]
        protected void TestSpecialStringData()
        {
            List<string> expectedSpecial1 = new List<string>
            {
                "Spe1 0","Spe1 1","Spe1 2","Spe1 3","Spe1 4","Spe1 5","Spe1 6","Spe1 7","Spe1 8","Spe1 9",
                "Spe1 10","Spe1 11","Spe1 12","Spe1 13","Spe1 14","Spe1 15","Spe1 16","Spe1 17","Spe1 18"
            };

            List<string> expectedSpecial2 = new List<string>
            {
                "Spe2 0","Spe2 1","Spe2 2","Spe2 3","Spe2 4","Spe2 5","Spe2 6","Spe2 7","Spe2 8","Spe2 9",
                "Spe2 10","Spe2 11","Spe2 12","Spe2 13","Spe2 14","Spe2 15","Spe2 16","Spe2 17","Spe2 18"
            };

            CompareStrings(expectedSpecial1, _script.SpecialNames1);
            CompareStrings(expectedSpecial2, _script.SpecialNames2);
        }

        [TestMethod]
        protected void TestTitleData()
        {
            List<string> expectedTitles = new List<string>
            {
                @"data\title.PSX", @"pix\title.raw", @"pix\bull.raw"
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