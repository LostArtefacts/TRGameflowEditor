namespace TRGE.Core
{
    public class TROperation
    {
        public TROpDef Definition { get; internal set; }
        public ushort Operand { get; internal set; }
        public bool IsActive { get; internal set; }

        public ushort OpCode => Definition.OpCode;
        public bool HasOperand => Definition.HasOperand;

        internal TROperation(TROpDef opDef, ushort operand, bool isActive)
        {
            Definition = opDef;
            Operand = operand;
            IsActive = isActive;
        }
    }
}