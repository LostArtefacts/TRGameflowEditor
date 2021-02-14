using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TRGE.Coord;
using TRGE.Core;

namespace TRGE.Extension
{
    public class TRLevelEditorExtensionExample : TR2LevelEditor
    {
        public int CustomInt { get; set; }
        public bool CustomBool { get; set; }

        public TRLevelEditorExtensionExample(TRDirectoryIOArgs args)
            : base(args) { }

        protected override void ApplyConfig(Dictionary<string, object> config)
        {
            CustomInt = int.Parse(config["CustomInt"].ToString());
            CustomBool = bool.Parse(config["CustomBool"].ToString());
        }

        protected override void StoreConfig(Dictionary<string, object> config)
        {
            config["CustomInt"] = CustomInt;
            config["CustomBool"] = CustomBool;
        }

        protected override int GetSaveTarget(int numLevels)
        {
            return CustomBool ? numLevels : 0;
        }

        protected override void SaveImpl(AbstractTRScriptEditor scriptEditor, TRSaveMonitor monitor)
        {
            if (CustomBool)
            {
                foreach (AbstractTRScriptedLevel level in scriptEditor.Levels)
                {
                    monitor.FireSaveStateBeginning(TRSaveCategory.Custom, string.Format("Doing action X on {0}", level.Name));
                    //do some action
                    monitor.FireSaveStateChanged(1);
                }
            }
        }
    }
}