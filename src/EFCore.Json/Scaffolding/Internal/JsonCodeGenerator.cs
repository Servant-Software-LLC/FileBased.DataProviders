using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Scaffolding;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using EFCore.Common.Utils;

namespace EFCore.Json.Scaffolding.Internal;

/// <summary>
/// Represents a code generator for the JSON data source in Entity Framework Core.
/// This class facilitates the generation of code that configures the DbContext to use the JSON provider.
/// </summary>
public class JsonCodeGenerator : ProviderCodeGenerator
{
    // Cached method info for the UseJson method extension on DbContextOptionsBuilder
    private static readonly MethodInfo _useJsonMethodInfo
        = typeof(JsonDbContextOptionsExtensions).GetRequiredRuntimeMethod(
            nameof(JsonDbContextOptionsExtensions.UseJson),
            typeof(DbContextOptionsBuilder),
            typeof(string));

    /// <summary>
    /// Initializes a new instance of the <see cref="JsonCodeGenerator"/> class.
    /// </summary>
    /// <param name="dependencies">The dependencies required for code generation.</param>
    public JsonCodeGenerator(ProviderCodeGeneratorDependencies dependencies)
            : base(dependencies)
    {

    }

    /// <summary>
    /// Generates a method call code fragment to configure the DbContext to use the JSON provider.
    /// </summary>
    /// <param name="connectionString">The connection string for the JSON data source.</param>
    /// <param name="providerOptions">Optional additional provider options.</param>
    /// <returns>A method call code fragment that represents the configuration.</returns>
    public override MethodCallCodeFragment GenerateUseProvider(
        string connectionString,
        MethodCallCodeFragment providerOptions)
        => new(
            _useJsonMethodInfo,
            providerOptions == null
                ? new object[] { connectionString }
                : new object[] { connectionString, new NestedClosureCodeFragment("x", providerOptions) });

}
