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
            ScriptModificationStamp = new GameStamp
            {
                [TRLanguage.English] = "Modified by TRGE",
                [TRLanguage.French] = "Modifié par TRGE",
                [TRLanguage.German] = "Geändert von TRGE"
            };
        }

        public static string ExecutingVersionName { get; set; }
        public static string ExecutingVersion { get; set; }
        public static string TaggedVersion { get; set; }
        public static string ConfigDirectory { get; set; }
        public static bool RandomisationSupported { get; set; }
        public static bool SecretRewardsSupported { get; set; }
        //public static string ScriptModificationStamp { get; set; }
        public static GameStamp ScriptModificationStamp { get; }
        public static bool UsingTRMain { get; set; }
    }
}