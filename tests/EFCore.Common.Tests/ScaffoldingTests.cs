using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Scaffolding;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace EFCore.Common.Tests;

/// <summary>
/// Contains test utilities for validating the scaffolding process in Entity Framework Core.
/// </summary>
public static class ScaffoldingTests
{
    /// <summary>
    /// Validates the EF Core scaffolding process by generating a model using a given connection string and design-time services.
    /// </summary>
    /// <param name="connectionString">The connection string for the data source to scaffold.</param>
    /// <param name="designTimeServices">The design-time services for the EF Core provider being tested.</param>
    public static void ValidateScaffolding(string connectionString, IDesignTimeServices designTimeServices)
    {
        // Arrange
        var serviceCollection = new ServiceCollection()
            .AddEntityFrameworkDesignTimeServices(); // Add EF Core design-time services.

        // Add provider-specific services.
        designTimeServices.ConfigureDesignTimeServices(serviceCollection);

        // Build the service provider.
        using var serviceProvider = serviceCollection.BuildServiceProvider();

        // Get the IReverseEngineerScaffolder service.
        var scaffolder = serviceProvider.GetService<IReverseEngineerScaffolder>();

        // Define scaffolding options.
        var databaseModelFactoryOptions = new DatabaseModelFactoryOptions(new List<string>(), new List<string>());
        var modelReverseEngineerOptions = new ModelReverseEngineerOptions();
        var modelCodeGenerationOptions = new ModelCodeGenerationOptions
        {
            ModelNamespace = "TestNamespace",
            ContextName = "TestDbContext",
            // Additional options can be specified here...
        };

        // Act

        // Invoke the scaffolding process using the provided connection string and options.
        var scaffoldedModel = scaffolder.ScaffoldModel(
            connectionString,
            databaseModelFactoryOptions,
            modelReverseEngineerOptions,
            modelCodeGenerationOptions);

        // Assert

        // Check the results of the scaffolding process.
        Assert.NotNull(scaffoldedModel);
        Assert.NotEmpty(scaffoldedModel.AdditionalFiles);
        Assert.NotNull(scaffoldedModel.ContextFile);

    }
}

