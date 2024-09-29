using Data.Common.Utils;
using Data.Json.JsonIO.SchemaAltering;

namespace System.Data.JsonClient;


/// <summary>
/// Represents a command for JSON operations.
/// </summary>
public class JsonCommand : FileCommand<JsonParameter>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="JsonCommand"/> class.
    /// </summary>
    public JsonCommand() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="JsonCommand"/> class with the specified command text.
    /// </summary>
    /// <param name="command">The text of the query.</param>
    public JsonCommand(string command) : base(command) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="JsonCommand"/> class with the specified connection.
    /// </summary>
    /// <param name="connection">The connection to the data source.</param>
    public JsonCommand(JsonConnection connection) : base(connection) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="JsonCommand"/> class with the specified command text and connection.
    /// </summary>
    /// <param name="cmdText">The text of the query.</param>
    /// <param name="connection">The connection to the data source.</param>
    public JsonCommand(string cmdText, JsonConnection connection) : base(cmdText, connection) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="JsonCommand"/> class with the specified command text, connection, and transaction.
    /// </summary>
    /// <param name="cmdText">The text of the query.</param>
    /// <param name="connection">The connection to the data source.</param>
    /// <param name="transaction">The transaction in which the <see cref="JsonCommand"/> participates.</param>
    public JsonCommand(string cmdText, JsonConnection connection, JsonTransaction transaction)
        : base(cmdText, connection, transaction)
    {
    }

    /// <inheritdoc/>
    public override JsonParameter CreateParameter() => new();

    /// <inheritdoc/>
    public override JsonParameter CreateParameter(string parameterName, object value) => 
        value is DbType dbType ? new(parameterName, dbType) : new(parameterName, value);

    /// <inheritdoc/>
    protected override FileWriter CreateWriter(FileStatement fileStatement) => fileStatement switch
    {
        FileDelete deleteStatement => new JsonDelete(deleteStatement, (JsonConnection)Connection!, this),
        FileInsert insertStatement => new JsonInsert(insertStatement, (JsonConnection)Connection!, this),
        FileUpdate updateStatement => new JsonUpdate(updateStatement, (JsonConnection)Connection!, this),
        FileCreateTable createTableStatement => new JsonCreateTable(createTableStatement, (JsonConnection)Connection!, this),
        FileDropColumn dropTableStatement => new JsonDropColumn(dropTableStatement, (JsonConnection)Connection!, this),
        FileAddColumn addColumnStatement => new JsonAddColumn(addColumnStatement, (JsonConnection)Connection!, this),

        _ => throw new InvalidOperationException($"Cannot create writer for query {fileStatement.GetType()}.")
    };

#if NET7_0_OR_GREATER    // .NET 7 implementation that supports covariant return types

    /// <inheritdoc/>
    public override JsonDataAdapter CreateAdapter() => new(this);

    /// <inheritdoc/>
    protected override JsonDataReader CreateDataReader(IEnumerable<FileStatement> fileStatements, LoggerServices loggerServices) =>
        new(fileStatements, FileConnection.FileReader, FileTransaction == null ? null : FileTransaction.TransactionScopedRows, CreateWriter, loggerServices);

#else       // .NET Standard 2.0 compliant implementation.

    /// <inheritdoc/>
    public override FileDataAdapter<JsonParameter> CreateAdapter() => new JsonDataAdapter(this);

    /// <inheritdoc/>
    protected override FileDataReader CreateDataReader(IEnumerable<FileStatement> fileStatements, LoggerServices loggerServices) =>
        new JsonDataReader(fileStatements, FileConnection.FileReader, FileTransaction == null ? null : FileTransaction.TransactionScopedRows, CreateWriter, loggerServices);

#endif
}
