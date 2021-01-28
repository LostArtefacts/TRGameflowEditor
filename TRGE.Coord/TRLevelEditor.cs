using Newtonsoft.Json;
using System;
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
        private readonly TRDirectoryIOArgs _io;
        private readonly Dictionary<string, List<Location>> _defaultWeaponLocations;

        internal TRLevelEditor(TRDirectoryIOArgs io)
        {
            _io = io;
            _defaultWeaponLocations = JsonConvert.DeserializeObject<Dictionary<string, List<Location>>>(File.ReadAllText(@"Resources\ualocations.json"));
        }

        internal void ScriptedLevelModified(TRScriptedLevelEventArgs e)
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

            TR2Level level = null;
            TR2LevelReader reader = new TR2LevelReader();
            if (e.LevelRemovesWeapons)
            {
                level = reader.ReadLevel(levelFile);
                Location pistolLocation = GetLocationForLevel(e.LevelFileBaseName);
                if (pistolLocation == null)
                {
                    throw new IOException(string.Format("There is no default pistol location defined for {0} ({1})", e.LevelName, e.LevelFileBaseName));
                }

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
            }
            else if (e.LevelID == AbstractTRScriptedLevel.CreateID("HOUSE"))
            {
                //For the time being, we use the following base house.tr2 file, which was modified
                //using TRViewer to include all weapon animations and sprites. Copying the data
                //below across partially works, but it breaks the textures
                level = reader.ReadLevel(@"Resources\house.tr2");
                /*level.NumAnimations = armedHSHLevel.NumAnimations;
                level.Animations = armedHSHLevel.Animations;
                
                level.NumFrames = armedHSHLevel.NumFrames;
                level.Frames = armedHSHLevel.Frames;

                level.NumImages = armedHSHLevel.NumImages;
                level.Images8 = armedHSHLevel.Images8;
                level.Images16 = armedHSHLevel.Images16;

                level.NumMeshData = armedHSHLevel.NumMeshData;
                level.RawMeshData = armedHSHLevel.RawMeshData;

                level.NumMeshPointers = armedHSHLevel.NumMeshPointers;
                level.MeshPointers = armedHSHLevel.MeshPointers;

                level.NumMeshTrees = armedHSHLevel.NumMeshTrees;
                level.MeshTrees = armedHSHLevel.MeshTrees;

                level.NumModels = armedHSHLevel.NumModels;
                level.Models = armedHSHLevel.Models;

                level.NumObjectTextures = armedHSHLevel.NumObjectTextures;
                level.ObjectTextures = armedHSHLevel.ObjectTextures;

                level.NumSpriteSequences = armedHSHLevel.NumSpriteSequences;
                level.SpriteSequences = armedHSHLevel.SpriteSequences;

                level.NumSpriteTextures = armedHSHLevel.NumSpriteTextures;
                level.SpriteTextures = armedHSHLevel.SpriteTextures;*/
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
    }

    internal class Location
    {
        public short Room { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }
    }
}