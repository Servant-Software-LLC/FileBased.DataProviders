using Microsoft.EntityFrameworkCore.Scaffolding.Metadata;
using Microsoft.EntityFrameworkCore.Scaffolding;
using System.Data.CsvClient;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using EFCore.Common.Scaffolding.Internal;

namespace EFCore.Csv.Scaffolding.Internal;

public class CsvDatabaseModelFactory : FileDatabaseModelFactory
{
    public CsvDatabaseModelFactory(IDiagnosticsLogger<DbLoggerCategory.Scaffolding> logger)
        : base(logger)
    {
    }

    public override DatabaseModel Create(string connectionString, DatabaseModelFactoryOptions options)
    {
        using var connection = new CsvConnection(connectionString);
        return Create(connection, options);
    }

}