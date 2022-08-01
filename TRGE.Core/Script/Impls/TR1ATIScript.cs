using System.Collections.Generic;

namespace TRGE.Core
{
    public class TR1ATIScript : AbstractTRScript
    {
        public const int Version = 0;

        private TR1FrontEnd _frontEnd;
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
            set { }
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

        public override void Read(string filePath)
        {
            CalculateEdition();

            _frontEnd = new TR1FrontEnd
            {
                TitleLevel = new TR1ScriptedLevel
                {
                    Name = "Title",
                    LevelFile = @"data\title.phd",
                    Sequences = new List<BaseLevelSequence>()
                }
            };

            _assaultLevel = new TR1ScriptedLevel
            {
                Name = "Lara's Home",
                LevelFile = @"data\gym.phd",
                Sequences = new List<BaseLevelSequence>()
            };

            _levels = new List<AbstractTRScriptedLevel>
            {
                new TR1ScriptedLevel
                {
                    Name = "Caves",
                    LevelFile = @"data\level1.phd",
                    Sequence = 1,
                    Sequences = new List<BaseLevelSequence>()
                },
                new TR1ScriptedLevel
                {
                    Name = "City of Vilcabamba",
                    LevelFile = @"data\level2.phd",
                    Sequence = 2,
                    Sequences = new List<BaseLevelSequence>()
                },
                new TR1ScriptedLevel
                {
                    Name = "Lost Valley",
                    LevelFile = @"data\level3a.phd",
                    Sequence = 3,
                    Sequences = new List<BaseLevelSequence>()
                },
                new TR1ScriptedLevel
                {
                    Name = "Tomb of Qualopec",
                    LevelFile = @"data\level3b.phd",
                    Sequence = 4,
                    Sequences = new List<BaseLevelSequence>(),
                    CutSceneLevel = new TR1ScriptedLevel
                    {
                        Name = "Cut Scene 1",
                        LevelFile = @"data\cut1.phd",
                        Sequences = new List<BaseLevelSequence>()
                    }
                },
                new TR1ScriptedLevel
                {
                    Name = "St. Francis' Folly",
                    LevelFile = @"data\level4.phd",
                    Sequence = 5,
                    Sequences = new List<BaseLevelSequence>()
                },
                new TR1ScriptedLevel
                {
                    Name = "Colosseum",
                    LevelFile = @"data\level5.phd",
                    Sequence = 6,
                    Sequences = new List<BaseLevelSequence>()
                },
                new TR1ScriptedLevel
                {
                    Name = "Palace Midas",
                    LevelFile = @"data\level6.phd",
                    Sequence = 7,
                    Sequences = new List<BaseLevelSequence>()
                },
                new TR1ScriptedLevel
                {
                    Name = "The Cistern",
                    LevelFile = @"data\level7a.phd",
                    Sequence = 8,
                    Sequences = new List<BaseLevelSequence>()
                },
                new TR1ScriptedLevel
                {
                    Name = "Tomb of Tihocan",
                    LevelFile = @"data\level7b.phd",
                    Sequence = 9,
                    Sequences = new List<BaseLevelSequence>(),
                    CutSceneLevel = new TR1ScriptedLevel
                    {
                        Name = "Cut Scene 2",
                        LevelFile = @"data\cut2.phd",
                        Sequences = new List<BaseLevelSequence>()
                    }
                },
                new TR1ScriptedLevel
                {
                    Name = "City of Khamoon",
                    LevelFile = @"data\level8a.phd",
                    Sequence = 10,
                    Sequences = new List<BaseLevelSequence>()
                },
                new TR1ScriptedLevel
                {
                    Name = "Obelisk of Khamoon",
                    LevelFile = @"data\level8b.phd",
                    Sequence = 11,
                    Sequences = new List<BaseLevelSequence>()
                },
                new TR1ScriptedLevel
                {
                    Name = "Sanctuary of the Scion",
                    LevelFile = @"data\level8c.phd",
                    Sequence = 12,
                    Sequences = new List<BaseLevelSequence>()
                },
                new TR1ScriptedLevel
                {
                    Name = "Natla's Mines",
                    LevelFile = @"data\level10a.phd",
                    Sequence = 13,
                    Sequences = new List<BaseLevelSequence>(),
                    CutSceneLevel = new TR1ScriptedLevel
                    {
                        Name = "Cut Scene 3",
                        LevelFile = @"data\cut3.phd",
                        Sequences = new List<BaseLevelSequence>()
                    }
                },
                new TR1ScriptedLevel
                {
                    Name = "Atlantis",
                    LevelFile = @"data\level10b.phd",
                    Sequence = 14,
                    Sequences = new List<BaseLevelSequence>(),
                    CutSceneLevel = new TR1ScriptedLevel
                    {
                        Name = "Cut Scene 4",
                        LevelFile = @"data\cut4.phd",
                        Sequences = new List<BaseLevelSequence>()
                    }
                },
                new TR1ScriptedLevel
                {
                    Name = "The Great Pyramid",
                    LevelFile = @"data\level10c.phd",
                    Sequence = 15,
                    Sequences = new List<BaseLevelSequence>()
                }
            };
        }

        protected override void CalculateEdition()
        {
            Edition = TREdition.TR1PC.Clone();
        }

        protected override void Stamp() { }
    }
}