using System.Collections.Generic;

namespace TRGE.Core.Test
{
    public class TR3PCAudioTests : AbstractTR23AudioTestCollection
    {
        protected override int ScriptFileIndex => 2;
        protected override ushort SampleTrack => 122;

        internal override Dictionary<string, ushort> ExpectedLevelTracks => new Dictionary<string, ushort>
        {
            ["TITLE"] = 5,
            ["SECRET"] = 0,
            [AbstractTRLevel.CreateID(@"data\jungle.TR2")] = 34,
            [AbstractTRLevel.CreateID(@"data\temple.TR2")] = 34,
            [AbstractTRLevel.CreateID(@"data\quadchas.TR2")] = 34,
            [AbstractTRLevel.CreateID(@"data\tonyboss.TR2")] = 30,
            [AbstractTRLevel.CreateID(@"data\shore.TR2")] = 32,
            [AbstractTRLevel.CreateID(@"data\crash.TR2")] = 33,
            [AbstractTRLevel.CreateID(@"data\rapids.TR2")] = 36,
            [AbstractTRLevel.CreateID(@"data\triboss.TR2")] = 30,
            [AbstractTRLevel.CreateID(@"data\roofs.TR2")] = 73,
            [AbstractTRLevel.CreateID(@"data\sewer.TR2")] = 74,
            [AbstractTRLevel.CreateID(@"data\tower.TR2")] = 31,
            [AbstractTRLevel.CreateID(@"data\office.TR2")] = 78,
            [AbstractTRLevel.CreateID(@"data\nevada.TR2")] = 33,
            [AbstractTRLevel.CreateID(@"data\compound.TR2")] = 27,
            [AbstractTRLevel.CreateID(@"data\area51.TR2")] = 27,
            [AbstractTRLevel.CreateID(@"data\antarc.TR2")] = 28,
            [AbstractTRLevel.CreateID(@"data\mines.TR2")] = 30,
            [AbstractTRLevel.CreateID(@"data\city.TR2")] = 26,
            [AbstractTRLevel.CreateID(@"data\chamber.TR2")] = 26,
            [AbstractTRLevel.CreateID(@"data\stpaul.TR2")] = 30
        };

        internal override Dictionary<string, ushort> NewLevelTracks => _newTracks;

        protected Dictionary<string, ushort> _newTracks = new Dictionary<string, ushort>
        {
            ["TITLE"] = 50,
            [AbstractTRLevel.CreateID(@"data\quadchas.TR2")] = 35,
            [AbstractTRLevel.CreateID(@"data\nevada.TR2")] = 28
        };
    }
}