namespace Data.Common.FileException;

public class QueryNotSupportedException : Exception
{
    public QueryNotSupportedException(string message): base(message)
    {
    }
}
