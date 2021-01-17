using System.Collections.Generic;

namespace TRGE.Core
{
    internal abstract class AbstractTROperationContainer
    {
        protected readonly List<TROperation> _operations;
        internal IReadOnlyList<TROperation> Operations => _operations.AsReadOnly();

        internal AbstractTROperationContainer()
        {
            _operations = new List<TROperation>();
        }

        internal void BuildOperations(ushort[] scriptData)
        {
            for (ushort i = 0; i < scriptData.Length; i++)
            {
                TROpDef opDef = GetOpDefFor(scriptData[i]);
                ushort operand = ushort.MaxValue;
                if (opDef.HasOperand)
                {
                    operand = scriptData[++i];
                }
                AddOperation(opDef, operand);
            }
        }

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

        protected abstract TROpDef GetOpDefFor(ushort scriptData);

        internal virtual TROperation AddOperation(TROpDef opDef, ushort operand = ushort.MaxValue, bool isActive = true)
        {
            TROperation op = new TROperation(opDef, operand, isActive);
            _operations.Add(op);
            return op;
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
                if (op.Definition == opDef && op.IsActive == activeStatus)
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
                if (op.Definition == opDef)
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
                if (_operations[i].Definition == opDef)
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
                if (_operations[i].Definition == opDef)
                {
                    return i;
                }
            }
            return -1;
        }

        internal void InsertOperation(TROpDef opDef, ushort operand, TROpDef afterOpDef, bool isActive = true)
        {
            int pos = 0;
            if (afterOpDef != null)
            {
                TROperation op = GetOperation(afterOpDef);
                if (op != null)
                {
                    pos = _operations.IndexOf(op) + 1;
                }
            }

            TROperation operation = new TROperation(opDef, operand, isActive);
            if (pos >= _operations.Count)
            {
                _operations.Add(operation);
            }
            else
            {
                _operations.Insert(pos, operation);
            }
        }

        internal void EnsureOperation(TROperation operation)
        {
            if (!HasOperation(operation.Definition))
            {
                InsertOperation(operation.Definition, operation.Operand, operation.Definition.Next, operation.IsActive);
            }
        }

        internal void RemoveOperation(TROpDef opDef)
        {
            TROperation op;
            while ((op = GetOperation(opDef)) != null)
            {
                _operations.Remove(op);
            }
        }

        internal bool SetOperationActive(TROpDef opDef, bool isActive)
        {
            bool found = false;
            foreach (TROperation op in _operations)
            {
                if (op.Definition == opDef)
                {
                    op.IsActive = isActive;
                    found = true;
                }
            }
            return found;
        }
    }
}