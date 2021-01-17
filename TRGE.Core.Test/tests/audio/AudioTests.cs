using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TRGE.Core.Test
{
    [TestClass]
    public class AudioTests : BaseTestCollection
    {
        [TestMethod]
        protected void TestTR2Audio()
        {
            TR23ScriptManager sm = TRGameflowEditor.Instance.GetScriptManager(_validScripts[0]) as TR23ScriptManager;
            try
            {
                Console.WriteLine((sm.Script as TR23Script).TitleSound);
            }
            finally
            {
                TRGameflowEditor.Instance.CloseScriptManager(sm);
            }
        }

        [TestMethod]
        protected void TestTR3Audio()
        {
            TR23ScriptManager sm = TRGameflowEditor.Instance.GetScriptManager(_validScripts[2]) as TR23ScriptManager;
            try
            {
                Console.WriteLine((sm.Script as TR23Script).TitleSound);
                TR3AudioProvider provider = new TR3AudioProvider(@"audio\cdaudio.wad");
                TR3AudioEntry entry = provider.GetTrack((sm.Script as TR23Script).TitleSound);
                byte[] data = provider.GetWavData(entry);
                System.IO.File.WriteAllBytes(entry.Name, data);
            }
            finally
            {
                TRGameflowEditor.Instance.CloseScriptManager(sm);
            }
        }
    }
}