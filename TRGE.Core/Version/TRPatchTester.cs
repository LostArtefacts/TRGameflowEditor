using System.Diagnostics;
using System.Text.RegularExpressions;

namespace TRGE.Core;

public static class TRPatchTester
{
    public static void Test(TREdition edition, TRScriptIOArgs ioArgs)
    {
        if (edition.Remastered)
        {
            return;
        }

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
        else
        {
            edition.ExeVersion = CalculateProductVersion(Path.Combine(ioArgs.OriginalDirectory.FullName, "../TR1X.exe"));
        }
    }

    private static void TestForTR2Main(TREdition edition, TRScriptIOArgs ioArgs)
    {
        string dllPath = Path.GetFullPath(Path.Combine(ioArgs.OriginalDirectory.FullName, "../TR2Main.dll"));
        edition.IsCommunityPatch = File.Exists(dllPath);
    }

    private static void TestForTR3Main(TREdition edition, TRScriptIOArgs ioArgs)
    {
        FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(Path.Combine(ioArgs.OriginalDirectory.FullName, "../tomb3.exe"));
        edition.IsCommunityPatch = versionInfo.InternalName != null;
        if (edition.IsCommunityPatch)
        {
            edition.ExeVersion = CalculateProductVersion(versionInfo);
        }
    }

    private static Version CalculateProductVersion(string exePath)
    {
        return CalculateProductVersion(FileVersionInfo.GetVersionInfo(exePath));
    }

    private static Version CalculateProductVersion(FileVersionInfo versionInfo)
    {
        try
        {
            string version = versionInfo.ProductVersion;
            if (version != null)
            {
                Match m = new Regex(@"\d+\.").Match(version);
                if (m.Success)
                {
                    version = version[m.Index..].Trim();
                }
            }

            int[] parts = new int[] { 0, 0, 0 };
            string[] productParts = version.Split('.');
            for (int i = 0; i < parts.Length && i < productParts.Length; i++)
            {
                int j = 0;
                string part = string.Empty;
                while (j < productParts[i].Length && char.IsDigit(productParts[i][j]))
                {
                    part += productParts[i][j++];
                }
                parts[i] = int.Parse(part);
            }
            return new Version(parts[0], parts[1], parts[2]);
        }
        catch
        {
            return null;
        }
    }
}