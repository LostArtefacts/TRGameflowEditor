using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace TRGE.Core.Test
{
    [TestClass]
    public class TR2PCGItemTests : AbstractTR2ItemTestCollection
    {
        protected override int ScriptFileIndex => 1;
        internal override Dictionary<string, List<TRItem>> ManualBonusData => new Dictionary<string, List<TRItem>>
        {
            { 
                Hashing.CreateMD5(@"data\level1.TR2"), new List<TRItem>
                {
                    ExpectedItems[2], ExpectedItems[15]
                }
            },
            { 
                Hashing.CreateMD5(@"data\level2.TR2"), new List<TRItem>
                {
                    ExpectedItems[6], ExpectedItems[13], ExpectedItems[14]
                }
            }
        };
    }
}