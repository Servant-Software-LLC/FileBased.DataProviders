using EFCore.Common.Scaffolding.Internal;
using EFCore.Common.Storage.Internal;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Scaffolding;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace EFCore.Common.Design.Internal;

public abstract class FileDesignTimeServices : IDesignTimeServices
{
    public virtual void ConfigureDesignTimeServices(IServiceCollection services)
    {
        services
            .AddSingleton<ProviderCodeGeneratorDependencies>()
            .AddSingleton<IAnnotationCodeGenerator, AnnotationCodeGenerator>()
            .AddSingleton<AnnotationCodeGeneratorDependencies>();

        new EntityFrameworkDesignServicesBuilder(services)
            .TryAddProviderSpecificServices(m => m
                    .TryAddScoped<IScaffoldingModelFactory, FileScaffoldingModelFactory>()
                    .TryAddScoped<IRelationalTypeMappingSource, FileTypeMappingSource>()
            )
            .TryAddCoreServices();
    }

}
