using System.IO;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("TRGE.Coord")]
[assembly: InternalsVisibleTo("TRGE.Core.Test")]

namespace TRGE.Core
{
    public static class TRInterop
    {
        public static string ConfigDirectory { get; set; }
    }
}