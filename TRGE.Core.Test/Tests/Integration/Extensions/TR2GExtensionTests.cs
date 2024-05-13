using TRGE.Coord;
using TRGE.Extension;

namespace TRGE.Core.Test;

public class TR2GExtensionTests : AbstractTRExtensionTests
{
    protected override string DataDirectory => @"ImportExport\TR2G";

    protected override TREdition Edition => TREdition.TR2G;

    protected override void Setup()
    {
        base.Setup();
        TRLevelEditorFactory.RegisterEditor(Edition, typeof(TRLevelEditorExtensionExample));
    }

    protected override void TearDown()
    {
        TRLevelEditorFactory.DeregisterEditor(Edition);
        base.TearDown();
    }
}