using EFCore.Common.Scaffolding.Internal;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Scaffolding.Metadata;
using Microsoft.EntityFrameworkCore.Scaffolding;
using System.Data.XmlClient;

namespace EFCore.Xml.Scaffolding.Internal;

/// <summary>
/// Represents a factory for creating database models from XML data sources in Entity Framework Core.
/// This class extends the base factory for file-based data sources to specifically cater to XML files.
/// </summary>
public class XmlDatabaseModelFactory : FileDatabaseModelFactory
{
    /// <summary>
    /// Initializes a new instance of the <see cref="XmlDatabaseModelFactory"/> class.
    /// </summary>
    /// <param name="logger">The diagnostics logger used for logging scaffolding operations.</param>
    public XmlDatabaseModelFactory(IDiagnosticsLogger<DbLoggerCategory.Scaffolding> logger)
        : base(logger)
    {

    }

    /// <summary>
    /// Creates a database model for the XML data source using the provided connection string and options.
    /// </summary>
    /// <param name="connectionString">The connection string for the XML data source.</param>
    /// <param name="options">The options for creating the database model.</param>
    /// <returns>The database model constructed from the XML data source.</returns>
    public override DatabaseModel Create(string connectionString, DatabaseModelFactoryOptions options)
    {
        using var connection = new XmlConnection(connectionString);
        return Create(connection, options);
    }

}
