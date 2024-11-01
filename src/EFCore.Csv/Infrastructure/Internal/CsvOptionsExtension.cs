using Data.Common.DataSource;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace EFCore.Csv.Infrastructure.Internal;

/// <summary>
/// Represents the CSV-specific implementation of relational options used for Entity Framework Core configuration.
/// This extension is used to enable and configure the usage of CSV as a data source.
/// </summary>
public class CsvOptionsExtension : FileOptionsExtension
{
    private CsvOptionsExtensionInfo info;

    /// <summary>
    /// Initializes a new instance of the <see cref="CsvOptionsExtension"/> class.
    /// </summary>
    public CsvOptionsExtension() : base((IDataSourceProvider)null){ }

    /// <summary>
    /// Initializes a new instance of the <see cref="CsvOptionsExtension"/> class with a custom data source.
    /// </summary>
    /// <param name="dataSourceProvider"></param>
    public CsvOptionsExtension(IDataSourceProvider dataSourceProvider) 
        :base(dataSourceProvider){ }

    /// <summary>
    /// Initializes a new instance of the <see cref="CsvOptionsExtension"/> class, copying settings from the provided instance.
    /// </summary>
    /// <param name="copyFrom">The instance to copy settings from.</param>
    protected internal CsvOptionsExtension(CsvOptionsExtension copyFrom)
        : base(copyFrom) { }

    /// <summary>
    /// Gets the information associated with this options extension.
    /// </summary>
    public override DbContextOptionsExtensionInfo Info => info ??= new CsvOptionsExtensionInfo(this);

    /// <summary>
    /// Configures the dependency injection services for the CSV provider.
    /// </summary>
    /// <param name="services">The collection of services to add to.</param>
    public override void ApplyServices(IServiceCollection services)
    {
        base.ApplyServices(services);
        services.AddEntityFrameworkCsv();
    }

    /// <summary>
    /// Validates the configuration for the CSV data source.
    /// </summary>
    /// <param name="options">The options to validate.</param>
    public override void Validate(IDbContextOptions options)
    {
        // You can add any validation logic here, if necessary.
    }

    /// <summary>
    /// Creates a copy of this options extension.
    /// </summary>
    /// <returns>A cloned instance of this options extension.</returns>
    protected override RelationalOptionsExtension Clone() => new CsvOptionsExtension(this);

}
