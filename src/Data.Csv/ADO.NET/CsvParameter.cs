namespace System.Data.CsvClient;

public class CsvParameter : FileParameter<CsvParameter>
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

    public override CsvParameter Clone() => new(ParameterName, DbType, SourceColumn);
}