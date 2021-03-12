using System.IO;

namespace TRGE.Core
{
    public static class TRScriptFactory
    {
        public static AbstractTRScriptEditor GetScriptEditor(TRScriptIOArgs ioArgs, TRScriptOpenOption openOption)
        {
            uint scriptVersion = GetDatFileVersion(ioArgs.OriginalFile.FullName);
            switch (scriptVersion)
            {
                case TR23Script.Version:
                    return new TR23ScriptEditor(ioArgs, openOption);
                default:
                    throw new UnsupportedScriptException(string.Format("An unsupported script version ({0}) was found in {1}.", scriptVersion, ioArgs.OriginalFile.Name));
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

        public static AbstractTRScript OpenScript(FileInfo file)
        {
            return OpenScript(file.FullName);
        }

        public static AbstractTRScript OpenScript(string filePath)
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