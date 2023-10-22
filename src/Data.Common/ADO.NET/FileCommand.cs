using Data.Common.Utils;
using Microsoft.Extensions.Logging;
using SqlBuildingBlocks.LogicalEntities;

namespace System.Data.FileClient;


/// <summary>
/// Represents a command for interacting with a file-based data store in Entity Framework Core.
/// </summary>
/// <typeparam name="TFileParameter">The type of the file parameter.</typeparam>
public abstract class FileCommand<TFileParameter> : DbCommand, IFileCommand
    where TFileParameter : FileParameter<TFileParameter>, new()
{
    private ILogger<FileCommand<TFileParameter>> log => FileConnection.LoggerServices.CreateLogger<FileCommand<TFileParameter>>();

    /// <inheritdoc/>
    public override string? CommandText { get; set; } = string.Empty;
    /// <inheritdoc/>
    public override int CommandTimeout { get; set; }
    /// <inheritdoc/>
    public override CommandType CommandType { get; set; }

    /// <inheritdoc/>
    public override UpdateRowSource UpdatedRowSource { get; set; }

    /// <inheritdoc/>
    protected override DbParameterCollection DbParameterCollection { get; } = new FileParameterCollection<TFileParameter>();

    /// <summary>
    /// Gets or sets the file connection for this command.
    /// </summary>
    public FileConnection<TFileParameter> FileConnection { get; private set; }
    IFileConnection IFileCommand.FileConnection => FileConnection;

    /// <summary>
    /// Gets or sets the file transaction for this command.
    /// </summary>
    public FileTransaction<TFileParameter> FileTransaction { get; private set; }
    IFileTransaction IFileCommand.FileTransaction => FileTransaction;

    /// <inheritdoc/>
    protected override DbConnection DbConnection
    {
        get => FileConnection;
        set
        {
            if (value is null)
            {
                FileConnection = null;
                return;
            }

            if (value is not FileConnection<TFileParameter> fileConnection)
                throw new ArgumentOutOfRangeException(nameof(value), $"{GetType()} must be a type that inherits {typeof(FileConnection<TFileParameter>)}");

            FileConnection = fileConnection;
        }
    }

    /// <inheritdoc/>
    protected override DbTransaction DbTransaction
    {
        get => FileTransaction;
        set
        {
            if (value is null)
            {
                FileTransaction = null;
                return;
            }

            if (value is not FileTransaction<TFileParameter> fileTransaction)
                throw new ArgumentOutOfRangeException(nameof(value), $"{GetType()} must be a type that inherits {typeof(FileTransaction<TFileParameter>)}");

            FileTransaction = fileTransaction;
        }
    }


    /// <summary>
    /// Initializes a new instance of the <see cref="FileCommand{TFileParameter}"/> class.
    /// </summary>
    public FileCommand()
        : this(null, null, null) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="FileCommand{TFileParameter}"/> class with the specified command text.
    /// </summary>
    /// <param name="command">The command text.</param>
    public FileCommand(string command)
        : this(command, null, null) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="FileCommand{TFileParameter}"/> class with the specified file connection.
    /// </summary>
    /// <param name="connection">The file connection.</param>
    public FileCommand(FileConnection<TFileParameter> connection)
        : this(null, connection, null) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="FileCommand{TFileParameter}"/> class with the specified command text and file connection.
    /// </summary>
    /// <param name="cmdText">The command text.</param>
    /// <param name="connection">The file connection.</param>
    public FileCommand(string cmdText, FileConnection<TFileParameter> connection)
        : this(cmdText, connection, null) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="FileCommand{TFileParameter}"/> class with the specified command text, file connection, and file transaction.
    /// </summary>
    /// <param name="cmdText">The command text.</param>
    /// <param name="connection">The file connection.</param>
    /// <param name="transaction">The file transaction.</param>
    public FileCommand(string cmdText, FileConnection<TFileParameter> connection, FileTransaction<TFileParameter> transaction)
    {
        if (cmdText != null) CommandText = cmdText;
        if (connection != null) FileConnection = connection;
        if (transaction != null) FileTransaction = transaction;
    }

    /// <inheritdoc/>
    public override void Cancel()
    {
        log.LogDebug($"{GetType()}.{nameof(Cancel)} () called.");
    }

    /// <summary>
    /// Gets or sets a value indicating whether the command is visible at design time.
    /// </summary>
    public override bool DesignTimeVisible { get; set; }

    /// <summary>
    /// Creates a new instance of a file parameter.
    /// </summary>
    /// <returns>A file parameter.</returns>
    public abstract TFileParameter CreateParameter();

    /// <summary>
    /// Creates a new instance of a file parameter with the specified name and value.
    /// </summary>
    /// <param name="parameterName">The name of the parameter.</param>
    /// <param name="value">The value of the parameter.</param>
    /// <returns>A file parameter.</returns>
    public abstract TFileParameter CreateParameter(string parameterName, object value);

    /// <inheritdoc/>
    protected override DbParameter CreateDbParameter() => CreateParameter();

    /// <summary>
    /// Creates a new instance of a file data adapter.
    /// </summary>
    /// <returns>A file data adapter.</returns>
    public abstract FileDataAdapter<TFileParameter> CreateAdapter();


    /// <inheritdoc/>
    public void Dispose()
    {
        log.LogInformation($"{GetType()}.{nameof(Dispose)}() called.  CommandText = {CommandText}");

        FileConnection?.Close();
        CommandText = string.Empty;
        Parameters.Clear();
    }

    /// <inheritdoc/>
    public override int ExecuteNonQuery()
    {
        log.LogInformation($"{GetType()}.{nameof(ExecuteNonQuery)}() called.  CommandText = {CommandText}");

        ThrowOnInvalidExecutionState();

        if (FileConnection!.State != ConnectionState.Open)
        {
            throw new InvalidOperationException("Connection should be opened before executing a command.");
        }

        if (FileConnection.AdminMode)
            return ExecuteAdminNonQuery();

        var fileStatement = FileStatementCreator.Create(this, log);

        var fileWriter = CreateWriter(fileStatement);

        return fileWriter.Execute();
    }

    private int ExecuteAdminNonQuery()
    {
        var fileAdminStatement = FileAdminStatement<TFileParameter>.Create(this);
        return fileAdminStatement.Execute() ? 1 : 0;
    }

    /// <summary>
    /// Creates a new file writer for the specified file statement.
    /// </summary>
    /// <param name="fileStatement">The file statement.</param>
    /// <returns>A file writer.</returns>
    protected abstract FileWriter CreateWriter(FileStatement fileStatement);

    /// <summary>
    /// Creates a new file data reader for the specified file statements and logger services.
    /// </summary>
    /// <param name="fileStatements">The file statements.</param>
    /// <param name="loggerServices">The logger services.</param>
    /// <returns>A file data reader.</returns>
    protected abstract FileDataReader CreateDataReader(IEnumerable<FileStatement> fileStatements, LoggerServices loggerServices);

    /// <inheritdoc/>
    protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
    {
        log.LogInformation($"{GetType()}.{nameof(ExecuteDbDataReader)}() called.  CommandText = {CommandText}");

        ThrowOnInvalidExecutionState();

        if (FileConnection.AdminMode)
            throw new ArgumentException($"The {nameof(ExecuteDbDataReader)} method cannot be used with an admin connection.");

        //When calling ExecuteDbDataReader, the CommandText property of a DbCommand can contain
        //multiple commands separated by semicolons
        var fileStatements = FileStatementCreator.CreateMultiCommandSupport(this, log);

        ProvideColumnNameHints(fileStatements);

        if (FileConnection!.State != ConnectionState.Open)
        {
            throw new InvalidOperationException("Connection should be opened before executing a command.");
        }

        return CreateDataReader(fileStatements, FileConnection.LoggerServices);
    }

    /// <inheritdoc/>
    public override void Prepare()
    {
        log.LogDebug($"{GetType()}.{nameof(Prepare)} () called.");
    }


    /// <inheritdoc/>
    public override object? ExecuteScalar()
    {
        log.LogInformation($"{GetType()}.{nameof(ExecuteScalar)}() called.  CommandText = {CommandText}");

        ThrowOnInvalidExecutionState();

        if (FileConnection.AdminMode)
            throw new ArgumentException($"The {nameof(ExecuteScalar)} method cannot be used with an admin connection.");

        //ExecuteScalar method of a class deriving from DbCommand is designed to execute a single command and
        //return the scalar value from the first column of the first row of the result set. It is not intended
        //to process multiple commands or handle multiple result sets.
        var fileStatement = FileStatementCreator.Create(this, log);
        if (fileStatement is not FileSelect selectQuery)
            throw new ArgumentException($"'{CommandText}' must be a SELECT query to call {nameof(ExecuteScalar)}");


        var columns = selectQuery.Columns.Select(col => ((SqlColumn)col).ColumnName);
        var reader = FileConnection!.FileReader;

        var transactionScopedRows = FileTransaction == null ? null : FileTransaction.TransactionScopedRows;
        var dataTable = reader.ReadFile(fileStatement, transactionScopedRows, true);

        object? result = null;

        //SELECT query - Per https://learn.microsoft.com/en-us/dotnet/api/system.data.idbcommand.executescalar?view=net-7.0#definition
        //       "Executes the query, and returns the first column of the first row in the resultset returned
        //       by the query. Extra columns or rows are ignored."
        if (dataTable.Rows.Count > 0)
        {
            var rowValues = dataTable.Rows[0].ItemArray;
            if (rowValues.Length > 0)
            {
                result = rowValues[0];
            }
        }

        return result;
    }

    /// <summary>
    /// Provides column name hints for the specified file statements.
    /// </summary>
    /// <remarks>
    /// If we have an INSERT statement followed by SELECT statements to the same table, then
    /// the SELECT statement can be used to determine columns that should be part of the schema
    /// of the table (in the case of the JSON provider where columns are only determined by rows
    /// in the table).  This is especially necessary to get 'right' when a column is the identity
    /// column.  EF Core provider has this as a common scenario with SQL statements together like
    /// so:
    ///   INSERT INTO Blogs(Url) VALUES (@p0); SELECT BlogId FROM Blogs WHERE ROW_COUNT() = 1 AND BlogId=LAST_INSERT_ID();
    /// 
    /// Here, if we look ahead to the SELECT statement, we can infer that there is not only a Url column, but also 
    /// a BlogId column that is an identity.
    /// </remarks>
    /// <param name="fileStatements">The file statements.</param>
    private void ProvideColumnNameHints(IList<FileStatement> fileStatements)
    {
        for (int iInsert = 0; iInsert < fileStatements.Count; iInsert++)
        {
            //Look for an INSERT statement.
            if (fileStatements[iInsert] is FileInsert fileInsert)
            {
                //Now that an INSERT is found, look through the rest of the statements for any SELECTs
                for (int iSelect = iInsert + 1; iSelect < fileStatements.Count; iSelect++)
                {
                    //If there is a future SELECT in the statements for the same table as this INSERT..
                    if (fileStatements[iSelect] is FileSelect fileSelect &&
                        fileSelect.FromTable == fileInsert.FromTable &&
                        fileSelect.Joins.Count == 0)
                    {
                        //Provide column name hints to be used for the JSON provider, because when a table has no data
                        //in it to start out with, then it doesn't know the schema of its table columns.
                        foreach (var column in fileSelect.Columns.OfType<SqlColumn>().Select(col => col.ColumnName))
                        {
                            fileInsert.ColumnNameHints.Add(column);
                        }
                    }
                }
            }
        }
    }


    private void ThrowOnInvalidExecutionState()
    {
        if (string.IsNullOrEmpty(CommandText))
            throw new ArgumentNullException(nameof(CommandText), $"The {nameof(CommandText)} property of {GetType().FullName} must be set prior to execution.");

        if (FileConnection == null)
            throw new ArgumentNullException(nameof(Connection), $"The {nameof(Connection)} property of {GetType().FullName} must be set prior to execution.");
    }

}
