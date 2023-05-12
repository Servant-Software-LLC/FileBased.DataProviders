using Data.Common.Extension;
using Data.Common.FileException;
using Data.Common.Utils.ConnectionString;
using System.Data.XmlClient;
using Xunit;

namespace Data.Xml.Tests.FolderAsDatabase;

public class XmlConnectionTests
{
    [Fact]
    public void Open_DatabaseExists()
    {
        //Arrange
        using (var connection = new XmlConnection(ConnectionStrings.Instance.FolderAsDB))
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

        using (var connection = new XmlConnection(connectionString))
        {

            //Act
            Assert.Throws<InvalidConnectionStringException>(() => connection.Open());
        }

    }

    [Fact]
    public void ChangeDatabase_DatabaseExists()
    {
        //Arrange
        using (var connection = new XmlConnection(ConnectionStrings.Instance.FolderAsDB))
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

        using (var connection = new XmlConnection(ConnectionStrings.Instance.FolderAsDB))
        {
            connection.Open();

            //Act
            Assert.Throws<InvalidConnectionStringException>(() => connection.ChangeDatabase(folder));
        }

    }

}
