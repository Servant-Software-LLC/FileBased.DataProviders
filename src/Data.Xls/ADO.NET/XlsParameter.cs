namespace System.Data.XlsClient;


/// <summary>
/// Represents a parameter for XLS operations.
/// </summary>
public class XlsParameter : FileParameter<XlsParameter>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="XlsParameter"/> class with no arguments.
    /// </summary>
    public XlsParameter() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="XlsParameter"/> class.
    /// </summary>
    /// <param name="parameterName">The name of the parameter.</param>
    /// <param name="type">The <see cref="DbType"/> of the parameter.</param>
    public XlsParameter(string parameterName, DbType type) : base(parameterName, type) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="XlsParameter"/> class.
    /// </summary>
    /// <param name="parameterName">The name of the parameter.</param>
    /// <param name="value">The value of the parameter.</param>
    public XlsParameter(string parameterName, object value) : base(parameterName, value) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="XlsParameter"/> class.
    /// </summary>
    /// <param name="parameterName">The name of the parameter.</param>
    /// <param name="dbType">The <see cref="DbType"/> of the parameter.</param>
    /// <param name="sourceColumn">The name of the source column.</param>
    public XlsParameter(string parameterName, DbType dbType, string sourceColumn)
        : base(parameterName, dbType, sourceColumn) { }

    /// <inheritdoc/>
    public override XlsParameter Clone() => new(ParameterName, Value) { SourceColumn = SourceColumn };
}
