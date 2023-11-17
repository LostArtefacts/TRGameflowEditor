namespace TRGE.Core
{
    public class MissingScriptException : Exception
    {
        public MissingScriptException()
            : base() { }

        public MissingScriptException(string message)
            : base(message) { }

        public MissingScriptException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}