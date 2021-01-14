using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TRGE.Core
{
    public class TR23ScriptManager : AbstractTRScriptManager
    {
        internal TR23ScriptManager(string originalFilePath)
            : base(originalFilePath, new TR23Script()) { }

        internal TR23ScriptManager(string originalFilePath, TR23Script script)
            : base(originalFilePath, script) { }

        protected override Dictionary<string, object> CreateConfig()
        {
            throw new NotImplementedException();
        }

        protected override void LoadConfig(Dictionary<string, object> config)
        {
            throw new NotImplementedException();
        }

        protected override void PrepareSave()
        {
            throw new NotImplementedException();
        }
    }
}