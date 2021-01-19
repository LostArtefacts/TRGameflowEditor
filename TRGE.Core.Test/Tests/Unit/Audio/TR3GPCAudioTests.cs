using System.Collections.Generic;

namespace TRGE.Core.Test
{
    public class TR3GPCAudioTests : AbstractTR23AudioTestCollection
    {
        protected override int ScriptFileIndex => 6;
        protected override ushort SampleTrack => 122;

        internal override Dictionary<string, ushort> ExpectedLevelTracks => new Dictionary<string, ushort>
        {
            ["TITLE"] = 5,
            ["SECRET"] = 2,
            [AbstractTRLevel.CreateID(@"data\scotland.TR2")] = 36,
            [AbstractTRLevel.CreateID(@"data\willsden.TR2")] = 30,
            [AbstractTRLevel.CreateID(@"data\chunnel.TR2")] = 74,
            [AbstractTRLevel.CreateID(@"data\undersea.TR2")] = 27,
            [AbstractTRLevel.CreateID(@"data\zoo.TR2")] = 34,
            [AbstractTRLevel.CreateID(@"data\slinc.TR2")] = 26
        };

        internal override Dictionary<string, ushort> NewLevelTracks => _newTracks;

        protected Dictionary<string, ushort> _newTracks = new Dictionary<string, ushort>
        {
            ["SECRET"] = 50,
            [AbstractTRLevel.CreateID(@"data\willsden.TR2")] = 35,
            [AbstractTRLevel.CreateID(@"data\undersea.TR2")] = 28
        };
    }
}