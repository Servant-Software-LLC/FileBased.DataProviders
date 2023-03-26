namespace Data.Common.FileException;

public class InvalidConnectionStringException : Exception
{
    public InvalidConnectionStringException(string message) : base(message)
    {
    }
}

