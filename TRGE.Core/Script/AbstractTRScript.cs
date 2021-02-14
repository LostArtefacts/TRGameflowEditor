using System.IO;
using System.Collections.Generic;

namespace TRGE.Core
{
    internal abstract class AbstractTRScript
    {
        public TREdition Edition { get; protected set; }

        internal void Read(FileInfo file)
        {
            Read(file.FullName);
        }

        internal void Read(string filePath)
        {
            //All we can go on to begin with is the file name to determine a generic edition.
            //Subclasses should implement CalculateEdition and call it as appropriate while reading the data.
            Edition = filePath.ToLower().Contains("tombpsx") ? TREdition.GenericPSX : TREdition.GenericPC;

            using (BinaryReader br = new BinaryReader(new FileStream(filePath, FileMode.Open)))
            {
                Read(br);
            }
        }

        internal void Write(string filePath)
        {
            Stamp();
            using (BinaryWriter bw = new BinaryWriter(new FileStream(filePath, FileMode.Create)))
            {
                bw.Write(Serialise());
            }
        }

        internal abstract void Read(BinaryReader br);
        internal abstract byte[] Serialise();
        protected abstract void CalculateEdition();
        internal abstract AbstractTRFrontEnd FrontEnd { get; }
        internal abstract List<AbstractTRScriptedLevel> Levels { get; set; }
        internal abstract ushort TitleSoundID { get; set; }
        internal abstract ushort SecretSoundID { get; set; }

        protected abstract void Stamp();
        protected string ApplyStamp(string gameString)
        {
            int i;
            if ((i = gameString.IndexOf(" - ")) != -1)
            {
                gameString = gameString.Substring(0, i);
            }

            string stamp = TRInterop.ScriptModificationStamp ?? string.Empty;
            if (stamp.Length > 0 && !stamp.Trim().StartsWith("-"))
            {
                stamp = " - " + stamp;
            }

            return gameString + stamp;
        }
    }
}