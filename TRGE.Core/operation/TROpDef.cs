using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TRGE.Core
{
    internal class TROpDef
    {
        internal ushort OpCode { get; private set; }
        internal bool HasOperand { get; private set; }

        internal TROpDef(ushort opCode, bool hasOperand)
        {
            OpCode = opCode;
            HasOperand = hasOperand;
        }
    }
}