using Data.Common.DataSource;
using EFCore.Csv.Infrastructure.Internal;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace EFCore.Xml.Infrastructure.Internal;

/// <summary>
/// Represents the XML-specific implementation of relational options used for Entity Framework Core configuration.
/// This extension is used to enable and configure the usage of XML as a data source.
/// </summary>
public class XmlOptionsExtension : FileOptionsExtension
{
    // Cached instance of the associated options extension info
    private XmlOptionsExtensionInfo info;

    /// <summary>
    /// Initializes a new instance of the <see cref="XmlOptionsExtension"/> class.
    /// </summary>
    public XmlOptionsExtension() : base((IDataSourceProvider)null) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="XmlOptionsExtension"/> class with a custom data source.
    /// </summary>
    public XmlOptionsExtension(IDataSourceProvider dataSourceProvider)
        : base(dataSourceProvider) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="XmlOptionsExtension"/> class, copying settings from the provided instance.
    /// </summary>
    /// <param name="copyFrom">The instance to copy settings from.</param>
    protected internal XmlOptionsExtension(XmlOptionsExtension copyFrom)
        : base(copyFrom)
    {

    }

    /// <summary>
    /// Gets the information associated with this options extension.
    /// </summary>
    public override DbContextOptionsExtensionInfo Info => info ??= new XmlOptionsExtensionInfo(this);

    /// <summary>
    /// Configures the dependency injection services for the XML provider.
    /// </summary>
    /// <param name="services">The collection of services to add to.</param>
    public override void ApplyServices(IServiceCollection services)
    {
        base.ApplyServices(services);
        services.AddEntityFrameworkXml();
    }

    /// <summary>
    /// Validates the configuration for the XML data source.
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
    protected override RelationalOptionsExtension Clone() => new XmlOptionsExtension(this);

}
