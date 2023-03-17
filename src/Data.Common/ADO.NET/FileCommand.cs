namespace System.Data.FileClient;
public abstract class FileCommand : IDbCommand
{
    private FileParameterCollection parameters;

    public string? CommandText { get; set; } = string.Empty;
    public int CommandTimeout { get; set; }
    public CommandType CommandType { get; set; }
    public IDbConnection? Connection { get; set; }

    public IDataParameterCollection Parameters { get { return parameters; } }
    public IDbTransaction? Transaction { get; set; }
    public UpdateRowSource UpdatedRowSource { get; set; }
        
    public FileCommand()
    {
        parameters = new FileParameterCollection();
    }

    public FileCommand(string command)
    {
        CommandText = command;
        parameters = new FileParameterCollection();
    }

    public FileCommand(FileConnection connection)
    {
        Connection = connection;
        parameters = new FileParameterCollection();
    }

    public FileCommand(string cmdText, FileConnection connection)
    {
        CommandText = cmdText;
        Connection = connection;
        parameters = new FileParameterCollection();
    }

    public FileCommand(string cmdText, FileConnection connection, FileTransaction transaction)
    {
        CommandText = cmdText;
        Connection = connection;
        Transaction = transaction;
        parameters = new FileParameterCollection();
    }

    public void Cancel()
    {

    }

    public abstract IDbDataParameter CreateParameter();


    public void Dispose()
    {
        Connection?.Close();
        CommandText = string.Empty;
        parameters.Clear();
    }
    public int ExecuteNonQuery()
    {
        ThrowOnInvalidExecutionState();

        var queryParser = FileQuery.Create(this);
        if (Connection!.State != ConnectionState.Open)
        {
            throw new InvalidOperationException("Connection should be opened before executing a command.");
        }
        FileWriter jsonWriter = CreateWriter(queryParser);

        return jsonWriter.Execute();
    }

    protected abstract FileWriter CreateWriter(FileQuery queryParser);
    protected abstract FileDataReader CreateDataReader(FileQuery queryParser);

    public  IDataReader ExecuteReader(CommandBehavior behavior)
    {
        ThrowOnInvalidExecutionState();

        var queryParser = FileQuery.Create(this);

        if (Connection!.State != ConnectionState.Open)
        {
            throw new InvalidOperationException("Connection should be opened before executing a command.");
        }
        return CreateDataReader(queryParser);
    }

    public void Prepare() => throw new NotImplementedException();
    public   object? ExecuteScalar()
    {
        ThrowOnInvalidExecutionState();

        var queryParser = FileQuery.Create(this);
        if (queryParser is not FileSelectQuery selectQuery)
            throw new ArgumentException($"'{CommandText}' must be a SELECT query to call {nameof(ExecuteScalar)}");

        var columns = selectQuery.GetColumnNames();
        var reader = ((FileConnection)Connection!).FileReader ;

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
            throw new ArgumentNullException(nameof(CommandText), $"The {nameof(CommandText)} property of {nameof(FileCommand)} must be set prior to execution.");

        if (Connection == null)
            throw new ArgumentNullException(nameof(Connection), $"The {nameof(Connection)} property of {nameof(FileCommand)} must be set prior to execution.");
    }

    public IDataReader ExecuteReader() => ExecuteReader(default);
  
}