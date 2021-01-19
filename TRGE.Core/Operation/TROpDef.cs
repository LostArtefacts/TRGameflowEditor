using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TRGE.Core
{
    internal class TROpDef
    {
        internal TROpDef Next { get; private set; }
        internal ushort OpCode { get; private set; }
        internal bool HasOperand { get; private set; }

        internal TROpDef(ushort opCode, bool hasOperand, TROpDef next = null)
        {
            Next = next;
            OpCode = opCode;
            HasOperand = hasOperand;
        }
    }
}