using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Scaffolding;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using EFCore.Common.Utils;

namespace EFCore.Json.Scaffolding.Internal;

public class JsonCodeGenerator : ProviderCodeGenerator
{
    private static readonly MethodInfo _useJsonMethodInfo
        = typeof(JsonDbContextOptionsExtensions).GetRequiredRuntimeMethod(
            nameof(JsonDbContextOptionsExtensions.UseJson),
            typeof(DbContextOptionsBuilder),
            typeof(string));

    public JsonCodeGenerator(ProviderCodeGeneratorDependencies dependencies)
            : base(dependencies)
    {

    }

    public override MethodCallCodeFragment GenerateUseProvider(
        string connectionString,
        MethodCallCodeFragment? providerOptions)
        => new(
            _useJsonMethodInfo,
            providerOptions == null
                ? new object[] { connectionString }
                : new object[] { connectionString, new NestedClosureCodeFragment("x", providerOptions) });

}
