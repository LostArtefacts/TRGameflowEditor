namespace TRGE.Core;

public class ChecksumMismatchException : Exception
{
    public List<string> FailedFiles { get; set; } = new();

    public ChecksumMismatchException()
        : base() { }

    public ChecksumMismatchException(string message)
        : base(message) { }

    public ChecksumMismatchException(string message, Exception innerException)
        : base(message, innerException) { }
}