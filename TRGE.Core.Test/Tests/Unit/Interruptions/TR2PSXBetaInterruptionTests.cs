using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TRGE.Core.Test;

[TestClass]
public class TR2PSXBetaInterruptionTests : AbstractTR23InterruptionTestCollection
{
    protected override int ScriptFileIndex => 3;
    protected override bool ExpectedCutScenes => true;
    protected override bool ExpectedFrontEndFMV => true;
    protected override bool ExpectedLevelsFMV => true;
    protected override bool ExpectedLevelsStartAnimation => true;
}