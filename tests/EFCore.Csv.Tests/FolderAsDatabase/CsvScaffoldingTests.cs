using EFCore.Csv.Tests.Utils;
using System.Reflection;
using Xunit;
using Data.Common.Extension;
using EFCore.Common.Tests;
using EFCore.Csv.Design.Internal;

namespace EFCore.Csv.Tests.FolderAsDatabase;

/// <summary>
/// This class contains tests for validating the scaffolding of CSV files as a database.
/// </summary>
public class CsvScaffoldingTests
{
    /// <summary>
    /// Validates the scaffolding of CSV files as a database.
    /// </summary>
    [Fact]
    public void ValidateScaffolding()
    {
        var sandboxId = $"{GetType().FullName}.{MethodBase.GetCurrentMethod()!.Name}";
        string connectionString = ConnectionStrings.Instance.gettingStartedFolderDB.Sandbox("Sandbox", sandboxId);
        ScaffoldingTests.ValidateScaffolding(connectionString, new CsvDesignTimeServices());
    }

}
