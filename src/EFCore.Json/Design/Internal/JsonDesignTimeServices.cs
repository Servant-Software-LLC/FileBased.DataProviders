using EFCore.Common.Design.Internal;
using EFCore.Json.Scaffolding.Internal;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Scaffolding;
using Microsoft.Extensions.DependencyInjection;

namespace EFCore.Json.Design.Internal;

/// <summary>
/// Represents the design-time services specific to the JSON data source for Entity Framework Core.
/// This class sets up services required for the design-time tasks related to scaffolding and code generation for JSON.
/// </summary>
public class JsonDesignTimeServices : FileDesignTimeServices
{
    /// <summary>
    /// Configures the design-time services for the JSON provider.
    /// </summary>
    /// <param name="services">The collection of services to add to.</param>
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
