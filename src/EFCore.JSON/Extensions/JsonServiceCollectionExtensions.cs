using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using EFCore.JSON.Infrastructure.Internal;
using Microsoft.EntityFrameworkCore.Diagnostics;
using EFCore.Json.Diagnostics.Internal;
using EFCore.JSON.Storage.Internal;

namespace EFCore.JSON.Extensions;

public static class JsonServiceCollectionExtensions
{
    public static IServiceCollection AddEntityFrameworkJson(this IServiceCollection serviceCollection)
    {
        if (serviceCollection is null)
            throw new ArgumentNullException(nameof(serviceCollection));

        new EntityFrameworkRelationalServicesBuilder(serviceCollection)

            .TryAdd<IDatabaseProvider, DatabaseProvider<JsonOptionsExtension>>()
            .TryAdd<LoggingDefinitions, JsonLoggingDefinitions>()

            .TryAddProviderSpecificServices(serviceCollectionMap => serviceCollectionMap
                    .TryAddScoped<IJsonRelationalConnection, JsonRelationalConnection>()
            )
            .TryAddCoreServices();

        return serviceCollection;
    }

}
