using System.Data.FileClient;

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
}
