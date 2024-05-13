using Newtonsoft.Json;
using TRGE.Core;

namespace TRGE.Coord;

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
        return e.Modification switch
        {
            TRScriptedLevelModification.WeaponlessStateChanged => true,
            _ => false,
        };
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

        // Copy the entire backup into the WIP folder ready for what lies ahead
        IOExtensions.Copy(_io.BackupDirectory, _io.OutputDirectory);
        IOExtensions.Copy(_io.BackupDirectory, _io.WIPOutputDirectory);

        PreSaveImpl(scriptEditor);
    }

    protected virtual void InitialiseUnarmedRNG(AbstractTRScriptEditor scriptEditor) { }

    protected virtual void PreSaveImpl(AbstractTRScriptEditor scriptEditor) { }
}