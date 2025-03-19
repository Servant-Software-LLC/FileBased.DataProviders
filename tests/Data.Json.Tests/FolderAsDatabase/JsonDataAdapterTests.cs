using Data.Json.Tests.FileAsDatabase;
using Data.Common.Extension;
using System.Data.JsonClient;
using System.Reflection;
using Xunit;
using Data.Tests.Common.FileAsDatabase;

namespace Data.Json.Tests.FolderAsDatabase;

public partial class JsonDataAdapterTests
{
    [Fact]
    public void Fill_ShouldPopulateTheDataSet()
    {
        DataAdapterTests.Fill_ShouldPopulateTheDataSet(
               () => new JsonConnection(ConnectionStrings.Instance.
               FolderAsDB));
    }

    [Fact]
    public void Adapter_ShouldReturnData()
    {
        DataAdapterTests.Adapter_ShouldReturnData(
               () => new JsonConnection(ConnectionStrings.Instance.
               FolderAsDB));
    }

    [Fact]
    public void Fill_ShouldPopulateTheDataSet_WithFilter()
    {
        DataAdapterTests.Fill_ShouldPopulateTheDataSet_WithFilter(
                       () => new JsonConnection(ConnectionStrings.Instance.
                       FolderAsDB));
    }

    [Fact(Skip = "Temp: Not needed for current goal")]
    public void Adapter_ShouldFillDatasetWithInnerJoinFromFolderAsDB()
    {
        DataAdapterTests.Adapter_ShouldFillDatasetWithInnerJoin(
                () => new JsonConnection(ConnectionStrings.Instance.eComFolderDB));
    }

    [Fact]
    public void Adapter_ShouldReadDataWithSelectedColumns()
    {
        DataAdapterTests.Adapter_ShouldReadDataWithSelectedColumns(
                     () => new JsonConnection(ConnectionStrings.Instance.
                     FolderAsDB));
    }

    [Fact]
    public void Update_DataAdapter_Should_Update_Existing_Row_LocationsTable()
    {
        var sandboxId = $"{GetType().FullName}.{MethodBase.GetCurrentMethod()!.Name}";
        DataAdapterTests.Update_DataAdapter_Should_Update_Existing_Row_LocationsTable(
            () => new JsonConnection(ConnectionStrings.Instance.FolderAsDB.Sandbox("Sandbox", sandboxId))
        );
    }

    [Fact]
    public void Update_DataAdapter_Should_Update_Existing_Row_EmployeesTable()
    {
        var sandboxId = $"{GetType().FullName}.{MethodBase.GetCurrentMethod()!.Name}";
        DataAdapterTests.Update_DataAdapter_Should_Update_Existing_Row_EmployeesTable(
            () => new JsonConnection(ConnectionStrings.Instance.FolderAsDB.Sandbox("Sandbox", sandboxId))
        );
    }

    [Fact]
    public void FillSchema_ShouldReturnDataTableWithAllColumns()
    {
        DataAdapterTests.FillSchema_ShouldReturnDataTableWithAllColumns(
             () => new JsonConnection(ConnectionStrings.Instance.
             FolderAsDB));
    }

    [Fact]
    public void FillSchema_ShouldThrowInvalidOperationException_WhenSelectCommandIsNull()
    {
        DataAdapterTests.FillSchema_ShouldThrowInvalidOperationException_WhenSelectCommandIsNull(
              () => new JsonDataAdapter());
    }

    [Fact]
    public void FillSchema_ShouldThrowInvalidOperationException_WhenSelectCommandConnectionIsNull()
    {
        DataAdapterTests.FillSchema_ShouldThrowInvalidOperationException_WhenSelectCommandConnectionIsNull(
             () => new JsonConnection(ConnectionStrings.Instance.
             FolderAsDB));
    }

    [Fact]
    public void CreateAdapter_ShouldThrowArgumentException_WhenSelectCommandTextIsNullOrEmpty()
    {
        DataAdapterTests.CreateAdapter_ShouldThrowArgumentException_WhenSelectCommandTextIsNullOrEmpty(
               () => new JsonConnection(ConnectionStrings.Instance.
               FolderAsDB));
    }

    [Fact]
    public void GetFillParameters_ShouldReturnCorrectParametersForQueryWithoutParameters()
    {
        DataAdapterTests.GetFillParameters_ShouldReturnCorrectParametersForQueryWithoutParameters(
              () => new JsonConnection(ConnectionStrings.Instance.
              FolderAsDB));
    }

    [Fact]
    public void GetFillParameters_ShouldReturnCorrectParametersForQueryWithParameters()
    {
        DataAdapterTests.GetFillParameters_ShouldReturnCorrectParametersForQueryWithParameters(
               () => new JsonConnection(ConnectionStrings.Instance.
               FolderAsDB));
    }

    [Fact]
    public void GetFillParameters_ShouldReturnEmptyParametersForNonSelectQuery()
    {
        DataAdapterTests.GetFillParameters_ShouldReturnEmptyParametersForNonSelectQuery(
               () => new JsonConnection(ConnectionStrings.Instance.
               FolderAsDB));
    }
}