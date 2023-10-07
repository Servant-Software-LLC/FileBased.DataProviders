using EFCore.Common.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Scaffolding;
using System.Reflection;

namespace EFCore.Csv.Scaffolding.Internal;

public class CsvCodeGenerator : ProviderCodeGenerator
{
    private static readonly MethodInfo _useCsvMethodInfo
        = typeof(CsvDbContextOptionsExtensions).GetRequiredRuntimeMethod(
            nameof(CsvDbContextOptionsExtensions.UseCsv),
            typeof(DbContextOptionsBuilder),
            typeof(string));

    public CsvCodeGenerator(ProviderCodeGeneratorDependencies dependencies)
            : base(dependencies)
    {

    }

    public override MethodCallCodeFragment GenerateUseProvider(
        string connectionString,
        MethodCallCodeFragment providerOptions)
        => new(
            _useCsvMethodInfo,
            providerOptions == null
                ? new object[] { connectionString }
                : new object[] { connectionString, new NestedClosureCodeFragment("x", providerOptions) });
}
