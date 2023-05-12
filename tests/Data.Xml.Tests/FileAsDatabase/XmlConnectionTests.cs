using Data.Common.Extension;
using Data.Common.FileException;
using Data.Common.Utils.ConnectionString;
using System.Data.XmlClient;
using Xunit;

namespace Data.Xml.Tests.FileAsDatabase;

public class XmlConnectionTests
{
    [Fact]
    public void Open_DatabaseExists()
    {
        //Arrange
        using (var connection = new XmlConnection(ConnectionStrings.Instance.FileAsDB))
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
        var file = Path.Combine(FileConnectionStringTestsExtensions.SourcesFolder, $"database_BOGUS.{ConnectionStrings.Instance.Extension}");
        var connectionString = new FileConnectionString() { DataSource = file };

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
        using (var connection = new XmlConnection(ConnectionStrings.Instance.FileAsDB))
        {
            connection.Open();

            //Act
            connection.ChangeDatabase(ConnectionStrings.Instance.eComFileDataBase);
        }
    }

    [Fact]
    public void ChangeDatabase_DatabaseDoesNotExist()
    {
        //Arrange

        //Create a connection string to a DataSource value that does not exist.
        var file = Path.Combine(FileConnectionStringTestsExtensions.SourcesFolder, $"database_BOGUS.{ConnectionStrings.Instance.Extension}");

        using (var connection = new XmlConnection(ConnectionStrings.Instance.FileAsDB))
        {
            connection.Open();

            //Act
            Assert.Throws<InvalidConnectionStringException>(() => connection.ChangeDatabase(file));
        }

    }

}
