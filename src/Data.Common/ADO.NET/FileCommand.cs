using Data.Common.Utils;
using Microsoft.Extensions.Logging;

namespace System.Data.FileClient;

public abstract class FileCommand<TFileParameter> : DbCommand, IFileCommand
    where TFileParameter : FileParameter<TFileParameter>, new()
{
    private ILogger<FileCommand<TFileParameter>> log => FileConnection.LoggerServices.CreateLogger<FileCommand<TFileParameter>>();

    public override string? CommandText { get; set; } = string.Empty;
    public override int CommandTimeout { get; set; }
    public override CommandType CommandType { get; set; }

    public override UpdateRowSource UpdatedRowSource { get; set; }

    protected override DbParameterCollection DbParameterCollection { get; } = new FileParameterCollection<TFileParameter>();

    public FileConnection<TFileParameter> FileConnection { get; private set; }
    IFileConnection IFileCommand.FileConnection => FileConnection;

    public FileTransaction<TFileParameter> FileTransaction { get; private set; }
    IFileTransaction IFileCommand.FileTransaction => FileTransaction;

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


    public FileCommand()
        : this(null, null, null) { }

    public FileCommand(string command)
        : this(command, null, null) { }

    public FileCommand(FileConnection<TFileParameter> connection)
        : this(null, connection, null) { }

    public FileCommand(string cmdText, FileConnection<TFileParameter> connection)
        :this(cmdText, connection, null) { }

    public FileCommand(string cmdText, FileConnection<TFileParameter> connection, FileTransaction<TFileParameter> transaction)
    {
        if (cmdText != null) CommandText = cmdText;
        if (connection != null) FileConnection = connection;
        if (transaction != null) FileTransaction = transaction;
    }

    public override void Cancel()
    {
        log.LogDebug($"{GetType()}.{nameof(Cancel)} () called.");
    }

    /// <summary>
    /// Design time visible.
    /// </summary>
    public override bool DesignTimeVisible { get; set; }

    public abstract TFileParameter CreateParameter();
    public abstract TFileParameter CreateParameter(string parameterName, object value);

    /// <summary>
    /// Creates a new instance of an <see cref="System.Data.Common.DbParameter"/> object.
    /// </summary>
    /// <returns>A <see cref="System.Data.Common.DbParameter"/> object.</returns>
    protected override DbParameter CreateDbParameter() => CreateParameter();

    public abstract FileDataAdapter<TFileParameter> CreateAdapter();


    public void Dispose()
    {
        log.LogInformation($"{GetType()}.{nameof(Dispose)}() called.  CommandText = {CommandText}");

        FileConnection?.Close();
        CommandText = string.Empty;
        Parameters.Clear();
    }

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

    protected abstract FileWriter CreateWriter(FileStatement fileStatement);
    protected abstract FileDataReader CreateDataReader(IEnumerable<FileStatement> fileStatements, LoggerServices loggerServices);

    /// <summary>
    /// Executes the command text against the connection.
    /// </summary>
    /// <returns>A task representing the operation.</returns>
    protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
    {
        log.LogInformation($"{GetType()}.{nameof(ExecuteDbDataReader)}() called.  CommandText = {CommandText}");

        ThrowOnInvalidExecutionState();

        if (FileConnection.AdminMode)
            throw new ArgumentException($"The {nameof(ExecuteDbDataReader)} method cannot be used with an admin connection.");

        //When calling ExecuteDbDataReader, the CommandText property of a DbCommand can contain
        //multiple commands separated by semicolons
        var fileStatements = FileStatementCreator.CreateMultiCommandSupport(this, log);

        if (FileConnection!.State != ConnectionState.Open)
        {
            throw new InvalidOperationException("Connection should be opened before executing a command.");
        }

        return CreateDataReader(fileStatements, FileConnection.LoggerServices);
    }

    public override void Prepare()
    {
        log.LogDebug($"{GetType()}.{nameof(Prepare)} () called.");
    }


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

        var columns = selectQuery.GetColumnNames();
        var reader = FileConnection!.FileReader ;

        var dataTable = reader.ReadFile(fileStatement, true);
        var dataView = new DataView(dataTable);

        if (fileStatement.Filter!=null)
            dataView.RowFilter = fileStatement.Filter.Evaluate();

        object? result = null;
        
        //If COUNT(*) query
        if (selectQuery.IsCountQuery)
            return dataView.Count;


        //SELECT query - Per https://learn.microsoft.com/en-us/dotnet/api/system.data.idbcommand.executescalar?view=net-7.0#definition
        //       "Executes the query, and returns the first column of the first row in the resultset returned
        //       by the query. Extra columns or rows are ignored."
        if (dataView.Count > 0)
        {
            var rowValues = dataView[0].Row.ItemArray;
            var firstColumn = columns.FirstOrDefault();
            if (firstColumn != null)
            {
                var columnIndex = dataView.Table!.Columns.IndexOf(firstColumn);
                result = rowValues[columnIndex];
            }                    
        }

        return result;
    }

    private void ThrowOnInvalidExecutionState()
    {
        if (string.IsNullOrEmpty(CommandText))
            throw new ArgumentNullException(nameof(CommandText), $"The {nameof(CommandText)} property of {GetType().FullName} must be set prior to execution.");

        if (FileConnection == null)
            throw new ArgumentNullException(nameof(Connection), $"The {nameof(Connection)} property of {GetType().FullName} must be set prior to execution.");
    }

}