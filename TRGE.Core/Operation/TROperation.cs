namespace TRGE.Core
{
    internal class TROperation
    {
        internal TROpDef Definition { get; set; }
        internal ushort Operand { get; set; }
        internal bool IsActive { get; set; }

        internal ushort OpCode => Definition.OpCode;
        internal bool HasOperand => Definition.HasOperand;

        internal TROperation(TROpDef opDef, ushort operand, bool isActive)
        {
            Definition = opDef;
            Operand = operand;
            IsActive = isActive;
        }
    }
}