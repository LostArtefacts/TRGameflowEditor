using Newtonsoft.Json;
using System.Text;
using TRGE.Coord.Properties;
using TRGE.Core;
using TRLevelControl;
using TRLevelControl.Model;

namespace TRGE.Coord;

public class SpriteDefinition
{
    public TRSpriteSequence Sequence { get; set; }
    public TRSpriteTexture Texture { get; set; }
    public TRTexImage16 Tile16 { get; set; }
    public TRTexImage8 Tile8 { get; set; }

    public static void LoadWeaponsIntoLevel(TR2Level level)
    {
        Dictionary<string, object> weaponDefinitions = JsonConvert.DeserializeObject<Dictionary<string, object>>(Encoding.Default.GetString(ResourceHelper.Decompress(Resources.Weapons)));
        Dictionary<TR2Type, TRSpriteSequence> loadedDefinitions = JsonConvert.DeserializeObject<Dictionary<TR2Type, TRSpriteSequence>>(weaponDefinitions["Weapons"].ToString());

        TRTexImage16 img16 = JsonConvert.DeserializeObject<TRTexImage16>(weaponDefinitions["Tile16"].ToString());
        TRTexImage8 img8 = JsonConvert.DeserializeObject<TRTexImage8>(weaponDefinitions["Tile8"].ToString());
                    
        if (loadedDefinitions.Count == 0)
        {
            throw new IOException("Failed to load default weapon textures.");
        }

        level.Images8.Add(img8);
        level.Images16.Add(img16);

        foreach (var (type, def) in loadedDefinitions)
        {
            def.Textures.ForEach(t => t.Atlas = (ushort)(level.Images16.Count - 1));
            level.Sprites[type] = def;
        }
    }

    public static void WriteWeaponDefinitions(TRTexImage8 img8, TRTexImage16 img16, string jsonPath)
    {
        Dictionary<string, object> output = new()
        {
            ["Weapons"] = new Dictionary<TR2Type, TRSpriteSequence>
            {
                [TR2Type.Pistols_S_P] = new()
                {
                    Textures = new()
                    {
                        new()
                        {
                            X = 104,
                            Y = 28,
                            Width = 18687,
                            Height = 8959,
                            Alignment = new()
                            {
                                Left = -93,
                                Right = 96,
                                Top = -102,
                                Bottom = 11
                            }
                        }
                    }
                },
                [TR2Type.Shotgun_S_P] = new()
                {
                    Textures = new()
                    {
                        new()
                        {
                            X = 0,
                            Y = 26,
                            Width = 26623,
                            Height = 5375,
                            Alignment = new()
                            {
                                Left = -240,
                                Right = 240,
                                Top = -64,
                                Bottom = 32
                            }
                        }
                    }
                },
                [TR2Type.Automags_S_P] = new()
                {
                    Textures = new()
                    {
                        new()
                        {
                            X = 0,
                            Y = 47,
                            Width = 23295,
                            Height = 9215,
                            Alignment = new()
                            {
                                Left = -100,
                                Right = 100,
                                Top = -76,
                                Bottom = 7
                            }
                        }
                    }
                },
                [TR2Type.Uzi_S_P] = new()
                {
                    Textures = new()
                    {
                        new()
                        {
                            X = 177,
                            Y = 28,
                            Width = 16897,
                            Height = 9985,
                            Alignment = new()
                            {
                                Left = -128,
                                Right = 128,
                                Top = -56,
                                Bottom =56
                            }
                        }
                    }
                },
                [TR2Type.Harpoon_S_P] = new()
                {
                    Textures = new()
                    {
                        new()
                        {
                            X = 124,
                            Y = 0,
                            Width = 28929,
                            Height = 7169,
                            Alignment = new()
                            {
                                Left = -154,
                                Right = 156,
                                Top = -76,
                                Bottom = 3
                            }
                        }
                    }
                },
                [TR2Type.M16_S_P] = new()
                {
                    Textures = new()
                    {
                        new()
                        {
                            X = 0,
                            Y = 0,
                            Width = 31745,
                            Height = 6657,
                            Alignment = new()
                            {
                                Left = -194,
                                Right = 205,
                                Top = -80,
                                Bottom = 3
                            }
                        }
                    }
                },
                [TR2Type.GrenadeLauncher_S_P] = new()
                {
                    Textures = new()
                    {
                        new()
                        {
                            X = 91,
                            Y = 63,
                            Width = 21249,
                            Height = 8193,
                            Alignment = new()
                            {
                                Left = -89,
                                Right = 92,
                                Top = -67,
                                Bottom = 5
                            }
                        }
                    }
                },
            },

            ["Tile8"] = img8,
            ["Tile16"] = img16
        };
                
        Dictionary<TR2Type, TRSpriteSequence> d = (Dictionary<TR2Type, TRSpriteSequence>)output["Weapons"];
        foreach (TRSpriteTexture t in d.Values.SelectMany(f => f.Textures))
        {
            t.Width = (ushort)((t.Width + 1) / TRConsts.TPageWidth);
            t.Height = (ushort)((t.Height + 1) / TRConsts.TPageHeight);
        }

        new FileInfo(jsonPath).WriteCompressedText(JsonConvert.SerializeObject(output, Formatting.Indented));
    }
}