using System.Collections.Generic;
using System.IO;
using System.Linq;
using TRGE.Core;
using TRLevelReader;
using TRLevelReader.Helpers;
using TRLevelReader.Model;
using TRLevelReader.Model.Enums;

namespace TRGE.Coord
{
    public class TR1LevelEditor : BaseTRLevelEditor
    {
        private readonly TR1LevelReader _reader;
        private readonly TR1LevelWriter _writer;

        public TR1LevelEditor(TRDirectoryIOArgs io, TREdition edition)
            : base(io, edition)
        {
            _reader = new TR1LevelReader();
            _writer = new TR1LevelWriter();
        }

        private TRLevel ReadLevel(string lvl)
        {
            string levelFile = GetReadLevelFilePath(lvl);
            if (!File.Exists(levelFile))
            {
                throw new IOException(string.Format("Missing level file {0}", levelFile));
            }

            return _reader.ReadLevel(levelFile);
        }

        private void WriteLevel(TRLevel level, string lvl)
        {
            _writer.WriteLevelToFile(level, GetWriteLevelFilePath(lvl));
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

            TRLevel level = ReadLevel(args.LevelFileBaseName);
            AbstractTRScriptedLevel scriptedLevel = args.ScriptedLevel;

            Location defaultLocation = GetDefaultUnarmedLocationForLevel(scriptedLevel);
            if (defaultLocation == null)
            {
                throw new IOException(string.Format("There is no default weapon location defined for {0} ({1})", scriptedLevel.Name, scriptedLevel.LevelFileBaseName));
            }

            List<TREntity> entities = level.Entities.ToList();
            IEnumerable<TREntity> existingInjections = entities.Where
            (
                e =>
                    e.Room == defaultLocation.Room &&
                    e.X == defaultLocation.X &&
                    e.Y == defaultLocation.Y &&
                    e.Z == defaultLocation.Z &&
                    e.TypeID == (short)TREntities.Pistols_S_P
            );

            // For HSC change the pistols into DEagle ammo if the level is no longer unarmed
            if (scriptedLevel.Is(TRLevelNames.MINES))
            {
                TREntity shackEntity = existingInjections.FirstOrDefault();
                if (shackEntity != null)
                {
                    if (!scriptedLevel.RemovesWeapons)
                    {
                        shackEntity.TypeID = (short)TREntities.MagnumAmmo_S_P;
                    }
                }
            }
            else if (scriptedLevel.RemovesWeapons)
            {
                defaultLocation = GetUnarmedLocationForLevel(scriptedLevel);
                entities.Add(new TREntity
                {
                    TypeID = (short)TREntities.Pistols_S_P,
                    Room = defaultLocation.Room,
                    X = defaultLocation.X,
                    Y = defaultLocation.Y,
                    Z = defaultLocation.Z,
                    Angle = 0,
                    Intensity = 6400,
                    Flags = 0
                });

                level.Entities = entities.ToArray();
                level.NumEntities++;
            }

            WriteLevel(level, args.LevelFileBaseName);
        }
    }
}