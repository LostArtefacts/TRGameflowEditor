using Microsoft.VisualStudio.TestTools.UnitTesting;
using TRGE.Coord;

namespace TRGE.Core.Test
{
    public abstract class AbstractTR23StampTestCollection : BaseTestCollection
    {
        protected abstract int ScriptFileIndex { get; }
        protected abstract int GameIndex { get; }
        protected abstract int InventoryIndex { get; }

        [TestMethod]
        protected virtual void TestStampGame()
        {
            TREditor editor = TRCoord.Instance.Open(_validScripts[ScriptFileIndex]);
            TR23Script script = editor.ScriptEditor.Script as TR23Script;
            Assert.AreEqual(script.GameStrings1[GameIndex], "Game");
            editor.Save();
            Assert.AreNotEqual(script.GameStrings1[GameIndex], "Game");

            editor = TRCoord.Instance.Open(_validScripts[ScriptFileIndex]);
            script = editor.ScriptEditor.Script as TR23Script;
            Assert.AreEqual(script.GameStrings1[GameIndex], "Game");
        }

        [TestMethod]
        protected virtual void TestStampInventory()
        {
            TREditor editor = TRCoord.Instance.Open(_validScripts[ScriptFileIndex]);
            TR23Script script = editor.ScriptEditor.Script as TR23Script;
            Assert.AreEqual(script.GameStrings1[InventoryIndex], "INVENTORY");
            editor.Save();
            Assert.AreNotEqual(script.GameStrings1[InventoryIndex], "INVENTORY");

            editor = TRCoord.Instance.Open(_validScripts[ScriptFileIndex]);
            script = editor.ScriptEditor.Script as TR23Script;
            Assert.AreEqual(script.GameStrings1[InventoryIndex], "INVENTORY");
        }
    }
}