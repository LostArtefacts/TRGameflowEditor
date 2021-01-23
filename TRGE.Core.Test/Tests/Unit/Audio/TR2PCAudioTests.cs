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
            [AbstractTRScriptedLevel.CreateID(@"data\wall.TR2")] = 33, 
            [AbstractTRScriptedLevel.CreateID(@"data\boat.TR2")] = 0, 
            [AbstractTRScriptedLevel.CreateID(@"data\venice.TR2")] = 0,
            [AbstractTRScriptedLevel.CreateID(@"data\opera.TR2")] = 31, 
            [AbstractTRScriptedLevel.CreateID(@"data\rig.TR2")] = 58,
            [AbstractTRScriptedLevel.CreateID(@"data\platform.TR2")] = 58,
            [AbstractTRScriptedLevel.CreateID(@"data\unwater.TR2")] = 34,
            [AbstractTRScriptedLevel.CreateID(@"data\keel.TR2")] = 31,
            [AbstractTRScriptedLevel.CreateID(@"data\living.TR2")] = 34,
            [AbstractTRScriptedLevel.CreateID(@"data\deck.TR2")] = 31,
            [AbstractTRScriptedLevel.CreateID(@"data\skidoo.TR2")] = 33,
            [AbstractTRScriptedLevel.CreateID(@"data\monastry.TR2")] = 0,
            [AbstractTRScriptedLevel.CreateID(@"data\catacomb.TR2")] = 31,
            [AbstractTRScriptedLevel.CreateID(@"data\icecave.TR2")] = 31,
            [AbstractTRScriptedLevel.CreateID(@"data\emprtomb.TR2")] = 59,
            [AbstractTRScriptedLevel.CreateID(@"data\floating.TR2")] = 59,
            [AbstractTRScriptedLevel.CreateID(@"data\xian.TR2")] = 59,
            [AbstractTRScriptedLevel.CreateID(@"data\house.TR2")] = 0
        };

        internal override Dictionary<string, ushort> NewLevelTracks => _newTracks;

        protected Dictionary<string, ushort> _newTracks = new Dictionary<string, ushort>
        {
            ["SECRET"] = 50,
            [AbstractTRScriptedLevel.CreateID(@"data\keel.TR2")] = 35,
            [AbstractTRScriptedLevel.CreateID(@"data\icecave.TR2")] = 28
        };
    }
}