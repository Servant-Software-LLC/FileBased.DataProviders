using System.Reflection;
using Xunit;
using Data.Common.Extension;
using EFCore.Common.Tests;
using EFCore.Json.Tests.Utils;
using EFCore.Json.Design.Internal;

namespace EFCore.Json.Tests.FolderAsDatabase;

/// <summary>
/// This class contains tests for validating the scaffolding of JSON files as a database.
/// </summary>
public class JsonScaffoldingTests
{
    /// <summary>
    /// Validates the scaffolding of JSON files as a database.
    /// </summary>
    [Fact]
    public void ValidateScaffolding()
    {
        var sandboxId = $"{GetType().FullName}.{MethodBase.GetCurrentMethod()!.Name}";
        var connectionString = ConnectionStrings.Instance.gettingStartedWithDataFolderDB.Sandbox("Sandbox", sandboxId);
        ScaffoldingTests.ValidateScaffolding(connectionString, new JsonDesignTimeServices());
    }

}
