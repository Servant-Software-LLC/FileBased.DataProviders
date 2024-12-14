using System.Data.FileClient;
using Xunit;

namespace Data.Tests.Common;

public static class CreateTableTests
{
    public static void CreateTable_WhenNoColumns_ShouldWork<TFileParameter>(Func<FileConnection<TFileParameter>> createFileConnection)
        where TFileParameter : FileParameter<TFileParameter>, new()
    {
        var tableName = "NoColumnedTable";

        // Arrange
        using (var connection = createFileConnection())
        {
            connection.Open();

            // Arrange

            //Create the table
            var createTable = $"CREATE TABLE {tableName} (Id INTEGER PRIMARY KEY)";
            var command = connection.CreateCommand(createTable);
            var rowsAffected = command.ExecuteNonQuery();

            //Drop the only column in this table.
            var addColumn = $"ALTER TABLE {tableName} DROP COLUMN Id";
            command = connection.CreateCommand(addColumn);
            rowsAffected = command.ExecuteNonQuery();

            // Act
            command = connection.CreateCommand(createTable);
            rowsAffected = command.ExecuteNonQuery();

            // Assert
            // No assert needed, if an exception is thrown, the test will fail
        }
    }

    public static void CreateTable_FollowedByInsert_ShouldWork<TFileParameter>(Func<FileConnection<TFileParameter>> createFileConnection)
        where TFileParameter : FileParameter<TFileParameter>, new()
    {
        var tableName = "GeneralSettings";

        // Arrange
        using (var connection = createFileConnection())
        {
            connection.Open();

            // Arrange

            //Create the table
            var createTable = $"CREATE TABLE {tableName} (CaseSensitive BOOLEAN)";
            var command = connection.CreateCommand(createTable);
            var rowsAffected = command.ExecuteNonQuery();

            // Act

            //Insert a row.  For JSON, if the table has been created from an empty file, then the columns would not be defined in the table,
            //but in this case, the CREATE TABLE does define the tables before the first INSERT.
            var addColumn = $"INSERT INTO {tableName} (CaseSensitive) VALUES (TRUE)";
            command = connection.CreateCommand(addColumn);
            rowsAffected = command.ExecuteNonQuery();

            // Assert
            Assert.Equal(1, rowsAffected);
        }
    }
}
