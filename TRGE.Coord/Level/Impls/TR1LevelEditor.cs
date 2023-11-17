using TRGE.Core;
using TRLevelControl;
using TRLevelControl.Helpers;
using TRLevelControl.Model;

namespace TRGE.Coord
{
    public class TR1LevelEditor : BaseTRLevelEditor
    {
        private readonly TR1LevelControl _control;

        public TR1LevelEditor(TRDirectoryIOArgs io, TREdition edition)
            : base(io, edition)
        {
            _control = new();
        }

        private TR1Level ReadLevel(string lvl)
        {
            string levelFile = GetReadLevelFilePath(lvl);
            if (!File.Exists(levelFile))
            {
                throw new IOException(string.Format("Missing level file {0}", levelFile));
            }

            return _control.Read(levelFile);
        }

        private void WriteLevel(TR1Level level, string lvl)
        {
            _control.Write(level, GetWriteLevelFilePath(lvl));
        }

        protected override void PreSaveImpl(AbstractTRScriptEditor scriptEditor)
        {
            if (!scriptEditor.Edition.IsCommunityPatch)
            {
                // Can't guarantee that the ATI levels will have been copied to WIP, so do that now
                foreach (AbstractTRScriptedLevel level in scriptEditor.Levels)
                {
                    IOExtensions.CopyFile(GetReadLevelFilePath(level.LevelFileBaseName), GetWriteLevelFilePath(level.LevelFileBaseName), true);
                    if (level.HasCutScene)
                    {
                        IOExtensions.CopyFile(GetReadLevelFilePath(level.CutSceneLevel.LevelFileBaseName), GetWriteLevelFilePath(level.CutSceneLevel.LevelFileBaseName), true);
                    }
                }
            }
        }

        protected override void InitialiseUnarmedRNG(AbstractTRScriptEditor scriptEditor)
        {
            if (scriptEditor.Edition.IsCommunityPatch)
            {
                // #84 If randomizing unarmed locations, keep a reference to the same RNG that is used to randomize the levels
                TR1ScriptEditor editor = scriptEditor as TR1ScriptEditor;
                if (_randomiseUnarmedLocations = editor.UnarmedLevelOrganisation == Organisation.Random)
                {
                    _unarmedRng = editor.UnarmedLevelRNG.Create();
                }
            }
        }

        protected override void HandleWeaponlessStateChanged(TRScriptedLevelEventArgs args)
        {
            if (!args.ScriptedLevel.Enabled || !_scriptEditor.Edition.IsCommunityPatch)
            {
                return;
            }

            TR1Level level = ReadLevel(args.LevelFileBaseName);
            AbstractTRScriptedLevel scriptedLevel = args.ScriptedLevel;

            Location defaultLocation = GetDefaultUnarmedLocationForLevel(scriptedLevel);
            if (defaultLocation == null)
            {
                throw new IOException(string.Format("There is no default weapon location defined for {0} ({1})", scriptedLevel.Name, scriptedLevel.LevelFileBaseName));
            }

            IEnumerable<TR1Entity> existingInjections = level.Entities.Where
            (
                e =>
                    e.Room == defaultLocation.Room &&
                    e.X == defaultLocation.X &&
                    e.Y == defaultLocation.Y &&
                    e.Z == defaultLocation.Z &&
                    e.TypeID == TR1Type.Pistols_S_P
            );

            // For HSC change the pistols into DEagle ammo if the level is no longer unarmed
            if (scriptedLevel.Is(TR1LevelNames.MINES))
            {
                TR1Entity shackEntity = existingInjections.FirstOrDefault();
                if (shackEntity != null)
                {
                    if (!scriptedLevel.RemovesWeapons)
                    {
                        shackEntity.TypeID = TR1Type.MagnumAmmo_S_P;
                    }
                }
            }
            else if (scriptedLevel.RemovesWeapons)
            {
                defaultLocation = GetUnarmedLocationForLevel(scriptedLevel);
                level.Entities.Add(new()
                {
                    TypeID = TR1Type.Pistols_S_P,
                    Room = defaultLocation.Room,
                    X = defaultLocation.X,
                    Y = defaultLocation.Y,
                    Z = defaultLocation.Z,
                    Angle = 0,
                    Intensity = 6400,
                    Flags = 0
                });
            }

            WriteLevel(level, args.LevelFileBaseName);
        }
    }
}