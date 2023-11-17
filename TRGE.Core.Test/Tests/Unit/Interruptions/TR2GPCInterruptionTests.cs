using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TRGE.Core.Test;

[TestClass]
public class TR2GPCInterruptionTests : AbstractTR23InterruptionTestCollection
{
    protected override int ScriptFileIndex => 1;
    protected override bool ExpectedCutScenes => false;
    protected override bool ExpectedFrontEndFMV => true;
    protected override bool ExpectedLevelsFMV => false;
    protected override bool ExpectedLevelsStartAnimation => false;
}