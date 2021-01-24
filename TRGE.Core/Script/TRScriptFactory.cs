using System.IO;

namespace TRGE.Core
{
    public static class TRScriptFactory
    {
        public static AbstractTRScriptManager GetScriptManager(string originalScriptFile, string backupScriptFile, string configFile, TRScriptOpenOption openOption)
        {
            return GetScriptManager(new FileInfo(originalScriptFile), new FileInfo(backupScriptFile), new FileInfo(configFile), openOption);
        }

        public static AbstractTRScriptManager GetScriptManager(FileInfo originalScriptFile, FileInfo backupScriptFile, FileInfo configFile, TRScriptOpenOption openOption)
        {
            uint scriptVersion = GetDatFileVersion(originalScriptFile.FullName);
            switch (scriptVersion)
            {
                case TR23Script.Version:
                    return new TR23ScriptManager(originalScriptFile, backupScriptFile, configFile, openOption);
                default:
                    throw new UnsupportedScriptException(string.Format("An unsupported script version ({0}) was found in {1}.", scriptVersion, originalScriptFile.Name));
            }
        }

        public static FileInfo FindScriptFile(DirectoryInfo directory)
        {
            foreach (FileInfo fi in directory.GetFiles())
            {
                string fileName = fi.Name.ToLower();
                foreach (TREdition edition in TREdition.All)
                {
                    if (fileName.Equals(edition.ScriptName.ToLower()))
                    {
                        return fi;
                    }
                }
            }
            return null;
        }

        internal static AbstractTRScript OpenScript(FileInfo file)
        {
            return OpenScript(file.FullName);
        }

        internal static AbstractTRScript OpenScript(string filePath)
        {
            AbstractTRScript script;
            switch (GetDatFileVersion(filePath))
            {
                case TR23Script.Version:
                    script = new TR23Script();
                    break;
                default:
                    throw new UnsupportedScriptException();
            }

            script.Read(filePath);
            return script;
        }

        private static uint GetDatFileVersion(string filePath)
        {
            using (BinaryReader br = new BinaryReader(new FileStream(filePath, FileMode.Open)))
            {
                return br.ReadUInt32();
            }
        }
    }
}