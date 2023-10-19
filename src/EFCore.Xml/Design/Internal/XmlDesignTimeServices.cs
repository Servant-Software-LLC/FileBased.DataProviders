using EFCore.Common.Design.Internal;
using EFCore.Xml.Scaffolding.Internal;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Scaffolding;
using Microsoft.Extensions.DependencyInjection;

namespace EFCore.Xml.Design.Internal;


/// <summary>
/// Provides design-time services for XML-based providers.
/// </summary>
public class XmlDesignTimeServices : FileDesignTimeServices
{
    /// <inheritdoc />
    public override void ConfigureDesignTimeServices(IServiceCollection services)
    {
        base.ConfigureDesignTimeServices(services);

        services
            .AddEntityFrameworkXml();

        new EntityFrameworkDesignServicesBuilder(services)
            .TryAddProviderSpecificServices(m => m
                    .TryAddScoped<IDatabaseModelFactory, XmlDatabaseModelFactory>()
                    .TryAddScoped<IProviderConfigurationCodeGenerator, XmlCodeGenerator>()
            );
    }

}
