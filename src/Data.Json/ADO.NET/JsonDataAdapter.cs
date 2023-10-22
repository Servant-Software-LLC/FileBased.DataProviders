namespace System.Data.JsonClient;


/// <summary>
/// Represents a data adapter for JSON operations.
/// </summary>
public class JsonDataAdapter : FileDataAdapter<JsonParameter>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="JsonDataAdapter"/> class.
    /// </summary>
    public JsonDataAdapter() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="JsonDataAdapter"/> class with a select command.
    /// </summary>
    /// <param name="selectCommand">The select command.</param>
    public JsonDataAdapter(JsonCommand selectCommand) : base(selectCommand) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="JsonDataAdapter"/> class with a query and connection.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <param name="connection">The connection.</param>
    public JsonDataAdapter(string query, JsonConnection connection) : base(query, connection) { }

    /// <inheritdoc/>
    protected override FileWriter CreateWriter(FileStatement fileStatement) => fileStatement switch
    {
        global::Data.Common.FileStatements.FileDelete deleteStatement => new JsonDelete(deleteStatement, (JsonConnection)UpdateCommand!.Connection!, (FileCommand<JsonParameter>)UpdateCommand),
        FileInsert insertStatement => new JsonInsert(insertStatement, (JsonConnection)UpdateCommand!.Connection!, (FileCommand<JsonParameter>)UpdateCommand),
        FileUpdate updateStatement => new JsonUpdate(updateStatement, (JsonConnection)UpdateCommand!.Connection!, (FileCommand<JsonParameter>)UpdateCommand),

        _ => throw new InvalidOperationException("query not supported")
    };
}
