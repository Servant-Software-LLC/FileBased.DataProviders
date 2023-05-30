using System.Data.FileClient;
using Xunit;

namespace Data.Json.Tests.FileAsDatabase;

public static class DataReaderTests
{
    public static void Reader_ShouldReadData<TFileParameter>(Func<FileConnection<TFileParameter>> createFileConnection)
        where TFileParameter : FileParameter<TFileParameter>, new()
    {
        // Arrange
        var connection = createFileConnection();
        connection.Open();

        // Act - Query the locations table
        var command = connection.CreateCommand("SELECT * FROM locations");
        var reader = command.ExecuteReader();

        // Assert
        Assert.True(reader.Read());
        var fieldCount = reader.FieldCount;
        Assert.Equal(4, fieldCount);

        // Act - Query the employees table
        command = connection.CreateCommand("SELECT * FROM employees");
        reader = command.ExecuteReader();

        // Assert
        Assert.True(reader.Read());
        fieldCount = reader.FieldCount;
        Assert.Equal(4, fieldCount);

        // Close the connection
        connection.Close();
    }

    public static void Reader_ShouldReturnData<TFileParameter>(Func<FileConnection<TFileParameter>> createFileConnection, bool dataTypeAlwaysString = false)
        where TFileParameter : FileParameter<TFileParameter>, new()
    {
        // Arrange
        var connection = createFileConnection();
        var command = connection.CreateCommand("SELECT * FROM [employees]");

        // Act
        connection.Open();
        using (var reader = command.ExecuteReader())
        {
            // Assert
            Assert.NotNull(reader);
            Assert.Equal(4, reader.FieldCount);

            //first Row
            Assert.True(reader.Read());
            Assert.Equal("Joe", reader["name"]);
            Assert.IsType<string>(reader["name"]);
            Assert.Equal("Joe@gmail.com", reader["email"]);
            Assert.IsType<string>(reader["email"]);
            Assert.Equal(dataTypeAlwaysString ? "56000" : 56000M, reader["salary"]);
            Assert.Equal(dataTypeAlwaysString ? "True" : true, reader["married"]);
            if (!dataTypeAlwaysString)
            {
                Assert.IsType<bool>(reader["married"]);
            }
            //second row
            Assert.True(reader.Read());
            Assert.Equal("Bob", reader["name"]);
            Assert.IsType<string>(reader["name"]);
            Assert.Equal("bob32@gmail.com", reader["email"]);
            Assert.IsType<string>(reader["email"]);
            Assert.Equal(dataTypeAlwaysString ? "95000" : 95000M, reader["salary"]);
            //this will be dbnull not bool?
            if (!dataTypeAlwaysString)
            {
                Assert.IsType<DBNull>(reader["married"]);
                Assert.IsType<decimal>(reader["salary"]);
                Assert.IsType<decimal>(reader["salary"]);
                Assert.Equal(DBNull.Value, reader["married"]);
                Assert.IsType<DBNull>(reader["married"]);
            }
        }

        connection.Close();
    }

    public static void Reader_ShouldReturnSchemaTablesData<TFileParameter>(Func<FileConnection<TFileParameter>> createFileConnection)
        where TFileParameter : FileParameter<TFileParameter>, new()
    {
        // Arrange
        var connection = createFileConnection();
        var databaseName = Path.GetFileNameWithoutExtension(connection.Database);
        var command = connection.CreateCommand("SELECT * FROM INFORMATION_SCHEMA.TABLES");

        // Act
        connection.Open();
        using (var reader = command.ExecuteReader())
        {
            // Assert
            Assert.NotNull(reader);
            Assert.Equal(3, reader.FieldCount);

            //first Row
            Assert.True(reader.Read());
            Assert.Equal(databaseName, reader["TABLE_CATALOG"]);
            var firstTableName = reader["TABLE_NAME"].ToString();
            Assert.True(string.Compare(firstTableName, "employees", true) == 0 || 
                        string.Compare(firstTableName, "locations", true) == 0);
            Assert.Equal("BASE TABLE", reader["TABLE_TYPE"]);

            //second row
            Assert.True(reader.Read());
            Assert.Equal(databaseName, reader["TABLE_CATALOG"]);
            var secondTableName = reader["TABLE_NAME"].ToString();
            Assert.True(string.Compare(secondTableName, "employees", true) == 0 || 
                        string.Compare(secondTableName, "locations", true) == 0);
            Assert.NotEqual(firstTableName, secondTableName);
            Assert.Equal("BASE TABLE", reader["TABLE_TYPE"]);

            //There is no third row.
            Assert.False(reader.Read());
        }

        connection.Close();
    }

    public static void Reader_ShouldReturnSchemaColumnsData<TFileParameter>(Func<FileConnection<TFileParameter>> createFileConnection, bool dataTypeAlwaysString = false)
        where TFileParameter : FileParameter<TFileParameter>, new()
    {
        // Arrange
        var connection = createFileConnection();
        var databaseName = Path.GetFileNameWithoutExtension(connection.Database);
        var command = connection.CreateCommand("SELECT * FROM INFORMATION_SCHEMA.COLUMNS");

        // Act
        connection.Open();
        using (var reader = command.ExecuteReader())
        {
            // Assert
            Assert.NotNull(reader);
            Assert.Equal(4, reader.FieldCount);

            //First Row
            Assert.True(reader.Read());
            Assert.Equal(databaseName, reader["TABLE_CATALOG"]);
            Assert.True(string.Compare("employees", reader["TABLE_NAME"].ToString(), true) == 0);
            Assert.True(string.Compare("email", reader["COLUMN_NAME"].ToString(), true) == 0);
            Assert.Equal(typeof(string).FullName, reader["DATA_TYPE"]);

            //Second Row
            Assert.True(reader.Read());
            Assert.Equal(databaseName, reader["TABLE_CATALOG"]);
            Assert.True(string.Compare("employees", reader["TABLE_NAME"].ToString(), true) == 0);
            Assert.True(string.Compare("married", reader["COLUMN_NAME"].ToString(), true) == 0);
            Assert.Equal((dataTypeAlwaysString ? typeof(string) : typeof(bool)).FullName, reader["DATA_TYPE"]);

            //Third Row
            Assert.True(reader.Read());
            Assert.Equal(databaseName, reader["TABLE_CATALOG"]);
            Assert.True(string.Compare("employees", reader["TABLE_NAME"].ToString(), true) == 0);
            Assert.True(string.Compare("name", reader["COLUMN_NAME"].ToString(), true) == 0);
            Assert.Equal(typeof(string).FullName, reader["DATA_TYPE"]);

            //Fourth Row
            Assert.True(reader.Read());
            Assert.Equal(databaseName, reader["TABLE_CATALOG"]);
            Assert.True(string.Compare("employees", reader["TABLE_NAME"].ToString(), true) == 0);
            Assert.True(string.Compare("salary", reader["COLUMN_NAME"].ToString(), true) == 0);
            Assert.Equal((dataTypeAlwaysString ? typeof(string) : typeof(decimal)).FullName, reader["DATA_TYPE"]);

            //Fifth Row
            Assert.True(reader.Read());
            Assert.Equal(databaseName, reader["TABLE_CATALOG"]);
            Assert.True(string.Compare("locations", reader["TABLE_NAME"].ToString(), true) == 0);
            Assert.True(string.Compare("city", reader["COLUMN_NAME"].ToString(), true) == 0);
            Assert.Equal(typeof(string).FullName, reader["DATA_TYPE"]);

            //Sixth Row
            Assert.True(reader.Read());
            Assert.Equal(databaseName, reader["TABLE_CATALOG"]);
            Assert.True(string.Compare("locations", reader["TABLE_NAME"].ToString(), true) == 0);
            Assert.True(string.Compare("id", reader["COLUMN_NAME"].ToString(), true) == 0);
            Assert.Equal((dataTypeAlwaysString ? typeof(string) : typeof(decimal)).FullName, reader["DATA_TYPE"]);

            //Seventh Row
            Assert.True(reader.Read());
            Assert.Equal(databaseName, reader["TABLE_CATALOG"]);
            Assert.True(string.Compare("locations", reader["TABLE_NAME"].ToString(), true) == 0);
            Assert.True(string.Compare("state", reader["COLUMN_NAME"].ToString(), true) == 0);
            Assert.Equal(typeof(string).FullName, reader["DATA_TYPE"]);

            //Eighth Row
            Assert.True(reader.Read());
            Assert.Equal(databaseName, reader["TABLE_CATALOG"]);
            Assert.True(string.Compare("locations", reader["TABLE_NAME"].ToString(), true) == 0);
            Assert.True(string.Compare("zip", reader["COLUMN_NAME"].ToString(), true) == 0);
            Assert.Equal((dataTypeAlwaysString ? typeof(string) : typeof(decimal)).FullName, reader["DATA_TYPE"]);

            //There is no ninth row.
            Assert.False(reader.Read());
        }

        connection.Close();
    }

    public static void Reader_ShouldReturnData_WithFilter<TFileParameter>(Func<FileConnection<TFileParameter>> createFileConnection)
        where TFileParameter : FileParameter<TFileParameter>, new()
    {
        // Arrange
        var connection = createFileConnection();
        var command = connection.CreateCommand("SELECT * FROM [locations] WHERE zip = 78132");

        // Act
        connection.Open();
        using (var reader = command.ExecuteReader())
        {
            // Assert
            Assert.NotNull(reader);
            Assert.Equal(4, reader.FieldCount);

            //first Row
            Assert.True(reader.Read());
            Assert.Equal("New Braunfels", reader["city"]);

            //No second row
            Assert.False(reader.Read());
        }

        connection.Close();
    }

    public static void Reader_ShouldReadDataWithInnerJoin<TFileParameter>(Func<FileConnection<TFileParameter>> createFileConnection)
        where TFileParameter : FileParameter<TFileParameter>, new()
    {
        // Arrange
        var connection = createFileConnection();
        connection.Open();

        // Act - Query two columns from the locations table
        var command = connection.CreateCommand("SELECT [c].[CustomerName], [o].[OrderDate], [oi].[Quantity], [p].[Name] FROM [Customers c] INNER JOIN [Orders o] ON [c].[ID] = [o].[CustomerID] INNER JOIN [OrderItems oi] ON [o].[ID] = [oi].[OrderID] INNER JOIN [Products p] ON [p].[ID] = [oi].[ProductID]");
        var reader = command.ExecuteReader();

        // Assert
        int count = 0;
        while (reader.Read())
        {
            count++;

            var fieldCount = reader.FieldCount;
            Assert.Equal(4, fieldCount);

            Assert.NotNull(reader[0]);
            Assert.NotNull(reader[1]);
            Assert.NotNull(reader[2]);
            Assert.NotNull(reader[3]);
        }

        Assert.True(count > 0, "No records where returned in the INNER JOINs");
    }

    public static void Reader_ShouldReadDataWithSelectedColumns<TFileParameter>(Func<FileConnection<TFileParameter>> createFileConnection)
        where TFileParameter : FileParameter<TFileParameter>, new()
    {
        // Arrange
        var connection = createFileConnection();
        connection.Open();

        // Act - Query two columns from the locations table
        var command = connection.CreateCommand("SELECT city, state FROM locations");
        var reader = command.ExecuteReader();

        // Assert
        Assert.True(reader.Read());
        var fieldCount = reader.FieldCount;
        Assert.Equal(2, fieldCount);

        // Act - Query two columns from the employees table
        command = connection.CreateCommand("SELECT name, salary FROM employees");
        reader = command.ExecuteReader();

        // Assert
        Assert.True(reader.Read());
        fieldCount = reader.FieldCount;
        Assert.Equal(2, fieldCount);

        // Close the connection
        connection.Close();
    }

    public static void Reader_NextResult_ShouldReadData<TFileParameter>(Func<FileConnection<TFileParameter>> createFileConnection)
        where TFileParameter : FileParameter<TFileParameter>, new()
    {
        // Arrange
        var connection = createFileConnection();
        connection.Open();

        // Act - Query the locations table
        var command = connection.CreateCommand("SELECT * FROM locations; SELECT name, email FROM employees");
        var reader = command.ExecuteReader();

        // Assert (on locations SELECT)
        Assert.True(reader.Read());
        var fieldCount = reader.FieldCount;
        Assert.Equal(4, fieldCount);

        //Act - Get the next resultset.
        var nextResult = reader.NextResult();

        // Assert
        Assert.True(nextResult);
        Assert.True(reader.Read());
        fieldCount = reader.FieldCount;
        Assert.Equal(2, fieldCount);

        //Act - There are no more resultsets.
        nextResult = reader.NextResult();

        Assert.False(nextResult);

        // Close the connection
        connection.Close();
    }

    public static void Reader_NextResult_WithInsert<TFileParameter>(Func<FileConnection<TFileParameter>> createFileConnection)
        where TFileParameter : FileParameter<TFileParameter>, new()
    {
        // Arrange
        var connection = createFileConnection();
        connection.Open();

        // Act - Query the locations table
        var command = connection.CreateCommand("INSERT INTO locations (id, city, state, zip) VALUES (50, 'New Braunfels', 'Texas', 78132); SELECT * FROM locations");
        var reader = command.ExecuteReader();

        // Assert (on locations SELECT)
        Assert.False(reader.Read());
        Assert.Equal(1, reader.RecordsAffected);
        Assert.Equal(0, reader.FieldCount);

        //Act - Get the next resultset.
        var nextResult = reader.NextResult();

        // Assert
        Assert.True(nextResult);
        Assert.True(reader.Read());
        Assert.Equal(-1, reader.RecordsAffected);
        Assert.Equal(4, reader.FieldCount);

        //Act - There are no more resultsets.
        nextResult = reader.NextResult();

        Assert.False(nextResult);

        // Close the connection
        connection.Close();
    }

}