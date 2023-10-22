using Microsoft.EntityFrameworkCore.Storage;
using System.Data.Common;
using System.Data.CsvClient;

namespace EFCore.Csv.Storage.Internal;

public class CsvRelationalConnection : RelationalConnection, ICsvRelationalConnection
{
    public CsvRelationalConnection(RelationalConnectionDependencies dependencies)
        : base(dependencies)
    {
    }

    protected override DbConnection CreateDbConnection() => new CsvConnection(ConnectionString);
}
