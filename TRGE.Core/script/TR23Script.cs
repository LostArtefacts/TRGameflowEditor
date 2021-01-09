using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

[assembly: InternalsVisibleTo("TRGE.Core.Test")]
namespace TRGE.Core
{
    internal class TR23Script : AbstractTRScript
    {
        internal const uint Version = 3;

        #region Script Variables
        private string _description;
        internal string Description
        {
            get => _description;
            set
            {
                if (value.Length > 256)
                {
                    throw new IndexOutOfRangeException();
                }
                _description = value;
            }
        }

        internal ushort GameflowSize { get; private set; }
        internal uint FirstOption { get; private set; }
        internal int TitleReplace { get; private set; }
        internal uint DeathDemoMode { get; private set; }
        internal uint DeathInGame { get; private set; }
        internal uint DemoTime { get; private set; }
        internal uint DemoInterrupt { get; private set; }
        internal uint DemoEnd { get; private set; }       
        internal ushort NumLevels { get; private set; }
        internal ushort NumPlayableLevels => (ushort)(NumLevels - NumDemoLevels);
        internal ushort NumPictures { get; private set; }
        internal ushort NumTitles { get; private set; }
        internal ushort NumRPLs { get; private set; }
        internal ushort NumCutScenes { get; private set; }
        internal ushort NumDemoLevels { get; private set; }
        internal ushort TitleSound { get; private set; }
        internal ushort SingleLevel { get; private set; }        
        internal ushort Flags { get; private set; }        
        internal byte Xor { get; private set; }
        internal byte Language { get; private set; }
        internal byte SecretSound { get; private set; }
        
        private List<string> _levelNames, _pictureNames, _titleFileNames, _rplFileNames, _levelFileNames, _cutSceneFileNames;
        internal IReadOnlyList<string> LevelNames => _levelNames;
        internal IReadOnlyList<string> PictureNames => _pictureNames;
        internal IReadOnlyList<string> TitleFileNames => _titleFileNames;
        internal IReadOnlyList<string> RPLFileNames => _rplFileNames;
        internal IReadOnlyList<string> LevelFileNames => _levelFileNames;
        internal IReadOnlyList<string> CutSceneFileNames => _cutSceneFileNames;

        private List<ushort[]> _scriptData;
        private List<ushort> _demoData;
        private List<uint[]> _psxFMVData;
        internal IReadOnlyList<ushort[]> ScriptData => _scriptData;
        internal IReadOnlyList<ushort> DemoData => _demoData;
        internal IReadOnlyList<uint[]> PSXFMVData => _psxFMVData;

        private List<string> _gameStrings1, _gameStrings2;
        private List<string> _puzzleNames1, _puzzleNames2, _puzzleNames3, _puzzleNames4;
        private List<string> _secretNames1, _secretNames2, _secretNames3, _secretNames4;
        private List<string> _specialNames1, _specialNames2;
        private List<string> _pickupNames1, _pickupNames2;
        private List<string> _keyNames1, _keyNames2, _keyNames3, _keyNames4;

        internal ushort NumGameStrings1 { get; private set; }
        internal ushort NumGameStrings2 { get; private set; }
        internal IReadOnlyList<string> GameStrings1 => _gameStrings1;
        internal IReadOnlyList<string> GameStrings2 => _gameStrings2;
                
        internal IReadOnlyList<string> PuzzleNames1 => _puzzleNames1;
        internal IReadOnlyList<string> PuzzleNames2 => _puzzleNames2;
        internal IReadOnlyList<string> PuzzleNames3 => _puzzleNames3;
        internal IReadOnlyList<string> PuzzleNames4 => _puzzleNames4;

        internal IReadOnlyList<string> SecretNames1 => _secretNames1;
        internal IReadOnlyList<string> SecretNames2 => _secretNames2;
        internal IReadOnlyList<string> SecretNames3 => _secretNames3;
        internal IReadOnlyList<string> SecretNames4 => _secretNames4;

        internal IReadOnlyList<string> SpecialNames1 => _specialNames1;
        internal IReadOnlyList<string> SpecialNames2 => _specialNames2;

        internal IReadOnlyList<string> PickupNames1 => _pickupNames1;
        internal IReadOnlyList<string> PickupNames2 => _pickupNames2;
                
        internal IReadOnlyList<string> KeyNames1 => _keyNames1;
        internal IReadOnlyList<string> KeyNames2 => _keyNames2;
        internal IReadOnlyList<string> KeyNames3 => _keyNames3;
        internal IReadOnlyList<string> KeyNames4 => _keyNames4;

        private byte[] _padding1;
        private byte[] _padding2;
        private byte[] _padding3;
        private byte[] _padding4;
        #endregion

        protected override void CalculateEdition()
        {
            if (_levelFileNames == null)
            {
                return;
            }

            foreach (string levelFile in _levelFileNames)
            {
                string llf = levelFile.ToLower();
                if (llf.EndsWith("wall.tr2"))
                {
                    Edition = TREdition.TR2PC;
                }
                else if (llf.EndsWith("wall.psx"))
                {
                    Edition = Xor == 0 ? TREdition.TR2PSXBETA : TREdition.TR2PSX;
                }
                else if (llf.EndsWith("level1.tr2"))
                {
                    Edition = TREdition.TR2G;
                }
                else if (llf.EndsWith("jungle.tr2"))
                {
                    Edition = TREdition.TR3PC;
                }
                else if (llf.EndsWith("jungle.psx"))
                {
                    Edition = TREdition.TR3PSX;
                }
                else if (llf.EndsWith("scotland.tr2"))
                {
                    Edition = TREdition.TR3G;
                }
            }
        }
        
        internal override void Read(BinaryReader br)
        {
            if (br.ReadUInt32() != Version)
            {
                throw new ArgumentException("Unsupported script data file.");
            }

            _description = Encoding.ASCII.GetString(br.ReadBytes(256));
            int nt = _description.IndexOf('\0');
            if (nt != -1)
            {
                _description = _description.Substring(0, nt);
            }

            GameflowSize = br.ReadUInt16();
            FirstOption = br.ReadUInt32();
            TitleReplace = br.ReadInt32();
            DeathDemoMode = br.ReadUInt32();
            DeathInGame = br.ReadUInt32();
            DemoTime = br.ReadUInt32();
            DemoInterrupt = br.ReadUInt32();
            DemoEnd = br.ReadUInt32();
            _padding1 = br.ReadBytes(36);

            NumLevels = br.ReadUInt16();
            NumPictures = br.ReadUInt16();
            NumTitles = br.ReadUInt16();
            NumRPLs = br.ReadUInt16();
            NumCutScenes = br.ReadUInt16();
            NumDemoLevels = br.ReadUInt16();
            TitleSound = br.ReadUInt16();
            SingleLevel = br.ReadUInt16();
            _padding2 = br.ReadBytes(32);

            Flags = br.ReadUInt16();
            _padding3 = br.ReadBytes(6);

            Xor = br.ReadByte();
            Language = br.ReadByte();
            SecretSound = br.ReadByte();
            _padding4 = br.ReadBytes(5);

            _levelNames = ReadStringData(br, NumLevels);
            _pictureNames = ReadStringData(br, NumPictures);
            _titleFileNames = ReadStringData(br, NumTitles);
            _rplFileNames = ReadStringData(br, NumRPLs);
            _levelFileNames = ReadStringData(br, NumLevels);
            _cutSceneFileNames = ReadStringData(br, NumCutScenes);

            _scriptData = ReadScriptData(br);
            _demoData = ReadDemoData(br);
            _psxFMVData = ReadPSXFMVData(br);

            NumGameStrings1 = br.ReadUInt16();
            _gameStrings1 = ReadStringData(br, NumGameStrings1);

            //we know the level file names at this point, so a decent estimate
            //can be made for the actual edition
            CalculateEdition();

            if (Edition == TREdition.TR2PSXBETA)
            {
                NumGameStrings2 = 79;
            }
            else if (Edition.Hardware == Hardware.PC)
            {
                NumGameStrings2 = 41;
            }
            else
            {
                NumGameStrings2 = 80;
            }
            _gameStrings2 = ReadStringData(br, NumGameStrings2);

            _puzzleNames1 = ReadStringData(br, NumLevels);
            _puzzleNames2 = ReadStringData(br, NumLevels);
            _puzzleNames3 = ReadStringData(br, NumLevels);
            _puzzleNames4 = ReadStringData(br, NumLevels);

            if (Edition == TREdition.TR2PSXBETA)
            {
                _secretNames1 = ReadStringData(br, NumLevels);
                _secretNames2 = ReadStringData(br, NumLevels);
                _secretNames3 = ReadStringData(br, NumLevels);
                _secretNames4 = ReadStringData(br, NumLevels);

                _specialNames1 = ReadStringData(br, NumLevels);
                _specialNames2 = ReadStringData(br, NumLevels);
            }

            _pickupNames1 = ReadStringData(br, NumLevels);
            _pickupNames2 = ReadStringData(br, NumLevels);

            _keyNames1 = ReadStringData(br, NumLevels);
            _keyNames2 = ReadStringData(br, NumLevels);
            _keyNames3 = ReadStringData(br, NumLevels);
            _keyNames4 = ReadStringData(br, NumLevels);
        }

        private List<string> ReadStringData(BinaryReader br, ushort size)
        {
            List<string> stringData = new List<string>(size);
            if (size == 0)
            {
                br.ReadUInt16(); //0
            }
            else
            {
                ushort[] offsets = new ushort[size + 1];
                for (int i = 0; i < offsets.Length; i++)
                {
                    offsets[i] = br.ReadUInt16();
                }

                for (int i = 0; i < size; i++)
                {
                    int target = offsets[i + 1] - offsets[i] - 1;
                    StringBuilder sb = new StringBuilder();
                    for (int k = 0; k < target; k++)
                    {
                        sb.Append(((char)((uint)br.ReadByte() ^ Xor)).ToString());
                    }
                    stringData.Add(sb.ToString());
                    br.ReadByte(); //xor
                }
            }

            return stringData;
        }

        private List<ushort[]> ReadScriptData(BinaryReader br)
        {
            ushort[] offsets = new ushort[NumLevels + 2];
            for (int i = 0; i < offsets.Length; i++)
            {
                offsets[i] = (ushort)(br.ReadUInt16() / 2U);
            }

            List<ushort[]> scriptData = new List<ushort[]>(NumLevels + 1);
            for (int i = 0; i < NumLevels + 1; i++)
            {
                scriptData.Add(new ushort[offsets[i + 1] - offsets[i]]);
                for (int j = 0; j < scriptData[i].Length; j++)
                {
                    scriptData[i][j] = br.ReadUInt16();
                }
            }

            return scriptData;
        }

        private List<ushort> ReadDemoData(BinaryReader br)
        {
            List<ushort> demoData = new List<ushort>(NumDemoLevels);
            for (int i = 0; i < NumDemoLevels; i++)
            {
                demoData.Add(br.ReadUInt16());
            }

            return demoData;
        }

        private List<uint[]> ReadPSXFMVData(BinaryReader br)
        {
            List<uint[]> fmvData = new List<uint[]>(NumRPLs);
            if (Edition.Hardware == Hardware.PSX)
            {
                for (int i = 0; i < NumRPLs; i++)
                {
                    fmvData.Add(new uint[]
                    {
                        br.ReadUInt32(), br.ReadUInt32()
                    });
                }
            }
            return fmvData;
        }

        internal override byte[] Serialise()
        {
            using (MemoryStream ms = new MemoryStream())
            using (BinaryWriter bw = new BinaryWriter(ms))
            {
                bw.Write(Version);

                byte[] description = Encoding.ASCII.GetBytes(_description);
                Array.Resize(ref description, 256);
                bw.Write(description);

                bw.Write(GameflowSize);
                bw.Write(FirstOption);
                bw.Write(TitleReplace);
                bw.Write(DeathDemoMode);
                bw.Write(DeathInGame);
                bw.Write(DemoTime);
                bw.Write(DemoInterrupt);
                bw.Write(DemoEnd);
                bw.Write(_padding1);

                bw.Write(NumLevels);
                bw.Write(NumPictures);
                bw.Write(NumTitles);
                bw.Write(NumRPLs);
                bw.Write(NumCutScenes);
                bw.Write(NumDemoLevels);
                bw.Write(TitleSound);
                bw.Write(SingleLevel);
                bw.Write(_padding2);

                bw.Write(Flags);
                bw.Write(_padding3);

                bw.Write(Xor);
                bw.Write(Language);
                bw.Write(SecretSound);
                bw.Write(_padding4);

                WriteStringData(bw, _levelNames);
                WriteStringData(bw, _pictureNames);
                WriteStringData(bw, _titleFileNames);
                WriteStringData(bw, _rplFileNames);
                WriteStringData(bw, _levelFileNames);
                WriteStringData(bw, _cutSceneFileNames);

                WriteScriptData(bw);
                WriteDemoData(bw);
                WritePSXFMVData(bw);

                bw.Write(NumGameStrings1);
                WriteStringData(bw, _gameStrings1);
                WriteStringData(bw, _gameStrings2);

                WriteStringData(bw, _puzzleNames1);
                WriteStringData(bw, _puzzleNames2);
                WriteStringData(bw, _puzzleNames3);
                WriteStringData(bw, _puzzleNames4);

                WriteStringData(bw, _pickupNames1);
                WriteStringData(bw, _pickupNames2);

                WriteStringData(bw, _keyNames1);
                WriteStringData(bw, _keyNames2);
                WriteStringData(bw, _keyNames3);
                WriteStringData(bw, _keyNames4);

                return ms.ToArray();
            }
        }

        private void WriteStringData(BinaryWriter bw, List<string> stringData)
        {
            if (stringData.Count == 0)
            {
                bw.Write((ushort)0);
                return;
            }

            ushort runningOffset = 0;
            bw.Write(runningOffset);

            List<byte> encodedStringData = new List<byte>();
            for (int i = 0; i < stringData.Count; i++)
            {
                char[] chars = stringData[i].ToCharArray();
                runningOffset += Convert.ToUInt16(chars.Length + 1); //xor
                bw.Write(runningOffset);
                
                foreach (int j in chars)
                {
                    encodedStringData.Add((byte)int.Parse((j ^ Xor).ToString()));
                }
                encodedStringData.Add(Xor);
            }

            bw.Write(encodedStringData.ToArray());
        }

        private void WriteScriptData(BinaryWriter bw)
        {
            if (_scriptData.Count == 0)
            {
                return;
            }

            ushort runningOffset = 0;
            bw.Write(runningOffset);
            List<ushort> collectedScriptData = new List<ushort>();

            for (int i = 0; i < _scriptData.Count; i++)
            {
                ushort[] data = _scriptData[i];
                runningOffset += Convert.ToUInt16(data.Length);
                bw.Write(Convert.ToUInt16(runningOffset * 2U));

                for (int j = 0; j < data.Length; j++)
                {
                    collectedScriptData.Add(data[j]);
                }
            }

            foreach (ushort dat in collectedScriptData)
            {
                bw.Write(dat);
            }
        }

        private void WriteDemoData(BinaryWriter bw)
        {
            if (NumDemoLevels > 0)
            {
                foreach (ushort dd in DemoData)
                {
                    bw.Write(dd);
                }
            }
        }

        private void WritePSXFMVData(BinaryWriter bw)
        {
            if (Edition.Hardware == Hardware.PSX)
            {
                foreach (uint[] fmvData in _psxFMVData)
                {
                    foreach (uint dd in fmvData)
                    {
                        bw.Write(dd);
                    }
                }
            }
        }
    }
}