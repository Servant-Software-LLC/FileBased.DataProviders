namespace System.Data.CsvClient;
/// <summary>
/// Represents a DataAdapter for CSV operations.
/// </summary>
/// <inheritdoc/>
public class CsvDataAdapter : FileDataAdapter<CsvParameter>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CsvDataAdapter"/> class.
    /// </summary>
    public CsvDataAdapter()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CsvDataAdapter"/> class with the specified select command.
    /// </summary>
    /// <param name="selectCommand">The select command to use for retrieving data.</param>
    public CsvDataAdapter(CsvCommand selectCommand) : base(selectCommand)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CsvDataAdapter"/> class with the specified query and connection.
    /// </summary>
    /// <param name="query">The query to use for retrieving data.</param>
    /// <param name="connection">The connection to use for retrieving and saving data.</param>
    public CsvDataAdapter(string query, CsvConnection connection) : base(query, connection)
    {
    }

    /// <inheritdoc/>
    protected override FileWriter CreateWriter(FileStatement fileStatement) => fileStatement switch
    {
        global::Data.Common.FileStatements.FileDelete deleteStatement =>
            new CsvDelete(deleteStatement, (CsvConnection)UpdateCommand!.Connection!, (FileCommand<CsvParameter>)UpdateCommand),
        global::Data.Common.FileStatements.FileInsert insertStatement =>
            new CsvInsert(insertStatement, (CsvConnection)UpdateCommand!.Connection!, (FileCommand<CsvParameter>)UpdateCommand),
        global::Data.Common.FileStatements.FileUpdate updateStatement =>
            new CsvUpdate(updateStatement, (CsvConnection)UpdateCommand!.Connection!, (FileCommand<CsvParameter>)UpdateCommand),

        _ => throw new InvalidOperationException("query not supported")
    };
}
