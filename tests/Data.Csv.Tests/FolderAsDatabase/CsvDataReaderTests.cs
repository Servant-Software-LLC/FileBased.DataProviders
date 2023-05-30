using Data.Common.Extension;
using Data.Json.Tests.FileAsDatabase;
using System.Data.CsvClient;
using System.Reflection;
using Xunit;

namespace Data.Csv.Tests.FolderAsDatabase;

/// <summary>
/// Tests that exercise the <see cref="CsvDataReader"/> class.
/// </summary>
public class CsvDataReaderTests
{
    [Fact]
    public void Reader_ShouldReadData()
    {
        DataReaderTests.Reader_ShouldReadData(() =>
            new CsvConnection(ConnectionStrings.Instance.FolderAsDB));
    }

    [Fact]
    public void Reader_ShouldReturnData()
    {
        DataReaderTests.Reader_ShouldReturnData(() =>
            new CsvConnection(ConnectionStrings.Instance.FolderAsDB), true);
    }

    [Fact]
    public void Reader_ShouldReturnSchemaTablesData()
    {
        DataReaderTests.Reader_ShouldReturnSchemaTablesData(() =>
            new CsvConnection(ConnectionStrings.Instance.FolderAsDB));
    }

    [Fact]
    public void Reader_ShouldReturnSchemaColumnsData()
    {
        DataReaderTests.Reader_ShouldReturnSchemaColumnsData(() =>
            new CsvConnection(ConnectionStrings.Instance.FolderAsDB), true);
    }

    [Fact]
    public void Reader_ShouldReturnData_WithFilter()
    {
        DataReaderTests.Reader_ShouldReturnData_WithFilter(() =>
            new CsvConnection(ConnectionStrings.Instance.FolderAsDB));
    }

    [Fact]
    public void Reader_ShouldReadDataWithInnerJoin()
    {
        DataReaderTests.Reader_ShouldReadDataWithInnerJoin(() =>
            new CsvConnection(ConnectionStrings.Instance.eComFolderDB));
    }

    [Fact]
    public void Reader_ShouldReadDataWithSelectedColumns()
    {
        DataReaderTests.Reader_ShouldReadDataWithSelectedColumns(() =>
            new CsvConnection(ConnectionStrings.Instance.FolderAsDB));
    }

    [Fact]
    public void Reader_NextResult_ShouldReadData()
    {
        DataReaderTests.Reader_NextResult_ShouldReadData(() =>
            new CsvConnection(ConnectionStrings.Instance.FolderAsDB));
    }

    [Fact]
    public void Reader_NextResult_WithInsert()
    {
        var sandboxId = $"{GetType().FullName}.{MethodBase.GetCurrentMethod()!.Name}";
        DataReaderTests.Reader_NextResult_WithInsert(() =>
            new CsvConnection(ConnectionStrings.Instance.FolderAsDB.Sandbox("Sandbox", sandboxId)));
    }

}