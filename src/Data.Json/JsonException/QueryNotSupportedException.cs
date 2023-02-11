namespace Data.Json.JsonException;

public class QueryNotSupportedException : Exception
{
    public QueryNotSupportedException(string message): base(message)
    {
    }
}
