namespace Data.Json.JsonException;

public class InvalidJsonFileException : Exception
{
    public InvalidJsonFileException(string? message)
      : base(message)
    {
    }
}
