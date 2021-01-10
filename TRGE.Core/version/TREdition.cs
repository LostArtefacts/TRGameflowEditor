namespace TRGE.Core
{
    public class TREdition
    {
        internal static readonly TREdition GENERICPC = new TREdition
        {
            Title = "Unknown (PC)",
            Version = TRVersion.Unknown,
            Hardware = Hardware.PC
        };

        internal static readonly TREdition GENERICPSX = new TREdition
        {
            Title = "Unknown (PSX)",
            Version = TRVersion.Unknown,
            Hardware = Hardware.PSX
        };

        internal static readonly TREdition TR2PC = new TREdition
        {
            Title = "Tomb Raider II (PC)",
            Version = TRVersion.TR2,
            Hardware = Hardware.PC
        };

        internal static readonly TREdition TR2PSX = new TREdition
        {
            Title = "Tomb Raider II (PSX)",
            Version = TRVersion.TR2,
            Hardware = Hardware.PSX
        };

        internal static readonly TREdition TR2PSXBETA = new TREdition
        {
            Title = "Tomb Raider II (PSX BETA)",
            Version = TRVersion.TR2,
            Hardware = Hardware.PSX
        };

        internal static readonly TREdition TR2G = new TREdition
        {
            Title = "Tomb Raider II Gold",
            Version = TRVersion.TR2G,
            Hardware = Hardware.PC
        };

        internal static readonly TREdition TR3PC = new TREdition
        {
            Title = "Tomb Raider III (PC)",
            Version = TRVersion.TR3,
            Hardware = Hardware.PC
        };

        internal static readonly TREdition TR3PSX = new TREdition
        {
            Title = "Tomb Raider III (PSX)",
            Version = TRVersion.TR3,
            Hardware = Hardware.PSX
        };

        internal static readonly TREdition TR3G = new TREdition
        {
            Title = "Tomb Raider III Gold",
            Version = TRVersion.TR3G,
            Hardware = Hardware.PC
        };

        public string Title { get; private set; }
        public TRVersion Version { get; private set; }
        public Hardware Hardware { get; private set; }

        private TREdition() { }
    }
}