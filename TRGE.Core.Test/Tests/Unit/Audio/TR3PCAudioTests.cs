namespace TRGE.Core.Test
{
    public class TR3PCAudioTests : AbstractTR23AudioTestCollection
    {
        protected override int ScriptFileIndex => 2;
        protected override ushort SampleTrack => 122;

        internal override Dictionary<string, ushort> ExpectedLevelTracks => new()
        {
            ["TITLE"] = 5,
            ["ASSAULT"] = 2,
            [AbstractTRScriptedLevel.CreateID(@"data\jungle.TR2")] = 34,
            [AbstractTRScriptedLevel.CreateID(@"data\temple.TR2")] = 34,
            [AbstractTRScriptedLevel.CreateID(@"data\quadchas.TR2")] = 34,
            [AbstractTRScriptedLevel.CreateID(@"data\tonyboss.TR2")] = 30,
            [AbstractTRScriptedLevel.CreateID(@"data\shore.TR2")] = 32,
            [AbstractTRScriptedLevel.CreateID(@"data\crash.TR2")] = 33,
            [AbstractTRScriptedLevel.CreateID(@"data\rapids.TR2")] = 36,
            [AbstractTRScriptedLevel.CreateID(@"data\triboss.TR2")] = 30,
            [AbstractTRScriptedLevel.CreateID(@"data\roofs.TR2")] = 73,
            [AbstractTRScriptedLevel.CreateID(@"data\sewer.TR2")] = 74,
            [AbstractTRScriptedLevel.CreateID(@"data\tower.TR2")] = 31,
            [AbstractTRScriptedLevel.CreateID(@"data\office.TR2")] = 78,
            [AbstractTRScriptedLevel.CreateID(@"data\nevada.TR2")] = 33,
            [AbstractTRScriptedLevel.CreateID(@"data\compound.TR2")] = 27,
            [AbstractTRScriptedLevel.CreateID(@"data\area51.TR2")] = 27,
            [AbstractTRScriptedLevel.CreateID(@"data\antarc.TR2")] = 28,
            [AbstractTRScriptedLevel.CreateID(@"data\mines.TR2")] = 30,
            [AbstractTRScriptedLevel.CreateID(@"data\city.TR2")] = 26,
            [AbstractTRScriptedLevel.CreateID(@"data\chamber.TR2")] = 26,
            [AbstractTRScriptedLevel.CreateID(@"data\stpaul.TR2")] = 30
        };

        internal override Dictionary<string, ushort> NewLevelTracks => _newTracks;

        protected Dictionary<string, ushort> _newTracks = new()
        {
            ["TITLE"] = 50,
            [AbstractTRScriptedLevel.CreateID(@"data\quadchas.TR2")] = 35,
            [AbstractTRScriptedLevel.CreateID(@"data\nevada.TR2")] = 28
        };
    }
}