using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TRGE.Core
{
    internal abstract class AbstractTRLevel : AbstractTROperationContainer
    {
        internal string ID { get; private set; }
        internal string Name;

        protected string _levelFile;
        internal string LevelFile
        {
            get => _levelFile;
            set
            {
                ID = CreateID(_levelFile = value);
            }
        }

        internal string LevelFileBaseName => Path.GetFileName(LevelFile);

        protected List<string> _puzzles, _keys, _pickups;
        internal IReadOnlyList<string> Puzzles => _puzzles;
        internal IReadOnlyList<string> Keys => _keys;
        internal IReadOnlyList<string> Pickups => _pickups;

        internal abstract ushort Sequence { get; set; }
        internal abstract ushort TrackID { get; set; }
        internal abstract bool HasFMV { get; set; }
        internal abstract bool SupportsFMVs { get; }
        internal abstract bool HasStartAnimation { get; set; }
        internal abstract bool SupportsStartAnimations { get; }
        internal abstract short StartAnimationID { get; set; }
        internal abstract bool HasCutScene { get; set; }
        internal abstract bool SupportsCutScenes { get; }
        internal abstract bool HasSunset { get; set; }
        internal abstract bool HasDeadlyWater { get; set; }
        internal abstract bool RemovesWeapons { get; set; }
        internal abstract bool RemovesAmmo { get; set; }
        internal abstract bool HasSecrets { get; set; }
        internal abstract bool KillToComplete { get; }
        internal abstract bool IsFinalLevel { get; set; }

        internal bool OptionallyRemovesWeapons => RemovesWeapons && !RemovesAmmo;
        internal bool ForciblyRemovesWeapons => RemovesWeapons && RemovesAmmo;

        internal AbstractTRLevel()
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

        internal void CopyOperation(TROpDef opDef, AbstractTRLevel other)
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
            return Hashing.CreateMD5(Path.GetFileNameWithoutExtension(identifier).ToUpper(), Encoding.Default);
        }

        public override bool Equals(object obj)
        {
            return obj is AbstractTRLevel && (obj as AbstractTRLevel).ID == ID;
        }

        public override int GetHashCode()
        {
            return 1213502048 + EqualityComparer<string>.Default.GetHashCode(ID);
        }
    }
}