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
            
        }

        [TestMethod]
        protected virtual void TestStampInventory()
        {

        }
    }
}