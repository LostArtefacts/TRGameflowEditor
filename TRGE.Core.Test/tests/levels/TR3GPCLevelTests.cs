using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TRGE.Core.Test
{
    [TestClass]
    public class TR3GPCLevelTests : AbstractTR23LevelTestCollection
    {
        protected override int ScriptFileIndex => 6;
        protected override TREdition Edition => TREdition.TR3G;

        protected override string[] LevelNames => new string[]
        {
            "Highland Fling", "Willard's Lair", "Shakespeare Cliff", "Sleeping with the Fishes",
            "It's a Madhouse!", "Reunion"
        };

        protected override string[] LevelFileNames => new string[]
        {
            @"data\scotland.TR2", @"data\willsden.TR2", @"data\chunnel.TR2", @"data\undersea.TR2",
            @"data\zoo.TR2", @"data\slinc.TR2"
        };
    }
}