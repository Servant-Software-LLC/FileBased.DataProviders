using System.Reflection;
using Xunit;
using Data.Common.Extension;
using EFCore.Common.Tests;
using Microsoft.Extensions.Logging;
using EFCore.Json.Tests.Utils;
using EFCore.Json.Design.Internal;

namespace EFCore.Json.Tests.FileAsDatabase;

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
        var connectionString = ConnectionStrings.Instance.gettingStartedWithDataFileDB.Sandbox("Sandbox", sandboxId).AddLogging(LogLevel.Debug);
        ScaffoldingTests.ValidateScaffolding(connectionString, new JsonDesignTimeServices());
    }

}
