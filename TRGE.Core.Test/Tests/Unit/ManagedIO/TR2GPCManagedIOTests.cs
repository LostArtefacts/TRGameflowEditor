using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TRGE.Core.Test;

[TestClass]
public class TR2GPCManagedIOTests : TR2PCManagedIOTests
{
    protected override int ScriptFileIndex => 1;
}