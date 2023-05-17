using Microsoft.EntityFrameworkCore.Storage;
using System.Data.Common;
using System.Data.JsonClient;

namespace EFCore.Json.Storage.Internal;

public class JsonRelationalConnection : RelationalConnection, IJsonRelationalConnection
{
    public JsonRelationalConnection(RelationalConnectionDependencies dependencies)
        : base(dependencies)
    {
    }

    protected override DbConnection CreateDbConnection() => new JsonConnection(ConnectionString);
}
