using TRGE.Coord;
using TRGE.Extension;

namespace TRGE.Core.Test;

public class TR2ExtensionTests : AbstractTRExtensionTests
{
    protected override string DataDirectory => @"ImportExport\TR2PC";

    protected override TREdition Edition => TREdition.TR2PC;

    protected override void Setup()
    {
        base.Setup();
        TRLevelEditorFactory.RegisterEditor(Edition.Version, typeof(TRLevelEditorExtensionExample));
    }

    protected override void TearDown()
    {
        TRLevelEditorFactory.DeregisterEditor(Edition.Version, typeof(TRLevelEditorExtensionExample));
        base.TearDown();
    }
}