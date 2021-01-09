using System.IO;

namespace TRGE.Core
{
    internal abstract class AbstractTRScript
    {
        internal Hardware Hardware { get; private set; }

        internal void Read(string filePath)
        {
            Hardware = filePath.ToLower().Contains("tombpsx") ? Hardware.PSX : Hardware.PC;

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
    }
}