namespace TRGE.Coord
{
    public class ScriptedLevelMismatchException : Exception
    {
        public ScriptedLevelMismatchException()
            : base() { }

        public ScriptedLevelMismatchException(string message)
            : base(message) { }

        public ScriptedLevelMismatchException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}