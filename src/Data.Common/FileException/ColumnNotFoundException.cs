namespace Data.Common.FileException;

public class ColumnNotFoundException : Exception
{
    public ColumnNotFoundException(string message) : base(message)
    {
    }
}
