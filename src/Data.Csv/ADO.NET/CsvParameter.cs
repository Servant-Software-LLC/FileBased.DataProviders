namespace System.Data.CsvClient;

public class CsvParameter : FileParameter
{
    public CsvParameter()
    {
    }

    public CsvParameter(string parameterName, DbType type) : base(parameterName, type)
    {
    }

    public CsvParameter(string parameterName, object value) : base(parameterName, value)
    {
    }

    public CsvParameter(string parameterName, DbType dbType, string sourceColumn) 
        : base(parameterName, dbType, sourceColumn)
    {
    }
}