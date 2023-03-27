using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data.Common;

namespace EFCore.Json.Storage.Internal;

public class JsonRelationalConnection : RelationalConnection, IJsonRelationalConnection
{
    public JsonRelationalConnection(
        RelationalConnectionDependencies dependencies,
        IRawSqlCommandBuilder rawSqlCommandBuilder,
        IDiagnosticsLogger<DbLoggerCategory.Infrastructure> logger)
        : base(dependencies)
    {
    }

    //TODO: Waiting for the Json Data Provider to be implemented in https://github.com/Servant-Software-LLC/ADO.NET.FileBased.DataProviders
    protected override DbConnection CreateDbConnection() => throw new NotImplementedException();
//        => new JsonConnection(ConnectionString);

}
