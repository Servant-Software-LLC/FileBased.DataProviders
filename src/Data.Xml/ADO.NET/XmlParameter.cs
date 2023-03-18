namespace System.Data.XmlClient;

public class XmlParameter : FileParameter
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

    public XmlParameter(string parameterName, DbType dbType, string sourceColumn) : base(parameterName, dbType, sourceColumn)
    {
    }
}