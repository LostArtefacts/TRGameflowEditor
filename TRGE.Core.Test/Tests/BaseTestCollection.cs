using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TRGE.Core.Test
{
    public class BaseTestCollection : AbstractTestCollection
    {
        protected string _testOutputPath;

        [ClassInitialize]
        protected override void Setup()
        {
            _testOutputPath = @"scripts\SCRIPT_OUTPUT.dat";
        }

        [ClassCleanup]
        protected override void TearDown()
        {
            File.Delete(_testOutputPath);
        }

        internal TR23Script SaveAndReload(TR23Script script)
        {
            File.WriteAllBytes(_testOutputPath, script.SerialiseScriptToBin());
            return TRScriptFactory.OpenScript(_testOutputPath) as TR23Script;
        }
    }
}