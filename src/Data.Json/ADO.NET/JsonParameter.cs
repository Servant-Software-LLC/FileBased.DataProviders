namespace System.Data.JsonClient;

/// <summary>
/// Represents a parameter to a JSON command.
/// </summary>
public class JsonParameter : FileParameter<JsonParameter>
{
    /// <summary>
    /// Represents a parameter for JSON operations.
    /// </summary>
    public JsonParameter()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="JsonParameter"/> class with a parameter name and database type.
    /// </summary>
    /// <param name="parameterName">The name of the parameter.</param>
    /// <param name="type">The database type of the parameter.</param>
    public JsonParameter(string parameterName, DbType type) : base(parameterName, type)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="JsonParameter"/> class with a parameter name and value.
    /// </summary>
    /// <param name="parameterName">The name of the parameter.</param>
    /// <param name="value">The value of the parameter.</param>
    public JsonParameter(string parameterName, object value) : base(parameterName, value)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="JsonParameter"/> class with a parameter name, database type, and source column.
    /// </summary>
    /// <param name="parameterName">The name of the parameter.</param>
    /// <param name="dbType">The database type of the parameter.</param>
    /// <param name="sourceColumn">The source column of the parameter.</param>
    public JsonParameter(string parameterName, DbType dbType, string sourceColumn)
        : base(parameterName, dbType, sourceColumn)
    {
    }

    /// <summary>
    /// Creates a new <see cref="JsonParameter"/> that is a copy of the current instance.
    /// </summary>
    /// <returns>A new <see cref="JsonParameter"/> that is a copy of this instance.</returns>
    public override JsonParameter Clone() => new(ParameterName, Value) { SourceColumn = SourceColumn };
}
