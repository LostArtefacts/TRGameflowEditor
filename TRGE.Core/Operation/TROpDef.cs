namespace TRGE.Core
{
    public class TROpDef
    {
        public TROpDef Next { get; private set; }
        public ushort OpCode { get; private set; }
        public bool HasOperand { get; private set; }

        internal TROpDef(ushort opCode, bool hasOperand, TROpDef next = null)
        {
            Next = next;
            OpCode = opCode;
            HasOperand = hasOperand;
        }
    }
}