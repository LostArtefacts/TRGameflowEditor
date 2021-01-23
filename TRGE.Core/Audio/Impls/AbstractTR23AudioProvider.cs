using System;
using System.IO;
using System.Text;

namespace TRGE.Core
{
    internal abstract class AbstractTR23AudioProvider : AbstractTRAudioProvider
    {
        protected static readonly TRAudioTrack _emptyTrack = new TRAudioTrack
        {
            ID = 0,
            Name = "Blank"
        };

        protected string _wadFileName;
        internal override TRAudioType AudioType => TRAudioType.WAV;

        internal AbstractTR23AudioProvider(byte[] data)
        {
            LoadTracks(data);
        }

        private void LoadTracks(byte[] data)
        {
            using (MemoryStream ms = new MemoryStream(data))
            using (BinaryReader br = new BinaryReader(ms))
            {
                ushort trackCount = br.ReadUInt16();
                ushort fileNameLength = br.ReadUInt16();
                _wadFileName = Encoding.ASCII.GetString(br.ReadBytes(fileNameLength));

                for (ushort i = 0; i < trackCount; i++)
                {
                    ushort id = br.ReadUInt16();
                    ushort trackNameLength = br.ReadUInt16();
                    byte[] name = br.ReadBytes(trackNameLength);
                    uint length = br.ReadUInt32();
                    uint offset = br.ReadUInt32();
                    if (length > 0)
                    {
                        _tracks.Add(new TRAudioTrack
                        {
                            ID = id,
                            Name = Encoding.ASCII.GetString(name),
                            Length = length,
                            Offset = offset
                        });
                    }
                }
            }
        }

        internal override byte[] GetTrackData(TRAudioTrack track)
        {
            if (track == null)
            {
                return null;
            }

            string wadFile = Path.Combine(TRInterop.ConfigDirectory, _wadFileName);
            if (!File.Exists(wadFile) && !TRDownloader.Download(_wadFileName, wadFile))
            {
                return null;
            }

            using (BinaryReader br = new BinaryReader(new FileStream(wadFile, FileMode.Open)))
            {
                br.BaseStream.Position = track.Offset;
                return br.ReadBytes(Convert.ToInt32(track.Length));
            }
        }

        internal override TRAudioTrack GetBlankTrack()
        {
            return _emptyTrack;
        }
    }
}