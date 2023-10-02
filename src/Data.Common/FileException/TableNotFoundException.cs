namespace Data.Common.FileException;

public class TableNotFoundException : Exception
{
    public TableNotFoundException(string message) : base(message)
    {
    }

    public TableNotFoundException(string message, Exception inner) : base(message, inner)
    {
        
    }
}
