using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TRGE.Core.Test
{
    [TestClass]
    public class TR3PCInterruptionTests : AbstractTR23InterruptionTestCollection
    {
        protected override int ScriptFileIndex => 2;
        protected override bool ExpectedCutScenes => true;
        protected override bool ExpectedFrontEndFMV => true;
        protected override bool ExpectedLevelsFMV => true;
        protected override bool ExpectedLevelsStartAnimation => false;
    }
}