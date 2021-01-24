using System.Collections.Generic;
using System.IO;
using System.Linq;
using TRGE.Core;
using TRLevelReader;
using TRLevelReader.Model;

namespace TRGE.Coord
{
    internal class TRLevelEditor
    {
        private readonly string _originalDirectory, _backupDirectory;

        internal TRLevelEditor(string originalDirectory, string backupDirectory)
        {
            _originalDirectory = originalDirectory;
            _backupDirectory = backupDirectory;
        }

        internal void LevelModified(TRScriptedLevelEventArgs e)
        {
            if (e.Modification != TRScriptedLevelModification.WeaponlessStateChanged || !e.LevelRemovesWeapons)
            {
                //we only care about levels that have become weaponless
                return;
            }

            string levelFile = Path.Combine(_backupDirectory, e.LevelFileBaseName);
            if (!File.Exists(levelFile))
            {
                throw new IOException(string.Format("Missing level file {0}", levelFile));
            }

            TR2LevelReader reader = new TR2LevelReader();
            TR2Level level = reader.ReadLevel(levelFile);
            
            Location pistolLocation = GetLocationForLevel(level);
            TR2Entity pistols = new TR2Entity
            {
                TypeID = 135, //TODO: set this in TR2Entities to allow: (short)TR2Entities.Pistols_S_P,
                Room = pistolLocation.Room,
                X = pistolLocation.X,
                Y = pistolLocation.Y,
                Z = pistolLocation.Z,
                Angle = 0,
                Intensity1 = -1,
                Intensity2 = -1,
                Flags = 0
            };

            List<TR2Entity> ents = level.Entities.ToList();
            ents.Add(pistols);
            level.NumEntities++;
            level.Entities = ents.ToArray();

            TR2LevelWriter writer = new TR2LevelWriter();
            //TODO:write to a temp folder, then allow rando to happen, then move files to original
            //maybe we should have functionality to remove it after all...
            writer.WriteLevelToFile(level, Path.Combine(_originalDirectory, e.LevelFileBaseName));
        }

        internal Location GetLocationForLevel(TR2Level level)
        {
            foreach (TR2Entity entity in level.Entities)
            {
                if (entity.TypeID == 151) //flares for Venice?
                {
                    return new Location
                    {
                        Room = entity.Room,
                        X = entity.X,
                        Y = entity.Y,
                        Z = entity.Z
                    };
                }
            }

            return new Location
            {
                Room = 0,
                X = 0,
                Y = 0,
                Z = 0
            };
        }
    }

    internal class Location
    {
        internal short Room { get; set; }
        internal int X { get; set; }
        internal int Y { get; set; }
        internal int Z { get; set; }
    }
}