using Data.Common.DataSource;
using EFCore.Common.Utils;
using EFCore.Json.Infrastructure.Internal;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Data.JsonClient;

namespace Microsoft.EntityFrameworkCore;

public static class JsonDbContextOptionsExtensions
{
    public static DbContextOptionsBuilder UseJson(this DbContextOptionsBuilder optionsBuilder,
                                                      string connectionString)
    {
        if (string.IsNullOrEmpty(connectionString))
            throw new ArgumentNullException(nameof(connectionString));

        return optionsBuilder.UseJson(extension => (JsonOptionsExtension)extension.WithConnectionString(connectionString));
    }

    public static DbContextOptionsBuilder UseJson(this DbContextOptionsBuilder optionsBuilder,
                                                      JsonConnection connection)
    {
        if (connection == null)
            throw new ArgumentNullException(nameof(connection));

        return optionsBuilder.UseJson(extension => (JsonOptionsExtension)extension.WithConnection(connection));
    }

    public static DbContextOptionsBuilder UseJson(this DbContextOptionsBuilder optionsBuilder,
                                                      string connectionString, IDataSourceProvider dataSourceProvider)
    {
        if (string.IsNullOrEmpty(connectionString))
            throw new ArgumentNullException(nameof(connectionString));

        var builder = optionsBuilder.UseJson(extension => (JsonOptionsExtension)extension.WithConnectionString(connectionString))
                                    .UseDataSource(dataSourceProvider);
        return builder;
    }

    public static DbContextOptionsBuilder UseJson(this DbContextOptionsBuilder optionsBuilder,
                                                      JsonConnection connection, IDataSourceProvider dataSourceProvider)
    {
        if (connection == null)
            throw new ArgumentNullException(nameof(connection));

        var builder = optionsBuilder.UseJson(
            extension => (JsonOptionsExtension)extension.WithConnection(connection));
        builder = builder.UseDataSource(dataSourceProvider);
        return builder;
    }

    public static DbContextOptionsBuilder UseDataSource(this DbContextOptionsBuilder optionsBuilder,
                                                             IDataSourceProvider dataSourceProvider)
    {
        if (dataSourceProvider == null)
            throw new ArgumentNullException(nameof(dataSourceProvider));

        return optionsBuilder.UseJson(extension => (JsonOptionsExtension)extension.WithDataSource(dataSourceProvider));
    }

    private static DbContextOptionsBuilder UseJson(this DbContextOptionsBuilder optionsBuilder,
                                                  Func<JsonOptionsExtension, JsonOptionsExtension> addConnection)
    {
        if (optionsBuilder == null)
            throw new ArgumentNullException(nameof(optionsBuilder));

        JsonOptionsExtension extension = GetOrCreateExtension(optionsBuilder);
        extension = addConnection(extension);
        ((IDbContextOptionsBuilderInfrastructure)optionsBuilder).AddOrUpdateExtension(extension);

        OptionsExtensionsHelper.ConfigureWarnings(optionsBuilder);

        return optionsBuilder;
    }

    /// <summary>
    /// Returns an existing instance of <see cref="JsonOptionsExtension"/>, or a new instance if one does not exist.
    /// </summary>
    /// <param name="optionsBuilder">The <see cref="DbContextOptionsBuilder"/> to search.</param>
    /// <returns>
    /// An existing instance of <see cref="JsonOptionsExtension"/>, or a new instance if one does not exist.
    /// </returns>
    private static JsonOptionsExtension GetOrCreateExtension(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.Options.FindExtension<JsonOptionsExtension>() is JsonOptionsExtension existing
            ? new JsonOptionsExtension(existing)
            : new JsonOptionsExtension();
}
