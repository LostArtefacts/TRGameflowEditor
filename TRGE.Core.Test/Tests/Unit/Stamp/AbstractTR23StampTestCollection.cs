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

        [TestMethod]
        protected virtual void TestStampLanguages()
        {
            TRInterop.ScriptModificationStamp[TRLanguage.French] = "Essai";
            TRInterop.ScriptModificationStamp[TRLanguage.German] = "Prüfung";
            TRInterop.ScriptModificationStamp[TRLanguage.American] = "Test US";
            TRInterop.ScriptModificationStamp[TRLanguage.Japanese] = "テスト";

            TREditor editor = TRCoord.Instance.Open(_validScripts[ScriptFileIndex]);
            TR23Script script = editor.ScriptEditor.Script as TR23Script;

            SaveAndTestLanguage(editor, script, TRLanguage.French);

            editor = TRCoord.Instance.Open(_validScripts[ScriptFileIndex]);
            script = editor.ScriptEditor.Script as TR23Script;
            SaveAndTestLanguage(editor, script, TRLanguage.German);

            editor = TRCoord.Instance.Open(_validScripts[ScriptFileIndex]);
            script = editor.ScriptEditor.Script as TR23Script;
            SaveAndTestLanguage(editor, script, TRLanguage.American);

            editor = TRCoord.Instance.Open(_validScripts[ScriptFileIndex]);
            script = editor.ScriptEditor.Script as TR23Script;
            SaveAndTestLanguage(editor, script, TRLanguage.Japanese);
        }

        private void SaveAndTestLanguage(TREditor editor, TR23Script script, TRLanguage lang)
        {
            script.TRLanguage = lang;
            editor.Save();
            Assert.IsTrue(script.GameStrings1[InventoryIndex].EndsWith(TRInterop.ScriptModificationStamp[lang]));
        }
    }
}