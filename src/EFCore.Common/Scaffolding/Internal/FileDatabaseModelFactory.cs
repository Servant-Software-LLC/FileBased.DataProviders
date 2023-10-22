using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Scaffolding;
using Microsoft.EntityFrameworkCore.Scaffolding.Metadata;
using Microsoft.Extensions.Logging;
using System.Data.Common;

namespace EFCore.Common.Scaffolding.Internal;

/// <summary>
/// Provides an abstraction for scaffolding database models from files.
/// This class serves as the base class for file-based database model factories.
/// </summary>
public abstract class FileDatabaseModelFactory : IDatabaseModelFactory
{
    /// <summary>
    /// Logger instance for capturing scaffolding diagnostics.
    /// </summary>
    protected readonly IDiagnosticsLogger<DbLoggerCategory.Scaffolding> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="FileDatabaseModelFactory"/> class.
    /// </summary>
    /// <param name="logger">The logger instance to be used for logging scaffolding operations.</param>
    /// <exception cref="ArgumentNullException">Thrown when the provided logger instance is null.</exception>
    public FileDatabaseModelFactory(IDiagnosticsLogger<DbLoggerCategory.Scaffolding> logger)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Abstract method for creating a <see cref="DatabaseModel"/> using a connection string.
    /// </summary>
    /// <param name="connectionString">The connection string to the file-based database.</param>
    /// <param name="options">Options for the factory.</param>
    /// <returns>A constructed <see cref="DatabaseModel"/>.</returns>
    public abstract DatabaseModel Create(string connectionString, DatabaseModelFactoryOptions options);

    /// <summary>
    /// Creates a <see cref="DatabaseModel"/> from a given database connection.
    /// </summary>
    /// <param name="connection">The connection to the file-based database.</param>
    /// <param name="options">Options for the factory.</param>
    /// <returns>A constructed <see cref="DatabaseModel"/>.</returns>
    /// <remarks>
    /// This implementation assumes that the TABLE_NAME returned by the query is unique.
    /// In real-world scenarios, handling of multiple tables with the same name, perhaps in different schemas, would be required.
    /// </remarks>
    public DatabaseModel Create(DbConnection connection, DatabaseModelFactoryOptions options)
    {
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
