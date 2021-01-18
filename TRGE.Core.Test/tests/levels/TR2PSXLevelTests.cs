using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TRGE.Core.Test
{
    [TestClass]
    public class TR2PSXLevelTests : AbstractTR23LevelTestCollection
    {
        protected override int ScriptFileIndex => 4;
        protected override TREdition Edition => TREdition.TR2PSX;

        protected override string[] LevelNames => new string[]
        {
            "The Great Wall", "Venice", "Bartoli's Hideout", "Opera House", "Offshore Rig",
            "Diving Area", "40 Fathoms", "Wreck of the Maria Doria", "Living Quarters",
            "The Deck", "Tibetan Foothills", "Barkhang Monastery", "Catacombs of the Talion",
            "Ice Palace", "Temple of Xian", "Floating Islands", "The Dragon's Lair", "Home Sweet Home"
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