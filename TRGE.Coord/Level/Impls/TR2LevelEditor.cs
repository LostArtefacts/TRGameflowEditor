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
using TRLevelReader.Serialization;

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
                //if ((e.ScriptedLevel as TR23ScriptedLevel).RequiresPistolTextureInjection)
                //{
                //    changesMade |= InjectHSHStuff(level);
                //}
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
        /*private TRModel model;
        private TRAnimation animation;
        private List<ushort> frames;
        private List<TRMesh> meshes;
        private TR2Level wallLevel;*/
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
                TRModel model = level.Models[level.Models.ToList().FindIndex(e => e.ID == 157)];
                /*animation = level.Animations[model.Animation];
                frames = new List<ushort>();
                uint frameIndex = animation.FrameOffset / 2;
                uint numFrames = (uint)(animation.FrameEnd - animation.FrameStart + 1);
                for (uint i = frameIndex; i < frameIndex + numFrames; i++)
                {
                    frames.Add(level.Frames[i]);
                }*/

                /*uint meshTreeIndex = model.MeshTree / 16; // sizeof(uint) + 3*sizeof(int)
                TRMeshTreeNode meshTree = level.MeshTrees[meshTreeIndex];
                List<uint> meshPointers = new List<uint>(model.NumMeshes);
                for (int i = model.StartingMesh; i < model.StartingMesh + model.NumMeshes; i++)
                {
                    meshPointers.Add(level.MeshPointers[i]);
                }*/

                //wallLevel = level;

                /*ushort meshPointerIndex = model.StartingMesh;
                meshes = new List<TRMesh>();
                for (int i = meshPointerIndex; i < meshPointerIndex + model.NumMeshes; i++)
                {
                    uint pointer = level.MeshPointers[i];
                    TRMesh mesh = level.Meshes.ToList().Find(e => e.Pointer == pointer);
                    if (mesh != null)
                    {
                        meshes.Add(mesh);
                    }
                }*/

                TR2LevelReader r = new TR2LevelReader();
                TR2Level wallDefault = r.ReadLevel(@"C:\users\lewis\desktop\meshstuff\wall_default.tr2");
                TR2Level wallNoPistols = r.ReadLevel(@"C:\users\lewis\desktop\meshstuff\wall_nopistols.tr2");

                int breakPoint = 0;
                #region Animations
                TRAnimation animation = null;
                for (int i = 0; i < wallDefault.Animations.Length; i++)
                {
                    TRAnimation defAnim = wallDefault.Animations[i];
                    if (defAnim.FrameOffset == 362718)
                    {
                        //break;
                    }
                    TRAnimation altAnim = wallNoPistols.Animations[i];
                    if (!SerialisablesEqual(defAnim, altAnim))
                    {
                        animation = defAnim;
                        break;
                    }
                }
                #endregion

                #region Frames
                //180 different frames
                List<ushort> frames = new List<ushort>();
                for (int i = 0; i < wallDefault.Frames.Length; i++)
                {
                    if (wallDefault.Frames[i] != wallNoPistols.Frames[i])
                    {
                        breakPoint = i;
                        
                        for (int j = i; j < i + 180; j++)
                        {
                            frames.Add(wallDefault.Frames[j]);
                        }
                        break;
                    }
                }

                //verification
                List<ushort> noPistolFrames = wallNoPistols.Frames.ToList();
                noPistolFrames.InsertRange(breakPoint, frames);

                if (noPistolFrames.Count == wallDefault.Frames.Length)
                {
                    for (int i = 0; i < wallDefault.Frames.Length; i++)
                    {
                        if (wallDefault.Frames[i] != noPistolFrames[i])
                        {
                            break;
                        }
                    }
                }
                #endregion

                #region Models
                for (int i = 0; i < wallDefault.Models.Length; i++)
                {
                    TRModel m1 = wallDefault.Models[i];
                    TRModel m2 = wallNoPistols.Models[i];
                    if (!SerialisablesEqual(m1, m2))
                    {
                        breakPoint = i;
                        model = m1;
                        break;
                    }
                }
                #endregion

                #region MeshTrees
                List<TRMeshTreeNode> meshTrees = new List<TRMeshTreeNode>();
                for (int i = 0; i < wallDefault.MeshTrees.Length; i++)
                {
                    TRMeshTreeNode m1 = wallDefault.MeshTrees[i];
                    TRMeshTreeNode m2 = wallNoPistols.MeshTrees[i];
                    if (!SerialisablesEqual(m1, m2))
                    {
                        breakPoint = i;
                        meshTrees.Add(m1);
                        meshTrees.Add(wallDefault.MeshTrees[i + 1]); //still don't know why it's two for this
                        if (wallNoPistols.MeshTrees.Length + 2 != wallDefault.MeshTrees.Length)
                        {
                            break;
                        }
                        break;
                    }
                }

                //verification
                List<TRMeshTreeNode> noPistolTrees = wallNoPistols.MeshTrees.ToList();
                noPistolTrees.InsertRange(breakPoint, meshTrees);

                if (noPistolTrees.Count == wallDefault.MeshTrees.Length)
                {
                    for (int i = 0; i < wallDefault.MeshTrees.Length; i++)
                    {
                        if (!SerialisablesEqual(noPistolTrees[i], wallDefault.MeshTrees[i]))
                        {
                            break;
                        }
                    }
                }
                #endregion

                #region Meshes
                ushort meshPointerIndex = model.StartingMesh;
                List<TRMesh> meshes = new List<TRMesh>();
                for (int i = meshPointerIndex; i < meshPointerIndex + model.NumMeshes; i++)
                {
                    uint pointer = level.MeshPointers[i];
                    TRMesh mesh = level.Meshes.ToList().Find(e => e.Pointer == pointer);
                    if (mesh != null)
                    {
                        meshes.Add(mesh);
                    }
                }
                /*for (int i = 0; i < wallDefault.Meshes.Length; i++)
                {
                    TRMesh m1 = wallDefault.Meshes[i];
                    TRMesh m2 = wallNoPistols.Meshes[i];
                    if (!SerialisablesEqual(m1, m2))
                    {
                        breakPoint = i;
                        meshes.Add(m1);
                        meshes.Add(wallDefault.Meshes[i + 1]);
                        meshes.Add(wallDefault.Meshes[i + 2]); //model.NumMeshes
                        break;
                    }
                }*/
                #endregion

                #region ObjTextures
                List<TRObjectTexture> objTextures = new List<TRObjectTexture>();
                for (int i = 0; i < wallDefault.ObjectTextures.Length; i++)
                {
                    TRObjectTexture m1 = wallDefault.ObjectTextures[i];
                    TRObjectTexture m2 = wallNoPistols.ObjectTextures[i];
                    if (!SerialisablesEqual(m1, m2))
                    {
                        breakPoint = i;
                        objTextures.Add(m1);
                        for (int j = 1; j <= 12; j++)
                        {
                            objTextures.Add(wallDefault.ObjectTextures[i + j]);
                        }
                        break;
                    }
                }
                #endregion

                #region SpriteTextures
                List<TRSpriteTexture> spriteTextures = new List<TRSpriteTexture>();
                for (int i = 0; i < wallDefault.SpriteTextures.Length; i++)
                {
                    TRSpriteTexture m1 = wallDefault.SpriteTextures[i];
                    TRSpriteTexture m2 = wallNoPistols.SpriteTextures[i];
                    if (!SerialisablesEqual(m1, m2))
                    {
                        spriteTextures.Add(m1);
                        for (int j = 1; j <= 3; j++)
                        {
                            spriteTextures.Add(wallDefault.SpriteTextures[i + j]);
                        }
                        break;
                    }
                }
                #endregion

                Dictionary<string, object> pistolMap = new Dictionary<string, object>
                {
                    { "Animation", animation },
                    { "Frames", frames },
                    //{ "Images", new object[] { level.Images8[8], level.Images16[8] } },
                    { "Model", model },
                    { "MeshTrees", meshTrees },
                    { "Meshes", meshes },
                    { "ObjectTextures", objTextures },
                    { "SpriteTextures", spriteTextures }
                };
                File.WriteAllText("pistols.json", JsonConvert.SerializeObject(pistolMap, Formatting.Indented));
                int k = 0;
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

        private bool SerialisablesEqual(ISerializableCompact s1, ISerializableCompact s2)
        {
            byte[] b1 = s1.Serialize();
            byte[] b2 = s2.Serialize();

            if (b1.Length == b2.Length)
            {
                for (int i = 0; i < b1.Length; i++)
                {
                    if (b1[i] != b2[i])
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
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

                Dictionary<string, object> pistolMap = JsonConvert.DeserializeObject<Dictionary<string, object>>(File.ReadAllText("pistols.json"));
                TRAnimation newAnimation = JsonConvert.DeserializeObject<TRAnimation>(pistolMap["Animation"].ToString());
                List<ushort> newFrames = JsonConvert.DeserializeObject<List<ushort>>(pistolMap["Frames"].ToString());
                TRModel newModel = JsonConvert.DeserializeObject<TRModel>(pistolMap["Model"].ToString());
                List<TRMeshTreeNode> newMeshTreeNodes = JsonConvert.DeserializeObject<List<TRMeshTreeNode>>(pistolMap["MeshTrees"].ToString());
                List<TRMesh> newMeshes = JsonConvert.DeserializeObject<List<TRMesh>>(pistolMap["Meshes"].ToString());
                List<TRObjectTexture> newObjTextures = JsonConvert.DeserializeObject<List<TRObjectTexture>>(pistolMap["ObjectTextures"].ToString());
                List<TRSpriteTexture> newSpriteTextures = JsonConvert.DeserializeObject<List<TRSpriteTexture>>(pistolMap["SpriteTextures"].ToString());

                List<ushort> levelFrames = level.Frames.ToList();
                newModel.FrameOffset = newAnimation.FrameOffset = (uint)levelFrames.Count * 2;
                levelFrames.AddRange(newFrames);
                level.Frames = levelFrames.ToArray();
                level.NumFrames = (uint)levelFrames.Count;

                List<TRAnimation> levelAnimations = level.Animations.ToList();
                newModel.Animation = newAnimation.NextAnimation = (ushort)levelAnimations.Count;
                levelAnimations.Add(newAnimation);
                level.Animations = levelAnimations.ToArray();
                level.NumAnimations++;
                
                for (int i = 0; i < newMeshes.Count; i++)
                {
                    TRMesh mesh = newMeshes[i];
                    int pointerIndex = level.InsertMesh(mesh);
                    if (i == 0)
                    {
                        newModel.StartingMesh = (ushort)pointerIndex;
                    }
                }

                for (int i = 0; i < newMeshTreeNodes.Count; i++)
                {
                    TRMeshTreeNode node = newMeshTreeNodes[i];
                    int index = level.InsertMeshTreeNode(node);
                    if (i == 0)
                    {
                        newModel.MeshTree = (uint)index * 4;
                    }
                }

                List<TRModel> levelModels = level.Models.ToList();
                levelModels.Add(newModel);
                level.Models = levelModels.ToArray();
                level.NumModels++;

                //still need to work out how IDs in Texture of Mesh.TexturedTriangles etc match these
                /*
                 * Textured surfaces map textures from the texture atlases (textiles) to each point on the mesh surface. 
                 * This is done using conventional UV mapping, which is specified in “Object Textures” below; each object 
                 * texture specifies a mapping from a set of vertices to locations in an atlas, and these texture vertices 
                 * are associated with position vertices specified here. Each atlas has a size of 256×256 pixels.
                 * 
                 * The 16-bit atlas array, which contains [tr_image16] structures, specifies colours using 16-bit 1-5-5-5 
                 * ARGB encoding.
                 * 
                 * TR1 onlyTR2 onlyTR3 only If, for some reason, 16-bit textures are turned off, all colours and textures 
                 * use an 8-bit palette that is stored in the level file. This palette consists of a 256-element array of 
                 * [tr_colour] structures, each designating some colour; textures and other elements that need to reference 
                 * a colour specify an index (0..255) into the Palette[] array. There is also a 16-bit palette, which is 
                 * used for identifying colours of solid polygons. The 16-bit palette contains up to 256 four-byte entries; 
                 * the first three bytes are a [tr_colour], while the last byte is ignored (set to 0).
                 **/
                List<TRObjectTexture> levelObjTextures = level.ObjectTextures.ToList();
                levelObjTextures.AddRange(newObjTextures);
                level.ObjectTextures = levelObjTextures.ToArray();
                level.NumObjectTextures = (uint)levelObjTextures.Count;

                List<TRSpriteTexture> levelSpriteTextures = level.SpriteTextures.ToList();
                levelSpriteTextures.AddRange(newSpriteTextures);
                level.SpriteTextures = levelSpriteTextures.ToArray();
                level.NumSpriteTextures = (uint)levelSpriteTextures.Count;

                /*List<ushort> levelFrames = level.Frames.ToList();
                model.FrameOffset = animation.FrameOffset = (uint)levelFrames.Count * 2;
                levelFrames.AddRange(frames);
                
                List<TRAnimation> levelAnimations = level.Animations.ToList();
                model.Animation = animation.NextAnimation = (ushort)levelAnimations.Count;
                levelAnimations.Add(animation);
                
                //check state changes

                List<TRModel> levelModels = level.Models.ToList();
                levelModels.Add(model);

                //check mesh stuff
                for (int i = 0; i < meshes.Count; i++)
                {
                    TRMesh mesh = meshes[i];
                    int pointerIndex = level.InsertMesh(mesh);
                    if (i == 0)
                    {
                        model.StartingMesh = (ushort)pointerIndex;
                    }
                }

                int meshTreeIndex = (int)model.MeshTree / 4;
                TRMeshTreeNode node = wallLevel.MeshTrees[meshTreeIndex];
                model.MeshTree = (uint)level.InsertMeshTreeNode(node) * 4;
                TRMeshTreeNode node2 = wallLevel.MeshTrees[meshTreeIndex + 1]; //what determines how many we get?
                level.InsertMeshTreeNode(node2);

                level.Frames = levelFrames.ToArray();
                level.NumFrames = (uint)levelFrames.Count;
                level.Animations = levelAnimations.ToArray();
                level.NumAnimations++;
                level.Models = levelModels.ToArray();
                level.NumModels++;*/

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