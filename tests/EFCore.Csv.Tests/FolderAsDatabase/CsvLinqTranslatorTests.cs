using Data.Common.Extension;
using EFCore.Csv.Tests.Utils;
using System.Reflection;
using Xunit;
using LinqTests = EFCore.Common.Tests.LinqTranslatorTests<EFCore.Csv.Tests.Models.BloggingContext>;

namespace EFCore.Csv.Tests.FolderAsDatabase;

/// <summary>
/// Tests verifying LINQ query translators work correctly with the CSV EF Core provider.
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

    [Fact]
    public void StringStartsWith_FiltersCorrectly()
    {
        var sandboxId = $"{GetType().FullName}.{MethodBase.GetCurrentMethod()!.Name}";
        var cs = ConnectionStrings.Instance.gettingStartedWithDataFolderDB.Sandbox("Sandbox", sandboxId);
        LinqTests.StringStartsWith_FiltersCorrectly(cs);
    }

    [Fact]
    public void StringEndsWith_FiltersCorrectly()
    {
        var sandboxId = $"{GetType().FullName}.{MethodBase.GetCurrentMethod()!.Name}";
        var cs = ConnectionStrings.Instance.gettingStartedWithDataFolderDB.Sandbox("Sandbox", sandboxId);
        LinqTests.StringEndsWith_FiltersCorrectly(cs);
    }

    [Fact(Skip = "SqlBuildingBlocks #112: built-in function table projection mapping bug causes CurrentSourceRow column lookup to fail")]
    public void StringToUpper_TranslatesCorrectly()
    {
        var sandboxId = $"{GetType().FullName}.{MethodBase.GetCurrentMethod()!.Name}";
        var cs = ConnectionStrings.Instance.gettingStartedWithDataFolderDB.Sandbox("Sandbox", sandboxId);
        LinqTests.StringToUpper_TranslatesCorrectly(cs);
    }

    [Fact(Skip = "SqlBuildingBlocks #112: built-in function table projection mapping bug causes CurrentSourceRow column lookup to fail")]
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

    [Fact(Skip = "SqlBuildingBlocks #112: functions in WHERE clause expressions are not resolved by built-in function mechanism")]
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

    [Fact(Skip = "SqlBuildingBlocks #112: EvaluateAggregates result format mismatches EF Core expectations (Nullable object must have a value)")]
    public void Max_Aggregate_Works()
    {
        var sandboxId = $"{GetType().FullName}.{MethodBase.GetCurrentMethod()!.Name}";
        var cs = ConnectionStrings.Instance.gettingStartedWithDataFolderDB.Sandbox("Sandbox", sandboxId);
        LinqTests.Max_Aggregate_Works(cs);
    }

    [Fact(Skip = "SqlBuildingBlocks #112: EvaluateAggregates result format mismatches EF Core expectations (Nullable object must have a value)")]
    public void Min_Aggregate_Works()
    {
        var sandboxId = $"{GetType().FullName}.{MethodBase.GetCurrentMethod()!.Name}";
        var cs = ConnectionStrings.Instance.gettingStartedWithDataFolderDB.Sandbox("Sandbox", sandboxId);
        LinqTests.Min_Aggregate_Works(cs);
    }
}
