using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TRGE.Core.Test
{
    [TestClass]
    public class TR23MiscTests : BaseTestCollection
    {
        [TestMethod]
        protected void TestDemoTime()
        {
            foreach (string scriptFile in _validScripts)
            {
                TestDemoTime(TRScriptFactory.OpenScript(scriptFile) as TR23Script);
            }
        }

        private void TestDemoTime(TR23Script script)
        {
            uint newDemoTime = script.DemoTimeSeconds + 300;
            script.DemoTimeSeconds = newDemoTime;
            Assert.IsTrue(script.DemoTimeSeconds == newDemoTime);
            Assert.IsTrue(SaveAndReload(script).DemoTimeSeconds == newDemoTime);
        }
    }
}