using EFCore.Common.Scaffolding.Internal;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Scaffolding.Metadata;
using Microsoft.EntityFrameworkCore.Scaffolding;
using System.Data.JsonClient;

namespace EFCore.Json.Scaffolding.Internal;

public class JsonDatabaseModelFactory : FileDatabaseModelFactory
{
    public JsonDatabaseModelFactory(IDiagnosticsLogger<DbLoggerCategory.Scaffolding> logger)
        : base(logger)
    {

    }

    public override DatabaseModel Create(string connectionString, DatabaseModelFactoryOptions options)
    {
        using var connection = new JsonConnection(connectionString);
        return Create(connection, options);
    }

}
