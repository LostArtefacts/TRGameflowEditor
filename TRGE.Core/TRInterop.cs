using System.Reflection;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("TRGE.Coord")]
[assembly: InternalsVisibleTo("TRGE.Core.Test")]

namespace TRGE.Core
{
    public static class TRInterop
    {
        static TRInterop()
        {
            ExecutingVersionName = "TRGE";
            ExecutingVersion = TaggedVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            RandomisationSupported = true;
            ScriptModificationStamp = "Modified by TRGE";
        }

        public static string ExecutingVersionName { get; set; }
        public static string ExecutingVersion { get; set; }
        public static string TaggedVersion { get; set; }
        public static string ConfigDirectory { get; set; }
        public static bool RandomisationSupported { get; set; }
        public static string ScriptModificationStamp { get; set; }
    }
}