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

        protected override void ApplyConfig(Config config)
        {
            CustomInt = config.GetInt("CustomInt", 10);
            CustomBool = config.GetBool("CustomBool", true);
        }

        protected override void StoreConfig(Config config)
        {
            config["CustomInt"] = CustomInt;
            config["CustomBool"] = CustomBool;
        }

        protected override int GetSaveTarget(int numLevels)
        {
            return base.GetSaveTarget(numLevels) + (CustomBool ? numLevels : 0);
        }

        protected override void SaveImpl(AbstractTRScriptEditor scriptEditor, TRSaveMonitor monitor)
        {
            if (CustomBool)
            {
                foreach (AbstractTRScriptedLevel level in scriptEditor.EnabledScriptedLevels)
                {
                    monitor.FireSaveStateBeginning(TRSaveCategory.Custom, string.Format("Doing action X on {0}", level.Name));
                    //do some action
                    monitor.FireSaveStateChanged(1);
                }

                if (scriptEditor.GymAvailable)
                {
                    monitor.FireSaveStateBeginning(TRSaveCategory.Custom, string.Format("Doing action X on {0}", scriptEditor.AssaultLevel.Name));
                    //do some action
                    monitor.FireSaveStateChanged(1);
                }
            }

            // Simulate re-saving the script
            scriptEditor.SaveScript();
        }
    }
}