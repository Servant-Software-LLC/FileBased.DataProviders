using Data.Common.Utils;

namespace System.Data.XlsClient;

/// <summary>
/// Represents a command for XLS operations.
/// </summary>
/// <inheritdoc/>
public class XlsCommand : FileCommand<XlsParameter>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="XlsCommand"/> class.
    /// </summary>
    public XlsCommand() : base() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="XlsCommand"/> class with the specified command text.
    /// </summary>
    /// <param name="command">The text of the command.</param>
    public XlsCommand(string command) : base(command) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="XlsCommand"/> class with the specified connection.
    /// </summary>
    /// <param name="connection">The <see cref="XlsConnection"/> to use.</param>
    public XlsCommand(XlsConnection connection) : base(connection) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="XlsCommand"/> class with the specified command text and connection.
    /// </summary>
    /// <param name="cmdText">The text of the command.</param>
    /// <param name="connection">The <see cref="XlsConnection"/> to use.</param>
    public XlsCommand(string cmdText, XlsConnection connection) : base(cmdText, connection) { }

    /// <inheritdoc/>
    public override XlsParameter CreateParameter() => new();

    /// <inheritdoc/>
    public override XlsParameter CreateParameter(string parameterName, object value) => 
        value is DbType dbType ? new(parameterName, dbType) : new(parameterName, value);

    /// <inheritdoc />
    protected override FileWriter CreateWriter(FileStatement fileStatement) => fileStatement switch
    {
        FileDelete deleteStatement => throw new InvalidOperationException("The XSL provider does not support deletes."),
        FileInsert insertStatement => throw new InvalidOperationException("The XSL provider does not support inserts."),
        FileUpdate updateStatement => throw new InvalidOperationException("The XSL provider does not support updates."),
        FileCreateTable createTableStatement => throw new InvalidOperationException("The XSL provider does not support table creates."),
        FileDropColumn dropTableStatement => throw new InvalidOperationException("The XSL provider does not support table drops."),
        FileAddColumn addColumnStatement => throw new InvalidOperationException("The XSL provider does not support colunm adds."),

        _ => throw new InvalidOperationException($"Cannot create writer for query {fileStatement.GetType()}.")
    };

    /// <inheritdoc/>
    public override FileDataAdapter<XlsParameter> CreateAdapter() => throw new InvalidOperationException("The XSL provider does not support adapters.");

#if NET7_0_OR_GREATER    // .NET 7 implementation that supports covariant return types

    /// <inheritdoc />
    protected override XlsDataReader CreateDataReader(IEnumerable<FileStatement> fileStatements, LoggerServices loggerServices) =>
        new(fileStatements, FileConnection.FileReader, FileTransaction == null ? null : FileTransaction.TransactionScopedRows, CreateWriter, loggerServices);

#else       // .NET Standard 2.0 compliant implementation.

    /// <inheritdoc />
    protected override FileDataReader CreateDataReader(IEnumerable<FileStatement> fileStatements, LoggerServices loggerServices) =>
        new XlsDataReader(fileStatements, FileConnection.FileReader, FileTransaction == null ? null : FileTransaction.TransactionScopedRows, CreateWriter, loggerServices);

#endif

}
