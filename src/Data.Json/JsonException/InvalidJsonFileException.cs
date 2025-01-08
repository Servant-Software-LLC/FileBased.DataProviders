namespace Data.Json.JsonException;

public class InvalidJsonFileException : InvalidFileException
{
    public InvalidJsonFileException(string message)
      : base(message)
    {
    }
}
