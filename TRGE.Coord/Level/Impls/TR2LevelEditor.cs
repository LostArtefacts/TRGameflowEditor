using TRGE.Core;
using TRLevelControl;
using TRLevelControl.Helpers;
using TRLevelControl.Model;

namespace TRGE.Coord;

public class TR2LevelEditor(TRDirectoryIOArgs io, TREdition edition)
    : BaseTRLevelEditor(io, edition)
{
    internal override bool ShouldHandleModification(TRScriptedLevelEventArgs e)
    {
        if (_edition.Remastered)
        {
            return false;
        }

        return e.Modification switch
        {
            TRScriptedLevelModification.SunsetChanged => true,
            _ => base.ShouldHandleModification(e),
        };
    }

    internal override void ProcessModification(TRScriptedLevelEventArgs e)
    {
        switch (e.Modification)
        {
            case TRScriptedLevelModification.SunsetChanged:
                HandleSunsetStateChanged(e);
                break;
            default:
                base.ProcessModification(e);
                break;
        }
    }

    protected override void HandleWeaponlessStateChanged(TRScriptedLevelEventArgs e)
    {
        string levelFile = GetReadLevelFilePath(e.LevelFileBaseName);
        if (!File.Exists(levelFile))
        {
            throw new IOException(string.Format("Missing level file {0}", levelFile));
        }

        TR2LevelControl control = new();
        TR2Level level = control.Read(levelFile);
        SetDefaultWeaponsAvailable(level, e.ScriptedLevel);
        control.Write(level, GetWriteLevelFilePath(e.LevelFileBaseName));
    }

    protected virtual void SetDefaultWeaponsAvailable(TR2Level level, AbstractTRScriptedLevel scriptedLevel)
    {
        Location defaultLocation = GetDefaultUnarmedLocationForLevel(scriptedLevel)
            ?? throw new IOException(string.Format("There is no default weapon location defined for {0} ({1})", scriptedLevel.Name, scriptedLevel.LevelFileBaseName));
        IEnumerable<TR2Entity> existingInjections = level.Entities.Where
        (
            e =>
                e.Room == defaultLocation.Room &&
                e.X == defaultLocation.X &&
                e.Y == defaultLocation.Y &&
                e.Z == defaultLocation.Z &&
                (
                    e.TypeID == TR2Type.Pistols_S_P || TR2TypeUtilities.IsGunType(e.TypeID) || TR2TypeUtilities.IsAmmoType(e.TypeID)
                )
        );

        // #69 Offshore Rig - the Pistol item index is low in the list (#4), so if we remove it we'd need to change
        // everything that references the items above it in the list. So, for simplicity's sake, we will just
        // replace the pistols with something else if the level is now armed, or revert it to pistols.
        if (scriptedLevel.Is("RIG"))
        {
            TR2Entity cargoEntity = existingInjections.FirstOrDefault();
            if (cargoEntity != null)
            {
                if (scriptedLevel.RemovesWeapons)
                {
                    cargoEntity.TypeID = TR2Type.Pistols_S_P;
                }
                else
                {
                    cargoEntity.TypeID = TR2Type.UziAmmo_S_P; //need a way to be able to define this somewhere
                }
            }
            else if (!scriptedLevel.RemovesWeapons)
            {
                cargoEntity = level.Entities.Find(e => e.Room == 1 && e.TypeID == TR2Type.Pistols_S_P);
                if (cargoEntity != null)
                {
                    cargoEntity.TypeID = TR2Type.UziAmmo_S_P;
                }
            }
        }
        else if (scriptedLevel.RemovesWeapons)
        {
            //only inject if it hasn't been done already i.e. no pistols, other weapon or ammo found in the default spot
            if (!existingInjections.Any())
            {
                defaultLocation = GetUnarmedLocationForLevel(scriptedLevel);
                level.Entities.Add(new()
                {
                    TypeID = TR2Type.Pistols_S_P,
                    Room = defaultLocation.Room,
                    X = defaultLocation.X,
                    Y = defaultLocation.Y,
                    Z = defaultLocation.Z,
                    Angle = 0,
                    Intensity1 = -1,
                    Intensity2 = -1,
                    Flags = 0
                });
            }
        }
    }

    protected override void InitialiseUnarmedRNG(AbstractTRScriptEditor scriptEditor)
    {
        if (_edition.Remastered)
        {
            return;
        }

        // #84 If randomizing unarmed locations, keep a reference to the same RNG that is used to randomize the levels
        var editor = scriptEditor as TRXScriptEditor;
        if (_randomiseUnarmedLocations = editor.UnarmedLevelOrganisation == Organisation.Random)
        {
            _unarmedRng = editor.UnarmedLevelRNG.Create();
        }
    }

    // If the Sunset flag is set in the script, rooms with lighting set to type 3 (and with this also set
    // for each vertex inthe room) will dim over 20 minutes in the game. We don't need to undo this as it 
    // will be ignored if the script flag isn't set.
    // https://opentomb.github.io/TRosettaStone3/trosettastone.html#_the_whole_room_structure
    protected virtual void HandleSunsetStateChanged(TRScriptedLevelEventArgs e)
    {
        if (!e.LevelHasSunset)
        {
            return;
        }

        string levelFile = GetReadLevelFilePath(e.LevelFileBaseName);
        if (!File.Exists(levelFile))
        {
            throw new IOException(string.Format("Missing level file {0}", levelFile));
        }

        TR2LevelControl control = new();
        TR2Level level = control.Read(levelFile);

        bool changesMade = false;
        foreach (TR2Room room in level.Rooms)
        {
            if (room.LightMode == 0)
            {
                room.LightMode = 3;
                changesMade = true;
                foreach (TR2RoomVertex vert in room.Mesh.Vertices)
                {
                    vert.Attributes |= 3;
                }
            }
        }

        if (changesMade)
        {
            control.Write(level, GetWriteLevelFilePath(e.LevelFileBaseName));
        }
    }
}
