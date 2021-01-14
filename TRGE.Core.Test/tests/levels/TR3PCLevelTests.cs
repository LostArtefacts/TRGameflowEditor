using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TRGE.Core.Test
{
    [TestClass]
    public class TR3PCLevelTests : AbstractTR23LevelTestCollection
    {
        protected override int ScriptFileIndex => 2;

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
            @"data\jungle.TR2", @"data\temple.TR2", @"data\quadchas.TR2", @"data\tonyboss.TR2",
            @"data\shore.TR2", @"data\crash.TR2", @"data\rapids.TR2", @"data\triboss.TR2",
            @"data\roofs.TR2", @"data\sewer.TR2", @"data\tower.TR2", @"data\office.TR2",
            @"data\nevada.TR2", @"data\compound.TR2", @"data\area51.TR2",
            @"data\antarc.TR2", @"data\mines.TR2", @"data\city.TR2", @"data\chamber.TR2",
            @"data\stpaul.TR2"
        };
    }
}