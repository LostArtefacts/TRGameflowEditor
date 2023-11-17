namespace TRGE.Core
{
    [AttributeUsage(AttributeTargets.Method)]
    public class TestSequenceAttribute : Attribute
    {
        public int Sequence;

        public TestSequenceAttribute(int sequence)
        {
            Sequence = sequence;
        }
    }
}