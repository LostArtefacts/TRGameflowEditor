﻿namespace TRGE.Core;

public static class TRScriptFactory
{
    public static AbstractTRScriptEditor GetScriptEditor(TRScriptIOArgs ioArgs, TRScriptOpenOption openOption)
    {
        if (ioArgs.TRScriptFile != null)
        {
            switch (ioArgs.TRScriptFile.Name)
            {
                case TRRScript.TR1PlaceholderName:
                case TRRScript.TR2PlaceholderName:
                case TRRScript.TR3PlaceholderName:
                    return new TRRScriptEditor(ioArgs, openOption);
                default:
                    break;
            }
        }

        uint scriptVersion = GetDatFileVersion(ioArgs.TRScriptFile?.FullName);
        return scriptVersion switch
        {
            TR1ATIScript.Version => new TR1ATIScriptEditor(ioArgs, openOption),
            TR1Script.Version => new TR1ScriptEditor(ioArgs, openOption),
            TR23Script.Version => new TR23ScriptEditor(ioArgs, openOption),
            _ => throw new UnsupportedScriptException(string.Format("An unsupported script version ({0}) was found in {1}.", scriptVersion, ioArgs.TRScriptFile.Name)),
        };
    }

    public static FileInfo FindScriptFile(DirectoryInfo directory, bool gold)
    {
        string dir = directory.FullName;
        foreach (TREdition edition in TREdition.All)
        {
            string script = null;
            if (gold && edition.HasGold)
            {
                script = Path.GetFullPath(Path.Combine(dir, edition.GoldScriptName));
            }
            else if (edition.HasScript)
            {
                script = Path.GetFullPath(Path.Combine(dir, edition.ScriptName));
            }

            if (script != null && File.Exists(script))
            {
                // We need to return the matching file name exactly to preserve edits.
                // The matched script may not necessarily be in the current folder.
                string scriptDir = Path.GetDirectoryName(script);
                string scriptName = Path.GetFileName(script);
                string match = Array.Find(Directory.GetFiles(scriptDir), f => string.Compare(Path.GetFileName(f), scriptName, true) == 0);

                // Additional check for TRR in case the player has copied an old script file into the remastered folder. Also ugly.
                if (File.Exists(Path.Combine(dir, "TITLE.PDP")))
                {
                    continue;
                }

                return new FileInfo(match);
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
                string config = Path.GetFullPath(Path.Combine(dir, edition.ConfigName));
                if (File.Exists(config))
                {
                    return new FileInfo(config);
                }
                else if (edition.HasDefaultConfig && Directory.Exists(Path.GetDirectoryName(config)))
                {
                    // T1M no longer ships with Tomb1Main.json5 as standard, so we create
                    // a dummy file to trigger the correct processes further down the line.
                    File.WriteAllText(config, edition.DefaultConfig);
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
        AbstractTRScript script = filePath switch
        {
            TRRScript.TR1PlaceholderName => new TRRScript(TRVersion.TR1),
            TRRScript.TR2PlaceholderName => new TRRScript(TRVersion.TR2),
            TRRScript.TR3PlaceholderName => new TRRScript(TRVersion.TR3),
            _ => GetDatFileVersion(filePath) switch
            {
                TR1ATIScript.Version => new TR1ATIScript(),
                TR1Script.Version => new TR1Script(),
                TR23Script.Version => new TR23Script(),
                _ => throw new UnsupportedScriptException(),
            },
        };
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
                using (BinaryReader br = new(new FileStream(filePath, FileMode.Open)))
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