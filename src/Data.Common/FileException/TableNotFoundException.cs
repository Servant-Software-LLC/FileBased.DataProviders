namespace Data.Common.FileException;

public class TableNotFoundException : Exception
{
    public TableNotFoundException(string message) : base(message)
    {
    }
}
