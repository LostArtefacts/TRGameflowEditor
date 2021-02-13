using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TRGE.Core;

namespace TRGE.Coord
{
    public abstract class AbstractTRLevelEditorPlugin : AbstractTRGEEditor
    {
        public string Name { get; protected set; }

        internal AbstractTRLevelEditorPlugin(string pluginName)
        {
            Name = pluginName;
        }

        protected override void ReadConfig(Dictionary<string, object> config)
        {
            
        }

        internal virtual void PrepareEdit(TRSaveMonitor monitor) { }

        internal abstract void ApplyEdit(AbstractTRScriptedLevel level, string readFilePath, string writeFilePath, TRSaveMonitor monitor);

        internal virtual void PostEdit(TRSaveMonitor monitor) { }

        internal override void Restore() { }
    }
}