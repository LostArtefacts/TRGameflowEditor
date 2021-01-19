using System.IO;
using System.Collections.Generic;

namespace TRGE.Core
{
    internal abstract class AbstractTRScript
    {
        public TREdition Edition { get; protected set; }

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
            using (BinaryWriter bw = new BinaryWriter(new FileStream(filePath, FileMode.Create)))
            {
                bw.Write(Serialise());
            }
        }

        internal abstract void Read(BinaryReader br);
        internal abstract byte[] Serialise();
        protected abstract void CalculateEdition();
        internal abstract AbstractTRFrontEnd FrontEnd { get; }
        internal abstract List<AbstractTRLevel> Levels { get; set; }
        internal abstract ushort TitleSoundID { get; set; }
        internal abstract ushort SecretSoundID { get; set; }
    }
}