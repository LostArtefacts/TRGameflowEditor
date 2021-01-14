using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TRGE.Core.Test
{
    [TestClass]
    public class TR2PSXBetaLevelTests : AbstractTR23LevelTestCollection
    {
        protected override int ScriptFileIndex => 3;

        protected override string[] LevelNames => new string[]
        {
            "The Great Wall W", "Venice B", "Bartoli's Hideout V", "Opera House O", "Offshore Rig R",
            "Diving Area P", "40 Fathoms U", "Wreck of the Maria Doria K", "Living Quarters L",
            "The Deck D", "Tibetan Foothills S", "Barkhang Monastery M", "Catacombs of the Talion C",
            "Ice Palace I", "Temple of Xian E", "Floating Islands F", "Dragon's Lair X", "Home Sweet Home H"
        };

        protected override string[] LevelFileNames => new string[]
        {
            @"data\wall.PSX", @"data\boat.PSX", @"data\venice.PSX", @"data\opera.PSX", @"data\rig.PSX",
            @"data\platform.PSX", @"data\unwater.PSX", @"data\keel.PSX", @"data\living.PSX",
            @"data\deck.PSX", @"data\skidoo.PSX", @"data\monastry.PSX", @"data\catacomb.PSX",
            @"data\icecave.PSX", @"data\emprtomb.PSX", @"data\floating.PSX", @"data\xian.PSX", @"data\house.PSX"
        };
    }
}