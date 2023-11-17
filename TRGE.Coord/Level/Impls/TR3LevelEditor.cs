using TRFDControl;
using TRFDControl.Utilities;
using TRGE.Core;
using TRLevelControl;
using TRLevelControl.Helpers;
using TRLevelControl.Model;

namespace TRGE.Coord;

public class TR3LevelEditor : BaseTRLevelEditor
{
    private readonly TR3LevelControl _control;

    public TR3LevelEditor(TRDirectoryIOArgs io, TREdition edition)
        : base(io, edition)
    {
        _control = new();
    }

    private TR3Level ReadLevel(string lvl)
    {
        string levelFile = GetReadLevelFilePath(lvl);
        if (!File.Exists(levelFile))
        {
            throw new IOException(string.Format("Missing level file {0}", levelFile));
        }

        return _control.Read(levelFile);
    }

    private void WriteLevel(TR3Level level, string lvl)
    {
        _control.Write(level, GetWriteLevelFilePath(lvl));
    }

    internal override bool ShouldHandleModification(TRScriptedLevelEventArgs e)
    {
        return e.Modification switch
        {
            TRScriptedLevelModification.SequenceChanged => true,
            _ => base.ShouldHandleModification(e),
        };
    }

    internal override void ProcessModification(TRScriptedLevelEventArgs e)
    {
        switch (e.Modification)
        {
            case TRScriptedLevelModification.SequenceChanged:
                HandleSequenceChanged(e);
                break;
            default:
                base.ProcessModification(e);
                break;
        }
    }

    protected override void InitialiseUnarmedRNG(AbstractTRScriptEditor scriptEditor)
    {
        // #84 If randomizing unarmed locations, keep a reference to the same RNG that is used to randomize the levels
        TR23ScriptEditor editor = scriptEditor as TR23ScriptEditor;
        if (_randomiseUnarmedLocations = editor.UnarmedLevelOrganisation == Organisation.Random)
        {
            _unarmedRng = editor.UnarmedLevelRNG.Create();
        }
    }

    protected override void HandleWeaponlessStateChanged(TRScriptedLevelEventArgs args)
    {
        if (!args.ScriptedLevel.Enabled)
        {
            return;
        }

        TR3Level level = ReadLevel(args.LevelFileBaseName);
        AbstractTRScriptedLevel scriptedLevel = args.ScriptedLevel;
        
        Location defaultLocation = GetDefaultUnarmedLocationForLevel(scriptedLevel);
        if (defaultLocation == null)
        {
            throw new IOException(string.Format("There is no default weapon location defined for {0} ({1})", scriptedLevel.Name, scriptedLevel.LevelFileBaseName));
        }

        IEnumerable<TR3Entity> existingInjections = level.Entities.Where
        (
            e =>
                e.Room == defaultLocation.Room &&
                e.X == defaultLocation.X &&
                e.Y == defaultLocation.Y &&
                e.Z == defaultLocation.Z &&
                e.TypeID == TR3Type.Pistols_P
        );

        // For HSC change the pistols into DEagle ammo if the level is no longer unarmed
        if (scriptedLevel.Is(TR3LevelNames.HSC))
        {
            TR3Entity armouryEntity = existingInjections.FirstOrDefault();
            if (armouryEntity != null)
            {
                if (!scriptedLevel.RemovesWeapons)
                {
                    armouryEntity.TypeID = TR3Type.DeagleAmmo_P;
                }
            }
        }
        else if (scriptedLevel.RemovesWeapons)
        {
            defaultLocation = GetUnarmedLocationForLevel(scriptedLevel);
            level.Entities.Add(new()
            {
                TypeID = TR3Type.Pistols_P,
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

        WriteLevel(level, args.LevelFileBaseName);
    }

    protected void HandleSequenceChanged(TRScriptedLevelEventArgs args)
    {
        if (!args.ScriptedLevel.Enabled || _scriptEditor.LevelSequencingOrganisation == Organisation.Default)
        {
            return;
        }

        // Fish cause the game to crash if levels are off-sequence due to hardcoded offsets.
        // So we just remove their triggers.

        TR3Level level = ReadLevel(args.LevelFileBaseName);

        List<TR3Entity> fishies = level.Entities.FindAll(e => e.TypeID == TR3Type.Fish || e.TypeID == TR3Type.Piranhas_N);
        if (fishies.Count > 0)
        {
            FDControl control = new();
            control.ParseFromLevel(level);

            foreach (TR3Entity fish in fishies)
            {
                FDUtilities.RemoveEntityTriggers(level, level.Entities.IndexOf(fish), control);
            }

            control.WriteToLevel(level);
        }

        WriteLevel(level, args.LevelFileBaseName);
    }

    protected override int GetSaveTarget(int numLevels)
    {
        return -1; // We don't change Assault course
    }
}