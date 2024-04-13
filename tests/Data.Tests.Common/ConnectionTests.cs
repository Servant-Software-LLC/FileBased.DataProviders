using Data.Common.Extension;
using Data.Common.FileException;
using Data.Common.Utils.ConnectionString;
using Data.Tests.Common.Utils;
using System.Data;
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

    public static void GetSchema_Tables<TFileParameter>(Func<FileConnection<TFileParameter>> createConnection)
        where TFileParameter : FileParameter<TFileParameter>, new()
    {
        //Arrange
        DataTable schemaTable = null;
        using (var connection = createConnection())
        {
            var connectionStringWithCreate = new FileConnectionString(connection.ConnectionString).AddCreateIfNotExist(true);
            connection.ConnectionString = connectionStringWithCreate.ConnectionString;

            connection.Open();

            //Act
            schemaTable = connection.GetSchema("Tables");
        }

        //Assert
        Assert.NotNull(schemaTable);
        Assert.Equal(2, schemaTable.Columns.Count);
        Assert.Equal(2, schemaTable.Rows.Count);

        var dataView = new DataView(schemaTable);
        dataView.Sort = "TABLE_NAME ASC";
        var sortedSchemaTable = dataView.ToTable();

        AssertTableMetadata(sortedSchemaTable.Rows[0], "employees", "BASE TABLE");
        AssertTableMetadata(sortedSchemaTable.Rows[1], "locations", "BASE TABLE");

    }

    public static void GetSchema_Columns<TFileParameter>(Func<FileConnection<TFileParameter>> createConnection)
        where TFileParameter : FileParameter<TFileParameter>, new()
    {
        //Arrange
        DataTable schemaTable = null;
        bool dataTypeAlwaysString;
        using (var connection = createConnection())
        {
            var connectionStringWithCreate = new FileConnectionString(connection.ConnectionString).AddCreateIfNotExist(true);
            connection.ConnectionString = connectionStringWithCreate.ConnectionString;

            connection.Open();

            dataTypeAlwaysString = createConnection().DataTypeAlwaysString;

            //Act
            schemaTable = connection.GetSchema("Columns");
        }

        //Assert
        Assert.NotNull(schemaTable);
        Assert.Equal(3, schemaTable.Columns.Count);
        Assert.Equal(8, schemaTable.Rows.Count);

        var dataView = new DataView(schemaTable);
        dataView.Sort = "TABLE_NAME ASC, COLUMN_NAME ASC";
        var sortedSchemaTable = dataView.ToTable();

        var boolDataType = (dataTypeAlwaysString ? typeof(string) : typeof(bool)).FullName!;
        var decimalDataType = (dataTypeAlwaysString ? typeof(string) : typeof(decimal)).FullName!;

        AssertColumnMetadata(sortedSchemaTable.Rows[0], "employees", "email", typeof(string).FullName!);
        AssertColumnMetadata(sortedSchemaTable.Rows[1], "employees", "married", boolDataType);
        AssertColumnMetadata(sortedSchemaTable.Rows[2], "employees", "name", typeof(string).FullName!);
        AssertColumnMetadata(sortedSchemaTable.Rows[3], "employees", "salary", decimalDataType);

        AssertColumnMetadata(sortedSchemaTable.Rows[4], "locations", "city", typeof(string).FullName!);
        AssertColumnMetadata(sortedSchemaTable.Rows[5], "locations", "id", decimalDataType);
        AssertColumnMetadata(sortedSchemaTable.Rows[6], "locations", "state", typeof(string).FullName!);
        AssertColumnMetadata(sortedSchemaTable.Rows[7], "locations", "zip", decimalDataType);
    }

    private static void AssertTableMetadata(DataRow row, string tableName, string tableType)
    {
        Assert.Equal(tableName, row["TABLE_NAME"]);
        Assert.Equal(tableType, row["TABLE_TYPE"]);
    }

    private static void AssertColumnMetadata(DataRow row, string tableName, string columnName, string dataType)
    {
        Assert.Equal(tableName, row["TABLE_NAME"]);
        Assert.Equal(columnName, row["COLUMN_NAME"]);
        Assert.Equal(dataType, row["DATA_TYPE"]);
    }   
}
