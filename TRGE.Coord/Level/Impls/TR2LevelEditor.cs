using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TRGE.Coord.Properties;
using TRGE.Core;
using TRLevelReader;
using TRLevelReader.Helpers;
using TRLevelReader.Model;
using TRLevelReader.Model.Enums;
using TRModelTransporter.Transport;

namespace TRGE.Coord
{
    public class TR2LevelEditor : AbstractTRLevelEditor
    {
        //private static readonly string _hshChecksum = "8a5fdb4fef02395840eb2b8d0a2623e1"; // #75
        private static readonly string _hshChecksum = "422969d461ebb57739d1a3b775f7f88d"; // #75
        private static readonly string _flUKChecksum = "b8fc5d8444b15527cec447bc0387c41a"; // #83
        private static readonly string _flMPChecksum = "1e7d0d88ff9d569e22982af761bb006b"; // #83

        protected readonly Dictionary<string, List<Location>> _defaultWeaponLocations;
        protected readonly Dictionary<string, Dictionary<TR2Entities, List<Location>>> _defaultVehicleLocations;
        private bool _randomiseUnarmedLocations;
        private Random _unarmedRng;

        public TR2LevelEditor(TRDirectoryIOArgs io)
            : base(io)
        {
            _defaultWeaponLocations = JsonConvert.DeserializeObject<Dictionary<string, List<Location>>>(File.ReadAllText(@"Resources\unarmed_locations.json"));
            _defaultVehicleLocations = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<TR2Entities, List<Location>>>>(File.ReadAllText(@"Resources\vehicle_locations.json"));
            CheckFloaterBackup();
            CheckHSHBackup();
        }

        internal override bool ShouldHandleModification(TRScriptedLevelEventArgs e)
        {
            switch (e.Modification)
            {
                case TRScriptedLevelModification.WeaponlessStateChanged:
                case TRScriptedLevelModification.SunsetChanged:
                case TRScriptedLevelModification.StartingWeaponsAdded:
                case TRScriptedLevelModification.StartingWeaponsRemoved:
                case TRScriptedLevelModification.SkidooAdded:
                case TRScriptedLevelModification.SkidooRemoved:
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
                case TRScriptedLevelModification.SunsetChanged:
                    HandleSunsetStateChanged(e);
                    break;
                case TRScriptedLevelModification.StartingWeaponsAdded:
                    HandleStartingWeaponsChanged(e, true);
                    break;
                case TRScriptedLevelModification.StartingWeaponsRemoved:
                    HandleStartingWeaponsChanged(e, false);
                    break;
                case TRScriptedLevelModification.SkidooAdded:
                    HandleSkidooPresenceChanged(e, true);
                    break;
                case TRScriptedLevelModification.SkidooRemoved:
                    HandleSkidooPresenceChanged(e, false);
                    break;
            }
        }

        protected virtual void HandleWeaponlessStateChanged(TRScriptedLevelEventArgs e)
        {
            string levelFile = GetReadLevelFilePath(e.LevelFileBaseName);
            if (!File.Exists(levelFile))
            {
                throw new IOException(string.Format("Missing level file {0}", levelFile));
            }

            if (e.ScriptedLevel.Is("HOUSE"))
            {
                // We only need to copy house once to the WIP directory during this session
                string wipHouseFile = GetWriteLevelFilePath(e.LevelFileBaseName);
                if (!File.Exists(wipHouseFile))
                {
                    File.Copy(levelFile, wipHouseFile);
                }
            }
            else
            {
                TR2LevelReader reader = new TR2LevelReader();
                TR2Level level = reader.ReadLevel(levelFile);

                SetDefaultWeaponsAvailable(level, e.ScriptedLevel);
                if ((e.ScriptedLevel as TR23ScriptedLevel).RequiresWeaponTextureInjection)
                {
                    MaybeInjectWeaponTexture(level);
                }

                TR2LevelWriter writer = new TR2LevelWriter();
                writer.WriteLevelToFile(level, GetWriteLevelFilePath(e.LevelFileBaseName));
            }
        }

        internal Location GetLocationForLevel(AbstractTRScriptedLevel level)
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

        protected virtual bool SetDefaultWeaponsAvailable(TR2Level level, AbstractTRScriptedLevel scriptedLevel)
        {
            Location defaultLocation = GetLocationForLevel(scriptedLevel);
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
                    (
                        e.TypeID == (short)TR2Entities.Pistols_S_P || TR2EntityUtilities.IsGunType((TR2Entities)e.TypeID) || TR2EntityUtilities.IsAmmoType((TR2Entities)e.TypeID)
                    )
            );

            bool changesMade = false;

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
                        cargoEntity.TypeID = (short)TR2Entities.Pistols_S_P;
                    }
                    else
                    {
                        cargoEntity.TypeID = (short)TR2Entities.UziAmmo_S_P; //need a way to be able to define this somewhere
                    }
                }
                else if (!scriptedLevel.RemovesWeapons)
                {
                    cargoEntity = entities.Find(e => e.Room == 1 && e.TypeID == (short)TR2Entities.Pistols_S_P);
                    if (cargoEntity != null)
                    {
                        cargoEntity.TypeID = (short)TR2Entities.UziAmmo_S_P;
                    }
                }
            }
            else if (scriptedLevel.RemovesWeapons)
            {
                //only inject if it hasn't been done already i.e. no pistols, other weapon or ammo found in the default spot
                if (existingInjections.Count() == 0)
                {
                    entities.Add(new TR2Entity
                    {
                        TypeID = (short)TR2Entities.Pistols_S_P,
                        Room = defaultLocation.Room,
                        X = defaultLocation.X,
                        Y = defaultLocation.Y,
                        Z = defaultLocation.Z,
                        Angle = 0,
                        Intensity1 = -1,
                        Intensity2 = -1,
                        Flags = 0
                    });
                    level.NumEntities++;
                    changesMade = true;
                }
            }
            else if (existingInjections.Count() > 0)
            {
                entities.RemoveAll(e => existingInjections.Contains(e));
                level.NumEntities = (uint)entities.Count();
                changesMade = true;
            }

            level.Entities = entities.ToArray();
            return changesMade;
        }

        protected virtual void CheckFloaterBackup()
        {
            // #83 If the version swapping tool has been used after having already
            // backed-up the level files, TRGE will always used the file that was
            // originally backed-up. The swapper tool replaces floating.tr2, so
            // we need to check if this happens and replace the backed up file.

            string gameFile = Path.Combine(_io.OriginalDirectory.FullName, "floating.tr2");
            string backupFile = Path.Combine(_io.BackupDirectory.FullName, "floating.tr2");
            if (File.Exists(gameFile) && File.Exists(backupFile))
            {
                string gameFileChecksum = new FileInfo(gameFile).Checksum();
                if (gameFileChecksum == _flUKChecksum || gameFileChecksum == _flMPChecksum)
                {
                    // The swapper tool has either been used or we have not saved anything yet
                    // but in this instance the backup should match the game file
                    string backupFileChecksum = new FileInfo(backupFile).Checksum();
                    if (backupFileChecksum != gameFileChecksum)
                    {
                        File.Copy(gameFile, backupFile, true);
                    }
                }
            }
        }

        protected virtual void CheckHSHBackup()
        {
            // HSH is a special case as we need to make sure all weapons, ammo,
            // and relevant animations are available in the level in case it no
            // longer removes weapons, but at the moment this isn't possible
            // natively. We rely on the pre-made level file (made in TRViewer)
            // and only use this once.
            // To create this file, the following moveables/sprites were exported from 
            // The Great Wall level in .trmvb/.trspr format and and then imported into
            // a 'clean' HSH. The names are those found in TRViewer.
            //
            //   - Auto-pistol Ammo.trmvb
            //   - Auto-pistol.trmvb
            //   - Grenade launcher.trmvb
            //   - Grenades.trmvb
            //   - Gunflare (spiky).trmvb
            //   - Harpoon gun.trmvb
            //   - Harpoons.trmvb
            //   - Lara auto-pistol animation.trmvb
            //   - Lara grenade-launcher animation.trmvb
            //   - Lara harpoon-gun animation.trmvb
            //   - Lara M16 animation.trmvb
            //   - Lara pistol animation.trmvb
            //   - Lara Uzi animation.trmvb
            //   - M16 ammo.trmvb
            //   - M16.trmvb
            //   - Pistol ammo.trmvb
            //   - Pistol.trmvb
            //   - Rod?.trmbv (this is actually a harpoon as its fired)
            //   - Support?.trmvb (this is actually a grenade as its fired)
            //   - Uzi ammo.trmvb
            //   - Uzi.trmvb
            //   - Auto pistols.trspr
            //   - Auto-pistol clips.trspr
            //   - Grenade blast.trspr
            //   - Grenade launcher.trspr
            //   - Grenades.trspr
            //   - Harpoon gun.trspr
            //   - Harpoons.trspr
            //   - M16 clips.trspr
            //   - M16.trspr
            //   - Pistols.trspr
            //   - Uzi clips.trspr
            //   - Uzis.trspr

            string currentBackupFile = Path.Combine(_io.BackupDirectory.FullName, "house.tr2");
            string fullOriginalBackup = Path.Combine(_io.BackupDirectory.FullName, "house.tr2.bak");
            if (File.Exists(currentBackupFile))
            {
                if (!File.Exists(fullOriginalBackup))
                {
                    File.Copy(currentBackupFile, fullOriginalBackup, true);
                }
                if (!new FileInfo(currentBackupFile).Checksum().Equals(_hshChecksum)) // #75
                {
                    byte[] houseData = ResourceHelper.Decompress(Resources.House);
                    File.WriteAllBytes(currentBackupFile, houseData);
                    File.WriteAllBytes(Path.Combine(_io.OutputDirectory.FullName, "house.tr2"), houseData);
                }
            }
        }

        internal sealed override void PreSave(AbstractTRScriptEditor scriptEditor)
        {
            // #83 Check in case the version swapping tool has been used since the last edit
            CheckFloaterBackup();

            // #75 Check that the HSH backup integrity is still in place i.e. it hasn't been overwritten manually externally
            CheckHSHBackup();

            // #84 If randomizing unarmed locations, keep a reference to the same RNG that is used to randomize the levels
            TR23ScriptEditor editor = scriptEditor as TR23ScriptEditor;
            if (_randomiseUnarmedLocations = editor.UnarmedLevelOrganisation == Organisation.Random)
            {
                _unarmedRng = editor.UnarmedLevelRNG.Create();
            }

            //Ensure cutscene files are copied initially
            foreach (AbstractTRScriptedLevel level in scriptEditor.Levels)
            {
                if (level.SupportsCutScenes)
                {
                    File.Copy(GetReadLevelFilePath(level.CutSceneLevel.LevelFileBaseName), GetWriteLevelFilePath(level.CutSceneLevel.LevelFileBaseName));
                }
            }

            PreSaveImpl(scriptEditor);
        }

        protected virtual void PreSaveImpl(AbstractTRScriptEditor scriptEditor) { }

        protected virtual bool MaybeInjectWeaponTexture(TR2Level level)
        {
            int pistolIndex = level.SpriteSequences.ToList().FindIndex(e => e.SpriteID == (short)TR2Entities.Pistols_S_P);
            if (pistolIndex == -1)
            {
                SpriteDefinition.LoadWeaponsIntoLevel(level);
                return true;
            }

            return false;
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

            TR2LevelReader reader = new TR2LevelReader();
            TR2Level level = reader.ReadLevel(levelFile);

            bool changesMade = false;
            foreach (TR2Room room in level.Rooms)
            {
                if (room.LightMode == 0)
                {
                    room.LightMode = 3;
                    changesMade = true;
                    foreach (TR2RoomVertex vert in room.RoomData.Vertices)
                    {
                        vert.Attributes |= 3;
                    }
                }
            }

            if (changesMade)
            {
                TR2LevelWriter writer = new TR2LevelWriter();
                writer.WriteLevelToFile(level, GetWriteLevelFilePath(e.LevelFileBaseName));
            }
        }

        protected virtual void HandleStartingWeaponsChanged(TRScriptedLevelEventArgs e, bool weaponsAvailable)
        {
            string levelFile = GetReadLevelFilePath(e.LevelFileBaseName);
            if (!File.Exists(levelFile))
            {
                throw new IOException(string.Format("Missing level file {0}", levelFile));
            }

            TR2LevelReader reader = new TR2LevelReader();
            TR2Level level = reader.ReadLevel(levelFile);

            if (weaponsAvailable)
            {
                ImportModels(level, e.LevelFileBaseName, new List<TR2Entities>
                {
                    TR2Entities.Pistols_M_H, TR2Entities.Shotgun_M_H, TR2Entities.Autos_M_H, TR2Entities.Uzi_M_H,
                    TR2Entities.Harpoon_M_H, TR2Entities.M16_M_H, TR2Entities.GrenadeLauncher_M_H
                });
            }

            TR2LevelWriter writer = new TR2LevelWriter();
            writer.WriteLevelToFile(level, GetWriteLevelFilePath(e.LevelFileBaseName));
        }

        protected virtual void HandleSkidooPresenceChanged(TRScriptedLevelEventArgs e, bool skidooAvailable)
        {
            string levelFile = GetReadLevelFilePath(e.LevelFileBaseName);
            if (!File.Exists(levelFile))
            {
                throw new IOException(string.Format("Missing level file {0}", levelFile));
            }

            TR2LevelReader reader = new TR2LevelReader();
            TR2Level level = reader.ReadLevel(levelFile);

            string levelName = e.LevelFileBaseName.ToUpper();
            if
            (
                skidooAvailable &&
                _defaultVehicleLocations.ContainsKey(levelName) &&
                _defaultVehicleLocations[levelName].ContainsKey(TR2Entities.RedSnowmobile) &&
                _defaultVehicleLocations[levelName][TR2Entities.RedSnowmobile].Count > 0
            )
            {
                ImportModels(level, e.LevelFileBaseName, new List<TR2Entities> { TR2Entities.RedSnowmobile });

                List<TR2Entity> entities = level.Entities.ToList();
                Location location = _defaultVehicleLocations[levelName][TR2Entities.RedSnowmobile][0];
                entities.Add(new TR2Entity
                {
                    TypeID = (short)TR2Entities.RedSnowmobile,
                    Room = location.Room,
                    X = location.X,
                    Y = location.Y,
                    Z = location.Z,
                    Angle = 16384,
                    Flags = 0,
                    Intensity1 = -1,
                    Intensity2 = -1
                });
                level.Entities = entities.ToArray();
                level.NumEntities++;
            }

            TR2LevelWriter writer = new TR2LevelWriter();
            writer.WriteLevelToFile(level, GetWriteLevelFilePath(e.LevelFileBaseName));
        }

        private void ImportModels(TR2Level level, string lvlName, List<TR2Entities> entities)
        {
            TRModelImporter importer = new TRModelImporter
            {
                Level = level,
                LevelName = lvlName,
                EntitiesToImport = entities
            };

            importer.Import();
        }

        protected override void ApplyRestore()
        {
            string hshBackup = Path.Combine(_io.BackupDirectory.FullName, "house.tr2.bak");
            if (File.Exists(hshBackup))
            {
                File.Copy(hshBackup, Path.Combine(_io.OriginalDirectory.FullName, "house.tr2"), true);
            }
        }
    }
}