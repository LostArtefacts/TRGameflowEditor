using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TRGE.Core.Test
{
    public class TR2PSXBetaScriptIOTests : AbstractTestCollection
    {
        private string _validFilePath, _invalidFilePath;
        private TR23Script _script;

        [ClassInitialize]
        protected override void Setup()
        {
            _invalidFilePath = @"scripts\INVALID.dat";
            _validFilePath = @"scripts\TOMBPSX_BETA_TR2.dat";
        }

        [ClassCleanup]
        protected override void TearDown() { }

        [TestMethod]
        protected void TestOpenInvalidScript()
        {
            try
            {
                ScriptFactory.OpenScript(_invalidFilePath);
                Assert.Fail();
            }
            catch (UnsupportedScriptException)
            {
                Assert.IsTrue(true);
            }
        }

        [TestMethod]
        protected void TestOpenValidScript()
        {
            try
            {
                AbstractTRScript script = ScriptFactory.OpenScript(_validFilePath);
                Assert.IsTrue(script is TR23Script);
                Assert.IsTrue(script.Edition == TREdition.TR2PSXBETA);
                _script = script as TR23Script;
            }
            catch (UnsupportedScriptException)
            {
                Assert.Fail();
            }
        }
    }
}