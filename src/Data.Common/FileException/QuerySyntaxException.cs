namespace Data.Common.FileException;

public class QuerySyntaxException : Exception
{
    public QuerySyntaxException(string message) : base(message)
    {
    }
}
