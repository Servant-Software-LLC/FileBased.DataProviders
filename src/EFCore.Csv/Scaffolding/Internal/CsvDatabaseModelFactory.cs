using Microsoft.EntityFrameworkCore.Scaffolding.Metadata;
using Microsoft.EntityFrameworkCore.Scaffolding;
using System.Data.CsvClient;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using EFCore.Common.Scaffolding.Internal;

namespace EFCore.Csv.Scaffolding.Internal;

/// <summary>
/// Represents the database model factory for scaffolding CSV data sources.
/// This class extends the <see cref="FileDatabaseModelFactory"/> and provides specific logic to scaffold CSV files.
/// </summary>
public class CsvDatabaseModelFactory : FileDatabaseModelFactory
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CsvDatabaseModelFactory"/> class.
    /// </summary>
    /// <param name="logger">The logger instance to be used for logging scaffolding operations.</param>
    public CsvDatabaseModelFactory(IDiagnosticsLogger<DbLoggerCategory.Scaffolding> logger)
        : base(logger)
    {
    }

    /// <summary>
    /// Creates a <see cref="DatabaseModel"/> from a given connection string specific to CSV data source.
    /// </summary>
    /// <param name="connectionString">The connection string to the CSV source.</param>
    /// <param name="options">Options for the factory.</param>
    /// <returns>A constructed <see cref="DatabaseModel"/> based on the provided CSV connection string.</returns>
    public override DatabaseModel Create(string connectionString, DatabaseModelFactoryOptions options)
    {
        using var connection = new CsvConnection(connectionString);
        return Create(connection, options);
    }

}