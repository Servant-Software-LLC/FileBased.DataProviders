using Data.Tests.Common;
using Data.Tests.Common.Utils;
using System.Data.XmlClient;
using Xunit;

namespace Data.Xml.Tests.FolderAsDatabase;

/// <summary>
/// Tests that exercise the <see cref="XmlCommand"/> class while using the 'Folder as Database' approach.
/// </summary>
public class XmlCommandTests
{
    [Fact]
    public void ExecuteScalar_ShouldReturnFirstRowFirstColumn()
    {
        CommandTests.ExecuteScalar_ShouldReturnFirstRowFirstColumn(
           () => new XmlConnection(ConnectionStrings.Instance.
           FolderAsDB));
    }

    [Fact]
    public void ExecuteScalar_ShouldCountRecords()
    {
        CommandTests.ExecuteScalar_ShouldCountRecords(
          () => new XmlConnection(ConnectionStrings.Instance.
          FolderAsDB));
    }

    [Fact]
    public void ExecuteScalar_ShouldCountSchemaTableRecords()
    {
        CommandTests.ExecuteScalar_ShouldCountSchemaTableRecords(
          () => new XmlConnection(ConnectionStrings.Instance.
          FolderAsDB));
    }

    [Fact]
    public void ExecuteNonQuery_CreateDatabase_WithExisting()
    {
        var tempFolder = FileUtils.GetTempFolderName();

        CommandTests.ExecuteNonQuery_Admin_CreateDatabase(getConnectionString =>
        {
            var connectionString = getConnectionString(ConnectionStrings.Instance);
            return new XmlConnection(connectionString);
        }, tempFolder, 0);

    }

    [Fact]
    public void ExecuteNonQuery_CreateDatabase_NewDatabase()
    {
        var tempFolder = FileUtils.GetTempFolderName();
        var myNewDatabase = Path.Combine(tempFolder, "MyNewDatabase");

        CommandTests.ExecuteNonQuery_Admin_CreateDatabase(getConnectionString =>
        {
            var connectionString = getConnectionString(ConnectionStrings.Instance);
            return new XmlConnection(connectionString);
        }, myNewDatabase, 1);


    }

    [Fact]
    public void ExecuteNonQuery_DropDatabase_WithExisting()
    {
        var tempFolder = FileUtils.GetTempFolderName();

        CommandTests.ExecuteNonQuery_Admin_DropDatabase(getConnectionString =>
        {
            var connectionString = getConnectionString(ConnectionStrings.Instance);
            return new XmlConnection(connectionString);
        }, tempFolder, 1);

    }

    [Fact]
    public void ExecuteNonQuery_DropDatabase_WithSubfolder()
    {
        var tempFolder = FileUtils.GetTempFolderName();

        //Create a sub-folder
        Directory.CreateDirectory(Path.Combine(tempFolder, "SubFolder"));

        Assert.Throws<InvalidOperationException>(() =>
            CommandTests.ExecuteNonQuery_Admin_DropDatabase(getConnectionString =>
            {
                var connectionString = getConnectionString(ConnectionStrings.Instance);
                return new XmlConnection(connectionString);
            }, tempFolder, 1)
        );


    }

    [Fact]
    public void ExecuteNonQuery_DropDatabase_WithNonDatabaseFile()
    {
        var tempFolder = FileUtils.GetTempFolderName();

        //Create a file without the provider's extension
        File.Create(Path.Combine(tempFolder, "Readme.txt"));

        Assert.Throws<InvalidOperationException>(() =>
            CommandTests.ExecuteNonQuery_Admin_DropDatabase(getConnectionString =>
            {
                var connectionString = getConnectionString(ConnectionStrings.Instance);
                return new XmlConnection(connectionString);
            }, tempFolder, 1)
        );

    }

    [Fact]
    public void ExecuteNonQuery_DropDatabase_NoDatabase()
    {
        var tempFolder = FileUtils.GetTempFolderName();
        var unexistingDatabase = $"{tempFolder}\\UnexistingDatabase";

        CommandTests.ExecuteNonQuery_Admin_DropDatabase(getConnectionString =>
        {
            var connectionString = getConnectionString(ConnectionStrings.Instance);
            return new XmlConnection(connectionString);
        }, unexistingDatabase, 0);


    }

}