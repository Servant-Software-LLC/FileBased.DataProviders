using Data.Common.Extension;
using Data.Common.Utils.ConnectionString;
using EFCore.Json.Tests.Utils;
using System.Reflection;
using Xunit;
using TypeMappingTest = EFCore.Common.Tests.TypeMappingTests<EFCore.Json.Tests.Models.TypeMappingContext>;

namespace EFCore.Json.Tests.FolderAsDatabase;

public class JsonTypeMappingTests
{
    [Fact]
    public void ModelBuilder_CanMapAllExtendedTypes()
    {
        var sandboxId = $"{GetType().FullName}.{MethodBase.GetCurrentMethod()!.Name}";
        var connectionString = new FileConnectionString
        {
            DataSource = Path.Combine(FileConnectionStringTestsExtensions.SourcesFolder, "TypeMapping")
        };
        TypeMappingTest.ModelBuilder_CanMapAllExtendedTypes(connectionString.Sandbox("Sandbox", sandboxId));
    }

    [Fact]
    public void EnsureCreated_WithExtendedTypes()
    {
        var sandboxId = $"{GetType().FullName}.{MethodBase.GetCurrentMethod()!.Name}";
        var connectionString = new FileConnectionString
        {
            DataSource = Path.Combine(FileConnectionStringTestsExtensions.SourcesFolder, "TypeMapping")
        };
        TypeMappingTest.EnsureCreated_WithExtendedTypes(connectionString.Sandbox("Sandbox", sandboxId));
    }
}
