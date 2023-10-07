using EFCore.Common.Design.Internal;
using EFCore.Csv.Scaffolding.Internal;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Scaffolding;
using Microsoft.Extensions.DependencyInjection;

namespace EFCore.Csv.Design.Internal;

public class CsvDesignTimeServices : FileDesignTimeServices
{
    public override void ConfigureDesignTimeServices(IServiceCollection services)
    {
        base.ConfigureDesignTimeServices(services);

        services
            .AddEntityFrameworkCsv();

        new EntityFrameworkDesignServicesBuilder(services)
            .TryAddProviderSpecificServices(m => m
                    .TryAddScoped<IDatabaseModelFactory, CsvDatabaseModelFactory>()
                    .TryAddScoped<IProviderConfigurationCodeGenerator, CsvCodeGenerator>()
            );
    }
}
