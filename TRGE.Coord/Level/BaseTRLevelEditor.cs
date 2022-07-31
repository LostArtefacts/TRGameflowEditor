using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TRGE.Core;

namespace TRGE.Coord
{
    public class BaseTRLevelEditor : AbstractTRLevelEditor
    {
        protected readonly Dictionary<string, List<Location>> _defaultWeaponLocations;
        protected bool _randomiseUnarmedLocations;
        protected Random _unarmedRng;

        public BaseTRLevelEditor(TRDirectoryIOArgs io, TREdition edition)
            :base(io, edition)
        {
            _defaultWeaponLocations = JsonConvert.DeserializeObject<Dictionary<string, List<Location>>>(ReadResource(@"Locations\unarmed_locations.json"));
        }

        internal override bool ShouldHandleModification(TRScriptedLevelEventArgs e)
        {
            switch (e.Modification)
            {
                case TRScriptedLevelModification.WeaponlessStateChanged:
                    return true;
                default:
                    return false;
            }
        }

        internal override void ProcessModification(TRScriptedLevelEventArgs e)
        {
            switch (e.Modification)
            {
                case TRScriptedLevelModification.WeaponlessStateChanged:
                    HandleWeaponlessStateChanged(e);
                    break;
            }
        }

        protected virtual void HandleWeaponlessStateChanged(TRScriptedLevelEventArgs e) { }

        internal Location GetDefaultUnarmedLocationForLevel(AbstractTRScriptedLevel level)
        {
            string levelFileName = level.LevelFileBaseName.ToUpper();
            if (_defaultWeaponLocations.ContainsKey(levelFileName))
            {
                List<Location> locations = _defaultWeaponLocations[levelFileName];
                if (locations.Count > 0)
                {
                    return locations[0];
                }
            }
            return null;
        }

        internal Location GetUnarmedLocationForLevel(AbstractTRScriptedLevel level)
        {
            string levelFileName = level.LevelFileBaseName.ToUpper();
            if (_defaultWeaponLocations.ContainsKey(levelFileName))
            {
                List<Location> locations = _defaultWeaponLocations[levelFileName];
                if (locations.Count > 0)
                {
                    if (_randomiseUnarmedLocations)
                    {
                        int index = 0;
                        // This avoids getting the same location index for each level
                        for (int i = 0; i < level.Sequence; i++)
                        {
                            index = _unarmedRng.Next(0, locations.Count);
                        }
                        return locations[index];
                    }
                    return locations[0];
                }
            }
            return null;
        }

        internal sealed override void PreSave(AbstractTRScriptEditor scriptEditor)
        {
            InitialiseUnarmedRNG(scriptEditor);

            // Ensure cutscene files are copied initially
            foreach (AbstractTRScriptedLevel level in scriptEditor.Levels)
            {
                if (level.SupportsCutScenes)
                {
                    File.Copy(GetReadLevelFilePath(level.CutSceneLevel.LevelFileBaseName), GetWriteLevelFilePath(level.CutSceneLevel.LevelFileBaseName));
                }
            }

            // And assault if supported
            if (scriptEditor.GymAvailable)
            {
                File.Copy(GetReadLevelFilePath(scriptEditor.AssaultLevel.LevelFileBaseName), GetWriteLevelFilePath(scriptEditor.AssaultLevel.LevelFileBaseName));
            }

            PreSaveImpl(scriptEditor);
        }

        protected virtual void InitialiseUnarmedRNG(AbstractTRScriptEditor scriptEditor) { }

        protected virtual void PreSaveImpl(AbstractTRScriptEditor scriptEditor) { }
    }
}