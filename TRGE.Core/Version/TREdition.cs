namespace TRGE.Core
{
    public class TREdition
    {
        internal static readonly TREdition GenericPC = new TREdition
        {
            Title = "Unknown (PC)",
            Version = TRVersion.Unknown,
            Hardware = Hardware.PC,
            LevelCompleteOffset = 0,
            SecretBonusesSupported = false
        };

        internal static readonly TREdition GenericPSX = new TREdition
        {
            Title = "Unknown (PSX)",
            Version = TRVersion.Unknown,
            Hardware = Hardware.PSX,
            LevelCompleteOffset = 0,
            SecretBonusesSupported = false
        };

        internal static readonly TREdition TR2PC = new TREdition
        {
            Title = "Tomb Raider II (PC)",
            Version = TRVersion.TR2,
            Hardware = Hardware.PC,
            LevelCompleteOffset = 0,
            SecretBonusesSupported = true
        };

        internal static readonly TREdition TR2PSX = new TREdition
        {
            Title = "Tomb Raider II (PSX)",
            Version = TRVersion.TR2,
            Hardware = Hardware.PSX,
            LevelCompleteOffset = 0,
            SecretBonusesSupported = true
        };

        internal static readonly TREdition TR2PSXBeta = new TREdition
        {
            Title = "Tomb Raider II (PSX BETA)",
            Version = TRVersion.TR2,
            Hardware = Hardware.PSX,
            LevelCompleteOffset = 0,
            SecretBonusesSupported = true
        };

        internal static readonly TREdition TR2G = new TREdition
        {
            Title = "Tomb Raider II Gold",
            Version = TRVersion.TR2G,
            Hardware = Hardware.PC,
            LevelCompleteOffset = 1,
            SecretBonusesSupported = true
        };

        internal static readonly TREdition TR3PC = new TREdition
        {
            Title = "Tomb Raider III (PC)",
            Version = TRVersion.TR3,
            Hardware = Hardware.PC,
            LevelCompleteOffset = 1,
            SecretBonusesSupported = false
        };

        internal static readonly TREdition TR3PSX = new TREdition
        {
            Title = "Tomb Raider III (PSX)",
            Version = TRVersion.TR3,
            Hardware = Hardware.PSX,
            LevelCompleteOffset = 1,
            SecretBonusesSupported = false
        };

        internal static readonly TREdition TR3G = new TREdition
        {
            Title = "Tomb Raider III Gold",
            Version = TRVersion.TR3G,
            Hardware = Hardware.PC,
            LevelCompleteOffset = 0,
            SecretBonusesSupported = false
        };

        public string Title { get; private set; }
        public TRVersion Version { get; private set; }
        public Hardware Hardware { get; private set; }
        /// <summary>
        /// Indicates which level in the game is the final level. The offset
        /// marks the point from the back of a list of level sequences.
        /// </summary>
        public ushort LevelCompleteOffset { get; private set; }
        /// <summary>
        /// Whether or not secret bonus selection/organisation is supported
        /// </summary>
        public bool SecretBonusesSupported { get; private set; }

        private TREdition() { }

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
    }
}