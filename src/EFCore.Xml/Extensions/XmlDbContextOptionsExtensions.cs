using Data.Common.DataSource;
using EFCore.Common.Utils;
using EFCore.Xml.Infrastructure.Internal;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Data.XmlClient;

namespace Microsoft.EntityFrameworkCore;

public static class XmlDbContextOptionsExtensions
{
    public static DbContextOptionsBuilder UseXml(this DbContextOptionsBuilder optionsBuilder,
                                                      string connectionString)
    {
        if (string.IsNullOrEmpty(connectionString))
            throw new ArgumentNullException(nameof(connectionString));

        return optionsBuilder.UseXml(extension => (XmlOptionsExtension)extension.WithConnectionString(connectionString));
    }

    public static DbContextOptionsBuilder UseXml(this DbContextOptionsBuilder optionsBuilder,
                                                      XmlConnection connection)
    {
        if (connection == null)
            throw new ArgumentNullException(nameof(connection));

        return optionsBuilder.UseXml(extension => (XmlOptionsExtension)extension.WithConnection(connection));
    }

    public static DbContextOptionsBuilder UseXml(this DbContextOptionsBuilder optionsBuilder,
                                                      string connectionString, IDataSourceProvider dataSourceProvider)
    {
        if (string.IsNullOrEmpty(connectionString))
            throw new ArgumentNullException(nameof(connectionString));

        var builder = optionsBuilder.UseXml(extension => (XmlOptionsExtension)extension.WithConnectionString(connectionString))
                                    .UseDataSource(dataSourceProvider);
        return builder;
    }

    public static DbContextOptionsBuilder UseXml(this DbContextOptionsBuilder optionsBuilder,
                                                      XmlConnection connection, IDataSourceProvider dataSourceProvider)
    {
        if (connection == null)
            throw new ArgumentNullException(nameof(connection));

        var builder = optionsBuilder.UseXml(
            extension => (XmlOptionsExtension)extension.WithConnection(connection));
        builder = builder.UseDataSource(dataSourceProvider);
        return builder;
    }

    public static DbContextOptionsBuilder UseDataSource(this DbContextOptionsBuilder optionsBuilder,
                                                             IDataSourceProvider dataSourceProvider)
    {
        if (dataSourceProvider == null)
            throw new ArgumentNullException(nameof(dataSourceProvider));

        return optionsBuilder.UseXml(extension => (XmlOptionsExtension)extension.WithDataSource(dataSourceProvider));
    }

    private static DbContextOptionsBuilder UseXml(this DbContextOptionsBuilder optionsBuilder,
                                                  Func<XmlOptionsExtension, XmlOptionsExtension> addConnection)
    {
        if (optionsBuilder == null)
            throw new ArgumentNullException(nameof(optionsBuilder));

        XmlOptionsExtension extension = GetOrCreateExtension(optionsBuilder);
        extension = addConnection(extension);
        ((IDbContextOptionsBuilderInfrastructure)optionsBuilder).AddOrUpdateExtension(extension);

        OptionsExtensionsHelper.ConfigureWarnings(optionsBuilder);

        return optionsBuilder;
    }

    /// <summary>
    /// Returns an existing instance of <see cref="XmlOptionsExtension"/>, or a new instance if one does not exist.
    /// </summary>
    /// <param name="optionsBuilder">The <see cref="DbContextOptionsBuilder"/> to search.</param>
    /// <returns>
    /// An existing instance of <see cref="XmlOptionsExtension"/>, or a new instance if one does not exist.
    /// </returns>
    private static XmlOptionsExtension GetOrCreateExtension(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.Options.FindExtension<XmlOptionsExtension>() is XmlOptionsExtension existing
            ? new XmlOptionsExtension(existing)
            : new XmlOptionsExtension();
}
