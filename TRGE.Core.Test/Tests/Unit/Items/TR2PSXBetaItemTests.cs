using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace TRGE.Core.Test
{
    [TestClass]
    public class TR2PSXBetaItemTests : AbstractTR2ItemTestCollection
    {
        protected override int ScriptFileIndex => 3;
        internal override Dictionary<string, List<TRItem>> ManualBonusData => new Dictionary<string, List<TRItem>>
        {
            {
                AbstractTRLevel.CreateID(@"data\venice.PSX"), new List<TRItem>
                {
                    ExpectedItems[2], ExpectedItems[15]
                }
            },
            {
                AbstractTRLevel.CreateID(@"data\floating.PSX"), new List<TRItem>
                {
                    ExpectedItems[6], ExpectedItems[13], ExpectedItems[14]
                }
            }
        };
    }
}