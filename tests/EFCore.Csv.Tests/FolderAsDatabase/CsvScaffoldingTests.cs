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

    [Fact]
    public void Scaffolding_AllColumnsPresent()
    {
        var sandboxId = $"{GetType().FullName}.{MethodBase.GetCurrentMethod()!.Name}";
        string connectionString = ConnectionStrings.Instance.gettingStartedWithDataFolderDB.Sandbox("Sandbox", sandboxId);
        ScaffoldingTests.ValidateScaffolding(connectionString, new CsvDesignTimeServices(), "MockDB.RemoteTable.Csv.GettingStartedWithData", null,
            scaffoldedModel =>
            {
                Assert.NotNull(scaffoldedModel);
                Assert.NotEmpty(scaffoldedModel.AdditionalFiles);
                Assert.NotNull(scaffoldedModel.ContextFile);

                var firstAdditionalFile = scaffoldedModel.AdditionalFiles[0];
                Assert.Contains("BlogId { get; set; }", firstAdditionalFile.Code);
                Assert.Contains("Url { get; set; }", firstAdditionalFile.Code);

                var secondAdditionalFile = scaffoldedModel.AdditionalFiles[1];
                Assert.Contains("BlogId { get; set; }", secondAdditionalFile.Code);
                Assert.Contains("Content { get; set; }", secondAdditionalFile.Code);
                Assert.Contains("PostId { get; set; }", secondAdditionalFile.Code);
                Assert.Contains("Title { get; set; }", secondAdditionalFile.Code);
            }
        );
    }

}
