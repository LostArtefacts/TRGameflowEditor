using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TRGE.Core
{
    internal class TR23AudioExtractor
    {
        //set to wherever the local copy of the wav files should be put if building the WAD files
        private readonly string _localWavFolder;

        internal TR23AudioExtractor(string localWavFolder)
        {
            _localWavFolder = localWavFolder;
        }

        internal void BuildTR2AudioJson()
        {
            Dictionary<int, Tuple<ushort, string, TRAudioCategory[]>> mp3Map = GetTR2Map();
            string dir = Path.Combine(_localWavFolder, "TR2Audio");

            List<TRAudioTrack> tracks = new List<TRAudioTrack>();
            Dictionary<string, object> output = new Dictionary<string, object>
            {
                ["WAD"] = "tr2audio.wad",
                ["Tracks"] = tracks
            };

            uint offset = 0;
            foreach (int key in mp3Map.Keys)
            {
                TRAudioTrack track = new TRAudioTrack
                {
                    ID = mp3Map[key].Item1,
                    Name = mp3Map[key].Item2
                };
                if (track.ID > 0)
                {
                    track.Length = Convert.ToUInt32(new FileInfo(Path.Combine(dir, key + ".wav")).Length);
                    track.Offset = offset;
                    offset += track.Length;
                }
                track.Categories.AddRange(mp3Map[key].Item3);
                tracks.Add(track);
            }

            File.WriteAllText(Path.Combine(dir, "tr2audio.json"), JsonConvert.SerializeObject(output, Formatting.Indented));
        }

        internal void BuildTR2AudioWad()
        {
            Dictionary<int, Tuple<ushort, string, TRAudioCategory[]>> mp3Map = GetTR2Map();
            string dir = Path.Combine(_localWavFolder, "TR2Audio");
            using (BinaryWriter bw = new BinaryWriter(new FileStream("tr2audio.wad", FileMode.Create, FileAccess.Write)))
            {
                foreach (int key in mp3Map.Keys)
                {
                    bw.Write(File.ReadAllBytes(Path.Combine(dir, key + ".wav")));
                }
            }
        }

        internal void ExtractTR3Audio(string originalWadPath)
        {
            List<TRAudioTrack> tracks = new List<TRAudioTrack>();
            string dir = Path.Combine(_localWavFolder, "TR3Audio");
            using (BinaryReader br = new BinaryReader(new FileStream(originalWadPath, FileMode.Open)))
            {
                for (ushort i = 0; i < 130; i++)
                {
                    byte[] name = br.ReadBytes(260);
                    uint length = br.ReadUInt32();
                    uint offset = br.ReadUInt32();
                    if (length > 0)
                    {
                        tracks.Add(new TRAudioTrack
                        {
                            ID = i,
                            Name = Encoding.ASCII.GetString(name).TrimEnd((char)0),
                            Length = length,
                            Offset = offset
                        });
                    }
                }

                foreach (TRAudioTrack track in tracks)
                {
                    br.BaseStream.Position = track.Offset;
                    File.WriteAllBytes(Path.Combine(dir, track.ID + "_.wav"), br.ReadBytes(Convert.ToInt32(track.Length)));
                }
            }

            //use ffmpeg at this point to compress the files a bit e.g.
            //ffmpeg.exe -i 2_.wav -ac 1 -ar 14000 -acodec pcm_s16le 2.wav
        }

        internal void BuildTR3AudioJson()
        {
            Dictionary<ushort, Tuple<string, TRAudioCategory[]>> tr3Map = GetTR3Map();
            string dir = Path.Combine(_localWavFolder, "TR3Audio");

            List<TRAudioTrack> tracks = new List<TRAudioTrack>();
            Dictionary<string, object> output = new Dictionary<string, object>
            {
                ["WAD"] = "tr3audio.wad",
                ["Tracks"] = tracks
            };

            uint offset = 0;
            foreach (ushort key in tr3Map.Keys)
            {
                TRAudioTrack track = new TRAudioTrack
                {
                    ID = key,
                    Name = tr3Map[key].Item1
                };
                if (track.ID > 0)
                {
                    track.Length = Convert.ToUInt32(new FileInfo(Path.Combine(dir, key + ".wav")).Length);
                    track.Offset = offset;
                    offset += track.Length;
                }
                track.Categories.AddRange(tr3Map[key].Item2);
                tracks.Add(track);
            }

            File.WriteAllText(Path.Combine(dir, "tr3audio.json"), JsonConvert.SerializeObject(output, Formatting.Indented));
        }

        internal void BuildTR3AudioWad()
        {
            Dictionary<ushort, Tuple<string, TRAudioCategory[]>> tr3Map = GetTR3Map();
            string dir = Path.Combine(_localWavFolder, "TR3Audio");

            using (BinaryWriter bw = new BinaryWriter(new FileStream("tr3audio.wad", FileMode.Create, FileAccess.Write)))
            {
                foreach (int key in tr3Map.Keys)
                {
                    bw.Write(File.ReadAllBytes(Path.Combine(dir, key + ".wav")));
                }
            }
        }

        private Dictionary<int, Tuple<ushort, string, TRAudioCategory[]>> GetTR2Map()
        {
            return new Dictionary<int, Tuple<ushort, string, TRAudioCategory[]>>
            {
                 [0] = new Tuple<ushort, string, TRAudioCategory[]>(0,  "Blank",                new TRAudioCategory[] { TRAudioCategory.Blank, TRAudioCategory.Ambient }),
                 [2] = new Tuple<ushort, string, TRAudioCategory[]>(3,  "Great Wall Cutscene",  new TRAudioCategory[] { TRAudioCategory.CutScene }),
                 [3] = new Tuple<ushort, string, TRAudioCategory[]>(4,  "Opera House Cutscene", new TRAudioCategory[] { TRAudioCategory.CutScene }),
                 [4] = new Tuple<ushort, string, TRAudioCategory[]>(5,  "Diving Area Cutscene", new TRAudioCategory[] { TRAudioCategory.CutScene }),
                 [5] = new Tuple<ushort, string, TRAudioCategory[]>(6,  "Welcome Back",         new TRAudioCategory[] { TRAudioCategory.General }),
                 [6] = new Tuple<ushort, string, TRAudioCategory[]>(7,  "Training 1",           new TRAudioCategory[] { TRAudioCategory.General }),
                 [7] = new Tuple<ushort, string, TRAudioCategory[]>(8,  "Training 2",           new TRAudioCategory[] { TRAudioCategory.General }),
                 [8] = new Tuple<ushort, string, TRAudioCategory[]>(9,  "Training 3",           new TRAudioCategory[] { TRAudioCategory.General }),
                 [9] = new Tuple<ushort, string, TRAudioCategory[]>(10, "Training 4",           new TRAudioCategory[] { TRAudioCategory.General }),
                [10] = new Tuple<ushort, string, TRAudioCategory[]>(11, "Training 5",           new TRAudioCategory[] { TRAudioCategory.General }),
                [11] = new Tuple<ushort, string, TRAudioCategory[]>(12, "Training 6",           new TRAudioCategory[] { TRAudioCategory.General }),
                [12] = new Tuple<ushort, string, TRAudioCategory[]>(13, "Training 7",           new TRAudioCategory[] { TRAudioCategory.General }),
                [13] = new Tuple<ushort, string, TRAudioCategory[]>(14, "Training 8",           new TRAudioCategory[] { TRAudioCategory.General }),
                [14] = new Tuple<ushort, string, TRAudioCategory[]>(15, "Training 9",           new TRAudioCategory[] { TRAudioCategory.General }),
                [15] = new Tuple<ushort, string, TRAudioCategory[]>(16, "Training 10",          new TRAudioCategory[] { TRAudioCategory.General }),
                [16] = new Tuple<ushort, string, TRAudioCategory[]>(17, "Training 11",          new TRAudioCategory[] { TRAudioCategory.General }),
                [17] = new Tuple<ushort, string, TRAudioCategory[]>(18, "Training 12",          new TRAudioCategory[] { TRAudioCategory.General }),
                [18] = new Tuple<ushort, string, TRAudioCategory[]>(19, "Training 13",          new TRAudioCategory[] { TRAudioCategory.General }),
                [19] = new Tuple<ushort, string, TRAudioCategory[]>(20, "Training 14",          new TRAudioCategory[] { TRAudioCategory.General }),
                [20] = new Tuple<ushort, string, TRAudioCategory[]>(21, "Training 15",          new TRAudioCategory[] { TRAudioCategory.General }),
                [21] = new Tuple<ushort, string, TRAudioCategory[]>(22, "Training 16",          new TRAudioCategory[] { TRAudioCategory.General }),
                [22] = new Tuple<ushort, string, TRAudioCategory[]>(23, "Training 17",          new TRAudioCategory[] { TRAudioCategory.General }),
                [23] = new Tuple<ushort, string, TRAudioCategory[]>(27, "Shower Scene",         new TRAudioCategory[] { TRAudioCategory.CutScene, TRAudioCategory.Secret }),
                [24] = new Tuple<ushort, string, TRAudioCategory[]>(28, "Dragon Death",         new TRAudioCategory[] { TRAudioCategory.CutScene }),
                [25] = new Tuple<ushort, string, TRAudioCategory[]>(29, "Feel Free",            new TRAudioCategory[] { TRAudioCategory.General }),
                [26] = new Tuple<ushort, string, TRAudioCategory[]>(30, "Xian Cutscene",        new TRAudioCategory[] { TRAudioCategory.CutScene }),
                [27] = new Tuple<ushort, string, TRAudioCategory[]>(31, "Caves Ambience",       new TRAudioCategory[] { TRAudioCategory.Ambient, TRAudioCategory.Title }),
                [28] = new Tuple<ushort, string, TRAudioCategory[]>(32, "Water Ambience",       new TRAudioCategory[] { TRAudioCategory.Ambient, TRAudioCategory.Title }),
                [29] = new Tuple<ushort, string, TRAudioCategory[]>(33, "Wind Ambience",        new TRAudioCategory[] { TRAudioCategory.Ambient, TRAudioCategory.Title }),
                [30] = new Tuple<ushort, string, TRAudioCategory[]>(34, "Heartbeat Ambience",   new TRAudioCategory[] { TRAudioCategory.Ambient, TRAudioCategory.Title }),
                [31] = new Tuple<ushort, string, TRAudioCategory[]>(35, "Danger 1",             new TRAudioCategory[] { TRAudioCategory.General, TRAudioCategory.Secret }),
                [32] = new Tuple<ushort, string, TRAudioCategory[]>(36, "Danger 2",             new TRAudioCategory[] { TRAudioCategory.General, TRAudioCategory.Secret }),
                [33] = new Tuple<ushort, string, TRAudioCategory[]>(37, "Danger 3",             new TRAudioCategory[] { TRAudioCategory.General, TRAudioCategory.Secret }),
                [34] = new Tuple<ushort, string, TRAudioCategory[]>(38, "Sacred",               new TRAudioCategory[] { TRAudioCategory.General, TRAudioCategory.Secret }),
                [35] = new Tuple<ushort, string, TRAudioCategory[]>(39, "Awe",                  new TRAudioCategory[] { TRAudioCategory.General, TRAudioCategory.Secret }),
                [36] = new Tuple<ushort, string, TRAudioCategory[]>(40, "Venice Violins",       new TRAudioCategory[] { TRAudioCategory.General, TRAudioCategory.Title }),
                [37] = new Tuple<ushort, string, TRAudioCategory[]>(41, "End of Level",         new TRAudioCategory[] { TRAudioCategory.General, TRAudioCategory.Secret }),
                [38] = new Tuple<ushort, string, TRAudioCategory[]>(42, "Mystical",             new TRAudioCategory[] { TRAudioCategory.General, TRAudioCategory.Secret }),
                [39] = new Tuple<ushort, string, TRAudioCategory[]>(43, "Revelation 1",         new TRAudioCategory[] { TRAudioCategory.General, TRAudioCategory.Secret }),
                [40] = new Tuple<ushort, string, TRAudioCategory[]>(44, "Be Careful 1",         new TRAudioCategory[] { TRAudioCategory.General, TRAudioCategory.Secret }),
                [41] = new Tuple<ushort, string, TRAudioCategory[]>(45, "Chandelier Room",      new TRAudioCategory[] { TRAudioCategory.General, TRAudioCategory.Title }),
                [42] = new Tuple<ushort, string, TRAudioCategory[]>(46, "Drama",                new TRAudioCategory[] { TRAudioCategory.General, TRAudioCategory.Secret }),
                [43] = new Tuple<ushort, string, TRAudioCategory[]>(47, "Secret",               new TRAudioCategory[] { TRAudioCategory.Secret }),
                [44] = new Tuple<ushort, string, TRAudioCategory[]>(48, "It's Coming! 1",       new TRAudioCategory[] { TRAudioCategory.General, TRAudioCategory.Secret }),
                [45] = new Tuple<ushort, string, TRAudioCategory[]>(49, "It's Coming! 2",       new TRAudioCategory[] { TRAudioCategory.General, TRAudioCategory.Secret }),
                [46] = new Tuple<ushort, string, TRAudioCategory[]>(50, "Warning 1",            new TRAudioCategory[] { TRAudioCategory.General, TRAudioCategory.Secret }),
                [47] = new Tuple<ushort, string, TRAudioCategory[]>(51, "Warning 2",            new TRAudioCategory[] { TRAudioCategory.General, TRAudioCategory.Secret }),
                [48] = new Tuple<ushort, string, TRAudioCategory[]>(52, "Skidoo Techno",        new TRAudioCategory[] { TRAudioCategory.General, TRAudioCategory.Title }),
                [49] = new Tuple<ushort, string, TRAudioCategory[]>(53, "Skidoo Percussion",    new TRAudioCategory[] { TRAudioCategory.General, TRAudioCategory.Title }),
                [50] = new Tuple<ushort, string, TRAudioCategory[]>(54, "Synth Pads",           new TRAudioCategory[] { TRAudioCategory.General }),
                [51] = new Tuple<ushort, string, TRAudioCategory[]>(55, "Revelation 2",         new TRAudioCategory[] { TRAudioCategory.General, TRAudioCategory.Secret }),
                [52] = new Tuple<ushort, string, TRAudioCategory[]>(56, "Be Careful 2",         new TRAudioCategory[] { TRAudioCategory.General, TRAudioCategory.Secret }),
                [53] = new Tuple<ushort, string, TRAudioCategory[]>(57, "Revelation 3",         new TRAudioCategory[] { TRAudioCategory.General, TRAudioCategory.Title }),
                [54] = new Tuple<ushort, string, TRAudioCategory[]>(58, "Industrial Ambience",  new TRAudioCategory[] { TRAudioCategory.Ambient, TRAudioCategory.Title }),
                [55] = new Tuple<ushort, string, TRAudioCategory[]>(59, "Spooky Ambience",      new TRAudioCategory[] { TRAudioCategory.Ambient, TRAudioCategory.Title }),
                [56] = new Tuple<ushort, string, TRAudioCategory[]>(60, "Barkhang",             new TRAudioCategory[] { TRAudioCategory.General, TRAudioCategory.Title }),
                [57] = new Tuple<ushort, string, TRAudioCategory[]>(61, "Gong",                 new TRAudioCategory[] { TRAudioCategory.General, TRAudioCategory.Secret}),
                [58] = new Tuple<ushort, string, TRAudioCategory[]>(62, "OK, Marco",            new TRAudioCategory[] { TRAudioCategory.CutScene }),
                [59] = new Tuple<ushort, string, TRAudioCategory[]>(63, "Home Sweet Home",      new TRAudioCategory[] { TRAudioCategory.CutScene }),
                [60] = new Tuple<ushort, string, TRAudioCategory[]>(64, "Main Theme",           new TRAudioCategory[] { TRAudioCategory.Title })
            };
        }

        private Dictionary<ushort, Tuple<string, TRAudioCategory[]>> GetTR3Map()
        {
            return new Dictionary<ushort, Tuple<string, TRAudioCategory[]>>
            {
                 [0] = new Tuple<string, TRAudioCategory[]>("Blank",                                new TRAudioCategory[] { TRAudioCategory.Blank }),
                 [2] = new Tuple<string, TRAudioCategory[]>("The Puzzle Element",                   new TRAudioCategory[] { TRAudioCategory.General, TRAudioCategory.Title }),
                 [3] = new Tuple<string, TRAudioCategory[]>("No Waiting Around 1",                  new TRAudioCategory[] { TRAudioCategory.General, TRAudioCategory.Title }),
                 [4] = new Tuple<string, TRAudioCategory[]>("Something Spooky is in that Jungle",   new TRAudioCategory[] { TRAudioCategory.General, TRAudioCategory.Title }),
                 [5] = new Tuple<string, TRAudioCategory[]>("Lara's Themes",                        new TRAudioCategory[] { TRAudioCategory.Title }),
                 [6] = new Tuple<string, TRAudioCategory[]>("The Cavern Sewers",                    new TRAudioCategory[] { TRAudioCategory.General, TRAudioCategory.Title }),
                 [7] = new Tuple<string, TRAudioCategory[]>("Geordie Bob",                          new TRAudioCategory[] { TRAudioCategory.General, TRAudioCategory.Title }),
                 [8] = new Tuple<string, TRAudioCategory[]>("Tony (The Loon)",                      new TRAudioCategory[] { TRAudioCategory.General, TRAudioCategory.Title }),
                 [9] = new Tuple<string, TRAudioCategory[]>("No Waiting Around 2",                  new TRAudioCategory[] { TRAudioCategory.General, TRAudioCategory.Title }),
                [10] = new Tuple<string, TRAudioCategory[]>("The Greedy Mob",                       new TRAudioCategory[] { TRAudioCategory.General, TRAudioCategory.Ambient }),
                [11] = new Tuple<string, TRAudioCategory[]>("A Long Way Up",                        new TRAudioCategory[] { TRAudioCategory.General }),
                [12] = new Tuple<string, TRAudioCategory[]>("No Waiting Around 3",                  new TRAudioCategory[] { TRAudioCategory.General, TRAudioCategory.Title }),
                [13] = new Tuple<string, TRAudioCategory[]>("There Be Butterflies Here 2",          new TRAudioCategory[] { TRAudioCategory.General }),
                [14] = new Tuple<string, TRAudioCategory[]>("She's Cool",                           new TRAudioCategory[] { TRAudioCategory.General, TRAudioCategory.Title }),
                [15] = new Tuple<string, TRAudioCategory[]>("Mind the Gap 2",                       new TRAudioCategory[] { TRAudioCategory.General, TRAudioCategory.Title }),
                [16] = new Tuple<string, TRAudioCategory[]>("Around the Corner 2",                  new TRAudioCategory[] { TRAudioCategory.General }),
                [17] = new Tuple<string, TRAudioCategory[]>("Around the Corner 1",                  new TRAudioCategory[] { TRAudioCategory.General }),
                [18] = new Tuple<string, TRAudioCategory[]>("Kneel and Pray",                       new TRAudioCategory[] { TRAudioCategory.General }),
                [19] = new Tuple<string, TRAudioCategory[]>("Around the Corner 4",                  new TRAudioCategory[] { TRAudioCategory.General }),
                [20] = new Tuple<string, TRAudioCategory[]>("Around the Corner 3",                  new TRAudioCategory[] { TRAudioCategory.General }),
                [21] = new Tuple<string, TRAudioCategory[]>("Seeing is Believing 1",                new TRAudioCategory[] { TRAudioCategory.General }),
                [22] = new Tuple<string, TRAudioCategory[]>("Look What We Have Here 3",             new TRAudioCategory[] { TRAudioCategory.General }),
                [23] = new Tuple<string, TRAudioCategory[]>("There Be Butterflies Here 4",          new TRAudioCategory[] { TRAudioCategory.General }),
                [24] = new Tuple<string, TRAudioCategory[]>("Stone the Crows 10",                   new TRAudioCategory[] { TRAudioCategory.General }),
                [25] = new Tuple<string, TRAudioCategory[]>("There Be Butterflies Here 3",          new TRAudioCategory[] { TRAudioCategory.General }),
                [26] = new Tuple<string, TRAudioCategory[]>("Meteorite Cavern",                     new TRAudioCategory[] { TRAudioCategory.General, TRAudioCategory.Ambient }),
                [27] = new Tuple<string, TRAudioCategory[]>("Steady",                               new TRAudioCategory[] { TRAudioCategory.General, TRAudioCategory.Ambient }),
                [28] = new Tuple<string, TRAudioCategory[]>("Antarctica",                           new TRAudioCategory[] { TRAudioCategory.General, TRAudioCategory.Ambient }),
                [29] = new Tuple<string, TRAudioCategory[]>("Things",                               new TRAudioCategory[] { TRAudioCategory.General, TRAudioCategory.Ambient }),
                [30] = new Tuple<string, TRAudioCategory[]>("Anyone There",                         new TRAudioCategory[] { TRAudioCategory.General, TRAudioCategory.Ambient }),
                [31] = new Tuple<string, TRAudioCategory[]>("Grotto",                               new TRAudioCategory[] { TRAudioCategory.General, TRAudioCategory.Ambient }),
                [32] = new Tuple<string, TRAudioCategory[]>("On the Beach",                         new TRAudioCategory[] { TRAudioCategory.General, TRAudioCategory.Ambient }),
                [33] = new Tuple<string, TRAudioCategory[]>("Gamma Pals",                           new TRAudioCategory[] { TRAudioCategory.General, TRAudioCategory.Ambient }),
                [34] = new Tuple<string, TRAudioCategory[]>("In the Jungle",                        new TRAudioCategory[] { TRAudioCategory.General, TRAudioCategory.Ambient }),
                [35] = new Tuple<string, TRAudioCategory[]>("Piranha Waters",                       new TRAudioCategory[] { TRAudioCategory.General, TRAudioCategory.Ambient }),
                [36] = new Tuple<string, TRAudioCategory[]>("The Rapids",                           new TRAudioCategory[] { TRAudioCategory.General, TRAudioCategory.Ambient }),
                [37] = new Tuple<string, TRAudioCategory[]>("Supper Time",                          new TRAudioCategory[] { TRAudioCategory.General, TRAudioCategory.Ambient }),
                [38] = new Tuple<string, TRAudioCategory[]>("Look out 5",                           new TRAudioCategory[] { TRAudioCategory.General }),
                [39] = new Tuple<string, TRAudioCategory[]>("Look What We Have Here 1",             new TRAudioCategory[] { TRAudioCategory.General }),
                [40] = new Tuple<string, TRAudioCategory[]>("Around the Corner 5",                  new TRAudioCategory[] { TRAudioCategory.General }),
                [41] = new Tuple<string, TRAudioCategory[]>("Seeing is Believing 2",                new TRAudioCategory[] { TRAudioCategory.General }),
                [42] = new Tuple<string, TRAudioCategory[]>("Stone the Crows 9.wav",                new TRAudioCategory[] { TRAudioCategory.General }),
                [43] = new Tuple<string, TRAudioCategory[]>("Look out 8",                           new TRAudioCategory[] { TRAudioCategory.General }),
                [44] = new Tuple<string, TRAudioCategory[]>("Look out 4",                           new TRAudioCategory[] { TRAudioCategory.General }),
                [45] = new Tuple<string, TRAudioCategory[]>("Stone the Crows 7",                    new TRAudioCategory[] { TRAudioCategory.General }),
                [46] = new Tuple<string, TRAudioCategory[]>("Stone the Crows 3",                    new TRAudioCategory[] { TRAudioCategory.General }),
                [47] = new Tuple<string, TRAudioCategory[]>("Stone the Crows 8",                    new TRAudioCategory[] { TRAudioCategory.General }),
                [48] = new Tuple<string, TRAudioCategory[]>("Look What We Have Here 2",             new TRAudioCategory[] { TRAudioCategory.General }),
                [49] = new Tuple<string, TRAudioCategory[]>("Stone the Crows 4",                    new TRAudioCategory[] { TRAudioCategory.General }),
                [50] = new Tuple<string, TRAudioCategory[]>("Stone the Crows 6",                    new TRAudioCategory[] { TRAudioCategory.General }),
                [51] = new Tuple<string, TRAudioCategory[]>("Look out 3",                           new TRAudioCategory[] { TRAudioCategory.General }),
                [52] = new Tuple<string, TRAudioCategory[]>("Look out 1",                           new TRAudioCategory[] { TRAudioCategory.General }),
                [53] = new Tuple<string, TRAudioCategory[]>("There Be Butterflies Here 1",          new TRAudioCategory[] { TRAudioCategory.General }),
                [54] = new Tuple<string, TRAudioCategory[]>("Stone the Crows 1",                    new TRAudioCategory[] { TRAudioCategory.General }),
                [55] = new Tuple<string, TRAudioCategory[]>("Stone the Crows 5",                    new TRAudioCategory[] { TRAudioCategory.General }),
                [56] = new Tuple<string, TRAudioCategory[]>("Mind the Gap 1",                       new TRAudioCategory[] { TRAudioCategory.General }),
                [57] = new Tuple<string, TRAudioCategory[]>("There Be Butterflies Here 5",          new TRAudioCategory[] { TRAudioCategory.General }),
                [58] = new Tuple<string, TRAudioCategory[]>("Look out 2",                           new TRAudioCategory[] { TRAudioCategory.General }),
                [59] = new Tuple<string, TRAudioCategory[]>("Look out 7",                           new TRAudioCategory[] { TRAudioCategory.General }),
                [60] = new Tuple<string, TRAudioCategory[]>("Stone the Crows (2)",                  new TRAudioCategory[] { TRAudioCategory.General }),
                [61] = new Tuple<string, TRAudioCategory[]>("Look out 6",                           new TRAudioCategory[] { TRAudioCategory.General }),
                [62] = new Tuple<string, TRAudioCategory[]>("Scott's Hut",                          new TRAudioCategory[] { TRAudioCategory.CutScene }),
                [63] = new Tuple<string, TRAudioCategory[]>("Cavern Sewers",                        new TRAudioCategory[] { TRAudioCategory.CutScene }),
                [64] = new Tuple<string, TRAudioCategory[]>("Jungle Camp",                          new TRAudioCategory[] { TRAudioCategory.CutScene }),
                [65] = new Tuple<string, TRAudioCategory[]>("Worship Room",                         new TRAudioCategory[] { TRAudioCategory.CutScene }),
                [66] = new Tuple<string, TRAudioCategory[]>("Cavern",                               new TRAudioCategory[] { TRAudioCategory.CutScene }),
                [67] = new Tuple<string, TRAudioCategory[]>("Rooftops",                             new TRAudioCategory[] { TRAudioCategory.CutScene }),
                [68] = new Tuple<string, TRAudioCategory[]>("Tree Shack",                           new TRAudioCategory[] { TRAudioCategory.CutScene }),
                [69] = new Tuple<string, TRAudioCategory[]>("Temple Exit",                          new TRAudioCategory[] { TRAudioCategory.CutScene }),
                [70] = new Tuple<string, TRAudioCategory[]>("Delivery Truck",                       new TRAudioCategory[] { TRAudioCategory.CutScene }),
                [71] = new Tuple<string, TRAudioCategory[]>("Penthouse",                            new TRAudioCategory[] { TRAudioCategory.CutScene }),
                [72] = new Tuple<string, TRAudioCategory[]>("Ravine",                               new TRAudioCategory[] { TRAudioCategory.General }),
                [73] = new Tuple<string, TRAudioCategory[]>("Old Smokey",                           new TRAudioCategory[] { TRAudioCategory.Ambient }),
                [74] = new Tuple<string, TRAudioCategory[]>("Under Smokey",                         new TRAudioCategory[] { TRAudioCategory.Ambient }),
                [75] = new Tuple<string, TRAudioCategory[]>("Refining Plant",                       new TRAudioCategory[] { TRAudioCategory.General }),
                [76] = new Tuple<string, TRAudioCategory[]>("Rumble Sub",                           new TRAudioCategory[] { TRAudioCategory.General }),
                [77] = new Tuple<string, TRAudioCategory[]>("Quake",                                new TRAudioCategory[] { TRAudioCategory.General }),
                [78] = new Tuple<string, TRAudioCategory[]>("Blank",                                new TRAudioCategory[] { TRAudioCategory.General }),
                [82] = new Tuple<string, TRAudioCategory[]>("Excellent",                            new TRAudioCategory[] { TRAudioCategory.General }),
                [83] = new Tuple<string, TRAudioCategory[]>("That's Great",                         new TRAudioCategory[] { TRAudioCategory.General }),
                [84] = new Tuple<string, TRAudioCategory[]>("Training 1",                           new TRAudioCategory[] { TRAudioCategory.General }),
                [85] = new Tuple<string, TRAudioCategory[]>("Training 2",                           new TRAudioCategory[] { TRAudioCategory.General }),
                [86] = new Tuple<string, TRAudioCategory[]>("Training 3",                           new TRAudioCategory[] { TRAudioCategory.General }),
                [87] = new Tuple<string, TRAudioCategory[]>("Training 4",                           new TRAudioCategory[] { TRAudioCategory.General }),
                [88] = new Tuple<string, TRAudioCategory[]>("Training 5",                           new TRAudioCategory[] { TRAudioCategory.General }),
                [89] = new Tuple<string, TRAudioCategory[]>("Fancy a Swim?",                        new TRAudioCategory[] { TRAudioCategory.General }),
                [90] = new Tuple<string, TRAudioCategory[]>("Let's Go Outside",                     new TRAudioCategory[] { TRAudioCategory.General }),
                [91] = new Tuple<string, TRAudioCategory[]>("Hey",                                  new TRAudioCategory[] { TRAudioCategory.General }),
                [92] = new Tuple<string, TRAudioCategory[]>("Now it's Time...",                     new TRAudioCategory[] { TRAudioCategory.General }),
                [93] = new Tuple<string, TRAudioCategory[]>("Training 6",                           new TRAudioCategory[] { TRAudioCategory.General }),
                [94] = new Tuple<string, TRAudioCategory[]>("Training 7",                           new TRAudioCategory[] { TRAudioCategory.General }),
                [95] = new Tuple<string, TRAudioCategory[]>("Gosh",                                 new TRAudioCategory[] { TRAudioCategory.General }),
                [96] = new Tuple<string, TRAudioCategory[]>("Training 8",                           new TRAudioCategory[] { TRAudioCategory.General }),
                [97] = new Tuple<string, TRAudioCategory[]>("Training 9",                           new TRAudioCategory[] { TRAudioCategory.General }),
                [98] = new Tuple<string, TRAudioCategory[]>("Training 10",                          new TRAudioCategory[] { TRAudioCategory.General }),
                [99] = new Tuple<string, TRAudioCategory[]>("Training 11",                          new TRAudioCategory[] { TRAudioCategory.General }),
                [100] = new Tuple<string, TRAudioCategory[]>("Training 12",                         new TRAudioCategory[] { TRAudioCategory.General }),
                [101] = new Tuple<string, TRAudioCategory[]>("Welcome Back",                        new TRAudioCategory[] { TRAudioCategory.General }),
                [102] = new Tuple<string, TRAudioCategory[]>("Training 13",                         new TRAudioCategory[] { TRAudioCategory.General }),
                [103] = new Tuple<string, TRAudioCategory[]>("Training 14",                         new TRAudioCategory[] { TRAudioCategory.General }),
                [104] = new Tuple<string, TRAudioCategory[]>("Training 15",                         new TRAudioCategory[] { TRAudioCategory.General }),
                [105] = new Tuple<string, TRAudioCategory[]>("Training 16",                         new TRAudioCategory[] { TRAudioCategory.General }),
                [106] = new Tuple<string, TRAudioCategory[]>("Training 17",                         new TRAudioCategory[] { TRAudioCategory.General }),
                [107] = new Tuple<string, TRAudioCategory[]>("Training 18",                         new TRAudioCategory[] { TRAudioCategory.General }),
                [108] = new Tuple<string, TRAudioCategory[]>("Training 19",                         new TRAudioCategory[] { TRAudioCategory.General }),
                [109] = new Tuple<string, TRAudioCategory[]>("Training 20",                         new TRAudioCategory[] { TRAudioCategory.General }),
                [110] = new Tuple<string, TRAudioCategory[]>("Training 21",                         new TRAudioCategory[] { TRAudioCategory.General }),
                [111] = new Tuple<string, TRAudioCategory[]>("Training 22",                         new TRAudioCategory[] { TRAudioCategory.General }),
                [112] = new Tuple<string, TRAudioCategory[]>("Training 23",                         new TRAudioCategory[] { TRAudioCategory.General }),
                [113] = new Tuple<string, TRAudioCategory[]>("Training 24",                         new TRAudioCategory[] { TRAudioCategory.General }),
                [114] = new Tuple<string, TRAudioCategory[]>("Training 25",                         new TRAudioCategory[] { TRAudioCategory.General }),
                [115] = new Tuple<string, TRAudioCategory[]>("Training 26",                         new TRAudioCategory[] { TRAudioCategory.General }),
                [116] = new Tuple<string, TRAudioCategory[]>("Training 27",                         new TRAudioCategory[] { TRAudioCategory.General }),
                [117] = new Tuple<string, TRAudioCategory[]>("Training 28",                         new TRAudioCategory[] { TRAudioCategory.General }),
                [118] = new Tuple<string, TRAudioCategory[]>("Training 29",                         new TRAudioCategory[] { TRAudioCategory.General }),
                [119] = new Tuple<string, TRAudioCategory[]>("Training 30",                         new TRAudioCategory[] { TRAudioCategory.General }),
                [120] = new Tuple<string, TRAudioCategory[]>("In the Hut",                          new TRAudioCategory[] { TRAudioCategory.Ambient }),
                [121] = new Tuple<string, TRAudioCategory[]>("And so on...",                        new TRAudioCategory[] { TRAudioCategory.General }),
                [122] = new Tuple<string, TRAudioCategory[]>("Secret 1",                            new TRAudioCategory[] { TRAudioCategory.Secret }),
                [123] = new Tuple<string, TRAudioCategory[]>("Secret 2",                            new TRAudioCategory[] { TRAudioCategory.Secret })
            };
        }
    }
}