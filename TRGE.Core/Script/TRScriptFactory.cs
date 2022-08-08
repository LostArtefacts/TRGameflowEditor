using System;
using System.IO;

namespace TRGE.Core
{
    public static class TRScriptFactory
    {
        public static AbstractTRScriptEditor GetScriptEditor(TRScriptIOArgs ioArgs, TRScriptOpenOption openOption)
        {
            uint scriptVersion = GetDatFileVersion(ioArgs.TRScriptFile == null ? null : ioArgs.TRScriptFile.FullName);
            switch (scriptVersion)
            {
                case TR1ATIScript.Version:
                    return new TR1ATIScriptEditor(ioArgs, openOption);
                case TR1Script.Version:
                    return new TR1ScriptEditor(ioArgs, openOption);
                case TR23Script.Version:
                    return new TR23ScriptEditor(ioArgs, openOption);
                default:
                    throw new UnsupportedScriptException(string.Format("An unsupported script version ({0}) was found in {1}.", scriptVersion, ioArgs.TRScriptFile.Name));
            }
        }

        public static FileInfo FindScriptFile(DirectoryInfo directory)
        {
            string dir = directory.FullName;
            foreach (TREdition edition in TREdition.All)
            {
                if (edition.HasScript)
                {
                    string script = Path.GetFullPath(Path.Combine(dir, edition.ScriptName));
                    if (File.Exists(script))
                    {
                        // We need to return the matching file name exactly to preserve edits.
                        // The matched script may not necessarily be in the current folder.
                        string scriptDir = Path.GetDirectoryName(script);
                        string scriptName = Path.GetFileName(script);
                        string match = Array.Find(Directory.GetFiles(scriptDir), f => string.Compare(Path.GetFileName(f), scriptName, true) == 0);
                        return new FileInfo(match);
                    }
                }
            }
            return null;
        }

        public static FileInfo FindConfigFile(DirectoryInfo directory)
        {
            string dir = directory.FullName;
            foreach (TREdition edition in TREdition.All)
            {
                if (edition.HasConfig)
                {
                    string config = Path.Combine(dir, edition.ConfigName);
                    if (File.Exists(config))
                    {
                        return new FileInfo(config);
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
                case TR1ATIScript.Version:
                    script = new TR1ATIScript();
                    break;
                case TR1Script.Version:
                    script = new TR1Script();
                    break;
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
            if (filePath == null)
            {
                return TR1ATIScript.Version;
            }

            string ext = Path.GetExtension(filePath).ToUpper();
            switch (ext)
            {
                case ".DAT":
                    using (BinaryReader br = new BinaryReader(new FileStream(filePath, FileMode.Open)))
                    {
                        return br.ReadUInt32();
                    }
                case ".JSON":
                case ".JSON5":
                    return TR1Script.Version;
                default:
                    return 0;
            }
        }
    }
}