using Data.Tests.Common;
using Data.Common.Extension;
using Data.Tests.Common.Utils;
using System.Data.XlsClient;
using System.Reflection;
using Xunit;

namespace Data.Xls.Tests.FileAsDatabase;

/// <summary>
/// Tests that exercise the <see cref="XlsCommand"/> class while using the 'File as Database' approach.
/// </summary>
public class XlsCommandTests
{
    [Fact]
    public void ExecuteScalar_ShouldReturnFirstRowFirstColumn()
    {
        CommandTests.ExecuteScalar_ShouldReturnFirstRowFirstColumn(
          () => new XlsConnection(ConnectionStrings.Instance.FileAsDB), ConnectionStrings.Instance.Database.DatabaseFileName);
    }

    [Fact]
    public void ExecuteScalar_ShouldCountRecords()
    {
        CommandTests.ExecuteScalar_ShouldCountRecords(
           () => new XlsConnection(ConnectionStrings.Instance.FileAsDB), ConnectionStrings.Instance.Database.DatabaseFileName);
    }

    [Fact]
    public void ExecuteScalar_ShouldCountSchemaTableRecords()
    {
        CommandTests.ExecuteScalar_ShouldCountSchemaTableRecords(
           () => new XlsConnection(ConnectionStrings.Instance.
           FileAsDB));
    }

    [Fact]
    public void ExecuteNonQuery_CreateDatabase_WithExisting()
    {
        var tempFolder = FileUtils.GetTempFolderName();
        var databaseName = Path.Combine(tempFolder, "MyDatabase.xls");
        File.Create(databaseName);

        CommandTests.ExecuteNonQuery_Admin_CreateDatabase(getConnectionString =>
        {
            var connectionString = getConnectionString(ConnectionStrings.Instance);
            return new XlsConnection(connectionString);
        }, databaseName, 0);
    }

    [Fact]
    public void ExecuteNonQuery_CreateDatabase_NewDatabase()
    {
        var tempFolder = FileUtils.GetTempFolderName();
        var databaseName = Path.Combine(tempFolder, "MyDatabase.xls");

        CommandTests.ExecuteNonQuery_Admin_CreateDatabase(getConnectionString =>
        {
            var connectionString = getConnectionString(ConnectionStrings.Instance);
            return new XlsConnection(connectionString);
        }, databaseName, 1);
    }

    [Fact]
    public void ExecuteNonQuery_DropDatabase_WithExisting()
    {
        var tempFolder = FileUtils.GetTempFolderName();
        var databaseName = Path.Combine(tempFolder, "MyDatabase.xls");
        var fileStream = File.Create(databaseName);
        fileStream.Close();

        CommandTests.ExecuteNonQuery_Admin_DropDatabase(getConnectionString =>
        {
            var connectionString = getConnectionString(ConnectionStrings.Instance);
            return new XlsConnection(connectionString);
        }, databaseName, 1);
    }

    [Fact]
    public void ExecuteNonQuery_DropDatabase_NewDatabase()
    {
        var tempFolder = FileUtils.GetTempFolderName();
        var databaseName = Path.Combine(tempFolder, "MyDatabase.xls");

        CommandTests.ExecuteNonQuery_Admin_DropDatabase(getConnectionString =>
        {
            var connectionString = getConnectionString(ConnectionStrings.Instance);
            return new XlsConnection(connectionString);
        }, databaseName, 0);
    }

    [Fact (Skip = "XSLX table create not implemented")]
    public void ExecuteNonQuery_CreateTable()
    {
        var sandboxId = $"{GetType().FullName}.{MethodBase.GetCurrentMethod()!.Name}";
        CommandTests.ExecuteNonQuery_CreateTable(
          () => new XlsConnection(ConnectionStrings.Instance.FileAsDB.Sandbox("Sandbox", sandboxId)));
    }
}