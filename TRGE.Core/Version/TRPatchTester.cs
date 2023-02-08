﻿using System;
using System.Diagnostics;
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
            else
            {
                edition.ExeVersion = CalculateProductVersion(Path.Combine(ioArgs.OriginalDirectory.FullName, @"..\Tomb1Main.exe"));
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

        private static Version CalculateProductVersion(string exePath)
        {
            try
            {
                FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(exePath);
                int[] parts = new int[] { 0, 0, 0 };
                string[] productParts = versionInfo.ProductVersion.Split('.');
                for (int i = 0; i < productParts.Length; i++)
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
}