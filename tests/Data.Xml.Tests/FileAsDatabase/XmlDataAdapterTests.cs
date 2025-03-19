using Data.Common.Extension;
using Data.Tests.Common.FileAsDatabase;
using System.Data.XmlClient;
using System.Reflection;
using Xunit;

namespace Data.Xml.Tests.FileAsDatabase;

public partial class XmlDataAdapterTests
{
    [Fact]
    public void Fill_ShouldPopulateTheDataSet()
    {
        DataAdapterTests.Fill_ShouldPopulateTheDataSet(
           () => new XmlConnection(ConnectionStrings.Instance.
           FileAsDB));
    }

    [Fact]
    public void Adapter_ShouldReturnData()
    {
        DataAdapterTests.Adapter_ShouldReturnData(
            () => new XmlConnection(ConnectionStrings.Instance.
            FileAsDB));
    }

    [Fact]
    public void Fill_ShouldPopulateTheDataSet_WithFilter()
    {
        DataAdapterTests.Fill_ShouldPopulateTheDataSet_WithFilter(
                   () => new XmlConnection(ConnectionStrings.Instance.
                   FileAsDB));
    }

    [Fact]
    public void Fill_ShouldPopulateDatasetWithInnerJoinFileAsDB()
    {
        DataAdapterTests.Fill_ShouldPopulateDatasetWithInnerJoinFileAsDB(
                () => new XmlConnection(ConnectionStrings.Instance.eComFileDB));
    }

    [Fact]
    public void Adapter_ShouldReadDataWithSelectedColumns()
    {
        DataAdapterTests.Adapter_ShouldReadDataWithSelectedColumns(
              () => new XmlConnection(ConnectionStrings.Instance.
              FileAsDB));
    }

    [Fact]
    public void Update_DataAdapter_Should_Update_Existing_Row_LocationsTable()
    {
        var sandboxId = $"{GetType().FullName}.{MethodBase.GetCurrentMethod()!.Name}";
        DataAdapterTests.Update_DataAdapter_Should_Update_Existing_Row_LocationsTable(
            () => new XmlConnection(ConnectionStrings.Instance.FileAsDB.Sandbox("Sandbox", sandboxId))
        );
    }

    [Fact]
    public void Update_DataAdapter_Should_Update_Existing_Row_EmployeesTable()
    {
        var sandboxId = $"{GetType().FullName}.{MethodBase.GetCurrentMethod()!.Name}";
        DataAdapterTests.Update_DataAdapter_Should_Update_Existing_Row_EmployeesTable(
            () => new XmlConnection(ConnectionStrings.Instance.FileAsDB.Sandbox("Sandbox", sandboxId))
        );
    }

    [Fact]
    public void FillSchema_ShouldReturnDataTableWithAllColumns()
    {
        DataAdapterTests.FillSchema_ShouldReturnDataTableWithAllColumns(
             () => new XmlConnection(ConnectionStrings.Instance.
             FileAsDB));
    }

    [Fact]
    public void FillSchema_ShouldThrowInvalidOperationException_WhenSelectCommandIsNull()
    {
        DataAdapterTests.FillSchema_ShouldThrowInvalidOperationException_WhenSelectCommandIsNull(
              () => new XmlDataAdapter());
    }

    [Fact]
    public void FillSchema_ShouldThrowInvalidOperationException_WhenSelectCommandConnectionIsNull()
    {
        DataAdapterTests.FillSchema_ShouldThrowInvalidOperationException_WhenSelectCommandConnectionIsNull(
             () => new XmlConnection(ConnectionStrings.Instance.
             FileAsDB));
    }

    [Fact]
    public void CreateAdapter_ShouldThrowArgumentException_WhenSelectCommandTextIsNullOrEmpty()
    {
        DataAdapterTests.CreateAdapter_ShouldThrowArgumentException_WhenSelectCommandTextIsNullOrEmpty(
               () => new XmlConnection(ConnectionStrings.Instance.
               FileAsDB));
    }

    [Fact]
    public void GetFillParameters_ShouldReturnCorrectParametersForQueryWithoutParameters()
    {
        DataAdapterTests.GetFillParameters_ShouldReturnCorrectParametersForQueryWithoutParameters(
              () => new XmlConnection(ConnectionStrings.Instance.
              FileAsDB));
    }

    [Fact]
    public void GetFillParameters_ShouldReturnCorrectParametersForQueryWithParameters()
    {
        DataAdapterTests.GetFillParameters_ShouldReturnCorrectParametersForQueryWithParameters(
               () => new XmlConnection(ConnectionStrings.Instance.
               FileAsDB));
    }

    [Fact]
    public void GetFillParameters_ShouldReturnEmptyParametersForNonSelectQuery()
    {
        DataAdapterTests.GetFillParameters_ShouldReturnEmptyParametersForNonSelectQuery(
               () => new XmlConnection(ConnectionStrings.Instance.
               FileAsDB));
    }
}