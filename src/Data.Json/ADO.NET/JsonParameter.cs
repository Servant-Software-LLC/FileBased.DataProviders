namespace System.Data.JsonClient;
public class JsonParameter : FileParameter<JsonParameter>
{
    public JsonParameter()
    {
    }

    public JsonParameter(string parameterName, DbType type) : base(parameterName, type)
    {
    }

    public JsonParameter(string parameterName, object value) : base(parameterName, value)
    {
    }

    public JsonParameter(string parameterName, DbType dbType, string sourceColumn) 
        : base(parameterName, dbType, sourceColumn)
    {
    }

    public override JsonParameter Clone() => new(ParameterName, Value) { SourceColumn = SourceColumn };
}