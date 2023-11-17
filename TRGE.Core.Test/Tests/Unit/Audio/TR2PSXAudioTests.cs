namespace TRGE.Core.Test;

public class TR2PSXAudioTests : AbstractTR23AudioTestCollection
{
    protected override int ScriptFileIndex => 4;
    protected override ushort SampleTrack => 47;

    internal override Dictionary<string, ushort> ExpectedLevelTracks => new()
    {
        ["TITLE"] = 64,
        ["SECRET"] = 47,
        ["ASSAULT"] = 0,
        [AbstractTRScriptedLevel.CreateID(@"data\wall.PSX")] = 33,
        [AbstractTRScriptedLevel.CreateID(@"data\boat.PSX")] = 0,
        [AbstractTRScriptedLevel.CreateID(@"data\venice.PSX")] = 0,
        [AbstractTRScriptedLevel.CreateID(@"data\opera.PSX")] = 31,
        [AbstractTRScriptedLevel.CreateID(@"data\rig.PSX")] = 58,
        [AbstractTRScriptedLevel.CreateID(@"data\platform.PSX")] = 58,
        [AbstractTRScriptedLevel.CreateID(@"data\unwater.PSX")] = 34,
        [AbstractTRScriptedLevel.CreateID(@"data\keel.PSX")] = 31,
        [AbstractTRScriptedLevel.CreateID(@"data\living.PSX")] = 34,
        [AbstractTRScriptedLevel.CreateID(@"data\deck.PSX")] = 31,
        [AbstractTRScriptedLevel.CreateID(@"data\skidoo.PSX")] = 33,
        [AbstractTRScriptedLevel.CreateID(@"data\monastry.PSX")] = 0,
        [AbstractTRScriptedLevel.CreateID(@"data\catacomb.PSX")] = 31,
        [AbstractTRScriptedLevel.CreateID(@"data\icecave.PSX")] = 31,
        [AbstractTRScriptedLevel.CreateID(@"data\emprtomb.PSX")] = 59,
        [AbstractTRScriptedLevel.CreateID(@"data\floating.PSX")] = 59,
        [AbstractTRScriptedLevel.CreateID(@"data\xian.PSX")] = 59,
        [AbstractTRScriptedLevel.CreateID(@"data\house.PSX")] = 0
    };

    internal override Dictionary<string, ushort> NewLevelTracks => _newTracks;

    protected Dictionary<string, ushort> _newTracks = new()
    {
        ["SECRET"] = 50,
        [AbstractTRScriptedLevel.CreateID(@"data\keel.PSX")] = 35,
        [AbstractTRScriptedLevel.CreateID(@"data\icecave.PSX")] = 28
    };
}