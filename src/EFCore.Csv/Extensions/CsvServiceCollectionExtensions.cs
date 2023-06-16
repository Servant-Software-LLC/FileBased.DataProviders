using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using EFCore.Csv.Storage.Internal;
using System.Diagnostics.CodeAnalysis;
using EFCore.Csv.Infrastructure.Internal;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Update;
using EFCore.Csv.Update.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using EFCore.Csv.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Query;
using EFCore.Csv.Query.Internal;
using EFCore.Csv.Diagnostics.Internal;
using Microsoft.EntityFrameworkCore.Scaffolding;
using EFCore.Csv.Scaffolding.Internal;
using EFCore.Common.Scaffolding.Internal;

namespace Microsoft.Extensions.DependencyInjection;

public static class CsvServiceCollectionExtensions
{
    public static IServiceCollection AddEntityFrameworkCsv([NotNull] this IServiceCollection serviceCollection)
    {
        var builder = new EntityFrameworkRelationalServicesBuilder(serviceCollection)
            .TryAdd<IRelationalTypeMappingSource, CsvTypeMappingSource>()
            .TryAdd<IDatabaseProvider, DatabaseProvider<CsvOptionsExtension>>()
            .TryAdd<LoggingDefinitions, CsvLoggingDefinitions>()
            .TryAdd<IModificationCommandBatchFactory, CsvModificationCommandBatchFactory>()
            .TryAdd<IUpdateSqlGenerator, CsvUpdateSqlGenerator>()

            //Found that this was necessary, because the default convension of determining a
            //Model's primary key automatically based off of properties that have 'Id' in their
            //name was getting ignored.
            .TryAdd<IProviderConventionSetBuilder, CsvConventionSetBuilder>()

            .TryAdd<IQuerySqlGeneratorFactory, CsvQuerySqlGeneratorFactory>()

            .TryAddProviderSpecificServices(m => m
                    .TryAddScoped<ICsvRelationalConnection, CsvRelationalConnection>()
                    .TryAddSingleton<IDatabaseModelFactory, CsvDatabaseModelFactory>()
                    .TryAddSingleton<IScaffoldingModelFactory, FileScaffoldingModelFactory>()
            )

            .TryAdd<IRelationalConnection>(p => p.GetService<ICsvRelationalConnection>())
        ;

        builder.TryAddCoreServices();

        serviceCollection
            .AddScoped<IRelationalConnection, CsvRelationalConnection>()
            .AddSingleton<ISqlGenerationHelper, RelationalSqlGenerationHelper>()
            .AddScoped<IRelationalDatabaseCreator, CsvDatabaseCreator>();

        ;

        return serviceCollection;

    }
}
