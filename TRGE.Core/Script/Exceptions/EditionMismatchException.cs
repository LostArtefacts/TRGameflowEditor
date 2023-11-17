namespace TRGE.Core;

public class EditionMismatchException : Exception
{
    public EditionMismatchException()
        : base() { }

    public EditionMismatchException(string message)
        : base(message) { }

    public EditionMismatchException(string message, Exception innerException)
        : base(message, innerException) { }
}