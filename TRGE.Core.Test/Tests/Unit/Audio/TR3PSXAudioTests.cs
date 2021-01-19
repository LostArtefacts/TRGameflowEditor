using System.Collections.Generic;

namespace TRGE.Core.Test
{
    public class TR3PSXAudioTests : AbstractTR23AudioTestCollection
    {
        protected override int ScriptFileIndex => 5;
        protected override ushort SampleTrack => 122;

        internal override Dictionary<string, ushort> ExpectedLevelTracks => new Dictionary<string, ushort>
        {
            ["TITLE"] = 5,
            ["SECRET"] = 2,
            [AbstractTRLevel.CreateID(@"data\jungle.PSX")] = 34,
            [AbstractTRLevel.CreateID(@"data\temple.PSX")] = 34,
            [AbstractTRLevel.CreateID(@"data\quadchas.PSX")] = 34,
            [AbstractTRLevel.CreateID(@"data\tonyboss.PSX")] = 30,
            [AbstractTRLevel.CreateID(@"data\shore.PSX")] = 32,
            [AbstractTRLevel.CreateID(@"data\crash.PSX")] = 33,
            [AbstractTRLevel.CreateID(@"data\rapids.PSX")] = 36,
            [AbstractTRLevel.CreateID(@"data\triboss.PSX")] = 30,
            [AbstractTRLevel.CreateID(@"data\roofs.PSX")] = 73,
            [AbstractTRLevel.CreateID(@"data\sewer.PSX")] = 74,
            [AbstractTRLevel.CreateID(@"data\tower.PSX")] = 31,
            [AbstractTRLevel.CreateID(@"data\office.PSX")] = 78,
            [AbstractTRLevel.CreateID(@"data\nevada.PSX")] = 33,
            [AbstractTRLevel.CreateID(@"data\compound.PSX")] = 27,
            [AbstractTRLevel.CreateID(@"data\area51.PSX")] = 27,
            [AbstractTRLevel.CreateID(@"data\antarc.PSX")] = 28,
            [AbstractTRLevel.CreateID(@"data\mines.PSX")] = 30,
            [AbstractTRLevel.CreateID(@"data\city.PSX")] = 26,
            [AbstractTRLevel.CreateID(@"data\chamber.PSX")] = 26,
            [AbstractTRLevel.CreateID(@"data\stpaul.PSX")] = 30
        };

        internal override Dictionary<string, ushort> NewLevelTracks => _newTracks;

        protected Dictionary<string, ushort> _newTracks = new Dictionary<string, ushort>
        {
            ["TITLE"] = 50,
            [AbstractTRLevel.CreateID(@"data\quadchas.PSX")] = 35,
            [AbstractTRLevel.CreateID(@"data\nevada.PSX")] = 28
        };
    }
}