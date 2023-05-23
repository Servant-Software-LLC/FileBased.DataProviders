using Data.Csv.Tests.FolderAsDatabase;
using Data.Tests.Common.Utils;
using System.Data.JsonClient;
using Xunit;

namespace Data.Json.Tests.FileAsDatabase;

/// <summary>
/// Tests that exercise the <see cref="JsonCommand"/> class while using the 'File as Database' approach.
/// </summary>
public class JsonCommandTests
{
    [Fact]
    public void ExecuteScalar_ShouldReturnFirstRowFirstColumn()
    {
        CommandTests.ExecuteScalar_ShouldReturnFirstRowFirstColumn(
          () => new JsonConnection(ConnectionStrings.Instance.
          FileAsDB));
    }

    [Fact]
    public void ExecuteScalar_ShouldCountRecords()
    {
        CommandTests.ExecuteScalar_ShouldCountRecords(
           () => new JsonConnection(ConnectionStrings.Instance.
           FileAsDB));
    }

    [Fact]
    public void ExecuteScalar_ShouldCountSchemaTableRecords()
    {
        CommandTests.ExecuteScalar_ShouldCountSchemaTableRecords(
           () => new JsonConnection(ConnectionStrings.Instance.
           FileAsDB));
    }

    [Fact]
    public void ExecuteNonQuery_CreateDatabase_WithExisting()
    {
        var tempFolder = FileUtils.GetTempFolderName();
        var databaseName = Path.Combine(tempFolder, "MyDatabase.json");
        File.Create(databaseName);

        CommandTests.ExecuteNonQuery_Admin_CreateDatabase(getConnectionString =>
        {
            var connectionString = getConnectionString(ConnectionStrings.Instance);
            return new JsonConnection(connectionString);
        }, databaseName, 0);
    }

    [Fact]
    public void ExecuteNonQuery_CreateDatabase_NewDatabase()
    {
        var tempFolder = FileUtils.GetTempFolderName();
        var databaseName = Path.Combine(tempFolder, "MyDatabase.json");

        CommandTests.ExecuteNonQuery_Admin_CreateDatabase(getConnectionString =>
        {
            var connectionString = getConnectionString(ConnectionStrings.Instance);
            return new JsonConnection(connectionString);
        }, databaseName, 1);
    }

    [Fact]
    public void ExecuteNonQuery_DropDatabase_WithExisting()
    {
        var tempFolder = FileUtils.GetTempFolderName();
        var databaseName = Path.Combine(tempFolder, "MyDatabase.json");
        var fileStream = File.Create(databaseName);
        fileStream.Close();

        CommandTests.ExecuteNonQuery_Admin_DropDatabase(getConnectionString =>
        {
            var connectionString = getConnectionString(ConnectionStrings.Instance);
            return new JsonConnection(connectionString);
        }, databaseName, 1);
    }

    [Fact]
    public void ExecuteNonQuery_DropDatabase_NewDatabase()
    {
        var tempFolder = FileUtils.GetTempFolderName();
        var databaseName = Path.Combine(tempFolder, "MyDatabase.json");

        CommandTests.ExecuteNonQuery_Admin_DropDatabase(getConnectionString =>
        {
            var connectionString = getConnectionString(ConnectionStrings.Instance);
            return new JsonConnection(connectionString);
        }, databaseName, 0);
    }

}