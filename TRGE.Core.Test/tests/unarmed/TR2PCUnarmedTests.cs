using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TRGE.Core.Test
{
    [TestClass]
    public class TR2PCUnarmedTests : AbstractTR23UnarmedTestCollection
    {
        protected override int ScriptFileIndex => 0;

        protected override string[] LevelNames => new string[]
        {
            "The Great Wall", "Venice", "Bartoli's Hideout", "Opera House", "Offshore Rig",
            "Diving Area", "40 Fathoms", "Wreck of the Maria Doria", "Living Quarters",
            "The Deck", "Tibetan Foothills", "Barkhang Monastery", "Catacombs of the Talion",
            "Ice Palace", "Temple of Xian", "Floating Islands", "The Dragon's Lair", "Home Sweet Home"
        };

        protected override string[] LevelFileNames => new string[]
        {
            @"data\wall.TR2", @"data\boat.TR2", @"data\venice.TR2", @"data\opera.TR2", @"data\rig.TR2",
            @"data\platform.TR2", @"data\unwater.TR2", @"data\keel.TR2", @"data\living.TR2",
            @"data\deck.TR2", @"data\skidoo.TR2", @"data\monastry.TR2", @"data\catacomb.TR2",
            @"data\icecave.TR2", @"data\emprtomb.TR2", @"data\floating.TR2", @"data\xian.TR2", @"data\house.TR2"
        };
    }
}