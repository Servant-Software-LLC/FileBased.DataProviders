using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Scaffolding;
using Microsoft.EntityFrameworkCore.Scaffolding.Metadata;
using Microsoft.Extensions.Logging;
using System.Data.Common;

namespace EFCore.Common.Scaffolding.Internal;

public abstract class FileDatabaseModelFactory : IDatabaseModelFactory
{
    protected readonly IDiagnosticsLogger<DbLoggerCategory.Scaffolding> logger;

    public FileDatabaseModelFactory(IDiagnosticsLogger<DbLoggerCategory.Scaffolding> logger)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public abstract DatabaseModel Create(string connectionString, DatabaseModelFactoryOptions options);

    public DatabaseModel Create(DbConnection connection, DatabaseModelFactoryOptions options)
    {
#warning Need to implement in ADO.NET FileBased Providers and verify this implementation

        var databaseModel = new DatabaseModel();

        using (var command = connection.CreateCommand())
        {
            command.CommandText = "SELECT TABLE_NAME, COLUMN_NAME, DATA_TYPE FROM INFORMATION_SCHEMA.COLUMNS";

            connection.Open();
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                var tableName = reader.GetString(0);
                var columnName = reader.GetString(1);
                var dataType = reader.GetString(2);

                // Note: Here we are making an assumption that the TABLE_NAME returned by the query is unique,
                // which may not be the case in a real-world scenario. In a real implementation, you would need to handle
                // cases where there are multiple tables with the same name, perhaps in different schemas.
                var table = databaseModel.Tables.FirstOrDefault(t => t.Name == tableName);
                if (table == null)
                {
                    table = new DatabaseTable
                    {
                        Database = databaseModel,
                        Name = tableName
                    };
                    databaseModel.Tables.Add(table);
                }

                var column = new DatabaseColumn
                {
                    Table = table,
                    Name = columnName,
                    StoreType = dataType
                };
                table.Columns.Add(column);
            }
        }

        logger.Logger.LogDebug($"Loaded {databaseModel.Tables.Count} table(s)");
        return databaseModel;
    }

}
