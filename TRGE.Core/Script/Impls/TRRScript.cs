namespace TRGE.Core;

public class TRRScript : AbstractTRScript
{
    public const string TR1PlaceholderName = "TR1RGF.dummy";
    public const string TR2PlaceholderName = "TR2RGF.dummy";
    public const string TR3PlaceholderName = "TR3RGF.dummy";

    public static bool IsTRRScriptPath(string path)
    {
        string name = Path.GetFileName(path);
        return string.Equals(name, TR1PlaceholderName, StringComparison.CurrentCultureIgnoreCase)
            || string.Equals(name, TR2PlaceholderName, StringComparison.CurrentCultureIgnoreCase)
            || string.Equals(name, TR3PlaceholderName, StringComparison.CurrentCultureIgnoreCase);
    }

    private TRVersion _version;
    private TRRFrontEnd _frontEnd;
    public override AbstractTRFrontEnd FrontEnd => _frontEnd;

    private AbstractTRScriptedLevel _assaultLevel;
    public override AbstractTRScriptedLevel AssaultLevel
    {
        get => _assaultLevel;
        set { }
    }

    private List<AbstractTRScriptedLevel> _levels;
    public override List<AbstractTRScriptedLevel> Levels
    {
        get => _levels;
        set => _levels = value;
    }

    public override string[] GameStrings1
    {
        get => null;
        set { }
    }

    public override string[] GameStrings2
    {
        get => null;
        set { }
    }

    public override byte Language
    {
        get => byte.MaxValue;
        set { }
    }

    public override ushort TitleSoundID
    {
        get => ushort.MaxValue;
        set { }
    }

    public override ushort SecretSoundID
    {
        get => ushort.MaxValue;
        set { }
    }

    public TRRScript(TRVersion version)
    {
        _version = version;
    }

    protected override void CalculateEdition()
    {
        Edition = _version switch
        {
            TRVersion.TR1 => TREdition.TR1RM.Clone(),
            TRVersion.TR2 => TREdition.TR2RM.Clone(),
            TRVersion.TR3 => TREdition.TR3RM.Clone(),
            _ => throw new NotSupportedException(),
        };
    }

    protected override void Stamp()
    {
        //throw new NotImplementedException();
    }

    public override void Read(string filePath)
    {
        if (_version == TRVersion.Unknown)
        {
            switch (Path.GetFileName(filePath))
            {
                case TR1PlaceholderName:
                    _version = TRVersion.TR1;
                    break;
                case TR2PlaceholderName:
                    _version = TRVersion.TR2;
                    break;
                case TR3PlaceholderName:
                    _version = TRVersion.TR3;
                    break;
            }
        }
        CalculateEdition();

        switch (_version)
        {
            case TRVersion.TR1:
                BuildAsTR1();
                break;
            case TRVersion.TR2:
                BuildAsTR2();
                break;
            case TRVersion.TR3:
                BuildAsTR3();
                break;
            default:
                throw new NotSupportedException();
        }
    }

    private void BuildAsTR1()
    {
        _additionalFiles.Add(@"PIX\HD\ATLAN.DDS");
        _additionalFiles.Add(@"PIX\HD\AZTEC.DDS");
        _additionalFiles.Add(@"PIX\HD\EGYPT.DDS");
        _additionalFiles.Add(@"PIX\HD\GREEK.DDS");
        _additionalFiles.Add(@"PIX\HD\GYM.DDS");
        _additionalFiles.Add(@"PIX\HD\TITLE_EU.DDS");
        _additionalFiles.Add(@"PIX\HD\TITLE_JA.DDS");
        _additionalFiles.Add(@"PIX\HD\TITLE_US.DDS");

        _frontEnd = new()
        {
            TitleLevel = new TRRScriptedLevel(TRVersion.TR1)
            {
                Name = "Title",
                LevelFile = @"DATA\TITLE.PHD",
            }
        };

        _assaultLevel = new TRRScriptedLevel(TRVersion.TR1)
        {
            Name = "Lara's Home",
            LevelFile = @"DATA\GYM.PHD",
        };

        _levels = new()
        {
            new TRRScriptedLevel(TRVersion.TR1)
            {
                Name = "Caves",
                LevelFile = @"DATA\LEVEL1.PHD",
                Sequence = 1,
                OriginalSequence = 1,
            },
            new TRRScriptedLevel(TRVersion.TR1)
            {
                Name = "City of Vilcabamba",
                LevelFile = @"DATA\LEVEL2.PHD",
                Sequence = 2,
                OriginalSequence = 2,
            },
            new TRRScriptedLevel(TRVersion.TR1)
            {
                Name = "Lost Valley",
                LevelFile = @"DATA\LEVEL3A.PHD",
                Sequence = 3,
                OriginalSequence = 3,
            },
            new TRRScriptedLevel(TRVersion.TR1)
            {
                Name = "Tomb of Qualopec",
                LevelFile = @"DATA\LEVEL3B.PHD",
                Sequence = 4,
                OriginalSequence = 4,
                CutSceneLevel = new TRRScriptedLevel(TRVersion.TR1)
                {
                    Name = "Cut Scene 1",
                    LevelFile = @"DATA\CUT1.PHD",
                }
            },
            new TRRScriptedLevel(TRVersion.TR1)
            {
                Name = "St. Francis' Folly",
                LevelFile = @"DATA\LEVEL4.PHD",
                Sequence = 5,
                OriginalSequence = 5,
            },
            new TRRScriptedLevel(TRVersion.TR1)
            {
                Name = "Colosseum",
                LevelFile = @"DATA\LEVEL5.PHD",
                Sequence = 6,
                OriginalSequence = 6,
            },
            new TRRScriptedLevel(TRVersion.TR1)
            {
                Name = "Palace Midas",
                LevelFile = @"DATA\LEVEL6.PHD",
                Sequence = 7,
                OriginalSequence = 7,
            },
            new TRRScriptedLevel(TRVersion.TR1)
            {
                Name = "The Cistern",
                LevelFile = @"DATA\LEVEL7A.PHD",
                Sequence = 8,
                OriginalSequence = 8,
            },
            new TRRScriptedLevel(TRVersion.TR1)
            {
                Name = "Tomb of Tihocan",
                LevelFile = @"DATA\LEVEL7B.PHD",
                Sequence = 9,
                OriginalSequence = 9,
                CutSceneLevel = new TRRScriptedLevel(TRVersion.TR1)
                {
                    Name = "Cut Scene 2",
                    LevelFile = @"DATA\CUT2.PHD",
                    IgnoreMap = true
                }
            },
            new TRRScriptedLevel(TRVersion.TR1)
            {
                Name = "City of Khamoon",
                LevelFile = @"DATA\LEVEL8A.PHD",
                Sequence = 10,
                OriginalSequence = 10,
            },
            new TRRScriptedLevel(TRVersion.TR1)
            {
                Name = "Obelisk of Khamoon",
                LevelFile = @"DATA\LEVEL8B.PHD",
                Sequence = 11,
                OriginalSequence = 11,
            },
            new TRRScriptedLevel(TRVersion.TR1)
            {
                Name = "Sanctuary of the Scion",
                LevelFile = @"DATA\LEVEL8C.PHD",
                Sequence = 12,
                OriginalSequence = 12,
            },
            new TRRScriptedLevel(TRVersion.TR1)
            {
                Name = "Natla's Mines",
                LevelFile = @"DATA\LEVEL10A.PHD",
                Sequence = 13,
                OriginalSequence = 13,
                RemovesWeapons = true,
                CutSceneLevel = new TRRScriptedLevel(TRVersion.TR1)
                {
                    Name = "Cut Scene 3",
                    LevelFile = @"DATA\CUT3.PHD",
                }
            },
            new TRRScriptedLevel(TRVersion.TR1)
            {
                Name = "Atlantis",
                LevelFile = @"DATA\LEVEL10B.PHD",
                Sequence = 14,
                OriginalSequence = 14,
                CutSceneLevel = new TRRScriptedLevel(TRVersion.TR1)
                {
                    Name = "Cut Scene 4",
                    LevelFile = @"DATA\CUT4.PHD",
                }
            },
            new TRRScriptedLevel(TRVersion.TR1)
            {
                Name = "The Great Pyramid",
                LevelFile = @"DATA\LEVEL10C.PHD",
                Sequence = 15,
                OriginalSequence = 15,
                IsFinalLevel = true
            }
        };
    }

    private void BuildAsTR2()
    {
        _additionalFiles.Add(@"PIX\HD\CHINA.DDS");
        _additionalFiles.Add(@"PIX\HD\MANSION.DDS");
        _additionalFiles.Add(@"PIX\HD\RIG.DDS");
        _additionalFiles.Add(@"PIX\HD\TIBET.DDS");
        _additionalFiles.Add(@"PIX\HD\TITAN.DDS");
        _additionalFiles.Add(@"PIX\HD\TITLE_EU.DDS");
        _additionalFiles.Add(@"PIX\HD\TITLE_JA.DDS");
        _additionalFiles.Add(@"PIX\HD\TITLE_US.DDS");
        _additionalFiles.Add(@"PIX\HD\VENICE.DDS");

        _frontEnd = new()
        {
            TitleLevel = new TRRScriptedLevel(TRVersion.TR2)
            {
                Name = "Title",
                LevelFile = @"DATA\TITLE.TR2",
            }
        };

        _assaultLevel = new TRRScriptedLevel(TRVersion.TR2)
        {
            Name = "Lara's Home",
            LevelFile = @"DATA\ASSAULT.TR2",
        };

        _levels = new()
        {
            new TRRScriptedLevel(TRVersion.TR2)
            {
                Name = "The Great Wall",
                LevelFile = @"DATA\WALL.TR2",
                Sequence = 1,
                OriginalSequence = 1,
                CutSceneLevel = new TRRScriptedLevel(TRVersion.TR2)
                {
                    Name = "Cut Scene 1",
                    LevelFile = @"DATA\CUT1.TR2",
                }
            },
            new TRRScriptedLevel(TRVersion.TR2)
            {
                Name = "Venice",
                LevelFile = @"DATA\BOAT.TR2",
                Sequence = 2,
                OriginalSequence = 2,
            },
            new TRRScriptedLevel(TRVersion.TR2)
            {
                Name = "Bartoli's Hideout",
                LevelFile = @"DATA\VENICE.TR2",
                Sequence = 3,
                OriginalSequence = 3,
            },
            new TRRScriptedLevel(TRVersion.TR2)
            {
                Name = "Opera House",
                LevelFile = @"DATA\OPERA.TR2",
                Sequence = 4,
                OriginalSequence = 4,
                CutSceneLevel = new TRRScriptedLevel(TRVersion.TR2)
                {
                    Name = "Cut Scene 2",
                    LevelFile = @"DATA\CUT2.TR2",
                }
            },
            new TRRScriptedLevel(TRVersion.TR2)
            {
                Name = "Offshore Rig",
                LevelFile = @"DATA\RIG.TR2",
                Sequence = 5,
                OriginalSequence = 5,
                HasStartAnimation = true,
                RemovesWeapons = true,
            },
            new TRRScriptedLevel(TRVersion.TR2)
            {
                Name = "Diving Area",
                LevelFile = @"DATA\PLATFORM.TR2",
                Sequence = 6,
                OriginalSequence = 6,
                CutSceneLevel = new TRRScriptedLevel(TRVersion.TR2)
                {
                    Name = "Cut Scene 3",
                    LevelFile = @"DATA\CUT3.TR2",
                }
            },
            new TRRScriptedLevel(TRVersion.TR2)
            {
                Name = "40 Fathoms",
                LevelFile = @"DATA\UNWATER.TR2",
                Sequence = 7,
                OriginalSequence = 7,
            },
            new TRRScriptedLevel(TRVersion.TR2)
            {
                Name = "Wreck of the Maria Doria",
                LevelFile = @"DATA\KEEL.TR2",
                Sequence = 8,
                OriginalSequence = 8,
            },
            new TRRScriptedLevel(TRVersion.TR2)
            {
                Name = "Living Quarters",
                LevelFile = @"DATA\LIVING.TR2",
                Sequence = 9,
                OriginalSequence = 9,
            },
            new TRRScriptedLevel(TRVersion.TR2)
            {
                Name = "The Deck",
                LevelFile = @"DATA\DECK.TR2",
                Sequence = 10,
                OriginalSequence = 10,
            },
            new TRRScriptedLevel(TRVersion.TR2)
            {
                Name = "Tibetan Foothills",
                LevelFile = @"DATA\SKIDOO.TR2",
                Sequence = 11,
                OriginalSequence = 11,
            },
            new TRRScriptedLevel(TRVersion.TR2)
            {
                Name = "Barkhang Monastery",
                LevelFile = @"DATA\MONASTRY.TR2",
                Sequence = 12,
                OriginalSequence = 12,
            },
            new TRRScriptedLevel(TRVersion.TR2)
            {
                Name = "Catacombs of the Talion",
                LevelFile = @"DATA\CATACOMB.TR2",
                Sequence = 13,
                OriginalSequence = 13,
            },
            new TRRScriptedLevel(TRVersion.TR2)
            {
                Name = "Ice Palace",
                LevelFile = @"DATA\ICECAVE.TR2",
                Sequence = 14,
                OriginalSequence = 14,
            },
            new TRRScriptedLevel(TRVersion.TR2)
            {
                Name = "Temple of Xian",
                LevelFile = @"DATA\EMPRTOMB.TR2",
                Sequence = 15,
                OriginalSequence = 15,
                CutSceneLevel = new TRRScriptedLevel(TRVersion.TR2)
                {
                    Name = "Cut Scene 4",
                    LevelFile = @"DATA\CUT4.TR2",
                },
            },
            new TRRScriptedLevel(TRVersion.TR2)
            {
                Name = "Floating Islands",
                LevelFile = @"DATA\FLOATING.TR2",
                Sequence = 16,
                OriginalSequence = 16,
            },
            new TRRScriptedLevel(TRVersion.TR2)
            {
                Name = "The Dragon's Lair",
                LevelFile = @"DATA\XIAN.TR2",
                Sequence = 17,
                OriginalSequence = 17,
            },
            new TRRScriptedLevel(TRVersion.TR2)
            {
                Name = "Home Sweet Home",
                LevelFile = @"DATA\HOUSE.TR2",
                Sequence = 18,
                OriginalSequence = 18,
                IsFinalLevel = true,
                HasStartAnimation = true,
                RemovesWeapons = true,
                RemovesAmmo = true,
            },
        };
    }

    private void BuildAsTR3()
    {
        _additionalFiles.Add(@"PIX\HD\ANTARC.DDS");
        _additionalFiles.Add(@"PIX\HD\HOUSE.DDS");
        _additionalFiles.Add(@"PIX\HD\INDIA.DDS");
        _additionalFiles.Add(@"PIX\HD\LONDON.DDS");
        _additionalFiles.Add(@"PIX\HD\NEVADA.DDS");
        _additionalFiles.Add(@"PIX\HD\SOUTHPAC.DDS");
        _additionalFiles.Add(@"PIX\HD\TITLE_EU.DDS");
        _additionalFiles.Add(@"PIX\HD\TITLE_JA.DDS");
        _additionalFiles.Add(@"PIX\HD\TITLE_US.DDS");

        _frontEnd = new()
        {
            TitleLevel = new TRRScriptedLevel(TRVersion.TR3)
            {
                Name = "Title",
                LevelFile = @"DATA\TITLE.TR2",
            }
        };

        _assaultLevel = new TRRScriptedLevel(TRVersion.TR3)
        {
            Name = "Lara's Home",
            LevelFile = @"DATA\HOUSE.TR2",
        };

        _levels = new()
        {
            new TRRScriptedLevel(TRVersion.TR3)
            {
                Name = "Jungle",
                LevelFile = @"DATA\JUNGLE.TR2",
                Sequence = 1,
                OriginalSequence = 1,
                CutSceneLevel = new TRRScriptedLevel(TRVersion.TR3)
                {
                    Name = "Cut Scene 1",
                    LevelFile = @"CUTS\CUT6.TR2",
                }
            },
            new TRRScriptedLevel(TRVersion.TR3)
            {
                Name = "Temple Ruins",
                LevelFile = @"DATA\TEMPLE.TR2",
                Sequence = 2,
                OriginalSequence = 2,
                CutSceneLevel = new TRRScriptedLevel(TRVersion.TR3)
                {
                    Name = "Cut Scene 2",
                    LevelFile = @"CUTS\CUT9.TR2",
                }
            },
            new TRRScriptedLevel(TRVersion.TR3)
            {
                Name = "The River Ganges",
                LevelFile = @"DATA\QUADCHAS.TR2",
                Sequence = 3,
                OriginalSequence = 3,
            },
            new TRRScriptedLevel(TRVersion.TR3)
            {
                Name = "Caves of Kaliya",
                LevelFile = @"DATA\TONYBOSS.TR2",
                Sequence = 4,
                OriginalSequence = 4,
            },
            new TRRScriptedLevel(TRVersion.TR3)
            {
                Name = "Coastal Village",
                LevelFile = @"DATA\SHORE.TR2",
                Sequence = 5,
                OriginalSequence = 5,
                CutSceneLevel = new TRRScriptedLevel(TRVersion.TR3)
                {
                    Name = "Cut Scene 3",
                    LevelFile = @"CUTS\CUT1.TR2",
                }
            },
            new TRRScriptedLevel(TRVersion.TR3)
            {
                Name = "Crash Site",
                LevelFile = @"DATA\CRASH.TR2",
                Sequence = 6,
                OriginalSequence = 6,
                CutSceneLevel = new TRRScriptedLevel(TRVersion.TR3)
                {
                    Name = "Cut Scene 4",
                    LevelFile = @"CUTS\CUT4.TR2",
                }
            },
            new TRRScriptedLevel(TRVersion.TR3)
            {
                Name = "Madubu Gorge",
                LevelFile = @"DATA\RAPIDS.TR2",
                Sequence = 7,
                OriginalSequence = 7,
            },
            new TRRScriptedLevel(TRVersion.TR3)
            {
                Name = "Temple of Puna",
                LevelFile = @"DATA\TRIBOSS.TR2",
                Sequence = 8,
                OriginalSequence = 8,
            },
            new TRRScriptedLevel(TRVersion.TR3)
            {
                Name = "Thames Wharf",
                LevelFile = @"DATA\ROOFS.TR2",
                Sequence = 9,
                OriginalSequence = 9,
                CutSceneLevel = new TRRScriptedLevel(TRVersion.TR3)
                {
                    Name = "Cut Scene 5",
                    LevelFile = @"CUTS\CUT2.TR2",
                }
            },
            new TRRScriptedLevel(TRVersion.TR3)
            {
                Name = "Aldwych",
                LevelFile = @"DATA\SEWER.TR2",
                Sequence = 10,
                OriginalSequence = 10,
                CutSceneLevel = new TRRScriptedLevel(TRVersion.TR3)
                {
                    Name = "Cut Scene 6",
                    LevelFile = @"CUTS\CUT5.TR2",
                }
            },
            new TRRScriptedLevel(TRVersion.TR3)
            {
                Name = "Lud's Gate",
                LevelFile = @"DATA\TOWER.TR2",
                Sequence = 11,
                OriginalSequence = 11,
                CutSceneLevel = new TRRScriptedLevel(TRVersion.TR3)
                {
                    Name = "Cut Scene 7",
                    LevelFile = @"CUTS\CUT11.TR2",
                }
            },
            new TRRScriptedLevel(TRVersion.TR3)
            {
                Name = "City",
                LevelFile = @"DATA\OFFICE.TR2",
                Sequence = 12,
                OriginalSequence = 12,
            },
            new TRRScriptedLevel(TRVersion.TR3)
            {
                Name = "Nevada",
                LevelFile = @"DATA\NEVADA.TR2",
                Sequence = 13,
                OriginalSequence = 13,
                CutSceneLevel = new TRRScriptedLevel(TRVersion.TR3)
                {
                    Name = "Cut Scene 8",
                    LevelFile = @"CUTS\CUT7.TR2",
                }
            },
            new TRRScriptedLevel(TRVersion.TR3)
            {
                Name = "High Security Compound",
                LevelFile = @"DATA\COMPOUND.TR2",
                Sequence = 14,
                OriginalSequence = 14,
                HasStartAnimation = true,
                RemovesWeapons = true,
                RemovesAmmo = true,
                CutSceneLevel = new TRRScriptedLevel(TRVersion.TR3)
                {
                    Name = "Cut Scene 9",
                    LevelFile = @"CUTS\CUT8.TR2",
                }
            },
            new TRRScriptedLevel(TRVersion.TR3)
            {
                Name = "Area 51",
                LevelFile = @"DATA\AREA51.TR2",
                Sequence = 15,
                OriginalSequence = 15,
            },
            new TRRScriptedLevel(TRVersion.TR3)
            {
                Name = "Antarctica",
                LevelFile = @"DATA\ANTARC.TR2",
                Sequence = 16,
                OriginalSequence = 16,
                HasColdWater = true,
                CutSceneLevel = new TRRScriptedLevel(TRVersion.TR3)
                {
                    Name = "Cut Scene 10",
                    LevelFile = @"CUTS\CUT3.TR2",
                }
            },
            new TRRScriptedLevel(TRVersion.TR3)
            {
                Name = "RX-Tech Mines",
                LevelFile = @"DATA\MINES.TR2",
                Sequence = 17,
                OriginalSequence = 17,
                HasColdWater = true,
            },
            new TRRScriptedLevel(TRVersion.TR3)
            {
                Name = "Lost City of Tinnos",
                LevelFile = @"DATA\CITY.TR2",
                Sequence = 18,
                OriginalSequence = 18,
                CutSceneLevel = new TRRScriptedLevel(TRVersion.TR3)
                {
                    Name = "Cut Scene 11",
                    LevelFile = @"CUTS\CUT12.TR2",
                }
            },
            new TRRScriptedLevel(TRVersion.TR3)
            {
                Name = "Meteorite Cavern",
                LevelFile = @"DATA\CHAMBER.TR2",
                Sequence = 19,
                OriginalSequence = 19,
            },
            new TRRScriptedLevel(TRVersion.TR3)
            {
                Name = "All Hallows",
                LevelFile = @"DATA\STPAUL.TR2",
                Sequence = 20,
                OriginalSequence = 20,
                IsFinalLevel = true,
            },
        };
    }
}
