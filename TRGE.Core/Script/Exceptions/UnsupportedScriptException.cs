namespace TRGE.Core;

public class UnsupportedScriptException : Exception
{
    public UnsupportedScriptException()
        : base() { }

    public UnsupportedScriptException(string message)
        : base(message) { }

    public UnsupportedScriptException(string message, Exception innerException)
        : base(message, innerException) { }
}