using Data.Json.Enum;

namespace System.Data.JsonClient
{
    public class JsonConnection : IDbConnection
    {
        private string _connectionString;
        private ConnectionState _state;
        private JsonDocument? _database;
        public string ConnectionString { get => _connectionString; set => _connectionString = value; }
        public int ConnectionTimeout { get; }
        public string Database => _connectionString;
        public ConnectionState State => _state;
        //public JsonConnection(JsonDocument jsonDocument)
        //   : this($"Data Source=:memory:;Json={jsonDocument}")
        //{
        //    jsonDocument.
        //}
        public JsonConnection(string connectionString)
        {
            ArgumentNullException.ThrowIfNull(nameof(connectionString));
            if (!connectionString.ToLower().Replace(" ", "").Contains("datasource="))
                ThrowHelper.ThrowInvalidConnectionStringException();
            _connectionString = connectionString.Split('=')[1].TrimEnd(';');
            _state = ConnectionState.Closed;
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
            using var file = File.OpenRead(databaseName);
            _database = JsonDocument.Parse(file);
        }
        public void Close()
        {
            _state = ConnectionState.Closed;
        }
        public IDbCommand CreateCommand()
        {
            if (_state == ConnectionState.Open)
            {
                return new JsonCommand(this);
            }
            else
            {
                throw new InvalidOperationException("Connection should be opened before creating a command.");
            }
        }
       internal static ReaderWriterLockSlim  LockSlim = new ReaderWriterLockSlim();
       internal PathType PathType { get; private set; }
        public void Open()
        {
            PathType = this.GetPathType();
            ThrowHelper.ThrowIfInvalidPath(PathType);
            //while (JsonConnection.LockSlim.RecursiveWriteCount>0)
            //{

            //}
            //LockSlim.EnterReadLock();

            using (var file =new FileStream(_connectionString,FileMode.Open,FileAccess.Read))
            {
                _database = JsonDocument.Parse(file);
            }
            LockSlim.ExitReadLock();
            _state = ConnectionState.Open;
        }
        void IDisposable.Dispose()
        {
            _state = ConnectionState.Closed;
            _database?.Dispose();
            
        }
        internal JsonDocument GetDatabase()
        {
            return _database!;
        }
    }
}