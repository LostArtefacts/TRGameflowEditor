using System.Collections.Generic;

namespace TRGE.Core.Test
{
    public class TR2GPCAudioTests : AbstractTR23AudioTestCollection
    {
        protected override int ScriptFileIndex => 1;
        protected override ushort SampleTrack => 47;

        internal override Dictionary<string, ushort> ExpectedLevelTracks => new Dictionary<string, ushort>
        {
            ["TITLE"] = 64,
            ["SECRET"] = 47,
            [AbstractTRLevel.CreateID(@"data\level1.TR2")] = 33,
            [AbstractTRLevel.CreateID(@"data\level2.TR2")] = 58,
            [AbstractTRLevel.CreateID(@"data\level3.TR2")] = 59,
            [AbstractTRLevel.CreateID(@"data\level4.TR2")] = 31,
            [AbstractTRLevel.CreateID(@"data\level5.TR2")] = 34
        };

        internal override Dictionary<string, ushort> NewLevelTracks => _newTracks;

        protected Dictionary<string, ushort> _newTracks = new Dictionary<string, ushort>
        {
            ["SECRET"] = 50,
            [AbstractTRLevel.CreateID(@"data\level1.TR2")] = 35,
            [AbstractTRLevel.CreateID(@"data\level4.TR2")] = 28
        };
    }
}