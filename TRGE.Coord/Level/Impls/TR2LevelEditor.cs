using Newtonsoft.Json;
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
    internal class TR2LevelEditor : AbstractTRLevelEditor
    {
        private readonly Dictionary<string, Location> _defaultWeaponLocations;

        internal TR2LevelEditor(TRDirectoryIOArgs io)
            : base(io)
        {
            _defaultWeaponLocations = JsonConvert.DeserializeObject<Dictionary<string, Location>>(File.ReadAllText(@"Resources\ualocations.json"));
            CheckHSHBackup();
        }

        internal override bool ShouldHandleModification(TRScriptedLevelEventArgs e)
        {
            switch (e.Modification)
            {
                case TRScriptedLevelModification.WeaponlessStateChanged:
                    return !e.LevelID.Equals(AbstractTRScriptedLevel.CreateID("HOUSE"));
                case TRScriptedLevelModification.SunsetChanged:
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
            }
        }

        private void HandleWeaponlessStateChanged(TRScriptedLevelEventArgs e)
        {
            string levelFile = GetReadLevelFilePath(e.LevelFileBaseName);
            if (!File.Exists(levelFile))
            {
                throw new IOException(string.Format("Missing level file {0}", levelFile));
            }

            TR2LevelReader reader = new TR2LevelReader();
            TR2Level level = reader.ReadLevel(levelFile);

            SetDefaultWeaponsAvailable(level, e.ScriptedLevel);
            if ((e.ScriptedLevel as TR23ScriptedLevel).RequiresPistolTextureInjection)
            {
                MaybeInjectPistolTexture(level);
            }

            TR2LevelWriter writer = new TR2LevelWriter();
            writer.WriteLevelToFile(level, GetWriteLevelFilePath(e.LevelFileBaseName));
        }

        internal Location GetLocationForLevel(string levelFileName)
        {
            levelFileName = levelFileName.ToUpper();
            if (_defaultWeaponLocations.ContainsKey(levelFileName))
            {
                return _defaultWeaponLocations[levelFileName];
            }
            return null;
        }
        
        private bool SetDefaultWeaponsAvailable(TR2Level level, AbstractTRScriptedLevel scriptedLevel)
        {
            Location defaultLocation = GetLocationForLevel(scriptedLevel.LevelFileBaseName);
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
            
            if (scriptedLevel.RemovesWeapons)
            {
                //only inject if it hasn't been done already i.e. no pistols or randomised weapon found in the default spot
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

        private void CheckHSHBackup()
        {
            // HSH is a special case as we need to make sure all weapons, ammo,
            // and relevant animations are available in the level in case it no
            // longer removes weapons, but at the moment this isn't possible
            // natively. We rely on the pre-made level file (made in TRViewer)
            // and only use this once so to allow for successive randomisations,
            // if that mode is enabled.
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
            if (!File.Exists(fullOriginalBackup))
            {
                File.Move(currentBackupFile, fullOriginalBackup);
                File.Copy(@"Resources\house.tr2", currentBackupFile, true);
                File.Copy(@"Resources\house.tr2", Path.Combine(_io.OutputDirectory.FullName, "house.tr2"), true);
            }
        }

        private bool MaybeInjectPistolTexture(TR2Level level)
        {
            int pistolIndex = level.SpriteSequences.ToList().FindIndex(e => e.SpriteID == (short)TR2Entities.Pistols_S_P);
            if (pistolIndex == -1)
            {
                SpriteDefinition.Load(@"Resources\pistols.json").AddToLevel(level);
                return true;
            }

            return false;
        }

        // If the Sunset flag is set in the script, rooms with lighting set to type 3 (and with this also set
        // for each vertex inthe room) will dim over 20 minutes in the game. We don't need to undo this as it 
        // will be ignored if the script flag isn't set.
        // https://opentomb.github.io/TRosettaStone3/trosettastone.html#_the_whole_room_structure
        private void HandleSunsetStateChanged(TRScriptedLevelEventArgs e)
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

        internal override void Restore()
        {
            _io.BackupDirectory.Copy(_io.OriginalDirectory, true, new string[] { "*.dat", "*.tr2" });
            string hshBackup = Path.Combine(_io.BackupDirectory.FullName, "house.tr2.bak");
            if (File.Exists(hshBackup))
            {
                File.Copy(hshBackup, Path.Combine(_io.OriginalDirectory.FullName, "house.tr2"), true);
            }
        }

        internal override void SaveImpl(AbstractTRScriptEditor scriptEditor, TRSaveMonitor monitor)
        {

        }
    }
}