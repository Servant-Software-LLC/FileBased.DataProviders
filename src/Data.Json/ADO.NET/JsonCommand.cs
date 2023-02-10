using Data.Json.JsonQuery;

namespace System.Data.JsonClient;

public class JsonCommand : IDbCommand
{
    private JsonParameterCollection parameters;
    internal JsonQueryParser? QueryParser { get; private set; }

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

        QueryParser = JsonQueryParser.Create(CommandText!);
        if (Connection!.State != ConnectionState.Open)
        {
            throw new InvalidOperationException("Connection should be opened before executing a command.");
        }

        JsonWriter jsonWriter = QueryParser.IsInsertQuery ? new JsonInsert(this, Connection)
                              : QueryParser.IsUpdateQuery ? new JsonUpdate(this, Connection)
                              : new JsonDelete(this, Connection);

        return jsonWriter.Execute();
    }

    public IDataReader ExecuteReader() => ExecuteReader(CommandBehavior.Default);

    public IDataReader ExecuteReader(CommandBehavior behavior)
    {
        ThrowOnInvalidExecutionState();

        QueryParser = JsonQueryParser.Create(CommandText!);

        if (Connection!.State != ConnectionState.Open)
        {
            throw new InvalidOperationException("Connection should be opened before executing a command.");
        }

        return new JsonDataReader(this, Connection);
    }

    public void Prepare() => throw new NotImplementedException();

    public object? ExecuteScalar()
    {
        ThrowOnInvalidExecutionState();

        QueryParser = JsonQueryParser.Create(CommandText!);
        var selectQuery = (JsonSelectQuery)QueryParser;
        var reader = Connection!.JsonReader;
        Connection.JsonReader.JsonQueryParser = QueryParser;
        reader.ReadJson(true);
    
        if (QueryParser.Filter!=null)
        reader.DataSet!.Tables[selectQuery.Table!]!.DefaultView.RowFilter = QueryParser.Filter.Evaluate();

        object? result = null;
        if (selectQuery.IsCountQuery)
        {
            result = reader.DataSet!.Tables[selectQuery.Table!]!.DefaultView.Count;
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