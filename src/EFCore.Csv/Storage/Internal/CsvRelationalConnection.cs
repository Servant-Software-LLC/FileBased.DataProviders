using EFCore.Common.Infrastructure.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data.Common;
using System.Data.CsvClient;

namespace EFCore.Csv.Storage.Internal;

public class CsvRelationalConnection : FileRelationalConnection, ICsvRelationalConnection
{
    public CsvRelationalConnection(RelationalConnectionDependencies dependencies)
        : base(dependencies)
    {
    }

    protected override DbConnection CreateDbConnection()
    {
        var connection = new CsvConnection(ConnectionString);
        if (DataSourceProvider != null)
            connection.DataSourceProvider = DataSourceProvider;

        return connection;
    }
}
