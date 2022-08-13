using System.IO;

namespace TRGE.Core
{
    public static class TRPatchTester
    {
        public static void Test(TREdition edition, TRScriptIOArgs ioArgs)
        {
            switch (edition.Version)
            {
                case TRVersion.TR1:
                    TestForTR1Main(edition, ioArgs);
                    break;
                case TRVersion.TR2:
                    TestForTR2Main(edition, ioArgs);
                    break;
                case TRVersion.TR3:
                    TestForTR3Main(edition, ioArgs);
                    break;
            }
        }

        private static void TestForTR1Main(TREdition edition, TRScriptIOArgs ioArgs)
        {
            edition.IsCommunityPatch = ioArgs.TRScriptFile != null;
            if (!edition.IsCommunityPatch)
            {
                edition.ConfigName = null;
                edition.ScriptName = null;
            }
        }

        private static void TestForTR2Main(TREdition edition, TRScriptIOArgs ioArgs)
        {
            string dllPath = Path.GetFullPath(Path.Combine(ioArgs.OriginalDirectory.FullName, @"..\TR2Main.dll"));
            edition.IsCommunityPatch = File.Exists(dllPath);
        }

        private static void TestForTR3Main(TREdition edition, TRScriptIOArgs ioArgs)
        {
            string dllPath = Path.GetFullPath(Path.Combine(ioArgs.OriginalDirectory.FullName, @"..\tomb3decomp.dll"));
            edition.IsCommunityPatch = edition.ExportLevelData = File.Exists(dllPath);
        }
    }
}