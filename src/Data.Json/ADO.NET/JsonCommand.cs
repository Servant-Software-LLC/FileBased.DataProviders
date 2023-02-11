using Data.Json.JsonQuery;

namespace System.Data.JsonClient;

public class JsonCommand : IDbCommand
{
    private JsonParameterCollection parameters;

    public string? CommandText { get; set; } = string.Empty;
    public int CommandTimeout { get; set; }
    public CommandType CommandType { get; set; }
    IDbConnection? IDbCommand.Connection { get { return Connection; } set { Connection = value as JsonConnection; } }
    public JsonConnection? Connection { get; set; }

    public IDataParameterCollection Parameters { get { return parameters; } }
    public IDbTransaction? Transaction { get; set; }
    public UpdateRowSource UpdatedRowSource { get; set; }
        
    public JsonCommand()
    {
        parameters = new JsonParameterCollection();
    }

    public JsonCommand(string command)
    {
        CommandText = command;
        parameters = new JsonParameterCollection();
    }

    public JsonCommand(JsonConnection connection)
    {
        Connection = connection;
        parameters = new JsonParameterCollection();
    }

    public JsonCommand(string cmdText, JsonConnection connection)
    {
        CommandText = cmdText;
        Connection = connection;
        parameters = new JsonParameterCollection();
    }

    public JsonCommand(string cmdText, JsonConnection connection, JsonTransaction transaction)
    {
        CommandText = cmdText;
        Connection = connection;
        Transaction = transaction;
        parameters = new JsonParameterCollection();
    }

    public void Cancel()
    {

    }

    public IDbDataParameter CreateParameter()
    {
        return new JsonParameter();
    }

    public void Dispose()
    {
        Connection?.Close();
        CommandText = string.Empty;
        parameters.Clear();
    }

    public int ExecuteNonQuery()
    {
        ThrowOnInvalidExecutionState();

        var queryParser = JsonQueryParser.Create(CommandText!);
        if (Connection!.State != ConnectionState.Open)
        {
            throw new InvalidOperationException("Connection should be opened before executing a command.");
        }

        JsonWriter jsonWriter = queryParser.GetJsonWriter(Connection!);

        return jsonWriter.Execute();
    }

    public IDataReader ExecuteReader() => ExecuteReader(CommandBehavior.Default);

    public IDataReader ExecuteReader(CommandBehavior behavior)
    {
        ThrowOnInvalidExecutionState();

        var queryParser = JsonQueryParser.Create(CommandText!);

        if (Connection!.State != ConnectionState.Open)
        {
            throw new InvalidOperationException("Connection should be opened before executing a command.");
        }

        return new JsonDataReader(queryParser, Connection.JsonReader);
    }

    public void Prepare() => throw new NotImplementedException();

    public object? ExecuteScalar()
    {
        ThrowOnInvalidExecutionState();

        var queryParser = JsonQueryParser.Create(CommandText!);
        if (queryParser is not JsonSelectQuery selectQuery)
            throw new ArgumentException($"'{CommandText}' must be a SELECT query to call {nameof(ExecuteScalar)}");

        var columns = selectQuery.GetColumnNames(); 
        var reader = Connection!.JsonReader;

        var dataTable = reader.ReadJson(queryParser, true);
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
            throw new ArgumentNullException(nameof(CommandText), $"The {nameof(CommandText)} property of {nameof(JsonCommand)} must be set prior to execution.");

        if (Connection == null)
            throw new ArgumentNullException(nameof(Connection), $"The {nameof(Connection)} property of {nameof(JsonCommand)} must be set prior to execution.");
    }
}