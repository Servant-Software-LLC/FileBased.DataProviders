using Data.Json.Tests.FileAsDatabase;
using System.Data.CsvClient;
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
        new CsvConnection(ConnectionStrings.Instance
        .FolderAsDBConnectionString));
    }

    [Fact]
    public void Reader_ShouldReturnData()
    {
        DataReaderTests.Reader_ShouldReturnData(() =>
       new CsvConnection(ConnectionStrings.Instance
       .FolderAsDBConnectionString), true);
    }

    [Fact]
    public void Reader_ShouldReturnData_WithFilter()
    {
        DataReaderTests.Reader_ShouldReturnData_WithFilter(() =>
       new CsvConnection(ConnectionStrings.Instance
       .FolderAsDBConnectionString));
    }

    [Fact]
    public void Reader_ShouldReadDataWithInnerJoin()
    {
        DataReaderTests.Reader_ShouldReadDataWithInnerJoin(() =>
       new CsvConnection(ConnectionStrings.Instance
       .eComFolderDBConnectionString));
    }

    [Fact]
    public void Reader_ShouldReadDataWithSelectedColumns()
    {
        DataReaderTests.Reader_ShouldReadDataWithSelectedColumns(() =>
     new CsvConnection(ConnectionStrings.Instance
     .FolderAsDBConnectionString));
    }
}