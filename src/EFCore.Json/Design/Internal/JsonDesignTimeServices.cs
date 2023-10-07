using EFCore.Common.Design.Internal;
using EFCore.Json.Scaffolding.Internal;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Scaffolding;
using Microsoft.Extensions.DependencyInjection;

namespace EFCore.Json.Design.Internal;

public class JsonDesignTimeServices : FileDesignTimeServices
{
    public override void ConfigureDesignTimeServices(IServiceCollection services)
    {
        base.ConfigureDesignTimeServices(services);

        services
            .AddEntityFrameworkJson();

        new EntityFrameworkDesignServicesBuilder(services)
            .TryAddProviderSpecificServices(m => m
                    .TryAddScoped<IDatabaseModelFactory, JsonDatabaseModelFactory>()
                    .TryAddScoped<IProviderConfigurationCodeGenerator, JsonCodeGenerator>()
            );
    }

}
