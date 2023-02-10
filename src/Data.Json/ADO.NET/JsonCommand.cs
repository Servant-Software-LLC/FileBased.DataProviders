using Data.Json.JsonQuery;
using System.Management;
using System.Net.Http.Headers;
using Utilities;

namespace System.Data.JsonClient
{
    public class JsonCommand : IDbCommand
    {
        private JsonConnection _connection;
        private string _commandText;
        private CommandType _commandType;
        private JsonParameterCollection _parameters;

        public string CommandText { get => _commandText; set => _commandText = value; }
        public int CommandTimeout { get; set; }
        public CommandType CommandType { get { return _commandType; } set { _commandType = value; } }
        public IDbConnection Connection { get { return _connection; } set { _connection = (JsonConnection)value; } }

        public IDataParameterCollection Parameters { get { return _parameters; } }
        public IDbTransaction Transaction { get; set; }
        public UpdateRowSource UpdatedRowSource { get; set; }
        JsonQueryParser _queryParser;
        internal JsonQueryParser QueryParser
        {
            get { return _queryParser; }
        }
        public JsonCommand()
        {
            _parameters = new JsonParameterCollection();
        }

        public JsonCommand(string command)
        {
            _commandText = command;
            _parameters = new JsonParameterCollection();
        }
        public JsonCommand(JsonConnection connection)
        {
            _connection = connection;
            _parameters = new JsonParameterCollection();
        }
        public JsonCommand(string cmdText, JsonConnection connection)
        {
            _commandText = cmdText;
            _connection = connection;
            _parameters = new JsonParameterCollection();
        }

        public JsonCommand(string cmdText, JsonConnection connection, JsonTransaction transaction)
        {
            _commandText = cmdText;
            _connection = connection;
            Transaction = transaction;
            _parameters = new JsonParameterCollection();
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
            _connection.Close();
            _commandText = string.Empty;
            _parameters.Clear();
        }
        public int ExecuteNonQuery()
        {
            _queryParser = JsonQueryParser.Create(this.CommandText);
            if (_connection.State != ConnectionState.Open)
            {
                throw new InvalidOperationException("Connection should be opened before executing a command.");
            }
            JsonWriter jsonWriter = QueryParser.IsInsertQuery ?
                new JsonInsert(this, (JsonConnection)Connection)
                : QueryParser.IsUpdateQuery ?
                new JsonUpdate(this, (JsonConnection)Connection)
                :
                new JsonDelete(this, (JsonConnection)Connection);

            var result= jsonWriter.Execute();
            if (result==0)
            {

            }
            return result;
        }

        public IDataReader ExecuteReader()
        {
            return ExecuteReader(CommandBehavior.Default);
        }
        public IDataReader ExecuteReader(CommandBehavior behavior)
        {
            _queryParser = JsonQueryParser.Create(this.CommandText);

            if (_connection.State != ConnectionState.Open)
            {
                throw new InvalidOperationException("Connection should be opened before executing a command.");
            }
            return new JsonDataReader(this, Connection);

        }
        public void Prepare()
        {
            throw new NotImplementedException();
        }

        public object? ExecuteScalar()
        {
            _queryParser = JsonQueryParser.Create(this.CommandText);
            var selectQuery = (JsonSelectQuery)QueryParser;
            var col = selectQuery.GetColumns();
            var reader = _connection.JsonReader;
            _connection.JsonReader.JsonQueryParser = QueryParser;
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
    }
}