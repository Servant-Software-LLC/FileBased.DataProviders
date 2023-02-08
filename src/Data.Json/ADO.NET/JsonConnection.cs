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
        private JsonDocument? _database;
        public string ConnectionString { get => _connectionString; set => _connectionString = value; }
        public int ConnectionTimeout { get; }
        public string Database => _connectionString;
        public ConnectionState State => _state;
     
        public JsonConnection(string connectionString)
        {
            //var query = "SELECT c.CustomerName, [o].[OrderDate], [oi].[Quantity], [p].[ProductName] FROM [Customers c] INNER JOIN [Orders o] ON [c].[ID] = [o].[CustomerID] INNER JOIN [Order_Items oi] ON [o].[ID] = [oi].[OrderID] INNER JOIN [Products p] ON [p].[ID] = [c].[ProductID] where id>8;";
            //var fg= query.Substring(96, (query.Length - 96));
            //var parser = new Parser(new JsonGrammar());
            //var parseTree = parser.Parse(query);
            //if (parseTree.HasErrors())
            //{
            //    ThrowHelper.ThrowSyntaxtErrorException(string.Join(Environment.NewLine, parseTree.ParserMessages));
            //}
            //var mainNode = parseTree.Root.ChildNodes[0];

            //var selectQuery = new JsonSelectQuery(mainNode);
            ////// Act
            //var table = selectQuery.GetTable();
            //var col = selectQuery.GetColumns();
            //var f = selectQuery.GetFilter();

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
            using var file = File.OpenRead(databaseName);
            _database = JsonDocument.Parse(file);
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
        void IDisposable.Dispose()
        {
            _state = ConnectionState.Closed;
            _database?.Dispose();
            JsonReader.Dispose();
            
        }
    }
}