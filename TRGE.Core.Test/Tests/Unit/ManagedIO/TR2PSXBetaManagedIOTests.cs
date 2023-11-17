using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TRGE.Core.Test;

[TestClass]
public class TR2PSXBetaManagedIOTests : TR2PCManagedIOTests
{
    protected override int ScriptFileIndex => 3;
}