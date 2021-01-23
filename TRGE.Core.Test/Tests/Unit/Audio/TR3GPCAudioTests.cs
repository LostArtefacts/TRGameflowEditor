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
            [AbstractTRScriptedLevel.CreateID(@"data\scotland.TR2")] = 36,
            [AbstractTRScriptedLevel.CreateID(@"data\willsden.TR2")] = 30,
            [AbstractTRScriptedLevel.CreateID(@"data\chunnel.TR2")] = 74,
            [AbstractTRScriptedLevel.CreateID(@"data\undersea.TR2")] = 27,
            [AbstractTRScriptedLevel.CreateID(@"data\zoo.TR2")] = 34,
            [AbstractTRScriptedLevel.CreateID(@"data\slinc.TR2")] = 26
        };

        internal override Dictionary<string, ushort> NewLevelTracks => _newTracks;

        protected Dictionary<string, ushort> _newTracks = new Dictionary<string, ushort>
        {
            ["SECRET"] = 50,
            [AbstractTRScriptedLevel.CreateID(@"data\willsden.TR2")] = 35,
            [AbstractTRScriptedLevel.CreateID(@"data\undersea.TR2")] = 28
        };
    }
}