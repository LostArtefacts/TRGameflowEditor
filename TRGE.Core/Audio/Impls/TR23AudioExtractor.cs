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

        internal void BuildTR2AudioDat()
        {
            Dictionary<int, Tuple<ushort, string>> mp3Map = GetTR2Map();
            string dir = Path.Combine(_localWavFolder, "TR2Audio");
            using (BinaryWriter bw = new BinaryWriter(new FileStream("tr2audio.dat", FileMode.Create, FileAccess.Write)))
            {
                ushort count = Convert.ToUInt16(mp3Map.Count);
                uint offset = 0;
                bw.Write(count);
                string wadName = "tr2audio.wad";
                bw.Write(Convert.ToUInt16(wadName.Length));
                bw.Write(Encoding.ASCII.GetBytes(wadName));

                foreach (int key in mp3Map.Keys)
                {
                    bw.Write(mp3Map[key].Item1);
                    bw.Write(Convert.ToUInt16(mp3Map[key].Item2.Length));
                    bw.Write(Encoding.ASCII.GetBytes(mp3Map[key].Item2));

                    uint length = Convert.ToUInt32(new FileInfo(Path.Combine(dir, key + ".wav")).Length);
                    bw.Write(length);
                    bw.Write(offset);
                    offset += length;
                }
            }
        }

        internal void BuildTR2AudioWad()
        {
            Dictionary<int, Tuple<ushort, string>> mp3Map = GetTR2Map();
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

        internal void BuildTR3AudioDat()
        {
            Dictionary<ushort, string> tr3Map = GetTR3Map();
            string dir = Path.Combine(_localWavFolder, "TR3Audio");

            using (BinaryWriter bw = new BinaryWriter(new FileStream("tr3audio.dat", FileMode.Create, FileAccess.Write)))
            {
                ushort count = Convert.ToUInt16(tr3Map.Count);
                uint offset = 0;
                bw.Write(count);
                string wadName = "tr3audio.wad";
                bw.Write(Convert.ToUInt16(wadName.Length));
                bw.Write(Encoding.ASCII.GetBytes(wadName));

                foreach (ushort key in tr3Map.Keys)
                {
                    bw.Write(key);
                    bw.Write(Convert.ToUInt16(tr3Map[key].Length));
                    bw.Write(Encoding.ASCII.GetBytes(tr3Map[key]));

                    uint length = Convert.ToUInt32(new FileInfo(Path.Combine(dir, key + ".wav")).Length);
                    bw.Write(length);
                    bw.Write(offset);
                    offset += length;
                }
            }
        }

        internal void BuildTR3AudioWad()
        {
            Dictionary<ushort, string> tr3Map = GetTR3Map();
            string dir = Path.Combine(_localWavFolder, "TR3Audio");

            using (BinaryWriter bw = new BinaryWriter(new FileStream("tr3audio.wad", FileMode.Create, FileAccess.Write)))
            {
                foreach (int key in tr3Map.Keys)
                {
                    bw.Write(File.ReadAllBytes(Path.Combine(dir, key + ".wav")));
                }
            }
        }

        private Dictionary<int, Tuple<ushort, string>> GetTR2Map()
        {
            return new Dictionary<int, Tuple<ushort, string>>
            {
                [2] = new Tuple<ushort, string>(3, "Great Wall Cutscene"),
                [3] = new Tuple<ushort, string>(4, "Opera House Cutscene"),
                [4] = new Tuple<ushort, string>(5, "Diving Area Cutscene"),
                [5] = new Tuple<ushort, string>(6, "Welcome Back"),
                [6] = new Tuple<ushort, string>(7, "Training 1"),
                [7] = new Tuple<ushort, string>(8, "Training 2"),
                [8] = new Tuple<ushort, string>(9, "Training 3"),
                [9] = new Tuple<ushort, string>(10, "Training 4"),
                [10] = new Tuple<ushort, string>(11, "Training 5"),
                [11] = new Tuple<ushort, string>(12, "Training 6"),
                [12] = new Tuple<ushort, string>(13, "Training 7"),
                [13] = new Tuple<ushort, string>(14, "Training 8"),
                [14] = new Tuple<ushort, string>(15, "Training 9"),
                [15] = new Tuple<ushort, string>(16, "Training 10"),
                [16] = new Tuple<ushort, string>(17, "Training 11"),
                [17] = new Tuple<ushort, string>(18, "Training 12"),
                [18] = new Tuple<ushort, string>(19, "Training 13"),
                [19] = new Tuple<ushort, string>(20, "Training 14"),
                [20] = new Tuple<ushort, string>(21, "Training 15"),
                [21] = new Tuple<ushort, string>(22, "Training 16"),
                [22] = new Tuple<ushort, string>(23, "Training 17"),
                [23] = new Tuple<ushort, string>(27, "Shower"),
                [24] = new Tuple<ushort, string>(28, "Dragon Death"),
                [25] = new Tuple<ushort, string>(29, "Feel Free"),
                [26] = new Tuple<ushort, string>(30, "Xian Cutscene"),
                [27] = new Tuple<ushort, string>(31, "Caves Ambience"),
                [28] = new Tuple<ushort, string>(32, "Water Ambience"),
                [29] = new Tuple<ushort, string>(33, "Wind Ambience"),
                [30] = new Tuple<ushort, string>(34, "Heartbeat Ambience"),
                [31] = new Tuple<ushort, string>(35, "Danger 1"),
                [32] = new Tuple<ushort, string>(36, "Danger 2"),
                [33] = new Tuple<ushort, string>(37, "Danger 3"),
                [34] = new Tuple<ushort, string>(38, "Sacred"),
                [35] = new Tuple<ushort, string>(39, "Awe"),
                [36] = new Tuple<ushort, string>(40, "Venice Violins"),
                [37] = new Tuple<ushort, string>(41, "End of Level"),
                [38] = new Tuple<ushort, string>(42, "Mystical"),
                [39] = new Tuple<ushort, string>(43, "Revelation 1"),
                [40] = new Tuple<ushort, string>(44, "Be Careful 1"),
                [41] = new Tuple<ushort, string>(45, "Chandelier Room"),
                [42] = new Tuple<ushort, string>(46, "Drama"),
                [43] = new Tuple<ushort, string>(47, "Secret"),
                [44] = new Tuple<ushort, string>(48, "It's Coming! 1"),
                [45] = new Tuple<ushort, string>(49, "It's Coming! 2"),
                [46] = new Tuple<ushort, string>(50, "Warning 1"),
                [47] = new Tuple<ushort, string>(51, "Warning 2"),
                [48] = new Tuple<ushort, string>(52, "Skidoo Techno"),
                [49] = new Tuple<ushort, string>(53, "Skidoo Percussion"),
                [50] = new Tuple<ushort, string>(54, "Synth Pads"),
                [51] = new Tuple<ushort, string>(55, "Revelation 2"),
                [52] = new Tuple<ushort, string>(56, "Be Careful 2"),
                [53] = new Tuple<ushort, string>(57, "Revelation 3"),
                [54] = new Tuple<ushort, string>(58, "Industrial Ambience"),
                [55] = new Tuple<ushort, string>(59, "Spooky Ambience"),
                [56] = new Tuple<ushort, string>(60, "Barkhang"),
                [57] = new Tuple<ushort, string>(61, "Gong"),
                [58] = new Tuple<ushort, string>(62, "OK, Marco"),
                [59] = new Tuple<ushort, string>(63, "Home Sweet Home"),
                [60] = new Tuple<ushort, string>(64, "Main Theme")
            };
        }

        private Dictionary<ushort, string> GetTR3Map()
        {
            return new Dictionary<ushort, string>
            {
                [2] = "The Puzzle Element",
                [3] = "No Waiting Around 1",
                [4] = "Something Spooky is in that Jungle",
                [5] = "Lara's Themes",
                [6] = "The Cavern Sewers",
                [7] = "Geordie Bob",
                [8] = "Tony (The Loon)",
                [9] = "No Waiting Around 2",
                [10] = "The Greedy Mob",
                [11] = "A Long Way Up",
                [12] = "No Waiting Around 3",
                [13] = "There Be Butterflies Here 2",
                [14] = "She's Cool",
                [15] = "Mind the Gap 2",
                [16] = "Around the Corner 2",
                [17] = "Around the Corner 1",
                [18] = "Kneel and Pray",
                [19] = "Around the Corner 4",
                [20] = "Around the Corner 3",
                [21] = "Seeing is Believing 1",
                [22] = "Look What We Have Here 3",
                [23] = "There Be Butterflies Here 4",
                [24] = "Stone the Crows 10",
                [25] = "There Be Butterflies Here 3",
                [26] = "Meteorite Cavern",
                [27] = "Steady",
                [28] = "Antarctica",
                [29] = "Things",
                [30] = "Anyone There",
                [31] = "Grotto",
                [32] = "On the Beach",
                [33] = "Gamma Pals",
                [34] = "In the Jungle",
                [35] = "Piranha Waters",
                [36] = "The Rapids",
                [37] = "Supper Time",
                [38] = "Look out 5",
                [39] = "Look What We Have Here 1",
                [40] = "Around the Corner 5",
                [41] = "Seeing is Believing 2",
                [42] = "Stone the Crows 9.wav",
                [43] = "Look out 8",
                [44] = "Look out 4",
                [45] = "Stone the Crows 7",
                [46] = "Stone the Crows 3",
                [47] = "Stone the Crows 8",
                [48] = "Look What We Have Here 2",
                [49] = "Stone the Crows 4",
                [50] = "Stone the Crows 6",
                [51] = "Look out 3",
                [52] = "Look out 1",
                [53] = "There Be Butterflies Here 1",
                [54] = "Stone the Crows 1",
                [55] = "Stone the Crows 5",
                [56] = "Mind the Gap 1",
                [57] = "There Be Butterflies Here 5",
                [58] = "Look out 2",
                [59] = "Look out 7",
                [60] = "Stone the Crows (2)",
                [61] = "Look out 6",
                [62] = "Scott's Hut",
                [63] = "Cavern Sewers",
                [64] = "Jungle Camp",
                [65] = "Worship Room",
                [66] = "Cavern",
                [67] = "Rooftops",
                [68] = "Tree Shack",
                [69] = "Temple Exit",
                [70] = "Delivery Truck",
                [71] = "Penthouse",
                [72] = "Ravine",
                [73] = "Old Smokey",
                [74] = "Under Smokey",
                [75] = "Refining Plant",
                [76] = "Rumble Sub",
                [77] = "Quake",
                [78] = "Blank",
                [82] = "Excellent",
                [83] = "That's Great",
                [84] = "Training 1",
                [85] = "Training 2",
                [86] = "Training 3",
                [87] = "Training 4",
                [88] = "Training 5",
                [89] = "Fancy a Swim?",
                [90] = "Let's Go Outside",
                [91] = "Hey",
                [92] = "Now it's Time...",
                [93] = "Training 6",
                [94] = "Training 7",
                [95] = "Gosh",
                [96] = "Training 8",
                [97] = "Training 9",
                [98] = "Training 10",
                [99] = "Training 11",
                [100] = "Training 12",
                [101] = "Welcome Back",
                [102] = "Training 13",
                [103] = "Training 14",
                [104] = "Training 15",
                [105] = "Training 16",
                [106] = "Training 17",
                [107] = "Training 18",
                [108] = "Training 19",
                [109] = "Training 20",
                [110] = "Training 21",
                [111] = "Training 22",
                [112] = "Training 23",
                [113] = "Training 24",
                [114] = "Training 25",
                [115] = "Training 26",
                [116] = "Training 27",
                [117] = "Training 28",
                [118] = "Training 29",
                [119] = "Training 30",
                [120] = "In the Hut",
                [121] = "And so on...",
                [122] = "Secret 1",
                [123] = "Secret 2"
            };
        }
    }
}