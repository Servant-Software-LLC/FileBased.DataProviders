namespace System.Data.XmlClient;

public class XmlParameter : FileParameter<XmlParameter>
{
    public XmlParameter()
    {
    }

    public XmlParameter(string parameterName, DbType type) : base(parameterName, type)
    {
    }

    public XmlParameter(string parameterName, object value) : base(parameterName, value)
    {
    }

    public XmlParameter(string parameterName, DbType dbType, string sourceColumn) 
        : base(parameterName, dbType, sourceColumn)
    {
    }

    public override XmlParameter Clone() => new(ParameterName, Value) { SourceColumn = SourceColumn };

}