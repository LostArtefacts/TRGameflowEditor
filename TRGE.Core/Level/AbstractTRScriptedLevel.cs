using System.Collections.Generic;
using System.IO;

namespace TRGE.Core
{
    public abstract class AbstractTRScriptedLevel : AbstractTROperationContainer
    {
        public string ID { get; private set; }
        public string Name { get; internal set; }

        protected string _levelFile;
        public string LevelFile
        {
            get => _levelFile;
            internal set
            {
                ID = CreateID(_levelFile = value);
            }
        }

        public string LevelFileBaseName => Path.GetFileName(LevelFile);

        protected List<string> _puzzles, _keys, _pickups;
        public IReadOnlyList<string> Puzzles => _puzzles;
        public IReadOnlyList<string> Keys => _keys;
        public IReadOnlyList<string> Pickups => _pickups;

        public abstract ushort Sequence { get; internal set; }
        public abstract ushort TrackID { get; internal set; }
        public abstract bool HasFMV { get; internal set; }
        public abstract bool SupportsFMVs { get; }
        public abstract bool HasStartAnimation { get; internal set; }
        public abstract bool SupportsStartAnimations { get; }
        public abstract short StartAnimationID { get; internal set; }
        public abstract bool HasCutScene { get; internal set; }
        public abstract bool SupportsCutScenes { get; }
        public abstract bool HasSunset { get; internal set; }
        public abstract bool HasDeadlyWater { get; internal set; }
        public abstract bool RemovesWeapons { get; internal set; }
        public abstract bool RemovesAmmo { get; internal set; }
        public abstract bool HasSecrets { get; internal set; }
        public abstract bool KillToComplete { get; }
        public abstract bool IsFinalLevel { get; internal set; }

        public bool OptionallyRemovesWeapons => RemovesWeapons && !RemovesAmmo;
        public bool ForciblyRemovesWeapons => RemovesWeapons && RemovesAmmo;

        internal AbstractTRScriptedLevel()
        {
            _puzzles = new List<string>();
            _keys = new List<string>();
            _pickups = new List<string>();
        }

        internal void AddPuzzle(string puzzle)
        {
            _puzzles.Add(puzzle);
        }

        internal void AddKey(string key)
        {
            _keys.Add(key);
        }

        internal void AddPickup(string pickup)
        {
            _pickups.Add(pickup);
        }

        internal void CopyOperation(TROpDef opDef, AbstractTRScriptedLevel other)
        {
            if (!HasOperation(opDef))
            {
                int i = other.GetOperationIndex(opDef);
                if (i != -1)
                {
                    _operations.Insert(i, other.GetOperation(opDef));
                }
            }
        }

        internal static string CreateID(string identifier)
        {
            return Path.GetFileNameWithoutExtension(identifier).ToUpper().CreateMD5();
        }

        /// <summary>
        /// Checks if the scripted level's file name matches the supplied argument.
        /// </summary>
        public bool Is(string levelFileName)
        {
            return CreateID(levelFileName).Equals(ID);
        }

        public override bool Equals(object obj)
        {
            return obj is AbstractTRScriptedLevel && (obj as AbstractTRScriptedLevel).ID == ID;
        }

        public override int GetHashCode()
        {
            return 1213502048 + EqualityComparer<string>.Default.GetHashCode(ID);
        }
    }
}