namespace System.Data.FileClient;

public abstract class FileCommand<TFileParameter> : DbCommand
    where TFileParameter : FileParameter<TFileParameter>, new()
{
    public override string? CommandText { get; set; } = string.Empty;
    public override int CommandTimeout { get; set; }
    public override CommandType CommandType { get; set; }

    public override UpdateRowSource UpdatedRowSource { get; set; }

    protected override DbParameterCollection DbParameterCollection { get; } = new FileParameterCollection<TFileParameter>();
    protected override DbConnection DbConnection { get; set; }
    protected override DbTransaction DbTransaction { get; set; }


    public FileCommand()
    {
    }

    public FileCommand(string command)
    {
        CommandText = command;
    }

    public FileCommand(FileConnection<TFileParameter> connection)
    {
        Connection = connection;
    }

    public FileCommand(string cmdText, FileConnection<TFileParameter> connection)
    {
        CommandText = cmdText;
        Connection = connection;
    }

    public FileCommand(string cmdText, FileConnection<TFileParameter> connection, FileTransaction<TFileParameter> transaction)
    {
        CommandText = cmdText;
        Connection = connection;
        Transaction = transaction;
    }

    public override void Cancel()
    {

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
        Connection?.Close();
        CommandText = string.Empty;
        Parameters.Clear();
    }

    public override int ExecuteNonQuery()
    {
        ThrowOnInvalidExecutionState();

        var queryParser = FileQuery<TFileParameter>.Create(this);
        if (Connection!.State != ConnectionState.Open)
        {
            throw new InvalidOperationException("Connection should be opened before executing a command.");
        }
        FileWriter<TFileParameter> fileWriter = CreateWriter(queryParser);

        return fileWriter.Execute();
    }

    protected abstract FileWriter<TFileParameter> CreateWriter(FileQuery<TFileParameter> queryParser);
    protected abstract FileDataReader<TFileParameter> CreateDataReader(FileQuery<TFileParameter> queryParser);

    /// <summary>
    /// Executes the command text against the connection.
    /// </summary>
    /// <returns>A task representing the operation.</returns>
    protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
    {
        ThrowOnInvalidExecutionState();

        var queryParser = FileQuery<TFileParameter>.Create(this);

        if (Connection!.State != ConnectionState.Open)
        {
            throw new InvalidOperationException("Connection should be opened before executing a command.");
        }
        return CreateDataReader(queryParser);
    }

    public override void Prepare() => throw new NotImplementedException();

    public override object? ExecuteScalar()
    {
        ThrowOnInvalidExecutionState();

        var queryParser = FileQuery<TFileParameter>.Create(this);
        if (queryParser is not FileSelectQuery<TFileParameter> selectQuery)
            throw new ArgumentException($"'{CommandText}' must be a SELECT query to call {nameof(ExecuteScalar)}");

        var columns = selectQuery.GetColumnNames();
        var reader = ((FileConnection<TFileParameter>)Connection!).FileReader ;

        var dataTable = reader.ReadFile(queryParser, true);
        var dataView = new DataView(dataTable);

        if (queryParser.Filter!=null)
            dataView.RowFilter = queryParser.Filter.Evaluate();

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

        if (Connection == null)
            throw new ArgumentNullException(nameof(Connection), $"The {nameof(Connection)} property of {GetType().FullName} must be set prior to execution.");
    }

}