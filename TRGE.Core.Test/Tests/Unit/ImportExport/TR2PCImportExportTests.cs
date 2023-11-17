using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TRGE.Core.Test;

[TestClass]
public class TR2PCImportExportTests : AbstractTR23ImportExportTestCollection
{
    protected override string DataDirectory => @"ImportExport\TR2PC";
}