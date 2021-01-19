using System.Collections.Generic;

namespace TRGE.Core.Test
{
    public class TR2PCAudioTests : AbstractTR23AudioTestCollection
    {
        protected override int ScriptFileIndex => 0;
        protected override ushort SampleTrack => 47;

        internal override Dictionary<string, ushort> ExpectedLevelTracks => new Dictionary<string, ushort>
        {
            ["TITLE"] = 64,
            ["SECRET"] = 47,
            [AbstractTRLevel.CreateID(@"data\wall.TR2")] = 33, 
            [AbstractTRLevel.CreateID(@"data\boat.TR2")] = 0, 
            [AbstractTRLevel.CreateID(@"data\venice.TR2")] = 0,
            [AbstractTRLevel.CreateID(@"data\opera.TR2")] = 31, 
            [AbstractTRLevel.CreateID(@"data\rig.TR2")] = 58,
            [AbstractTRLevel.CreateID(@"data\platform.TR2")] = 58,
            [AbstractTRLevel.CreateID(@"data\unwater.TR2")] = 34,
            [AbstractTRLevel.CreateID(@"data\keel.TR2")] = 31,
            [AbstractTRLevel.CreateID(@"data\living.TR2")] = 34,
            [AbstractTRLevel.CreateID(@"data\deck.TR2")] = 31,
            [AbstractTRLevel.CreateID(@"data\skidoo.TR2")] = 33,
            [AbstractTRLevel.CreateID(@"data\monastry.TR2")] = 0,
            [AbstractTRLevel.CreateID(@"data\catacomb.TR2")] = 31,
            [AbstractTRLevel.CreateID(@"data\icecave.TR2")] = 31,
            [AbstractTRLevel.CreateID(@"data\emprtomb.TR2")] = 59,
            [AbstractTRLevel.CreateID(@"data\floating.TR2")] = 59,
            [AbstractTRLevel.CreateID(@"data\xian.TR2")] = 59,
            [AbstractTRLevel.CreateID(@"data\house.TR2")] = 0
        };

        internal override Dictionary<string, ushort> NewLevelTracks => _newTracks;

        protected Dictionary<string, ushort> _newTracks = new Dictionary<string, ushort>
        {
            ["SECRET"] = 50,
            [AbstractTRLevel.CreateID(@"data\keel.TR2")] = 35,
            [AbstractTRLevel.CreateID(@"data\icecave.TR2")] = 28
        };
    }
}