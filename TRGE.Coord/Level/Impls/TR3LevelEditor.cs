using System.Collections.Generic;
using System.IO;
using System.Linq;
using TRFDControl;
using TRFDControl.Utilities;
using TRGE.Core;
using TRLevelControl;
using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRLevelControl.Model.Enums;

namespace TRGE.Coord
{
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
            switch (e.Modification)
            {
                case TRScriptedLevelModification.SequenceChanged:
                    return true;
                default:
                    return base.ShouldHandleModification(e);
            }
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

            List<TR2Entity> entities = level.Entities.ToList();
            IEnumerable<TR2Entity> existingInjections = entities.Where
            (
                e =>
                    e.Room == defaultLocation.Room &&
                    e.X == defaultLocation.X &&
                    e.Y == defaultLocation.Y &&
                    e.Z == defaultLocation.Z &&
                    e.TypeID == (short)TR3Entities.Pistols_P
            );

            // For HSC change the pistols into DEagle ammo if the level is no longer unarmed
            if (scriptedLevel.Is(TR3LevelNames.HSC))
            {
                TR2Entity armouryEntity = existingInjections.FirstOrDefault();
                if (armouryEntity != null)
                {
                    if (!scriptedLevel.RemovesWeapons)
                    {
                        armouryEntity.TypeID = (short)TR3Entities.DeagleAmmo_P;
                    }
                }
            }
            else if (scriptedLevel.RemovesWeapons)
            {
                defaultLocation = GetUnarmedLocationForLevel(scriptedLevel);
                entities.Add(new TR2Entity
                {
                    TypeID = (short)TR3Entities.Pistols_P,
                    Room = defaultLocation.Room,
                    X = defaultLocation.X,
                    Y = defaultLocation.Y,
                    Z = defaultLocation.Z,
                    Angle = 0,
                    Intensity1 = -1,
                    Intensity2 = -1,
                    Flags = 0
                });

                level.Entities = entities.ToArray();
                level.NumEntities++;
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
            // So we just move the fish to 0,0,0 and remove their triggers, unless TR3Main is being used.
            if (_scriptEditor.Edition.IsCommunityPatch)
            {
                return;
            }

            TR3Level level = ReadLevel(args.LevelFileBaseName);

            List<TR2Entity> entities = level.Entities.ToList();
            List<TR2Entity> fishies = entities.FindAll(e => IsTargetFish(e));
            if (fishies.Count > 0)
            {
                FDControl control = new FDControl();
                control.ParseFromLevel(level);

                foreach (TR2Entity fish in fishies)
                {
                    FDUtilities.RemoveEntityTriggers(level, entities.IndexOf(fish), control);

                    fish.X = fish.Y = fish.Z = 0;
                }

                control.WriteToLevel(level);
            }

            WriteLevel(level, args.LevelFileBaseName);
        }

        private bool IsTargetFish(TR2Entity e)
        {
            return e.X != 0 && e.Y != 0 && e.Z != 0 &&
                (e.TypeID == (short)TR3Entities.Fish || e.TypeID == (short)TR3Entities.Piranhas_N);
        }

        protected override int GetSaveTarget(int numLevels)
        {
            return -1; // We don't change Assault course
        }
    }
}