using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Scaffolding;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace EFCore.Common.Tests;

public static class ScaffoldingTests
{
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
            // Other options...
        };

        // Act
        // Scaffold the model.
        // Replace "YourConnectionString" with a valid connection string for your test database.
        var scaffoldedModel = scaffolder.ScaffoldModel(
            connectionString,
            databaseModelFactoryOptions,
            modelReverseEngineerOptions,
            modelCodeGenerationOptions);

        // Assert
        // Validate the scaffolded model.
        // This might involve checking the number of generated entities, the names of generated DbContext and entity classes, etc.
        Assert.NotNull(scaffoldedModel);
        Assert.NotEmpty(scaffoldedModel.AdditionalFiles);
        Assert.NotNull(scaffoldedModel.ContextFile);

    }
}

