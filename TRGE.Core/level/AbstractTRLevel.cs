using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TRGE.Core
{
    internal abstract class AbstractTRLevel
    {
        internal string ID { get; private set; }
        internal string Name;

        protected string _levelFile;
        internal string LevelFile
        {
            get => _levelFile;
            set
            {
                ID = Hashing.CreateMD5(_levelFile = value, Encoding.Default);
            }
        }

        protected readonly List<TROperation> _operations;
        internal IReadOnlyList<TROperation> Operations => _operations.AsReadOnly();

        protected List<string> _puzzles, _keys, _pickups;
        internal IReadOnlyList<string> Puzzles => _puzzles;
        internal IReadOnlyList<string> Keys => _keys;
        internal IReadOnlyList<string> Pickups => _pickups;

        internal abstract ushort Sequence { get; set; }
        internal abstract bool HasFMV { get; set; }
        internal abstract bool HasStartAnimation { get; set; }
        internal abstract bool HasCutScene { get; set; }
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
            _operations = new List<TROperation>();
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

        internal virtual void AddOperation(TROpDef opDef, ushort operand = ushort.MaxValue, bool isActive = true)
        {
            _operations.Add(new TROperation(opDef, operand, isActive));
        }

        internal void BuildOperations(ushort[] levelScriptData)
        {
            for (ushort i = 0; i < levelScriptData.Length; i++)
            {
                TROpDef opDef = GetOpDefFor(levelScriptData[i]);
                ushort operand = ushort.MaxValue;
                if (opDef.HasOperand)
                {
                    operand = levelScriptData[++i];
                }
                AddOperation(opDef, operand);
            }
        }

        protected abstract TROpDef GetOpDefFor(ushort scriptData);

        internal ushort[] TranslateOperations()
        {
            List<ushort> ret = new List<ushort>();
            foreach (TROperation op in _operations)
            {
                if (op.IsActive)
                {
                    ret.Add(op.OpCode);
                    if (op.HasOperand)
                    {
                        ret.Add(op.Operand);
                    }
                }
            }
            return ret.ToArray();
        }

        internal bool HasOperation(TROpDef opDef)
        {
            return GetOperation(opDef) != null;
        }

        internal bool HasActiveOperation(TROpDef opDef)
        {
            return HasOperationStrict(opDef, true);
        }

        internal bool HasInactiveOperation(TROpDef opDef)
        {
            return HasOperationStrict(opDef, false);
        }

        private bool HasOperationStrict(TROpDef opDef, bool activeStatus)
        {
            foreach (TROperation op in _operations)
            {
                if (op.TROpDef == opDef && op.IsActive == activeStatus)
                {
                    return true;
                }
            }
            return false;
        }

        internal TROperation GetOperation(TROpDef opDef)
        {
            foreach (TROperation op in _operations)
            {
                if (op.TROpDef == opDef)
                {
                    return op;
                }
            }
            return null;
        }

        internal int GetOperationIndex(TROpDef opDef)
        {
            for (int i = 0; i < _operations.Count; i++)
            {
                if (_operations[i].TROpDef == opDef)
                {
                    return i;
                }
            }
            return -1;
        }

        internal int GetLastOperationIndex(TROpDef opDef)
        {
            for (int i = _operations.Count - 1; i >= 0; i--)
            {
                if (_operations[i].TROpDef == opDef)
                {
                    return i;
                }
            }
            return -1;
        }

        internal void InsertOperation(TROpDef opDef, ushort operand, TROpDef beforeOpDef, bool isActive = true)
        {
            int pos = 0;
            TROperation op = GetOperation(beforeOpDef);
            if (op != null)
            {
                pos = _operations.IndexOf(op);
            }
            _operations.Insert(pos, new TROperation(opDef, operand, isActive));
        }

        internal void RemoveOperation(TROpDef opDef)
        {
            TROperation op;
            while ((op = GetOperation(opDef)) != null)
            {
                _operations.Remove(op);
            }
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

        internal bool SetOperationActive(TROpDef opDef, bool isActive)
        {
            bool found = false;
            foreach (TROperation op in _operations)
            {
                if (op.TROpDef == opDef)
                {
                    op.IsActive = isActive;
                    found = true;
                }
            }
            return found;
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