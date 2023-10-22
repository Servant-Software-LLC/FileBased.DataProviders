using Data.Common.Extension;
using Data.Common.FileException;
using Data.Common.Utils.ConnectionString;
using Data.Tests.Common.Utils;
using System.Data.FileClient;
using Xunit;

namespace Data.Tests.Common;

public static class ConnectionTests
{
    public static void OpenConnection_AdminMode_Success<TFileParameter>(Func<Func<ConnectionStringsBase, FileConnectionString>, FileConnection<TFileParameter>> createConnection)
        where TFileParameter : FileParameter<TFileParameter>, new()
    {
        // Arrange
        var connection = createConnection(connString => connString.Admin);

        // Act (and Assert that no exception occurs)
        connection.Open();

    }

    public static void Open_DatabaseExists<TFileParameter>(Func<FileConnection<TFileParameter>> createConnection)
        where TFileParameter : FileParameter<TFileParameter>, new()
    {
        //Arrange
        using (var connection = createConnection())
        {

            //Act (and Assert that no exception occurs)
            connection.Open();
        }
    }

    public static void Open_DatabaseDoesNotExist<TFileParameter>(Func<FileConnection<TFileParameter>> createConnection)
        where TFileParameter : FileParameter<TFileParameter>, new()
    {
        //Arrange

        //Create a connection string to a DataSource value that does not exist.
        var folder = Path.Combine(FileConnectionStringTestsExtensions.SourcesFolder, "Folder_BOGUS");
        var connectionString = new FileConnectionString() { DataSource = folder };

        using (var connection = createConnection())
        {
            //Act
            Assert.Throws<InvalidConnectionStringException>(() => connection.Open());
        }

    }

    public static void Open_DatabaseDoesNotExist_AutoCreate<TFileParameter>(Func<FileConnection<TFileParameter>> createConnection)
        where TFileParameter : FileParameter<TFileParameter>, new()
    {
        //Arrange
        using (var connection = createConnection())
        {
            var connectionStringWithCreate = new FileConnectionString(connection.ConnectionString).AddCreateIfNotExist(true);
            connection.ConnectionString = connectionStringWithCreate.ConnectionString;

            //Act (and Assert that no exception occurs)
            connection.Open();
        }
    }

    public static void ChangeDatabase_DatabaseExists<TFileParameter>(
            Func<FileConnection<TFileParameter>> createConnection,
            string databaseName)
        where TFileParameter : FileParameter<TFileParameter>, new()
    {
        //Arrange
        using (var connection = createConnection())
        {
            connection.Open();

            //Act
            connection.ChangeDatabase(databaseName);
        }
    }

    public static void ChangeDatabase_DatabaseDoesNotExist<TFileParameter>(
            Func<FileConnection<TFileParameter>> createConnection,
            string bogusDatabaseName)
        where TFileParameter : FileParameter<TFileParameter>, new()
    {
        //Arrange

        //Create a connection string to a DataSource value that does not exist.
        var file = Path.Combine(FileConnectionStringTestsExtensions.SourcesFolder, bogusDatabaseName);

        using (var connection = createConnection())
        {
            connection.Open();

            //Act
            Assert.Throws<InvalidConnectionStringException>(() => connection.ChangeDatabase(file));
        }
    }

    //Even if the connection string says to create the database, the standard behavior by other ADO.NET providers is to
    //still throw and NOT create the database.
    public static void ChangeDatabase_DatabaseDoesNotExist_AutoCreate<TFileParameter>(
            Func<FileConnection<TFileParameter>> createConnection,
            string bogusDatabaseName)
        where TFileParameter : FileParameter<TFileParameter>, new()
    {
        //Arrange

        //Create a connection string to a DataSource value that does not exist.
        var file = Path.Combine(FileConnectionStringTestsExtensions.SourcesFolder, bogusDatabaseName);

        using (var connection = createConnection())
        {
            var connectionStringWithCreate = new FileConnectionString(connection.ConnectionString).AddCreateIfNotExist(true);
            connection.ConnectionString = connectionStringWithCreate.ConnectionString;

            connection.Open();

            //Act
            Assert.Throws<InvalidConnectionStringException>(() => connection.ChangeDatabase(file));
        }
    }

}
