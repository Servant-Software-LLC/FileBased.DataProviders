using Data.Common.Extension;
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

    public static void Insert_IndentityColumn_NoLastRow<TFileParameter>(Func<FileConnection<TFileParameter>> createFileConnection, bool dataTypeAlwaysString = false)
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

    public static void Insert_IndentityColumn_LastRow_Decimal<TFileParameter>(Func<FileConnection<TFileParameter>> createFileConnection, bool dataTypeAlwaysString = false)
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
        
    }

}