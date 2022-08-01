using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TRGE.Core
{
    public class TREdition : ICloneable
    {
        internal static readonly TREdition GenericPC = new TREdition
        {
            Title = "Unknown (PC)",
            Version = TRVersion.Unknown,
            Hardware = Hardware.PC,
            LevelCompleteOffset = 0,
            SecretBonusesSupported = false,
            SunsetsSupported = false,
            SecretSoundSupported = false,
            AssaultCourseSupported = false,
            UnarmedLevelCount = 0,
            AmmolessLevelCount = 0,
            SunsetLevelCount = 0
        };

        internal static readonly TREdition GenericPSX = new TREdition
        {
            Title = "Unknown (PSX)",
            Version = TRVersion.Unknown,
            Hardware = Hardware.PSX,
            LevelCompleteOffset = 0,
            SecretBonusesSupported = false,
            SunsetsSupported = false,
            SecretSoundSupported = false,
            AssaultCourseSupported = false,
            UnarmedLevelCount = 0,
            AmmolessLevelCount = 0,
            SunsetLevelCount = 0
        };

        internal static readonly TREdition TR1PC = new TREdition
        {
            Title = "Tomb Raider I (PC)",
            Version = TRVersion.TR1,
            Hardware = Hardware.PC,
            ScriptName = @"..\cfg\Tomb1Main_gameflow.json5",
            ConfigName = @"..\cfg\Tomb1Main.json5",
            LevelCompleteOffset = 0,
            SecretBonusesSupported = false,
            SunsetsSupported = false,
            SecretSoundSupported = false,
            AssaultCourseSupported = true,
            UnarmedLevelCount = 1,
            AmmolessLevelCount = 0,
            SunsetLevelCount = 0
        };

        internal static readonly TREdition TR2PC = new TREdition
        {
            Title = "Tomb Raider II (PC)",
            Version = TRVersion.TR2,
            ScriptName = "TOMBPC.dat",
            Hardware = Hardware.PC,
            LevelCompleteOffset = 0,
            SecretBonusesSupported = true,
            SunsetsSupported = true,
            SecretSoundSupported = true,
            AssaultCourseSupported = true,
            UnarmedLevelCount = 2,
            AmmolessLevelCount = 1,
            SunsetLevelCount = 1
        };

        internal static readonly TREdition TR2PSX = new TREdition
        {
            Title = "Tomb Raider II (PSX)",
            Version = TRVersion.TR2,
            ScriptName = "TOMBPSX.dat",
            Hardware = Hardware.PSX,
            LevelCompleteOffset = 0,
            SecretBonusesSupported = true,
            SunsetsSupported = true,
            SecretSoundSupported = true,
            AssaultCourseSupported = true,
            UnarmedLevelCount = 2,
            AmmolessLevelCount = 1,
            SunsetLevelCount = 1
        };

        internal static readonly TREdition TR2PSXBeta = new TREdition
        {
            Title = "Tomb Raider II (PSX BETA)",
            Version = TRVersion.TR2,
            ScriptName = "TOMBPSX.dat",
            Hardware = Hardware.PSX,
            LevelCompleteOffset = 0,
            SecretBonusesSupported = true,
            SunsetsSupported = true,
            SecretSoundSupported = true,
            AssaultCourseSupported = true,
            UnarmedLevelCount = 2,
            AmmolessLevelCount = 1,
            SunsetLevelCount = 1
        };

        internal static readonly TREdition TR2G = new TREdition
        {
            Title = "Tomb Raider II Gold",
            Version = TRVersion.TR2G,
            ScriptName = "TOMBPC.dat",
            Hardware = Hardware.PC,
            LevelCompleteOffset = 1,
            SecretBonusesSupported = true,
            SunsetsSupported = true,
            SecretSoundSupported = true,
            AssaultCourseSupported = true,
            UnarmedLevelCount = 0,
            AmmolessLevelCount = 0,
            SunsetLevelCount = 0
        };

        internal static readonly TREdition TR3PC = new TREdition
        {
            Title = "Tomb Raider III (PC)",
            Version = TRVersion.TR3,
            ScriptName = "TOMBPC.dat",
            Hardware = Hardware.PC,
            LevelCompleteOffset = 1,
            SecretBonusesSupported = false,
            SunsetsSupported = false,
            SecretSoundSupported = false,
            AssaultCourseSupported = true,
            UnarmedLevelCount = 1,
            AmmolessLevelCount = 1,
            SunsetLevelCount = 0
        };

        internal static readonly TREdition TR3PSX = new TREdition
        {
            Title = "Tomb Raider III (PSX)",
            Version = TRVersion.TR3,
            ScriptName = "TOMBPSX.dat",
            Hardware = Hardware.PSX,
            LevelCompleteOffset = 1,
            SecretBonusesSupported = false,
            SunsetsSupported = false,
            SecretSoundSupported = false,
            AssaultCourseSupported = true,
            UnarmedLevelCount = 1,
            AmmolessLevelCount = 1,
            SunsetLevelCount = 0
        };

        internal static readonly TREdition TR3G = new TREdition
        {
            Title = "Tomb Raider III Gold",
            Version = TRVersion.TR3G,
            ScriptName = "TOMBPC.dat",
            Hardware = Hardware.PC,
            LevelCompleteOffset = 0,
            SecretBonusesSupported = false,
            SunsetsSupported = false,
            SecretSoundSupported = false,
            AssaultCourseSupported = false,
            UnarmedLevelCount = 0,
            AmmolessLevelCount = 0,
            SunsetLevelCount = 0
        };

        internal static readonly IReadOnlyList<TREdition> All = new List<TREdition>
        {
            TR1PC, TR2PC, TR2G, TR2PSXBeta, TR2PSX, TR3PC, TR3G, TR3PSX
        };

        internal static TREdition From(Hardware hardware, TRVersion version)
        {
            return All.FirstOrDefault(e => e.Hardware == hardware && e.Version == version);
        }

        public string Title { get; internal set; }
        public TRVersion Version { get; internal set; }
        public string ScriptName { get; set; }
        public bool HasScript => ScriptName != null;
        public string ConfigName { get; internal set; }
        public bool HasConfig => ConfigName != null;
        public Hardware Hardware { get; internal set; }
        /// <summary>
        /// Indicates which level in the game is the final level. The offset
        /// marks the point from the back of a list of level sequences.
        /// </summary>
        public ushort LevelCompleteOffset { get; internal set; }
        /// <summary>
        /// Whether or not secret bonus selection/organisation is supported
        /// </summary>
        public bool SecretBonusesSupported { get; internal set; }
        /// <summary>
        /// Whether or not sunsets can be set
        /// </summary>
        public bool SunsetsSupported { get; internal set; }
        /// <summary>
        /// Whether or not the secret sound can be changed
        /// </summary>
        public bool SecretSoundSupported { get; internal set; }
        /// <summary>
        /// Whether or not the game has an assault course level
        /// </summary>
        public bool AssaultCourseSupported { get; internal set; }
        /// <summary>
        /// The default number of levels in which Lara loses her weapons.
        /// </summary>
        public int UnarmedLevelCount { get; internal set; }
        /// <summary>
        /// The default number of levels in which Lara loses her ammo.
        /// </summary>
        public int AmmolessLevelCount { get; internal set; }
        /// <summary>
        /// The default number of levels with the sunset flag enabled.
        /// </summary>
        public int SunsetLevelCount { get; internal set; }
        /// <summary>
        /// Flag to export level data to external file for third-party tools.
        /// </summary>
        public bool ExportLevelData { get; internal set; }
        /// <summary>
        /// Flag to show this version is a community patch (e.g. Tomb1Main)
        /// </summary>
        public bool IsCommunityPatch { get; internal set; }

        private TREdition() { }

        public object ToJson()
        {
            return new Dictionary<string, object>
            {
                { "Hardware", (int)Hardware },
                { "Version", (int)Version }
            };
        }

        public override bool Equals(object obj)
        {
            return obj is TREdition edition &&
                   Version == edition.Version &&
                   Hardware == edition.Hardware;
        }

        public override int GetHashCode()
        {
            int hashCode = 1948033000;
            hashCode = hashCode * -1521134295 + Version.GetHashCode();
            hashCode = hashCode * -1521134295 + Hardware.GetHashCode();
            return hashCode;
        }

        public TREdition Clone()
        {
            return (TREdition)MemberwiseClone();
        }

        object ICloneable.Clone()
        {
            return Clone();
        }
    }
}