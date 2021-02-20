using System.Collections.Generic;

namespace TRGE.Core
{
    public abstract class AbstractTROperationContainer
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

        /// <summary>
        /// If the provided operation exists but the operand and IsActive values, don't match, they
        /// wiil be aligned. Otherwise, the operation will be inserted.
        /// </summary>
        internal void EnsureOperation(TROperation operation)
        {
            TROperation currentOp = GetOperation(operation.Definition);
            if (currentOp != null)
            {
                currentOp.Operand = operation.Operand;
                currentOp.IsActive = operation.IsActive;
            }
            else
            {
                InsertOperation(operation.Definition, operation.Operand, operation.Definition.Next, operation.IsActive);
            }
        }

        internal bool RemoveOperation(TROpDef opDef)
        {
            bool result = false;
            TROperation op;
            while ((op = GetOperation(opDef)) != null)
            {
                result = _operations.Remove(op);
            }
            return result;
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