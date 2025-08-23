namespace TRGE.Core;

public abstract class AbstractTRScript
{
    public TREdition Edition { get; protected set; }

    public void Read(FileInfo file)
    {
        Read(file?.FullName);
    }

    public virtual void Read(string filePath)
    {
        //All we can go on to begin with is the file name to determine a generic edition.
        //Subclasses should implement CalculateEdition and call it as appropriate while reading the data.
        Edition = filePath.ToLower().Contains("tombpsx") ? TREdition.GenericPSX : TREdition.GenericPC;

        string ext = Path.GetExtension(filePath).ToUpper();
        switch (ext)
        {
            case ".DAT":
                using (BinaryReader br = new(new FileStream(filePath, FileMode.Open)))
                {
                    ReadScriptBin(br);
                }
                break;
            case ".JSON":
            case ".JSON5":
                ReadScriptJson(File.ReadAllText(filePath));
                break;
            default:
                throw new UnsupportedScriptException();
        }
    }

    public void Write(string filePath)
    {
        if (filePath == null)
        {
            return;
        }

        Stamp();
        if (TRRScript.IsTRRScriptPath(filePath))
        {
            (this as TRRScript).WriteStrings(Path.GetDirectoryName(filePath));
            return;
        }

        string ext = Path.GetExtension(filePath).ToUpper();
        switch (ext)
        {
            case ".DAT":
                using (BinaryWriter bw = new(new FileStream(filePath, FileMode.Create)))
                {
                    bw.Write(SerialiseScriptToBin());
                }
                break;
            case ".JSON":
            case ".JSON5":
                File.WriteAllText(filePath, SerialiseScriptToJson());
                if (this is TR1Script tr1Script)
                {
                    tr1Script.WriteStrings(Path.GetDirectoryName(filePath));
                }
                break;
            default:
                throw new UnsupportedScriptException();
        }
    }

    public void WriteConfig(string outputFilePath, string originalFilePath = null)
    {
        string ext = Path.GetExtension(outputFilePath).ToUpper();
        switch (ext)
        {
            case ".JSON":
            case ".JSON5":
                string existingData = null;
                if (originalFilePath != null && File.Exists(originalFilePath))
                {
                    existingData = File.ReadAllText(originalFilePath);
                }
                File.WriteAllText(outputFilePath, SerialiseConfigToJson(existingData));
                break;
            default:
                throw new UnsupportedScriptException();
        }
    }

    public virtual void ReadScriptBin(BinaryReader br) { }
    public virtual void ReadScriptJson(string json) { }
    public virtual void ReadConfigJson(string json) { }
    public virtual byte[] SerialiseScriptToBin() { return Array.Empty<byte>(); }
    public virtual string SerialiseScriptToJson() { return string.Empty; }
    public virtual string SerialiseConfigToJson(string existingData) { return string.Empty; }
    protected abstract void CalculateEdition();
    public abstract AbstractTRFrontEnd FrontEnd { get; }
    public abstract AbstractTRScriptedLevel AssaultLevel { get; set; }
    public abstract List<AbstractTRScriptedLevel> Levels { get; set; }
    public abstract string[] GameStrings1 { get; set; }
    public abstract string[] GameStrings2 { get; set; }
    public abstract byte Language { get; set; }
    public TRLanguage TRLanguage
    {
        get => (TRLanguage)(Language);
        set => Language = (byte)value;
    }
    public abstract ushort TitleSoundID { get; set; }
    public abstract ushort SecretSoundID { get; set; }

    protected abstract void Stamp();
    protected string ApplyStamp(string gameString)
    {
        int i;
        if ((i = gameString.IndexOf(" - ")) != -1)
        {
            gameString = gameString[..i];
        }

        string stamp = TRInterop.ScriptModificationStamp.Encode(TRLanguage);
        if (this is TRRScript)
        {
            stamp = stamp.Replace("Modified by ", string.Empty);
        }
        if (stamp.Length > 0 && !stamp.Trim().StartsWith("-"))
        {
            stamp = " - " + stamp;
        }

        return gameString + stamp;
    }

    public Dictionary<string, string> GetAdditionalBackupFiles() => _additionalFiles;
    public virtual void AddAdditionalBackupFile(string sourceName, string backupName = null)
        => _additionalFiles[sourceName] = backupName ?? sourceName;

    protected Dictionary<string, string> _additionalFiles = [];
}