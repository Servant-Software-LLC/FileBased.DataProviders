namespace System.Data.XmlClient;

/// <summary>
/// Represents a parameter to a <see cref="XmlCommand"/> and optionally its mapping to <see cref="System.Data.DataSet"/> columns.
/// </summary>
public class XmlParameter : FileParameter<XmlParameter>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="XmlParameter"/> class.
    /// </summary>
    public XmlParameter() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="XmlParameter"/> class with the specified parameter name and data type.
    /// </summary>
    /// <param name="parameterName">The name of the parameter.</param>
    /// <param name="type">The data type of the parameter.</param>
    public XmlParameter(string parameterName, DbType type) : base(parameterName, type) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="XmlParameter"/> class with the specified parameter name and value.
    /// </summary>
    /// <param name="parameterName">The name of the parameter.</param>
    /// <param name="value">The value of the parameter.</param>
    public XmlParameter(string parameterName, object value) : base(parameterName, value) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="XmlParameter"/> class with the specified parameter name, data type, and source column name.
    /// </summary>
    /// <param name="parameterName">The name of the parameter.</param>
    /// <param name="dbType">The data type of the parameter.</param>
    /// <param name="sourceColumn">The name of the source column.</param>
    public XmlParameter(string parameterName, DbType dbType, string sourceColumn)
        : base(parameterName, dbType, sourceColumn) { }

    /// <summary>
    /// Creates a new <see cref="XmlParameter"/> that is a copy of the current instance.
    /// </summary>
    /// <returns>A new <see cref="XmlParameter"/> that is a copy of this instance.</returns>
    public override XmlParameter Clone() => new(ParameterName, Value) { SourceColumn = SourceColumn };

}