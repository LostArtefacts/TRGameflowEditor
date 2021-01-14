using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TRGE.Core
{
    internal class TROperation
    {
        internal TROpDef TROpDef { get; set; }
        internal ushort Operand { get; set; }
        internal bool IsActive { get; set; }

        internal ushort OpCode => TROpDef.OpCode;
        internal bool HasOperand => TROpDef.HasOperand;

        internal TROperation(TROpDef opDef, ushort operand, bool isActive)
        {
            TROpDef = opDef;
            Operand = operand;
            IsActive = isActive;
        }
    }
}