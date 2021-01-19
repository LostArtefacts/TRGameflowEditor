using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TRGE.Core.Test
{
    [TestClass]
    public class TR3PSXInterruptionTests : AbstractTR23InterruptionTestCollection
    {
        protected override int ScriptFileIndex => 5;
        protected override bool ExpectedCutScenes => true;
        protected override bool ExpectedFrontEndFMV => true;
        protected override bool ExpectedLevelsFMV => true;
        protected override bool ExpectedLevelsStartAnimation => false;
    }
}