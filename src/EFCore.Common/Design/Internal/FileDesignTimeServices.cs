using EFCore.Common.Scaffolding.Internal;
using EFCore.Common.Storage.Internal;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Scaffolding;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace EFCore.Common.Design.Internal;

/// <summary>
/// Represents a base design-time service configuration for file-based providers in Entity Framework Core.
/// This class provides a mechanism to register common services used by file-based data sources during design-time tasks such as migrations and scaffolding.
/// </summary>
public abstract class FileDesignTimeServices : IDesignTimeServices
{
    /// <summary>
    /// Configures services for design-time operations for file-based providers.
    /// </summary>
    /// <param name="services">The service collection to which services are added.</param>
    public virtual void ConfigureDesignTimeServices(IServiceCollection services)
    {
        // Register common design-time dependencies for providers.
        services
            .AddSingleton<ProviderCodeGeneratorDependencies>()
            .AddSingleton<IAnnotationCodeGenerator, AnnotationCodeGenerator>()
            .AddSingleton<AnnotationCodeGeneratorDependencies>();

        // Configure EF Core design-time services and add provider-specific services.
        new EntityFrameworkDesignServicesBuilder(services)
            .TryAddProviderSpecificServices(m => m
                    .TryAddScoped<IScaffoldingModelFactory, FileScaffoldingModelFactory>()
                    .TryAddScoped<IRelationalTypeMappingSource, FileTypeMappingSource>()
            )
            .TryAddCoreServices();
    }

}
