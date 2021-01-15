using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TRGE.Core.Test
{
    [TestClass]
    public class TR2GPCUnarmedTests : AbstractTR23UnarmedTestCollection
    {
        protected override int ScriptFileIndex => 1;

        protected override string[] LevelNames => new string[]
        {
            "The Cold War", "Fool's Gold", "Furnace of the Gods", "Kingdom", "Nightmare In Vegas"
        };

        protected override string[] LevelFileNames => new string[]
        {
            @"data\level1.TR2", @"data\level2.TR2", @"data\level3.TR2", @"data\level4.TR2", @"data\level5.TR2"
        };
    }
}