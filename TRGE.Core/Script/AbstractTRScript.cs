namespace TRGE.Core
{
    public abstract class AbstractTRScript
    {
        public TREdition Edition { get; protected set; }

        public void Read(FileInfo file)
        {
            Read(file == null ? null : file.FullName);
        }

        public void ReadConfig(FileInfo file)
        {
            ReadConfig(file.FullName);
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

        public void ReadConfig(string filePath)
        {
            string ext = Path.GetExtension(filePath).ToUpper();
            switch (ext)
            {
                case ".JSON":
                case ".JSON5":
                    ReadConfigJson(File.ReadAllText(filePath));
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
                gameString = gameString.Substring(0, i);
            }

            string stamp = TRInterop.ScriptModificationStamp.Encode(TRLanguage);
            if (stamp.Length > 0 && !stamp.Trim().StartsWith("-"))
            {
                stamp = " - " + stamp;
            }

            return gameString + stamp;
        }

        public List<string> GetAdditionalBackupFiles() => _additionalFiles;
        public virtual void AddAdditionalBackupFile(string file) => _additionalFiles.Add(file);

        protected List<string> _additionalFiles = new();
    }
}