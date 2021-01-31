using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TRLevelReader.Model;
using TRLevelReader.Model.Enums;

namespace TRGE.Coord
{
    public class SpriteDefinition
    {
        public TRSpriteSequence Sequence { get; set; }
        public TRSpriteTexture Texture { get; set; }
        public TRTexImage16 Tile16 { get; set; }
        public TRTexImage8 Tile8 { get; set; }

        public void AddToLevel(TR2Level level)
        {
            List<TRTexImage8> spriteTiles8 = level.Images8.ToList();
            spriteTiles8.Add(Tile8);
            level.Images8 = spriteTiles8.ToArray();

            List<TRTexImage16> spriteTiles16 = level.Images16.ToList();
            spriteTiles16.Add(Tile16);
            level.Images16 = spriteTiles16.ToArray();
            level.NumImages++;

            Texture.Atlas = (ushort)(spriteTiles16.Count - 1);

            List<TRSpriteTexture> spriteTextures = level.SpriteTextures.ToList();
            spriteTextures.Add(Texture);
            level.SpriteTextures = spriteTextures.ToArray();
            level.NumSpriteTextures++;

            Sequence.Offset = (short)(spriteTextures.Count - 1);

            List<TRSpriteSequence> spriteSequences = level.SpriteSequences.ToList();
            spriteSequences.Add(Sequence);
            level.SpriteSequences = spriteSequences.ToArray();
            level.NumSpriteSequences++;
        }

        public static SpriteDefinition Load(string filePath)
        {
            return JsonConvert.DeserializeObject<SpriteDefinition>(File.ReadAllText(filePath));
        }

        internal static void WritePistolsDefinition(TR2Level baseLevel, string filePath)
        {
            SpriteDefinition pistols = new SpriteDefinition
            {
                Tile8 = baseLevel.Images8[8],
                Tile16 = baseLevel.Images16[8],
                Texture = new TRSpriteTexture
                {
                    Atlas = 0,
                    BottomSide = 11,
                    Height = 12287,
                    LeftSide = -93,
                    RightSide = 96,
                    TopSide = -102,
                    Width = 20479,
                    X = 0,
                    Y = 200
                },
                Sequence = new TRSpriteSequence
                {
                    SpriteID = (short)TR2Entities.Pistols_S_P,
                    NegativeLength = -1,
                    Offset = 0
                }
            };

            File.WriteAllText(filePath, JsonConvert.SerializeObject(pistols, Formatting.Indented));
        }
    }
}