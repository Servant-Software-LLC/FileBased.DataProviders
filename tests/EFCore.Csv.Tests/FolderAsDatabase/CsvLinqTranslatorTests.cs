using Data.Common.Extension;
using EFCore.Csv.Tests.Utils;
using System.Reflection;
using Xunit;
using LinqTests = EFCore.Common.Tests.LinqTranslatorTests<EFCore.Csv.Tests.Models.BloggingContext>;

namespace EFCore.Csv.Tests.FolderAsDatabase;

/// <summary>
/// Tests verifying LINQ query translators work correctly with the CSV EF Core provider.
/// Some tests are skipped due to SqlBuildingBlocks parser limitations (IS NOT NULL,
/// SQL functions like UPPER/LOWER/LENGTH, and aggregate functions like MAX/MIN).
/// </summary>
public class CsvLinqTranslatorTests
{
    [Fact]
    public void StringContains_FiltersCorrectly()
    {
        var sandboxId = $"{GetType().FullName}.{MethodBase.GetCurrentMethod()!.Name}";
        var cs = ConnectionStrings.Instance.gettingStartedWithDataFolderDB.Sandbox("Sandbox", sandboxId);
        LinqTests.StringContains_FiltersCorrectly(cs);
    }

    [Fact(Skip = "SqlBuildingBlocks parser does not support IS NOT NULL syntax")]
    public void StringStartsWith_FiltersCorrectly()
    {
        var sandboxId = $"{GetType().FullName}.{MethodBase.GetCurrentMethod()!.Name}";
        var cs = ConnectionStrings.Instance.gettingStartedWithDataFolderDB.Sandbox("Sandbox", sandboxId);
        LinqTests.StringStartsWith_FiltersCorrectly(cs);
    }

    [Fact(Skip = "SqlBuildingBlocks parser does not support IS NOT NULL syntax")]
    public void StringEndsWith_FiltersCorrectly()
    {
        var sandboxId = $"{GetType().FullName}.{MethodBase.GetCurrentMethod()!.Name}";
        var cs = ConnectionStrings.Instance.gettingStartedWithDataFolderDB.Sandbox("Sandbox", sandboxId);
        LinqTests.StringEndsWith_FiltersCorrectly(cs);
    }

    [Fact(Skip = "SqlBuildingBlocks query engine does not handle UPPER() function")]
    public void StringToUpper_TranslatesCorrectly()
    {
        var sandboxId = $"{GetType().FullName}.{MethodBase.GetCurrentMethod()!.Name}";
        var cs = ConnectionStrings.Instance.gettingStartedWithDataFolderDB.Sandbox("Sandbox", sandboxId);
        LinqTests.StringToUpper_TranslatesCorrectly(cs);
    }

    [Fact(Skip = "SqlBuildingBlocks query engine does not handle LOWER() function")]
    public void StringToLower_TranslatesCorrectly()
    {
        var sandboxId = $"{GetType().FullName}.{MethodBase.GetCurrentMethod()!.Name}";
        var cs = ConnectionStrings.Instance.gettingStartedWithDataFolderDB.Sandbox("Sandbox", sandboxId);
        LinqTests.StringToLower_TranslatesCorrectly(cs);
    }

    [Fact]
    public void EFLike_FiltersCorrectly()
    {
        var sandboxId = $"{GetType().FullName}.{MethodBase.GetCurrentMethod()!.Name}";
        var cs = ConnectionStrings.Instance.gettingStartedWithDataFolderDB.Sandbox("Sandbox", sandboxId);
        LinqTests.EFLike_FiltersCorrectly(cs);
    }

    [Fact(Skip = "SqlBuildingBlocks query engine does not handle LENGTH() function")]
    public void StringLength_FiltersCorrectly()
    {
        var sandboxId = $"{GetType().FullName}.{MethodBase.GetCurrentMethod()!.Name}";
        var cs = ConnectionStrings.Instance.gettingStartedWithDataFolderDB.Sandbox("Sandbox", sandboxId);
        LinqTests.StringLength_FiltersCorrectly(cs);
    }

    [Fact]
    public void Count_Aggregate_Works()
    {
        var sandboxId = $"{GetType().FullName}.{MethodBase.GetCurrentMethod()!.Name}";
        var cs = ConnectionStrings.Instance.gettingStartedWithDataFolderDB.Sandbox("Sandbox", sandboxId);
        LinqTests.Count_Aggregate_Works(cs);
    }

    [Fact]
    public void CountWithPredicate_Aggregate_Works()
    {
        var sandboxId = $"{GetType().FullName}.{MethodBase.GetCurrentMethod()!.Name}";
        var cs = ConnectionStrings.Instance.gettingStartedWithDataFolderDB.Sandbox("Sandbox", sandboxId);
        LinqTests.CountWithPredicate_Aggregate_Works(cs);
    }

    [Fact(Skip = "SqlBuildingBlocks parser does not support aggregate functions in SELECT")]
    public void Max_Aggregate_Works()
    {
        var sandboxId = $"{GetType().FullName}.{MethodBase.GetCurrentMethod()!.Name}";
        var cs = ConnectionStrings.Instance.gettingStartedWithDataFolderDB.Sandbox("Sandbox", sandboxId);
        LinqTests.Max_Aggregate_Works(cs);
    }

    [Fact(Skip = "SqlBuildingBlocks parser does not support aggregate functions in SELECT")]
    public void Min_Aggregate_Works()
    {
        var sandboxId = $"{GetType().FullName}.{MethodBase.GetCurrentMethod()!.Name}";
        var cs = ConnectionStrings.Instance.gettingStartedWithDataFolderDB.Sandbox("Sandbox", sandboxId);
        LinqTests.Min_Aggregate_Works(cs);
    }
}
