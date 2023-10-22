namespace System.Data.CsvClient;


/// <summary>
/// Represents a parameter for CSV operations.
/// </summary>
public class CsvParameter : FileParameter<CsvParameter>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CsvParameter"/> class with no arguments.
    /// </summary>
    public CsvParameter() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="CsvParameter"/> class.
    /// </summary>
    /// <param name="parameterName">The name of the parameter.</param>
    /// <param name="type">The <see cref="DbType"/> of the parameter.</param>
    public CsvParameter(string parameterName, DbType type) : base(parameterName, type) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="CsvParameter"/> class.
    /// </summary>
    /// <param name="parameterName">The name of the parameter.</param>
    /// <param name="value">The value of the parameter.</param>
    public CsvParameter(string parameterName, object value) : base(parameterName, value) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="CsvParameter"/> class.
    /// </summary>
    /// <param name="parameterName">The name of the parameter.</param>
    /// <param name="dbType">The <see cref="DbType"/> of the parameter.</param>
    /// <param name="sourceColumn">The name of the source column.</param>
    public CsvParameter(string parameterName, DbType dbType, string sourceColumn)
        : base(parameterName, dbType, sourceColumn) { }

    /// <inheritdoc/>
    public override CsvParameter Clone() => new(ParameterName, Value) { SourceColumn = SourceColumn };
}
