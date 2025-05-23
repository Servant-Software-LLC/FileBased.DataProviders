using Data.Common.Extension;
using Data.Common.Utils.ConnectionString;
using Data.Tests.Common.FileAsDatabase;
using Data.Xls.DataSource;
using System.Data.XlsClient;
using System.Reflection;
using Xunit;

namespace Data.Xls.Tests.FileAsDatabase;

public class XlsDataReaderTests
{    
    [Fact]
    public void Reader_ShouldReadData()
    {
        DataReaderTests.Reader_ShouldReadData(() => new XlsConnection(ConnectionStrings.Instance.FileAsDB));
    }

    [Fact]
    public void Reader_ShouldReturnData()
    {
        DataReaderTests.Reader_ShouldReturnData(() => new XlsConnection(ConnectionStrings.Instance.FileAsDB));
    }

    [Fact]
    public void Reader_ShouldReturnData_StreamedDataSource()
    {
        DataReaderTests.Reader_ShouldReturnData(() =>
        {
            var connection = new XlsConnection(FileConnectionString.CustomDataSource);
            var databaseFile = ConnectionStrings.Instance.Database.File;
            var streamXlsFile = File.OpenRead(databaseFile);
            var dataSourceProvider = new XlsStreamedDataSource(databaseFile, streamXlsFile);
            connection.DataSourceProvider = dataSourceProvider;
            return connection;
        });
       
    }

    [Fact]
    public void Reader_ShouldReturnDateTimeType()
    {
        DataReaderTests.Reader_ShouldReturnDateTimeType(() =>
            new XlsConnection(ConnectionStrings.Instance.WithDateTimeFileAsDB));
    }

    [Fact]
    public void Reader_ShouldReadCellsWithCommaInItsValue()
    {
        DataReaderTests.Reader_ShouldReadCellsWithCommaInItsValue( () => 
            new XlsConnection(ConnectionStrings.Instance.CellsWithCommaAsDB));
    }

    [Fact]
    public void Reader_ShouldReadFormulasAsString()
    {
        DataReaderTests.Reader_ShouldReadFormulasAsString(() =>
            new XlsConnection(ConnectionStrings.Instance.WithFormulaFileAsDB));
    }

    [Fact]
    public void Reader_ShouldReadEmptyCells()
    {
        DataReaderTests.Reader_ShouldReadEmptyCells(() => new XlsConnection(ConnectionStrings.Instance.EmptyCellsAsDB));
    }

    [Fact]
    public void Reader_Limit_ShouldReturnOnlyFirstRow()
    {
        DataReaderTests.Reader_Limit_ShouldReturnOnlyFirstRow(() =>
       new XlsConnection(ConnectionStrings.Instance.FileAsDB));
    }

    [Fact]
    public void Reader_Limit_ShouldReturnOnlySecondRow()
    {
        DataReaderTests.Reader_Limit_ShouldReturnOnlySecondRow(() =>
       new XlsConnection(ConnectionStrings.Instance.FileAsDB));
    }

    [Fact]
    public void Reader_ShouldReturnSchemaTablesData()
    {
        DataReaderTests.Reader_ShouldReturnSchemaTablesData(() =>
            new XlsConnection(ConnectionStrings.Instance.FileAsDB));
    }

    [Fact]
    public void Reader_ShouldReturnSchemaColumnsData()
    {
        DataReaderTests.Reader_ShouldReturnSchemaColumnsData(() =>
            new XlsConnection(ConnectionStrings.Instance.FileAsDB));
    }

    [Fact]
    public void Reader_ShouldReturnData_WithFilter()
    {
        DataReaderTests.Reader_ShouldReturnData_WithFilter(() =>
       new XlsConnection(ConnectionStrings.Instance.FileAsDB));
    }

    [Fact(Skip = "Temp: Not needed for current goal")]
    public void Reader_ShouldReadDataWithInnerJoin()
    {
        DataReaderTests.Reader_ShouldReadDataWithInnerJoin(() =>
       new XlsConnection(ConnectionStrings.Instance.eComFileDB));
    }

    [Fact]
    public void Reader_ShouldReadDataWithSelectedColumns()
    {
        DataReaderTests.Reader_ShouldReadDataWithSelectedColumns(() =>
            new XlsConnection(ConnectionStrings.Instance.FileAsDB));
    }

    [Fact(Skip = "Update for XLSX not implemented")]
    public void Reader_NextResult_UpdateReturningOne()
    {
        var sandboxId = $"{GetType().FullName}.{MethodBase.GetCurrentMethod()!.Name}";
        DataReaderTests.Reader_NextResult_UpdateReturningOne(() =>
            new XlsConnection(ConnectionStrings.Instance.FileAsDB.Sandbox("Sandbox", sandboxId)));
    }

    [Fact]
    public void Reader_NextResult_ShouldReadData()
    {
        DataReaderTests.Reader_NextResult_ShouldReadData(() =>
            new XlsConnection(ConnectionStrings.Instance.FileAsDB));
    }

    [Fact(Skip = "Update for XLSX not implemented")]
    public void Reader_NextResult_WithInsert()
    {
        var sandboxId = $"{GetType().FullName}.{MethodBase.GetCurrentMethod()!.Name}";
        DataReaderTests.Reader_NextResult_WithInsert(() =>
            new XlsConnection(ConnectionStrings.Instance.FileAsDB.Sandbox("Sandbox", sandboxId)));
    }

    [Fact(Skip = "Insert for XLSX not implemented")]
    public void Reader_NextResult_WithFunctions()
    {
        var sandboxId = $"{GetType().FullName}.{MethodBase.GetCurrentMethod()!.Name}";
        DataReaderTests.Reader_NextResult_WithFunctions(() =>
            new XlsConnection(ConnectionStrings.Instance.FileAsDB.Sandbox("Sandbox", sandboxId)));
    }
}