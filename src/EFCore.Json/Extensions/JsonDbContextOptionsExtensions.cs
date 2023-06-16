using Microsoft.EntityFrameworkCore.Infrastructure;
using EFCore.Json.Infrastructure.Internal;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Data.JsonClient;

namespace Microsoft.EntityFrameworkCore;

public static class JsonDbContextOptionsExtensions
{
    public static DbContextOptionsBuilder UseJson(this DbContextOptionsBuilder optionsBuilder,
                                                      string connectionString)
    {
        if (optionsBuilder == null)
            throw new ArgumentNullException(nameof(optionsBuilder));
        if (string.IsNullOrEmpty(connectionString))
            throw new ArgumentNullException(nameof(connectionString));

        var extension = (JsonOptionsExtension)GetOrCreateExtension(optionsBuilder).WithConnectionString(connectionString);
        ((IDbContextOptionsBuilderInfrastructure)optionsBuilder).AddOrUpdateExtension(extension);

        ConfigureWarnings(optionsBuilder);

        return optionsBuilder;
    }

    public static DbContextOptionsBuilder UseJson(this DbContextOptionsBuilder optionsBuilder,
                                                      JsonConnection connection)
    {
        if (optionsBuilder == null)
            throw new ArgumentNullException(nameof(optionsBuilder));
        if (connection == null)
            throw new ArgumentNullException(nameof(connection));

        var extension = (JsonOptionsExtension)GetOrCreateExtension(optionsBuilder).WithConnection(connection);
        ((IDbContextOptionsBuilderInfrastructure)optionsBuilder).AddOrUpdateExtension(extension);

        ConfigureWarnings(optionsBuilder);

        return optionsBuilder;
    }

    /// <summary>
    /// Returns an existing instance of <see cref="NpgsqlOptionsExtension"/>, or a new instance if one does not exist.
    /// </summary>
    /// <param name="optionsBuilder">The <see cref="DbContextOptionsBuilder"/> to search.</param>
    /// <returns>
    /// An existing instance of <see cref="NpgsqlOptionsExtension"/>, or a new instance if one does not exist.
    /// </returns>
    private static JsonOptionsExtension GetOrCreateExtension(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.Options.FindExtension<JsonOptionsExtension>() is JsonOptionsExtension existing
            ? new JsonOptionsExtension(existing)
            : new JsonOptionsExtension();

    private static void ConfigureWarnings(DbContextOptionsBuilder optionsBuilder)
    {
        var coreOptionsExtension = optionsBuilder.Options.FindExtension<CoreOptionsExtension>()
            ?? new CoreOptionsExtension();

        coreOptionsExtension = coreOptionsExtension.WithWarningsConfiguration(
            coreOptionsExtension.WarningsConfiguration.TryWithExplicit(
                RelationalEventId.AmbientTransactionWarning, WarningBehavior.Throw));

        ((IDbContextOptionsBuilderInfrastructure)optionsBuilder).AddOrUpdateExtension(coreOptionsExtension);
    }

}
