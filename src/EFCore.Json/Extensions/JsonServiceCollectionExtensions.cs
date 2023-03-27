using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using EFCore.Json.Infrastructure.Internal;
using Microsoft.EntityFrameworkCore.Diagnostics;
using EFCore.Json.Diagnostics.Internal;
using EFCore.Json.Storage.Internal;

namespace EFCore.Json.Extensions;

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
