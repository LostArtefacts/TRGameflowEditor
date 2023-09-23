using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TRGE.Coord.Properties;
using TRGE.Core;
using TRLevelControl.Model;

namespace TRGE.Coord
{
    public class SpriteDefinition
    {
        public TRSpriteSequence Sequence { get; set; }
        public TRSpriteTexture Texture { get; set; }
        public TRTexImage16 Tile16 { get; set; }
        public TRTexImage8 Tile8 { get; set; }

        public static void LoadWeaponsIntoLevel(TR2Level level)
        {
            Dictionary<string, object> weaponDefinitions = JsonConvert.DeserializeObject<Dictionary<string, object>>(Encoding.Default.GetString(ResourceHelper.Decompress(Resources.Weapons)));
            List<SpriteDefinition> loadedDefinitions = JsonConvert.DeserializeObject<List<SpriteDefinition>>(weaponDefinitions["Weapons"].ToString());

            TRTexImage16 img16 = JsonConvert.DeserializeObject<TRTexImage16>(weaponDefinitions["Tile16"].ToString());
            TRTexImage8 img8 = JsonConvert.DeserializeObject<TRTexImage8>(weaponDefinitions["Tile8"].ToString());
                        
            if (loadedDefinitions.Count == 0)
            {
                throw new IOException("Failed to load default weapon textures.");
            }

            level.Images8.Add(img8);
            level.Images16.Add(img16);

            List<TRSpriteTexture> spriteTextures = level.SpriteTextures.ToList();
            List<TRSpriteSequence> spriteSequences = level.SpriteSequences.ToList();
            foreach (SpriteDefinition def in loadedDefinitions)
            {
                def.Texture.Atlas = (ushort)(level.Images16.Count - 1);
                spriteTextures.Add(def.Texture);
                
                def.Sequence.Offset = (short)(spriteTextures.Count - 1);
                spriteSequences.Add(def.Sequence);
            }

            level.SpriteTextures = spriteTextures.ToArray();
            level.NumSpriteTextures = (uint)spriteTextures.Count;
            level.SpriteSequences = spriteSequences.ToArray();
            level.NumSpriteSequences = (uint)spriteSequences.Count;
        }

        public static void WriteWeaponDefinitions(TRTexImage8 img8, TRTexImage16 img16, string jsonPath)
        {
            Dictionary<string, object> output = new Dictionary<string, object>
            {
                ["Weapons"] = new List<SpriteDefinition>
                {
                    new SpriteDefinition
                    {
                        Sequence = new TRSpriteSequence { SpriteID = 135, NegativeLength = -1 },
                        Texture = new TRSpriteTexture
                        {
                            X = 104, Y = 28,
                            Width = 18687, Height = 8959,
                            LeftSide = -93, RightSide = 96,
                            TopSide = -102, BottomSide = 11
                        }
                    },
                    new SpriteDefinition
                    {
                        Sequence = new TRSpriteSequence { SpriteID = 136, NegativeLength = -1 },
                        Texture = new TRSpriteTexture
                        {
                            X = 0, Y = 26,
                            Width = 26623, Height = 5375,
                            LeftSide = -240, RightSide = 240,
                            TopSide = -64, BottomSide = 32
                        }
                    },
                    new SpriteDefinition
                    {
                        Sequence = new TRSpriteSequence { SpriteID = 137, NegativeLength = -1 },
                        Texture = new TRSpriteTexture
                        {
                            X = 0, Y = 47,
                            Width = 23295, Height = 9215,
                            LeftSide = -100, RightSide = 100,
                            TopSide = -76, BottomSide = 7
                        }
                    },
                    new SpriteDefinition
                    {
                        Sequence = new TRSpriteSequence { SpriteID = 138, NegativeLength = -1 },
                        Texture = new TRSpriteTexture
                        {
                            X = 177, Y = 28,
                            Width = 16897, Height = 9985,
                            LeftSide = -128, RightSide = 128,
                            TopSide = -56, BottomSide = 56
                        }
                    },
                    new SpriteDefinition
                    {
                        Sequence = new TRSpriteSequence { SpriteID = 139, NegativeLength = -1 },
                        Texture = new TRSpriteTexture
                        {
                            X = 124, Y = 0,
                            Width = 28929, Height = 7169,
                            LeftSide = -154, RightSide = 156,
                            TopSide = -76, BottomSide = 3
                        }
                    },
                    new SpriteDefinition
                    {
                        Sequence = new TRSpriteSequence { SpriteID = 140, NegativeLength = -1 },
                        Texture = new TRSpriteTexture
                        {
                            X = 0, Y = 0,
                            Width = 31745, Height = 6657,
                            LeftSide = -194, RightSide = 205,
                            TopSide = -80, BottomSide = 3
                        }
                    },
                    new SpriteDefinition
                    {
                        Sequence = new TRSpriteSequence { SpriteID = 141, NegativeLength = -1 },
                        Texture = new TRSpriteTexture
                        {
                            X = 91, Y = 63,
                            Width = 21249, Height = 8193,
                            LeftSide = -89, RightSide = 92,
                            TopSide = -67, BottomSide = 5
                        }
                    }
                },

                ["Tile8"] = img8,
                ["Tile16"] = img16
            };

            new FileInfo(jsonPath).WriteCompressedText(JsonConvert.SerializeObject(output, Formatting.Indented));
        }
    }
}