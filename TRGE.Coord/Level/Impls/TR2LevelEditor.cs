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

        internal override void ScriptedLevelModified(TRScriptedLevelEventArgs e)
        {
            if (e.Modification != TRScriptedLevelModification.WeaponlessStateChanged || e.LevelID == AbstractTRScriptedLevel.CreateID("HOUSE"))
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

            bool changesMade = SetDefaultWeaponsAvailable(level, e.ScriptedLevel);
            if ((e.ScriptedLevel as TR23ScriptedLevel).RequiresPistolTextureInjection)
            {
                changesMade |= MaybeInjectPistolTexture(level);
            }

            if (changesMade)
            {
                TR2LevelWriter writer = new TR2LevelWriter();
                writer.WriteLevelToFile(level, GetWriteLevelFilePath(e.LevelFileBaseName));
            }
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

        internal override void Save(AbstractTRScriptEditor scriptEditor)
        {

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
    }
}