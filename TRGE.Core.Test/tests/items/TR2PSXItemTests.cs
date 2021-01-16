using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace TRGE.Core.Test
{
    [TestClass]
    public class TR2PSXItemTests : AbstractTR2ItemTestCollection
    {
        protected override int ScriptFileIndex => 4;
        internal override Dictionary<string, List<TRItem>> ManualBonusData => new Dictionary<string, List<TRItem>>
        {
            { 
                Hashing.CreateMD5(@"data\deck.PSX"), new List<TRItem>
                {
                    ExpectedItems[2], ExpectedItems[15]
                }
            },
            {
                Hashing.CreateMD5(@"data\skidoo.PSX"), new List<TRItem>
                {
                    ExpectedItems[6], ExpectedItems[13], ExpectedItems[14]
                }
            }
        };
    }
}