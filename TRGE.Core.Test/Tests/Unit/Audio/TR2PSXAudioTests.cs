using System.Collections.Generic;

namespace TRGE.Core.Test
{
    public class TR2PSXAudioTests : AbstractTR23AudioTestCollection
    {
        protected override int ScriptFileIndex => 4;
        protected override ushort SampleTrack => 47;

        internal override Dictionary<string, ushort> ExpectedLevelTracks => new Dictionary<string, ushort>
        {
            ["TITLE"] = 64,
            ["SECRET"] = 47,
            [AbstractTRLevel.CreateID(@"data\wall.PSX")] = 33,
            [AbstractTRLevel.CreateID(@"data\boat.PSX")] = 0,
            [AbstractTRLevel.CreateID(@"data\venice.PSX")] = 0,
            [AbstractTRLevel.CreateID(@"data\opera.PSX")] = 31,
            [AbstractTRLevel.CreateID(@"data\rig.PSX")] = 58,
            [AbstractTRLevel.CreateID(@"data\platform.PSX")] = 58,
            [AbstractTRLevel.CreateID(@"data\unwater.PSX")] = 34,
            [AbstractTRLevel.CreateID(@"data\keel.PSX")] = 31,
            [AbstractTRLevel.CreateID(@"data\living.PSX")] = 34,
            [AbstractTRLevel.CreateID(@"data\deck.PSX")] = 31,
            [AbstractTRLevel.CreateID(@"data\skidoo.PSX")] = 33,
            [AbstractTRLevel.CreateID(@"data\monastry.PSX")] = 0,
            [AbstractTRLevel.CreateID(@"data\catacomb.PSX")] = 31,
            [AbstractTRLevel.CreateID(@"data\icecave.PSX")] = 31,
            [AbstractTRLevel.CreateID(@"data\emprtomb.PSX")] = 59,
            [AbstractTRLevel.CreateID(@"data\floating.PSX")] = 59,
            [AbstractTRLevel.CreateID(@"data\xian.PSX")] = 59,
            [AbstractTRLevel.CreateID(@"data\house.PSX")] = 0
        };

        internal override Dictionary<string, ushort> NewLevelTracks => _newTracks;

        protected Dictionary<string, ushort> _newTracks = new Dictionary<string, ushort>
        {
            ["SECRET"] = 50,
            [AbstractTRLevel.CreateID(@"data\keel.PSX")] = 35,
            [AbstractTRLevel.CreateID(@"data\icecave.PSX")] = 28
        };
    }
}