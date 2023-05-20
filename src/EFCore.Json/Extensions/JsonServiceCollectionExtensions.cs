using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using EFCore.Json.Storage.Internal;
using System.Diagnostics.CodeAnalysis;
using EFCore.Json.Infrastructure.Internal;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Update;
using EFCore.Json.Update.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using EFCore.Json.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Query;
using EFCore.Json.Query.Internal;
using EFCore.Json.Diagnostics.Internal;
using Microsoft.EntityFrameworkCore.Scaffolding;
using EFCore.Json.Scaffolding.Internal;

namespace Microsoft.Extensions.DependencyInjection;

public static class JsonServiceCollectionExtensions
{
    public static IServiceCollection AddEntityFrameworkJson([NotNull] this IServiceCollection serviceCollection)
    {
        var builder = new EntityFrameworkRelationalServicesBuilder(serviceCollection)
            .TryAdd<IRelationalTypeMappingSource, JsonTypeMappingSource>()
            .TryAdd<IDatabaseProvider, DatabaseProvider<JsonOptionsExtension>>()
            .TryAdd<LoggingDefinitions, JsonLoggingDefinitions>()
            .TryAdd<IModificationCommandBatchFactory, JsonModificationCommandBatchFactory>()
            .TryAdd<IUpdateSqlGenerator, JsonUpdateSqlGenerator>()

            //Found that this was necessary, because the default convension of determining a
            //Model's primary key automatically based off of properties that have 'Id' in their
            //name was getting ignored.
            .TryAdd<IProviderConventionSetBuilder, JsonConventionSetBuilder>()

            .TryAdd<IQuerySqlGeneratorFactory, JsonQuerySqlGeneratorFactory>()

            .TryAddProviderSpecificServices(m => m
                    .TryAddScoped<IJsonRelationalConnection, JsonRelationalConnection>()
                    .TryAddSingleton<IDatabaseModelFactory, JsonDatabaseModelFactory>()
            )

            .TryAdd<IRelationalConnection>(p => p.GetService<IJsonRelationalConnection>())
        ;

        builder.TryAddCoreServices();

        serviceCollection
            .AddScoped<IRelationalConnection, JsonRelationalConnection>()
            .AddSingleton<ISqlGenerationHelper, RelationalSqlGenerationHelper>()
            .AddScoped<IRelationalDatabaseCreator, JsonDatabaseCreator>();

        ;

        return serviceCollection;

    }
}
