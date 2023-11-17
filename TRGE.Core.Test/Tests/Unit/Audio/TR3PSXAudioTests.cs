namespace TRGE.Core.Test;

public class TR3PSXAudioTests : AbstractTR23AudioTestCollection
{
    protected override int ScriptFileIndex => 5;
    protected override ushort SampleTrack => 122;

    internal override Dictionary<string, ushort> ExpectedLevelTracks => new()
    {
        ["TITLE"] = 5,
        ["ASSAULT"] = 2,
        [AbstractTRScriptedLevel.CreateID(@"data\jungle.PSX")] = 34,
        [AbstractTRScriptedLevel.CreateID(@"data\temple.PSX")] = 34,
        [AbstractTRScriptedLevel.CreateID(@"data\quadchas.PSX")] = 34,
        [AbstractTRScriptedLevel.CreateID(@"data\tonyboss.PSX")] = 30,
        [AbstractTRScriptedLevel.CreateID(@"data\shore.PSX")] = 32,
        [AbstractTRScriptedLevel.CreateID(@"data\crash.PSX")] = 33,
        [AbstractTRScriptedLevel.CreateID(@"data\rapids.PSX")] = 36,
        [AbstractTRScriptedLevel.CreateID(@"data\triboss.PSX")] = 30,
        [AbstractTRScriptedLevel.CreateID(@"data\roofs.PSX")] = 73,
        [AbstractTRScriptedLevel.CreateID(@"data\sewer.PSX")] = 74,
        [AbstractTRScriptedLevel.CreateID(@"data\tower.PSX")] = 31,
        [AbstractTRScriptedLevel.CreateID(@"data\office.PSX")] = 78,
        [AbstractTRScriptedLevel.CreateID(@"data\nevada.PSX")] = 33,
        [AbstractTRScriptedLevel.CreateID(@"data\compound.PSX")] = 27,
        [AbstractTRScriptedLevel.CreateID(@"data\area51.PSX")] = 27,
        [AbstractTRScriptedLevel.CreateID(@"data\antarc.PSX")] = 28,
        [AbstractTRScriptedLevel.CreateID(@"data\mines.PSX")] = 30,
        [AbstractTRScriptedLevel.CreateID(@"data\city.PSX")] = 26,
        [AbstractTRScriptedLevel.CreateID(@"data\chamber.PSX")] = 26,
        [AbstractTRScriptedLevel.CreateID(@"data\stpaul.PSX")] = 30
    };

    internal override Dictionary<string, ushort> NewLevelTracks => _newTracks;

    protected Dictionary<string, ushort> _newTracks = new()
    {
        ["TITLE"] = 50,
        [AbstractTRScriptedLevel.CreateID(@"data\quadchas.PSX")] = 35,
        [AbstractTRScriptedLevel.CreateID(@"data\nevada.PSX")] = 28
    };
}