using Data.Json.Tests.FileAsDatabase;
using Data.Tests.Common;
using System.Data;
using System.Data.XmlClient;
using System.Reflection;
using Xunit;

namespace Data.Xml.Tests.FileAsDatabase;

public partial class XmlDataAdapterTests
{
    [Fact]
    public void DataAdapter_ShouldFillTheDataSet()
    {
        DataAdapterTests.DataAdapter_ShouldFillTheDataSet(
           () => new XmlConnection(ConnectionStrings.Instance.
           FileAsDBConnectionString));
    }

    [Fact]
    public void Adapter_ShouldReturnData()
    {
        DataAdapterTests.Adapter_ShouldReturnData(
            () => new XmlConnection(ConnectionStrings.Instance.
            FileAsDBConnectionString));
    }

    [Fact]
    public void DataAdapter_ShouldFillTheDataSet_WithFilter()
    {
        DataAdapterTests.DataAdapter_ShouldFillTheDataSet_WithFilter(
                   () => new XmlConnection(ConnectionStrings.Instance.
                   FileAsDBConnectionString));
    }

    [Fact]
    public void Adapter_ShouldFillDatasetWithInnerJoinFileAsDB()
    {
        DataAdapterTests.Adapter_ShouldFillDatasetWithInnerJoinFileAsDB(
                () => new XmlConnection(ConnectionStrings.Instance.eComFileDBConnectionString));
    }

    [Fact]
    public void Adapter_ShouldReadDataWithSelectedColumns()
    {
        DataAdapterTests.Adapter_ShouldReadDataWithSelectedColumns(
              () => new XmlConnection(ConnectionStrings.Instance.
              FileAsDBConnectionString));
    }

    [Fact]
    public void Update_DataAdapter_Should_Update_Existing_Row()
    {
        var sandboxId = $"{GetType().FullName}.{MethodBase.GetCurrentMethod()!.Name}";
        DataAdapterTests.Update_DataAdapter_Should_Update_Existing_Row(
            () => new XmlConnection(ConnectionStrings.Instance.FileAsDBConnectionString.Sandbox("Sandbox", sandboxId))
        );
    }

    [Fact]
    public void FillSchema_ShouldReturnDataTableWithAllColumns()
    {
        DataAdapterTests.FillSchema_ShouldReturnDataTableWithAllColumns(
             () => new XmlConnection(ConnectionStrings.Instance.
             FileAsDBConnectionString));
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
             FileAsDBConnectionString));
    }

    [Fact]
    public void CreateAdapter_ShouldThrowArgumentException_WhenSelectCommandTextIsNullOrEmpty()
    {
        DataAdapterTests.CreateAdapter_ShouldThrowArgumentException_WhenSelectCommandTextIsNullOrEmpty(
               () => new XmlConnection(ConnectionStrings.Instance.
               FileAsDBConnectionString));
    }

    [Fact]
    public void GetFillParameters_ShouldReturnCorrectParametersForQueryWithoutParameters()
    {
        DataAdapterTests.GetFillParameters_ShouldReturnCorrectParametersForQueryWithoutParameters(
              () => new XmlConnection(ConnectionStrings.Instance.
              FileAsDBConnectionString));
    }

    [Fact]
    public void GetFillParameters_ShouldReturnCorrectParametersForQueryWithParameters()
    {
        DataAdapterTests.GetFillParameters_ShouldReturnCorrectParametersForQueryWithParameters(
               () => new XmlConnection(ConnectionStrings.Instance.
               FileAsDBConnectionString));
    }

    [Fact]
    public void GetFillParameters_ShouldReturnEmptyParametersForNonSelectQuery()
    {
        DataAdapterTests.GetFillParameters_ShouldReturnEmptyParametersForNonSelectQuery(
               () => new XmlConnection(ConnectionStrings.Instance.
               FileAsDBConnectionString));
    }
}