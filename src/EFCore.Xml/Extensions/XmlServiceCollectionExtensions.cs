using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using EFCore.Xml.Storage.Internal;
using System.Diagnostics.CodeAnalysis;
using EFCore.Xml.Infrastructure.Internal;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Update;
using EFCore.Xml.Update.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using EFCore.Xml.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Query;
using EFCore.Xml.Query.Internal;
using EFCore.Xml.Diagnostics.Internal;

namespace Microsoft.Extensions.DependencyInjection;

public static class XmlServiceCollectionExtensions
{
    public static IServiceCollection AddEntityFrameworkMyCustom([NotNull] this IServiceCollection serviceCollection)
    {
        var builder = new EntityFrameworkRelationalServicesBuilder(serviceCollection)
            .TryAdd<IRelationalTypeMappingSource, XmlTypeMappingSource>()
            .TryAdd<IDatabaseProvider, DatabaseProvider<XmlOptionsExtension>>()
            .TryAdd<LoggingDefinitions, XmlLoggingDefinitions>()
            .TryAdd<IModificationCommandBatchFactory, XmlModificationCommandBatchFactory>()
            .TryAdd<IUpdateSqlGenerator, XmlUpdateSqlGenerator>()

            //Found that this was necessary, because the default convension of determining a
            //Model's primary key automatically based off of properties that have 'Id' in their
            //name was getting ignored.
            .TryAdd<IProviderConventionSetBuilder, XmlConventionSetBuilder>()

            .TryAdd<IQuerySqlGeneratorFactory, XmlQuerySqlGeneratorFactory>()

            .TryAddProviderSpecificServices(m => m
                    .TryAddScoped<IXmlRelationalConnection, XmlRelationalConnection>()
            )

            .TryAdd<IRelationalConnection>(p => p.GetService<IXmlRelationalConnection>())
        ;

        builder.TryAddCoreServices();

        serviceCollection
            .AddScoped<IRelationalConnection, XmlRelationalConnection>()
            .AddSingleton<ISqlGenerationHelper, RelationalSqlGenerationHelper>()
            .AddScoped<IRelationalDatabaseCreator, XmlDatabaseCreator>();

        ;

        return serviceCollection;

    }
}
