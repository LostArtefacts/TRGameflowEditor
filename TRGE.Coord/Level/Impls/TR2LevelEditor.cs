using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TRGE.Core;
using TRLevelReader;
using TRLevelReader.Model;
using TRLevelReader.Model.Enums;

namespace TRGE.Coord
{
    internal class TR2LevelEditor : AbstractTRLevelEditor
    {
        private readonly Dictionary<string, List<Location>> _defaultWeaponLocations;

        internal TR2LevelEditor(TRDirectoryIOArgs io)
            :base(io)
        {
            _defaultWeaponLocations = JsonConvert.DeserializeObject<Dictionary<string, List<Location>>>(File.ReadAllText(@"Resources\ualocations.json"));
        }

        internal override void ScriptedLevelModified(TRScriptedLevelEventArgs e)
        {
            if (e.Modification != TRScriptedLevelModification.WeaponlessStateChanged)
            {
                return;
            }

            string levelFile = Path.Combine(_io.BackupDirectory.FullName, e.LevelFileBaseName);
            if (!File.Exists(levelFile))
            {
                throw new IOException(string.Format("Missing level file {0}", levelFile));
            }

            TR2LevelReader reader = new TR2LevelReader();
            TR2Level level = reader.ReadLevel(levelFile);
            
            if (e.LevelID == AbstractTRScriptedLevel.CreateID("HOUSE"))
            {
                //For the time being, we use the following base house.tr2 file, which was modified
                //using TRViewer to include all weapon animations and sprites. Copying the data
                //below across partially works, but it breaks the textures
                level = reader.ReadLevel(@"Resources\house.tr2");
            }
            else
            {
                SetDefaultWeaponsAvailable(level, e.LevelFileBaseName, e.LevelRemovesWeapons);
            }

            TR2LevelWriter writer = new TR2LevelWriter();
            writer.WriteLevelToFile(level, Path.Combine(_io.OutputDirectory.FullName, e.LevelFileBaseName));
        }

        internal Location GetLocationForLevel(string levelFileName)
        {
            levelFileName = levelFileName.ToUpper();
            if (_defaultWeaponLocations.ContainsKey(levelFileName) && _defaultWeaponLocations[levelFileName].Count > 0)
            {
                return _defaultWeaponLocations[levelFileName][0];
            }
            return null;
        }

        private void SetDefaultWeaponsAvailable(TR2Level level, string levelID, bool available)
        {
            Location defaultLocation = GetLocationForLevel(levelID);
            if (defaultLocation == null)
            {
                throw new IOException(string.Format("There is no default weapon location defined for {0}", levelID));
            }

            List<TR2Entity> ents = level.Entities.ToList();
            if (available)
            {
                ents.Add(new TR2Entity
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
            }
            else
            {
                //TODO:get anything that may be in the defualt location, pistols or otherwise as may have been randomised
            }

            level.Entities = ents.ToArray();
        }

        internal override void Save(AbstractTRScriptEditor scriptEditor)
        {
            
        }

        internal override void Restore()
        {
            
        }
    }

    internal class Location
    {
        public short Room { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }
    }
}