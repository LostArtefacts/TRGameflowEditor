using Microsoft.VisualStudio.TestTools.UnitTesting;
using TRGE.Coord;

namespace TRGE.Core.Test;

public abstract class AbstractTR23ItemTestCollection : BaseTestCollection
{
    protected abstract int ScriptFileIndex { get; }
    internal abstract List<TRItem> ExpectedItems { get; }

    [TestMethod]
    [TestSequence(0)]
    protected virtual void TestLoadItems()
    {
        TR23ScriptEditor sm = TRCoord.Instance.Open(_validScripts[ScriptFileIndex]).ScriptEditor as TR23ScriptEditor;
        foreach (TRItem item in ExpectedItems)
        {
            TRItem checkItem = sm.LevelManager.ItemProvider.GetItem(item.ID);
            Assert.IsNotNull(checkItem);
            Assert.IsTrue(item.ID == checkItem.ID, string.Format("Expected item ID {0}, found {1}", item.ID, checkItem.ID));
            Assert.IsTrue(item.Category == checkItem.Category, string.Format("Expected item category {0}, found {1}", item.Category, checkItem.Category));
            Assert.IsTrue(item.Name.Equals(checkItem.Name), string.Format("Expected item name {0}, found {1}", item.Name, checkItem.Name));
        }
    }
}