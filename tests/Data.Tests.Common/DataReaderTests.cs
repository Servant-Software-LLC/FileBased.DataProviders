using Data.Common.DataSource;
using Data.Common.Extension;
using Data.Tests.Common.Extensions;
using Data.Tests.Common.POCOs;
using Data.Tests.Common.Utils;
using System.Data;
using System.Data.Common;
using System.Data.FileClient;
using Xunit;

namespace Data.Json.Tests.FileAsDatabase;

public static class DataReaderTests
{
    public static async Task Reader_ShouldReadData2<TFileParameter>(Func<FileConnection<TFileParameter>> createFileConnection)
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

        int rowsRead = 0;
        while (await reader.ReadAsync().ConfigureAwait(false))
        {
            rowsRead++;
        }
        
        // Assert
        fieldCount = reader.FieldCount;
        Assert.Equal(5, fieldCount);
        Assert.Equal(1001, rowsRead);

        // Close the connection
        connection.Close();
    }
    
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

    public static void Reader_ShouldReturnData<TFileParameter>(Func<FileConnection<TFileParameter>> createFileConnection)
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
            Assert.Equal(connection.GetProperlyTypedValue(56000), reader["salary"]);
            Assert.Equal(connection.DataTypeAlwaysString ? "True" : true, reader["married"]);
            if (!connection.DataTypeAlwaysString)
            {
                Assert.IsType<bool>(reader["married"]);
            }
            //second row
            Assert.True(reader.Read());
            Assert.Equal("Bob", reader["name"]);
            Assert.IsType<string>(reader["name"]);
            Assert.Equal("bob32@gmail.com", reader["email"]);
            Assert.IsType<string>(reader["email"]);
            Assert.Equal(connection.GetProperlyTypedValue(95000), reader["salary"]);
            //this will be dbnull not bool?
            if (!connection.DataTypeAlwaysString)
            {
                Assert.IsType<DBNull>(reader["married"]);
                Assert.Equal(DBNull.Value, reader["married"]);
                Assert.IsType(connection.PreferredFloatingPointDataType.ToType(), reader["salary"]);
            }
        }

        connection.Close();
    }

    public static void Reader_Limit_ShouldReturnOnlyFirstRow<TFileParameter>(Func<FileConnection<TFileParameter>> createFileConnection)
        where TFileParameter : FileParameter<TFileParameter>, new()
    {
        // Arrange
        var connection = createFileConnection();
        var command = connection.CreateCommand("SELECT * FROM [employees] LIMIT 1");

        // Act
        connection.Open();
        using (var reader = command.ExecuteReader())
        {
            // Assert
            Assert.NotNull(reader);
            Assert.Equal(4, reader.FieldCount);

            //first Row
            Assert.True(reader.Read(), "Unable to read the first row from the DbDataReader");
            Assert.Equal("Joe", reader["name"]);
            Assert.IsType<string>(reader["name"]);
            Assert.Equal("Joe@gmail.com", reader["email"]);
            Assert.IsType<string>(reader["email"]);
            Assert.Equal(connection.GetProperlyTypedValue(56000), reader["salary"]);
            Assert.Equal(connection.DataTypeAlwaysString ? "True" : true, reader["married"]);
            if (!connection.DataTypeAlwaysString)
            {
                Assert.IsType<bool>(reader["married"]);
            }

            //No second row
            Assert.False(reader.Read());
        }

        connection.Close();
    }

    public static void Reader_Limit_ShouldReturnOnlySecondRow<TFileParameter>(Func<FileConnection<TFileParameter>> createFileConnection)
        where TFileParameter : FileParameter<TFileParameter>, new()
    {
        // Arrange
        var connection = createFileConnection();
        var command = connection.CreateCommand("SELECT * FROM [employees] LIMIT 1,1");

        // Act
        connection.Open();
        using (var reader = command.ExecuteReader())
        {
            // Assert
            Assert.NotNull(reader);
            Assert.Equal(4, reader.FieldCount);

            //Second row in Table, but first row in reader.
            Assert.True(reader.Read());
            Assert.Equal("Bob", reader["name"]);
            Assert.IsType<string>(reader["name"]);
            Assert.Equal("bob32@gmail.com", reader["email"]);
            Assert.IsType<string>(reader["email"]);
            Assert.Equal(connection.GetProperlyTypedValue(95000), reader["salary"]);
            //this will be dbnull not bool?
            if (!connection.DataTypeAlwaysString)
            {
                Assert.IsType<DBNull>(reader["married"]);
                Assert.Equal(DBNull.Value, reader["married"]);
                Assert.IsType(connection.PreferredFloatingPointDataType.ToType(), reader["salary"]);
            }

            //No more rows
            Assert.False(reader.Read());
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

    public static void Reader_ShouldReturnSchemaColumnsData<TFileParameter>(Func<FileConnection<TFileParameter>> createFileConnection)
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
            Assert.True(reader.Read(), $"Unable to read the first row of the {nameof(DbDataReader)} in INFORMATION_SCHEMA.COLUMNS");
            Assert.Equal(databaseName, reader["TABLE_CATALOG"]);
            Assert.True(string.Compare("employees", reader["TABLE_NAME"].ToString(), true) == 0);
            Assert.True(string.Compare("email", reader["COLUMN_NAME"].ToString(), true) == 0);
            Assert.Equal(typeof(string).FullName, reader["DATA_TYPE"]);

            //Second Row
            Assert.True(reader.Read());
            Assert.Equal(databaseName, reader["TABLE_CATALOG"]);
            Assert.True(string.Compare("employees", reader["TABLE_NAME"].ToString(), true) == 0);
            Assert.True(string.Compare("married", reader["COLUMN_NAME"].ToString(), true) == 0);
            Assert.Equal((connection.DataTypeAlwaysString ? typeof(string) : typeof(bool)).FullName, reader["DATA_TYPE"]);

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
            Assert.Equal((connection.DataTypeAlwaysString ? typeof(string) : connection.PreferredFloatingPointDataType.ToType()).FullName, reader["DATA_TYPE"]);

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
            Assert.Equal((connection.DataTypeAlwaysString ? typeof(string) : connection.PreferredFloatingPointDataType.ToType()).FullName, reader["DATA_TYPE"]);

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
            Assert.Equal((connection.DataTypeAlwaysString ? typeof(string) : connection.PreferredFloatingPointDataType.ToType()).FullName, reader["DATA_TYPE"]);

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
        var command = connection.CreateCommand(@"
SELECT [c].[CustomerName], [o].[OrderDate], [oi].[Quantity], [p].[Name] 
  FROM [Customers] c 
  INNER JOIN [Orders] o ON [c].[ID] = [o].[CustomerID] 
  INNER JOIN [OrderItems] oi ON [o].[ID] = [oi].[OrderID] 
  INNER JOIN [Products] p ON [p].[ID] = [oi].[ProductID]
");
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

    public static void Reader_NextResult_UpdateReturningOne<TFileParameter>(Func<FileConnection<TFileParameter>> createFileConnection)
        where TFileParameter : FileParameter<TFileParameter>, new()
    {
        // Arrange
        var connection = createFileConnection();
        connection.Open();

        // Act - Query the locations table
        var command = connection.CreateCommand("UPDATE locations SET city='Los Angeles' where id=2 RETURNING 1");
        var reader = command.ExecuteReader();

        // Assert
        Assert.True(reader.Read());
        var fieldCount = reader.FieldCount;
        Assert.Equal(1, fieldCount);
        Assert.Equal(1, reader.RecordsAffected);

        //Make sure that the value of 1 was returned in a row.
        Assert.Equal(1, reader.GetInt32(0));
        Assert.False(reader.Read());

        //Act - There are no more resultsets.
        var nextResult = reader.NextResult();

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

        //NOTE:  ADO.NET provider behavior is that statements will get executed in turn until a resultset is
        //produced.  Therefore, the ExecuteReader() method call here will execute both the INSERT and SELECT.
        //(i.e. in other words for this scenario, NextResult() does not have to be called.
        var command = connection.CreateCommand("INSERT INTO locations (id, city, state, zip) VALUES (50, 'New Braunfels', 'Texas', 78132); SELECT * FROM locations");
        var reader = command.ExecuteReader();

        // Assert (on locations SELECT)
        Assert.True(reader.Read());
        Assert.Equal(1, reader.RecordsAffected);
        Assert.Equal(4, reader.FieldCount);

        //Act - There are no more resultsets.
        var nextResult = reader.NextResult();

        Assert.False(nextResult);

        // Close the connection
        connection.Close();
    }


    /// <summary>
    /// Test functions in SELECT - specifically LAST_INSERT_ID() and ROW_COUNT() (See https://github.com/Servant-Software-LLC/ADO.NET.FileBased.DataProviders/issues/32)
    /// </summary>
    /// <typeparam name="TFileParameter"></typeparam>
    /// <param name="createFileConnection"></param>
    public static void Reader_NextResult_WithFunctions<TFileParameter>(Func<FileConnection<TFileParameter>> createFileConnection)
        where TFileParameter : FileParameter<TFileParameter>, new()
    {
        // Arrange
        var connection = createFileConnection();
        connection.Open();

        // Act - Query the locations table

        //NOTE:  ADO.NET provider behavior is that statements will get executed in turn until a resultset is
        //produced.  Therefore, the ExecuteReader() method call here will execute both the INSERT and SELECT.
        //(i.e. in other words for this scenario, NextResult() does not have to be called.
        var command = connection.CreateCommand("INSERT INTO locations (city, state, zip) VALUES ('New Braunfels', 'Texas', 78132); SELECT LAST_INSERT_ID() WHERE ROW_COUNT() = 1");
        var reader = command.ExecuteReader();

        // Assert (on locations SELECT)
        Assert.True(reader.Read());
        Assert.Equal(1, reader.RecordsAffected);
        Assert.Equal(1, reader.FieldCount);

        //Make sure that the new identity value for locations.id was created.
        Assert.Equal(3, reader.GetInt32(0));

        //Act - There are no more resultsets.
        var nextResult = reader.NextResult();

        Assert.False(nextResult);

        // Close the connection
        connection.Close();
    }

    public static void Reader_TableAlias<TFileParameter>(Func<FileConnection<TFileParameter>> createFileConnection)
        where TFileParameter : FileParameter<TFileParameter>, new()
    {
        // Arrange
        var connection = createFileConnection();
        connection.Open();

        // Act - Query the locations table

        var command = connection.CreateCommand("SELECT \"l\".\"id\", \"l\".\"city\"\r\n    FROM \"locations\" AS \"l\"\r\n    ORDER BY \"l\".\"id\"\r\n    LIMIT 1");
        var reader = command.ExecuteReader();

        // Assert

        //TODO
    }

    public static void Reader_Supports_Large_Data_Files<TFileParameter>(Func<FileConnection<TFileParameter>> createFileConnection, UnendingStream unendingStream)
        where TFileParameter : FileParameter<TFileParameter>, new()
    {
        const string tableName = nameof(TestRecord);

        var connection = createFileConnection();
        StreamedDataSource dataSourceProvider = new(tableName, unendingStream);
        connection.DataSourceProvider = dataSourceProvider;

        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = $"SELECT * FROM {tableName}";
        var reader = command.ExecuteReader();

        //This should not go into an infinite loop, reading in all of the stream.
        Assert.True(reader.Read());


        Assert.Equal(2, reader.FieldCount);

        //Validate the first record
        Assert.Equal(0, reader.GetInt32(nameof(TestRecord.Id)));
        Assert.Equal("Value 0", reader.GetString(nameof(TestRecord.Value)));
    }
}
