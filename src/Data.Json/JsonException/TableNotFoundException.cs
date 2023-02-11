namespace Data.Json.JsonException;

public class TableNotFoundException : Exception
{
    public TableNotFoundException(string message) : base(message)
    {
    }
}
