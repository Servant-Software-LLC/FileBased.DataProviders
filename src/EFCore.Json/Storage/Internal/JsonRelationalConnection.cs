using EFCore.Common.Infrastructure.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data.Common;
using System.Data.JsonClient;

namespace EFCore.Json.Storage.Internal;

public class JsonRelationalConnection : FileRelationalConnection, IJsonRelationalConnection
{
    public JsonRelationalConnection(RelationalConnectionDependencies dependencies)
        : base(dependencies)
    {
    }

    protected override DbConnection CreateDbConnection()
    {
        var connection = new JsonConnection(ConnectionString);
        if (DataSourceProvider != null)
            connection.DataSourceProvider = DataSourceProvider;

        return connection;
    }
}
