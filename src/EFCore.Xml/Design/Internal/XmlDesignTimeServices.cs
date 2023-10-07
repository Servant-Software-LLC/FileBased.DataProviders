using EFCore.Common.Design.Internal;
using EFCore.Xml.Scaffolding.Internal;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Scaffolding;
using Microsoft.Extensions.DependencyInjection;

namespace EFCore.Xml.Design.Internal;

public class XmlDesignTimeServices : FileDesignTimeServices
{
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
