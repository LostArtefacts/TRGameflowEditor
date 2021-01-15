using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TRGE.Core.Test
{
    [TestClass]
    public class TR3PSXUnarmedTests : AbstractTR23UnarmedTestCollection
    {
        protected override int ScriptFileIndex => 5;

        protected override string[] LevelNames => new string[]
        {
            "Jungle", "Temple Ruins", "The River Ganges", "Caves Of Kaliya",
            "Coastal Village", "Crash Site", "Madubu Gorge", "Temple Of Puna",
            "Thames Wharf", "Aldwych", "Lud's Gate", "City",
            "Nevada Desert", "High Security Compound", "Area 51",
            "Antarctica", "RX-Tech Mines", "Lost City Of Tinnos","Meteorite Cavern",
            "All Hallows"
        };

        protected override string[] LevelFileNames => new string[]
        {
            @"data\jungle.PSX", @"data\temple.PSX", @"data\quadchas.PSX", @"data\tonyboss.PSX",
            @"data\shore.PSX", @"data\crash.PSX", @"data\rapids.PSX", @"data\triboss.PSX",
            @"data\roofs.PSX", @"data\sewer.PSX", @"data\tower.PSX", @"data\office.PSX",
            @"data\nevada.PSX", @"data\compound.PSX", @"data\area51.PSX",
            @"data\antarc.PSX", @"data\mines.PSX", @"data\city.PSX", @"data\chamber.PSX",
            @"data\stpaul.PSX"
        };
    }
}