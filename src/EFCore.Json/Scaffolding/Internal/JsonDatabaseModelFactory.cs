using EFCore.Common.Scaffolding.Internal;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Scaffolding.Metadata;
using Microsoft.EntityFrameworkCore.Scaffolding;
using System.Data.JsonClient;

namespace EFCore.Json.Scaffolding.Internal;

/// <summary>
/// Represents a database model factory for the JSON data source in Entity Framework Core.
/// This class facilitates the creation of a database model based on a given JSON connection string.
/// </summary>
public class JsonDatabaseModelFactory : FileDatabaseModelFactory
{
    /// <summary>
    /// Initializes a new instance of the <see cref="JsonDatabaseModelFactory"/> class.
    /// </summary>
    /// <param name="logger">The logger used for diagnostic purposes.</param>
    public JsonDatabaseModelFactory(IDiagnosticsLogger<DbLoggerCategory.Scaffolding> logger)
        : base(logger)
    {

    }

    /// <summary>
    /// Creates a database model based on the provided connection string for a JSON data source.
    /// </summary>
    /// <param name="connectionString">The connection string for the JSON data source.</param>
    /// <param name="options">The options for creating the database model.</param>
    /// <returns>The constructed database model for the JSON data source.</returns>
    public override DatabaseModel Create(string connectionString, DatabaseModelFactoryOptions options)
    {
        using var connection = new JsonConnection(connectionString);
        return Create(connection, options);
    }

}
