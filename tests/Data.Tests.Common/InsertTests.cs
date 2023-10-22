using Data.Common.Extension;
using System.Data;
using System.Data.FileClient;
using Xunit;

namespace Data.Tests.Common;

public static class InsertTests
{
    public static void Insert_ShouldInsertData<TFileParameter>(Func<FileConnection<TFileParameter>> createFileConnection)
        where TFileParameter : FileParameter<TFileParameter>, new()
    {
        const int id = 123054;

        // Arrange
        using (var connection = createFileConnection())
        {
            connection.Open();

            // Act - Insert a new record into the locations table
            var command = connection.CreateCommand($"INSERT INTO locations (id, city, state, zip) VALUES ({id}, 'Seattle', 'Washington', 98101)");
            var rowsAffected = command.ExecuteNonQuery();

            // Assert
            Assert.Equal(1, rowsAffected);
            connection.Open();

            // Act - Verify the inserted record exists in the locations table
            command = connection.CreateCommand($"SELECT COUNT(*) FROM locations WHERE id = {id}");
            var count = (int)command.ExecuteScalar()!;

            // Assert
            Assert.True(1 == count, $"SELECTing COUNT(*) for id = {id} didn't yield 1 row.  Rows = {count}");

            // Act - Insert a new record into the employees table
            command = connection.CreateCommand("INSERT INTO employees (name, email, salary, married) VALUES ('Jim Convis', 'johndoe@example.com', 100000, 'true')");
            rowsAffected = command.ExecuteNonQuery();

            // Assert
            Assert.True(1 == rowsAffected, $"{nameof(command.ExecuteNonQuery)} indicated its rows affected wasn't 1.  RowsAffected = {rowsAffected}");

            // Close the connection
            connection.Close();
        }
    }

    public static void Insert_ShouldInsertNullData<TFileParameter>(Func<FileConnection<TFileParameter>> createFileConnection, bool dataTypeAlwaysString)
        where TFileParameter : FileParameter<TFileParameter>, new()
    {
        const decimal id = 123054;

        // Arrange
        using (var connection = createFileConnection())
        {
            connection.Open();

            // Act - Insert a new record into the locations table that has a NULL string and integer
            var command = connection.CreateCommand($"INSERT INTO locations (id, city, state, zip) VALUES ({id}, 'Seattle', NULL, NULL)");
            var rowsAffected = command.ExecuteNonQuery();

            // Assert
            Assert.Equal(1, rowsAffected);
            connection.Open();

            // Act - Verify the inserted record exists in the locations table
            command = connection.CreateCommand($"SELECT COUNT(*) FROM locations WHERE id = {id}");
            var count = (int)command.ExecuteScalar()!;

            // Assert
            Assert.True(1 == count, $"SELECTing COUNT(*) for id = {id} didn't yield 1 row.  Rows = {count}");


            //Read the row INSERT'd
            command = connection.CreateCommand($"SELECT * FROM locations WHERE id = {id}");
            var reader = command.ExecuteReader();
            Assert.True(reader.Read());
            var fieldCount = reader.FieldCount;
            Assert.Equal(4, fieldCount);

            //Verify values of the row.
            Assert.Equal(dataTypeAlwaysString ? id.ToString() : id, reader[0]);
            Assert.Equal("Seattle", reader[1]);
            Assert.True(reader.IsDBNull(2), $"Column 2 wasn't null: {(reader[2] == null ? "null" : reader[2].GetType()) }");
            Assert.True(reader.IsDBNull(3), $"Column 3 wasn't null: {(reader[3] == null ? "null" : reader[3].GetType()) }");

            // Close the connection
            connection.Close();
        }
    }

    public static void Insert_ShouldInsertDataIntoEmptyTables<TFileParameter>(Func<FileConnection<TFileParameter>> createFileConnection)
        where TFileParameter : FileParameter<TFileParameter>, new()
    {
        //Extra assertions to verify that our JSON tables now have a schema 
        using (var connection = createFileConnection())
        {
            //Copy the contents of the tables into a DataSet
            var adapter = connection.CreateDataAdapter("SELECT * FROM locations");
            var dataset = new DataSet();
            adapter.Fill(dataset);

            //Verify that the DataColumns are of the correct schema type.
            //Table: locations
            var locationsTable = dataset.Tables[0];
            var idColumn = locationsTable.Columns["id"];
            Assert.Equal(typeof(decimal), idColumn.DataType);
            var cityColumn = locationsTable.Columns["city"];
            Assert.Equal(typeof(string), cityColumn.DataType);
            var stateColumn = locationsTable.Columns["state"];
            Assert.Equal(typeof(string), stateColumn.DataType);
            var zipColumn = locationsTable.Columns["zip"];
            Assert.Equal(typeof(decimal), zipColumn.DataType);

            adapter = connection.CreateDataAdapter("SELECT * FROM employees");
            adapter.Fill(dataset);

            //Table: employees
            var employeesTable = dataset.Tables[0];
            var nameColumn = employeesTable.Columns["name"];
            Assert.Equal(typeof(string), nameColumn.DataType);
            var emailColumn = employeesTable.Columns["email"];
            Assert.Equal(typeof(string), emailColumn.DataType);
            var salaryColumn = employeesTable.Columns["salary"];
            Assert.Equal(typeof(decimal), salaryColumn.DataType);
            var marriedColumn = employeesTable.Columns["married"];

            //NOTE:  When https://github.com/Servant-Software-LLC/ADO.NET.FileBased.DataProviders/issues/22 is solved, then 
            //       this assert will fail, because it needs to have an expectation of:
            // Assert.Equal(typeof(boolean), marriedColumn.DataType);
            Assert.Equal(typeof(string), marriedColumn.DataType);
        }

    }

    public static void Insert_ShouldBeFormatted<TFileParameter>(Func<FileConnection<TFileParameter>> createFileConnection)
        where TFileParameter : FileParameter<TFileParameter>, new()
    {
        // Arrange
        var connection = createFileConnection();
        connection.Open();

        // Act - Insert a new record into the locations table
        const string locationsTableName = "locations";
        var command = connection.CreateCommand($"INSERT INTO {locationsTableName} (id, city, state, zip) VALUES (156, 'Seattle', 'Washington', 98101)");
        var rowsAffected = command.ExecuteNonQuery();

        // Assert
        var jsonFileContents = File.ReadAllText(connection.GetTablePath(locationsTableName));
        Assert.Contains("\n", jsonFileContents);
    }

    public static void Insert_ShouldBeFormattedForFile<TFileParameter>(Func<FileConnection<TFileParameter>> createFileConnection)
        where TFileParameter : FileParameter<TFileParameter>, new()
    {
        // Arrange
        var connection = createFileConnection();
        connection.Open();

        // Act - Insert a new record into the locations table
        const string locationsTableName = "locations";
        var command = connection.CreateCommand($"INSERT INTO {locationsTableName} (id, city, state, zip) VALUES (156, 'Seattle', 'Washington', 98101)");
        var rowsAffected = command.ExecuteNonQuery();

        // Assert
        var jsonFileContents = File.ReadAllText(connection.Database);
        Assert.Contains("\n", jsonFileContents);
    }

    public static void Insert_IndentityColumn_NoLastRow<TFileParameter>(Func<FileConnection<TFileParameter>> createFileConnection, bool dataTypeAlwaysString)
        where TFileParameter : FileParameter<TFileParameter>, new()
    {
        using (var connection = createFileConnection())
        {
            connection.Open();

            // Act - Insert a new record into the locations table
            var command = connection.CreateCommand($"INSERT INTO \"Blogs\" (\"Url\") VALUES ('https://www.theminimalists.com/blog/')");
            var rowsAffected = command.ExecuteNonQuery();

            // Assert
            Assert.Equal(1, rowsAffected);

            // Act - Verify that we now have a row with a BlogId == 1
            command = connection.CreateCommand($"SELECT * FROM \"Blogs\"");

            using (var reader = command.ExecuteReader())
            {
                // Assert
                Assert.NotNull(reader);
                Assert.Equal(2, reader.FieldCount);

                //first Row
                Assert.True(reader.Read());
                Assert.Equal(dataTypeAlwaysString ? "1" : 1m, reader["BlogId"]);

                //There should be no second row.
                Assert.False(reader.Read());
            }

            // Close the connection
            connection.Close();
        }
    }

    public static void Insert_IndentityColumn_LastRow_Decimal<TFileParameter>(Func<FileConnection<TFileParameter>> createFileConnection, bool dataTypeAlwaysString)
        where TFileParameter : FileParameter<TFileParameter>, new()
    {
        using (var connection = createFileConnection())
        {
            //Setup
            connection.Open();

            // Act - Insert a new record into the locations table
            var command = connection.CreateCommand($"INSERT INTO locations (city, state, zip) VALUES ('Seattle', 'Washington', 98101)");
            var rowsAffected = command.ExecuteNonQuery();

            // Assert
            Assert.Equal(1, rowsAffected);

            // Act - Verify that we now have a row with a id == 3, since the initial table's last row has an id of 2
            command = connection.CreateCommand($"SELECT * FROM locations WHERE id = 3");

            using (var reader = command.ExecuteReader())
            {
                // Assert
                Assert.NotNull(reader);
                Assert.Equal(4, reader.FieldCount);

                //first Row
                Assert.True(reader.Read());
                Assert.Equal(dataTypeAlwaysString ? "3" : 3m, reader["id"]);
                Assert.Equal("Seattle", reader["city"]);
                Assert.Equal("Washington", reader["state"]);
                Assert.Equal(dataTypeAlwaysString ? "98101" : 98101m, reader["zip"]);

                //There should be no second row.
                Assert.False(reader.Read());
            }

            // Close the connection
            connection.Close();
        }
    }

    public static void Insert_IndentityColumn_LastRow_Guid<TFileParameter>(Func<FileConnection<TFileParameter>> createFileConnection)
        where TFileParameter : FileParameter<TFileParameter>, new()
    {
        //TODO
    }

}