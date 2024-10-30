using Data.Common.DataSource;
using EFCore.Common.Utils;
using EFCore.Csv.Infrastructure.Internal;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Data.CsvClient;

namespace Microsoft.EntityFrameworkCore;

public static class CsvDbContextOptionsExtensions
{
    public static DbContextOptionsBuilder UseCsv(this DbContextOptionsBuilder optionsBuilder,
                                                      string connectionString)
    {
        if (string.IsNullOrEmpty(connectionString))
            throw new ArgumentNullException(nameof(connectionString));

       return optionsBuilder.UseCsv(extension => (CsvOptionsExtension)extension.WithConnectionString(connectionString));
    }

    public static DbContextOptionsBuilder UseCsv(this DbContextOptionsBuilder optionsBuilder,
                                                      CsvConnection connection)
    {
        if (connection == null)
            throw new ArgumentNullException(nameof(connection));

        return optionsBuilder.UseCsv(extension => (CsvOptionsExtension)extension.WithConnection(connection));
    }

    public static DbContextOptionsBuilder UseCsv(this DbContextOptionsBuilder optionsBuilder,
                                                      string connectionString, IDataSourceProvider dataSourceProvider)
    {
        if (string.IsNullOrEmpty(connectionString))
            throw new ArgumentNullException(nameof(connectionString));

        var builder = optionsBuilder.UseCsv(extension => (CsvOptionsExtension)extension.WithConnectionString(connectionString))
                                    .UseDataSource(dataSourceProvider);
        return builder;
    }

    public static DbContextOptionsBuilder UseCsv(this DbContextOptionsBuilder optionsBuilder,
                                                      CsvConnection connection, IDataSourceProvider dataSourceProvider)
    {
        if (connection == null)
            throw new ArgumentNullException(nameof(connection));

        var builder = optionsBuilder.UseCsv(
            extension => (CsvOptionsExtension)extension.WithConnection(connection));
        builder = builder.UseDataSource(dataSourceProvider);
        return builder;
    }

    public static DbContextOptionsBuilder UseDataSource(this DbContextOptionsBuilder optionsBuilder,
                                                             IDataSourceProvider dataSourceProvider)
    {
        if (dataSourceProvider == null)
            throw new ArgumentNullException(nameof(dataSourceProvider));

        return optionsBuilder.UseCsv(extension => (CsvOptionsExtension)extension.WithDataSource(dataSourceProvider));
    }

    private static DbContextOptionsBuilder UseCsv(this DbContextOptionsBuilder optionsBuilder,
                                                  Func<CsvOptionsExtension, CsvOptionsExtension> addConnection)
    {
        if (optionsBuilder == null)
            throw new ArgumentNullException(nameof(optionsBuilder));

        CsvOptionsExtension extension = GetOrCreateExtension(optionsBuilder);
        extension = addConnection(extension);
        ((IDbContextOptionsBuilderInfrastructure)optionsBuilder).AddOrUpdateExtension(extension);

        OptionsExtensionsHelper.ConfigureWarnings(optionsBuilder);

        return optionsBuilder;
    }

    /// <summary>
    /// Returns an existing instance of <see cref="CsvOptionsExtension"/>, or a new instance if one does not exist.
    /// </summary>
    /// <param name="optionsBuilder">The <see cref="DbContextOptionsBuilder"/> to search.</param>
    /// <returns>
    /// An existing instance of <see cref="CsvOptionsExtension"/>, or a new instance if one does not exist.
    /// </returns>
    private static CsvOptionsExtension GetOrCreateExtension(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.Options.FindExtension<CsvOptionsExtension>() is CsvOptionsExtension existing
            ? new CsvOptionsExtension(existing)
            : new CsvOptionsExtension();
}
