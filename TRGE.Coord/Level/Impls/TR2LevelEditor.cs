using Newtonsoft.Json;
using System;
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
            :base(io)
        {
            _defaultWeaponLocations = JsonConvert.DeserializeObject<Dictionary<string, Location>>(File.ReadAllText(@"Resources\ualocations.json"));
        }

        internal override void ScriptedLevelModified(TRScriptedLevelEventArgs e)
        {
            if (e.Modification != TRScriptedLevelModification.WeaponlessStateChanged)
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

            bool changesMade;
            if (e.LevelID == AbstractTRScriptedLevel.CreateID("HOUSE"))
            {
                //For the time being, we use the following base house.tr2 file, which was modified
                //using TRViewer to include all weapon animations and sprites. Copying the data
                //below across partially works, but it breaks the textures
                //level = reader.ReadLevel(@"Resources\house.tr2");
                changesMade = !e.LevelRemovesWeapons && InjectHSHStuff(level);
            }
            else
            {
                changesMade = SetDefaultWeaponsAvailable(level, e.ScriptedLevel);
                if ((e.ScriptedLevel as TR23ScriptedLevel).RequiresPistolTextureInjection)
                {
                    changesMade |= InjectHSHStuff(level);
                }
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
        private byte[] pistols8 = null;
        private ushort[] pistols16 = null;
        private TRModel model;
        private TRAnimation animation;
        private List<ushort> frames;
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
            if (pistols8 == null)
            {
                pistols8 = level.Images8[8].Pixels;
                pistols16 = level.Images16[8].Pixels;
                model = level.Models[level.Models.ToList().FindIndex(e => e.ID == 157)];
                animation = level.Animations[model.Animation];
                frames = new List<ushort>();
                uint frameIndex = animation.FrameOffset / 2;
                uint numFrames = (uint)(animation.FrameEnd - animation.FrameStart + 1);
                for (uint i = frameIndex; i < frameIndex + numFrames; i++)
                {
                    frames.Add(level.Frames[i]);
                }

                uint meshTreeIndex = model.MeshTree / 16; // sizeof(uint) + 3*sizeof(int)
                TRMeshTreeNode meshTree = level.MeshTrees[meshTreeIndex];
                List<uint> meshPointers = new List<uint>(model.NumMeshes);
                for (int i = model.StartingMesh; i < model.StartingMesh + model.NumMeshes; i++)
                {
                    meshPointers.Add(level.MeshPointers[i]);
                }

                TRMesh[] meshes = level.Meshes;
                int j = 0;

            }
            /*using (BinaryWriter bw = new BinaryWriter(new FileStream(@"pistols.bin", FileMode.Create)))
            {
                foreach (ushort u in level.Images16[8].Pixels)
                {
                    bw.Write(u);
                }
            }*/
            if (scriptedLevel.RemovesWeapons)
            {
                //only inject if it hasn't been done already i.e. no pistols or randomised weapon found in default spot
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
        private bool InjectHSHStuff(TR2Level level)
        {
            bool changesMade = false;

            List<TRSpriteSequence> spriteSequences = level.SpriteSequences.ToList();
            int pistolIndex = spriteSequences.FindIndex(e => e.SpriteID == (short)TR2Entities.Pistols_S_P);
            if (pistolIndex == -1)
            {
                /*List<TR2Entity> entities = level.Entities.ToList();
                entities.Add(new TR2Entity
                {
                    TypeID = (short)TR2Entities.Pistols_S_P,
                    Room = 52,
                    X = 35329,
                    Y = 256,
                    Z = 63897,
                    Angle = 0,
                    Intensity1 = -1,
                    Intensity2 = -1,
                    Flags = 0
                });
                level.Entities = entities.ToArray();
                level.NumEntities++;*/

                List<TRTexImage8> spriteTiles8 = level.Images8.ToList();
                spriteTiles8.Add(new TRTexImage8
                {
                    Pixels = pistols8
                });
                level.Images8 = spriteTiles8.ToArray();

                List<TRTexImage16> spriteTiles16 = level.Images16.ToList();
                spriteTiles16.Add(new TRTexImage16
                {
                    Pixels = pistols16
                });
                level.Images16 = spriteTiles16.ToArray();
                level.NumImages++;

                List<TRSpriteTexture> spriteTextures = level.SpriteTextures.ToList();
                spriteTextures.Add(new TRSpriteTexture
                {
                    //Atlas = 8,
                    Atlas = (ushort)(spriteTiles16.Count - 1),
                    BottomSide = 11,
                    Height = 12287,
                    LeftSide = -93,
                    RightSide = 96,
                    TopSide = -102,
                    Width = 20479,
                    X = 0,
                    Y = 200
                });
                level.SpriteTextures = spriteTextures.ToArray();
                level.NumSpriteTextures++;

                TRSpriteSequence lastSprite = spriteSequences[spriteSequences.Count - 1];
                spriteSequences.Add(new TRSpriteSequence
                {
                    SpriteID = (short)TR2Entities.Pistols_S_P,
                    NegativeLength = -1,
                    Offset = (short)(spriteTextures.Count - 1) //(short)(lastSprite.Offset - lastSprite.NegativeLength)
                });
                level.SpriteSequences = spriteSequences.ToArray();
                level.NumSpriteSequences++;

                //hsh
                List<ushort> levelFrames = level.Frames.ToList();
                model.FrameOffset = animation.FrameOffset = (uint)levelFrames.Count * 2;
                levelFrames.AddRange(frames);
                
                List<TRAnimation> levelAnimations = level.Animations.ToList();
                model.Animation = animation.NextAnimation = (ushort)levelAnimations.Count;
                levelAnimations.Add(animation);
                
                //check state changes

                List<TRModel> levelModels = level.Models.ToList();
                levelModels.Add(model);

                //check mesh stuff

                level.Frames = levelFrames.ToArray();
                level.NumFrames = (uint)levelFrames.Count;
                level.Animations = levelAnimations.ToArray();
                level.NumAnimations++;
                level.Models = levelModels.ToArray();
                level.NumModels++;

                changesMade = true;
            }

            return changesMade;
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