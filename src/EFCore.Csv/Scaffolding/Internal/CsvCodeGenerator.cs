using EFCore.Common.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Scaffolding;
using System.Reflection;

namespace EFCore.Csv.Scaffolding.Internal;

/// <summary>
/// Code generator for CSV provider in Entity Framework Core.
/// This class is responsible for generating the necessary code to set up the CSV provider during scaffolding operations.
/// </summary>
public class CsvCodeGenerator : ProviderCodeGenerator
{
    // MethodInfo object capturing the signature of the 'UseCsv' extension method from 'CsvDbContextOptionsExtensions'
    // This will be used during the code generation phase to plug in the appropriate setup for the CSV provider.
    private static readonly MethodInfo _useCsvMethodInfo
        = typeof(CsvDbContextOptionsExtensions).GetRequiredRuntimeMethod(
            nameof(CsvDbContextOptionsExtensions.UseCsv),
            typeof(DbContextOptionsBuilder),
            typeof(string));

    public CsvCodeGenerator(ProviderCodeGeneratorDependencies dependencies)
            : base(dependencies)
    {

    }

    /// <summary>
    /// Generates the method call required to set up the CSV provider.
    /// </summary>
    /// <param name="connectionString">The connection string to the CSV source.</param>
    /// <param name="providerOptions">Additional provider options, if any.</param>
    /// <returns>A <see cref="MethodCallCodeFragment"/> that represents the method call to set up the CSV provider.</returns>
    public override MethodCallCodeFragment GenerateUseProvider(
        string connectionString,
        MethodCallCodeFragment providerOptions)
        => new(
            _useCsvMethodInfo,
            providerOptions == null
                ? new object[] { connectionString }
                : new object[] { connectionString, new NestedClosureCodeFragment("x", providerOptions) });
}
