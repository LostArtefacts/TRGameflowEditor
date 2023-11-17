using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace TRGE.Core
{
    public abstract class BaseTRAudioProvider : AbstractTRAudioProvider
    {
        protected static readonly TRAudioTrack _emptyTrack = new()
        {
            ID = 0,
            Name = "Blank"
        };

        protected string _wadFileName;
        public override TRAudioType AudioType => TRAudioType.WAV;

        public BaseTRAudioProvider(string jsonFilePath)
        {
            Dictionary<string, object> data = JsonConvert.DeserializeObject<Dictionary<string, object>>(File.ReadAllText(jsonFilePath));
            _wadFileName = data["WAD"].ToString();
            _tracks.AddRange(JsonConvert.DeserializeObject<TRAudioTrack[]>(data["Tracks"].ToString()));
        }

        public override byte[] GetTrackData(TRAudioTrack track)
        {
            if (track == null)
            {
                return null;
            }

            string wadFile = Path.Combine(TRInterop.ConfigDirectory, _wadFileName);
            if (!File.Exists(wadFile) && !TRDownloader.Download(_wadFileName, wadFile, true))
            {
                return null;
            }

            using BinaryReader br = new(new FileStream(wadFile, FileMode.Open));
            br.BaseStream.Position = track.Offset;
            return br.ReadBytes(Convert.ToInt32(track.Length));
        }

        public override TRAudioTrack GetBlankTrack()
        {
            return _emptyTrack;
        }
    }
}