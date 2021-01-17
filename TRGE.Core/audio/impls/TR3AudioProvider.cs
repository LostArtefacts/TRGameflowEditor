using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TRGE.Core
{
    internal class TR3AudioProvider : AbstractTRAudioProvider
    {
        private readonly string _wadFilePath;
        internal List<TR3AudioEntry> Entries;

        internal TR3AudioProvider(string wadFilePath)
        {
            Entries = new List<TR3AudioEntry>();
            using (BinaryReader br = new BinaryReader(new FileStream(_wadFilePath = wadFilePath, FileMode.Open)))
            {
                for (ushort i = 0; i < 130; i++)
                {
                    byte[] name = br.ReadBytes(260);
                    uint length = br.ReadUInt32();
                    uint offset = br.ReadUInt32();
                    if (length > 0)
                    {
                        Entries.Add(new TR3AudioEntry
                        {
                            ID = i,
                            Name = Encoding.ASCII.GetString(name).TrimEnd((char)0),
                            WavLength = length,
                            WavOffset = offset
                        });
                    }
                }
            }
        }

        internal TR3AudioEntry GetTrack(ushort id)
        {
            foreach (TR3AudioEntry entry in Entries)
            {
                if (entry.ID == id)
                {
                    return entry;
                }
            }
            return null;
        }

        internal byte[] GetWavData(ushort id)
        {
            TR3AudioEntry entry = GetTrack(id);
            if (entry == null)
            {
                return null;
            }
            return GetWavData(entry);
        }

        internal byte[] GetWavData(TR3AudioEntry entry)
        {
            using (BinaryReader br = new BinaryReader(new FileStream(_wadFilePath, FileMode.Open)))
            {
                br.BaseStream.Position = entry.WavOffset;
                return br.ReadBytes(Convert.ToInt32(entry.WavLength));
            }
        }
    }

    class TR3AudioEntry
    {
        internal ushort ID;
        internal string Name;
        internal uint WavLength;
        internal uint WavOffset;
    }
}