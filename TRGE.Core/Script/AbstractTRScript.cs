using System.IO;
using System.Collections.Generic;

namespace TRGE.Core
{
    public abstract class AbstractTRScript
    {
        public TREdition Edition { get; protected set; }

        public void Read(FileInfo file)
        {
            Read(file.FullName);
        }

        public void Read(string filePath)
        {
            //All we can go on to begin with is the file name to determine a generic edition.
            //Subclasses should implement CalculateEdition and call it as appropriate while reading the data.
            Edition = filePath.ToLower().Contains("tombpsx") ? TREdition.GenericPSX : TREdition.GenericPC;

            using (BinaryReader br = new BinaryReader(new FileStream(filePath, FileMode.Open)))
            {
                Read(br);
            }
        }

        public void Write(string filePath)
        {
            Stamp();
            using (BinaryWriter bw = new BinaryWriter(new FileStream(filePath, FileMode.Create)))
            {
                bw.Write(Serialise());
            }
        }

        public abstract void Read(BinaryReader br);
        public abstract byte[] Serialise();
        protected abstract void CalculateEdition();
        public abstract AbstractTRFrontEnd FrontEnd { get; }
        public abstract List<AbstractTRScriptedLevel> Levels { get; set; }
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

            string stamp = TRInterop.ScriptModificationStamp[TRLanguage];
            if (stamp.Length > 0 && !stamp.Trim().StartsWith("-"))
            {
                stamp = " - " + stamp;
            }

            return gameString + stamp;
        }
    }
}