using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TRGE.Core.Test
{
    [TestClass]
    public class TR2PSXItemTests : AbstractTR2ItemTestCollection
    {
        protected override int ScriptFileIndex => 4;
        internal override Dictionary<string, List<TRItem>> ManualBonusData => new()
        {
            { 
                AbstractTRScriptedLevel.CreateID(@"data\deck.PSX"), new List<TRItem>
                {
                    ExpectedItems[2], ExpectedItems[15]
                }
            },
            {
                AbstractTRScriptedLevel.CreateID(@"data\skidoo.PSX"), new List<TRItem>
                {
                    ExpectedItems[6], ExpectedItems[13], ExpectedItems[14]
                }
            }
        };
    }
}