using Data.Json.Enum;
using Data.Json.JsonJoin;
using Data.Json.JsonQuery;
using Data.Json.New;
using Irony.Parsing;

namespace System.Data.JsonClient
{
    public class JsonConnection : IDbConnection
    {
        private string _connectionString;
        private ConnectionState _state;
        public string ConnectionString { get => _connectionString; set => _connectionString = value; }
        public int ConnectionTimeout { get; }
        public string Database => _connectionString;
        public ConnectionState State => _state;
     
        public JsonConnection(string connectionString)
        {
            ArgumentNullException.ThrowIfNull(nameof(connectionString));
            _connectionString = connectionString.Split('=')[1].TrimEnd(';');
            _state = ConnectionState.Closed;
            JsonReader = new Reader(this);
        }
        public IDbTransaction BeginTransaction()
        {
            throw new NotImplementedException();
        }
        public IDbTransaction BeginTransaction(IsolationLevel il)
        {
            throw new NotImplementedException();
        }
        public void ChangeDatabase(string databaseName)
        {
            PathType = this.GetPathType();
            ArgumentNullException.ThrowIfNull(nameof(databaseName));
            ThrowHelper.ThrowIfInvalidPath(PathType);
            JsonReader = new Reader(this);
            JsonReader.Dispose();
        }
        public void Close()
        {
            _state = ConnectionState.Closed;

        }

        internal Reader JsonReader { get; private set; }
        public IDbCommand CreateCommand()
        {
          return new JsonCommand(this);
        }
       internal PathType PathType { get; private set; }
        public void Open()
        {
            PathType = this.GetPathType();
            ThrowHelper.ThrowIfInvalidPath(PathType);
            _state = ConnectionState.Open;
        }
        public void Dispose()
        {
            _state = ConnectionState.Closed;
            JsonReader.Dispose();
            
        }
    }
}