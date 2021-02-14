using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("TRGE.Coord")]
[assembly: InternalsVisibleTo("TRGE.Core.Test")]

namespace TRGE.Core
{
    public static class TRInterop
    {
        static TRInterop()
        {
            RandomisationSupported = true;
        }

        public static string ExecutingVersion { get; set; }
        public static string TaggedVersion { get; set; }
        public static string ConfigDirectory { get; set; }
        public static bool RandomisationSupported { get; set; }
    }
}