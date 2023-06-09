using Data.Tests.Common;
using Data.Tests.Common.Utils;
using System.Data.XmlClient;
using Xunit;

namespace Data.Xml.Tests.FileAsDatabase;

/// <summary>
/// Tests that exercise the <see cref="XmlCommand"/> class while using the 'File as Database' approach.
/// </summary>
public class XmlCommandTests
{
    [Fact]
    public void ExecuteScalar_ShouldReturnFirstRowFirstColumn()
    {
        CommandTests.ExecuteScalar_ShouldReturnFirstRowFirstColumn(
           () => new XmlConnection(ConnectionStrings.Instance.
           FileAsDB));
    }

    [Fact]
    public void ExecuteScalar_ShouldCountRecords()
    {
        CommandTests.ExecuteScalar_ShouldCountRecords(
          () => new XmlConnection(ConnectionStrings.Instance.
          FileAsDB));
    }

    [Fact]
    public void ExecuteScalar_ShouldCountSchemaTableRecords()
    {
        CommandTests.ExecuteScalar_ShouldCountSchemaTableRecords(
          () => new XmlConnection(ConnectionStrings.Instance.
          FileAsDB));
    }

    [Fact]
    public void ExecuteNonQuery_CreateDatabase_WithExisting()
    {
        var tempFolder = FileUtils.GetTempFolderName();
        var databaseName = Path.Combine(tempFolder, "MyDatabase.xml");
        File.Create(databaseName);

        CommandTests.ExecuteNonQuery_Admin_CreateDatabase(getConnectionString =>
        {
            var connectionString = getConnectionString(ConnectionStrings.Instance);
            return new XmlConnection(connectionString);
        }, databaseName, 0);
    }

    [Fact]
    public void ExecuteNonQuery_CreateDatabase_NewDatabase()
    {
        var tempFolder = FileUtils.GetTempFolderName();
        var databaseName = Path.Combine(tempFolder, "MyDatabase.xml");

        CommandTests.ExecuteNonQuery_Admin_CreateDatabase(getConnectionString =>
        {
            var connectionString = getConnectionString(ConnectionStrings.Instance);
            return new XmlConnection(connectionString);
        }, databaseName, 1);
    }

    [Fact]
    public void ExecuteNonQuery_DropDatabase_WithExisting()
    {
        var tempFolder = FileUtils.GetTempFolderName();
        var databaseName = Path.Combine(tempFolder, "MyDatabase.xml");
        var fileStream = File.Create(databaseName);
        fileStream.Close();

        CommandTests.ExecuteNonQuery_Admin_DropDatabase(getConnectionString =>
        {
            var connectionString = getConnectionString(ConnectionStrings.Instance);
            return new XmlConnection(connectionString);
        }, databaseName, 1);
    }

    [Fact]
    public void ExecuteNonQuery_DropDatabase_NewDatabase()
    {
        var tempFolder = FileUtils.GetTempFolderName();
        var databaseName = Path.Combine(tempFolder, "MyDatabase.xml");

        CommandTests.ExecuteNonQuery_Admin_DropDatabase(getConnectionString =>
        {
            var connectionString = getConnectionString(ConnectionStrings.Instance);
            return new XmlConnection(connectionString);
        }, databaseName, 0);
    }

}