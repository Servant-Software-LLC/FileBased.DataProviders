using Data.Common.Extension;
using Data.Common.FileException;
using Data.Common.Utils.ConnectionString;
using System.Data.JsonClient;
using Xunit;

namespace Data.Json.Tests.FolderAsDatabase;

public class JsonConnectionTests
{
    [Fact]
    public void Open_DatabaseExists()
    {
        //Arrange
        using (var connection = new JsonConnection(ConnectionStrings.Instance.FolderAsDB))
        {

            //Act
            connection.Open();

        }
    }

    [Fact]
    public void Open_DatabaseDoesNotExist()
    {
        //Arrange

        //Create a connection string to a DataSource value that does not exist.
        var folder = Path.Combine(FileConnectionStringTestsExtensions.SourcesFolder, "Folder_BOGUS");
        var connectionString = new FileConnectionString() { DataSource = folder };

        using (var connection = new JsonConnection(connectionString))
        {

            //Act
            Assert.Throws<InvalidConnectionStringException>(() => connection.Open());
        }

    }

    [Fact]
    public void ChangeDatabase_DatabaseExists()
    {
        //Arrange
        using (var connection = new JsonConnection(ConnectionStrings.Instance.FolderAsDB))
        {
            connection.Open();

            //Act
            connection.ChangeDatabase(ConnectionStrings.Instance.eComFolderDataBase);
        }
    }

    [Fact]
    public void ChangeDatabase_DatabaseDoesNotExist()
    {
        //Arrange

        //Create a connection string to a DataSource value that does not exist.
        var folder = Path.Combine(FileConnectionStringTestsExtensions.SourcesFolder, "Folder_BOGUS");

        using (var connection = new JsonConnection(ConnectionStrings.Instance.FolderAsDB))
        {
            connection.Open();

            //Act
            Assert.Throws<InvalidConnectionStringException>(() => connection.ChangeDatabase(folder));
        }

    }


}
