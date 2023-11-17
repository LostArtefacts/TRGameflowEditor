using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TRGE.Core.Test;

[TestClass]
public class TR3AudioIntegrationTests : AbstractTR23AudioIntegrationTestCollection
{
    protected override int ScriptFileIndex => 0;

    [TestMethod]
    protected void BuildAudioJson()
    {
        TR23AudioExtractor ext = new("audiostuff");
        ext.BuildTR3AudioJson();
    }
}