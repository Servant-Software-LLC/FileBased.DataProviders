namespace Data.Json.JsonException;

public class InvalidConnectionStringException : Exception
{
    public InvalidConnectionStringException(string message) : base(message)
    {
    }
}

