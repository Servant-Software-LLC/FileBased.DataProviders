using System.Data.FileClient;
using Xunit;

namespace Data.Tests.Common;

public static class AlterTableTests
{
    public static void AddColumn_EmptyTable_ColumnAdded<TFileParameter>(Func<FileConnection<TFileParameter>> createFileConnection)
            where TFileParameter : FileParameter<TFileParameter>, new()
    {
        var tableName = "AddColumn_General";
        var columnName = "Column1";
        var columnType = "INTEGER";

        // Arrange
        using (var connection = createFileConnection())
        {
            connection.Open();

            // Arrange

            //Create the table
            var createTable = $"CREATE TABLE {tableName} (Id INTEGER PRIMARY KEY)";
            var command = connection.CreateCommand(createTable);
            var rowsAffected = command.ExecuteNonQuery();

            //Act
            var addColumn = $"ALTER TABLE {tableName} ADD COLUMN {columnName} {columnType}";
            command = connection.CreateCommand(addColumn);
            rowsAffected = command.ExecuteNonQuery();


            Assert.Equal(-1, rowsAffected);

            //Assert
            var selectColumns = $"SELECT TABLE_NAME, COLUMN_NAME, DATA_TYPE FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '{tableName}'";
            command = connection.CreateCommand(selectColumns);
            using var reader = command.ExecuteReader();

            Assert.True(reader.Read(), "Unable to read the first record.");
            Assert.Equal(tableName, reader.GetString(0));
            Assert.Equal(columnName, reader.GetString(1));
            Assert.Equal((connection.DataTypeAlwaysString ? typeof(string) : typeof(int)).FullName, reader.GetString(2));

            Assert.True(reader.Read(), "Unable to read the second record.");
            Assert.Equal(tableName, reader.GetString(0));
            Assert.Equal("Id", reader.GetString(1));
            Assert.Equal((connection.DataTypeAlwaysString ? typeof(string) : typeof(int)).FullName, reader.GetString(2));

            Assert.False(reader.Read(), "There shouldn't have been a third record.");
        }

    }

    public static void DropColumn_ColumnExists_Dropped<TFileParameter>(Func<FileConnection<TFileParameter>> createFileConnection)
            where TFileParameter : FileParameter<TFileParameter>, new()
    {
        var tableName = "DropColumn_General";
        var columnName = "Column1";
        var columnType = "INTEGER";

        // Arrange
        using (var connection = createFileConnection())
        {
            connection.Open();

            // Arrange

            //Create the table
            var createTable = $"CREATE TABLE {tableName} (Id INTEGER PRIMARY KEY, {columnName} {columnType})";
            var command = connection.CreateCommand(createTable);
            var rowsAffected = command.ExecuteNonQuery();

            //Act
            var addColumn = $"ALTER TABLE {tableName} DROP COLUMN {columnName}";
            command = connection.CreateCommand(addColumn);
            rowsAffected = command.ExecuteNonQuery();


            Assert.Equal(-1, rowsAffected);

            //Assert
            var selectColumns = $"SELECT TABLE_NAME, COLUMN_NAME, DATA_TYPE FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '{tableName}'";
            command = connection.CreateCommand(selectColumns);
            using var reader = command.ExecuteReader();

            Assert.True(reader.Read());
            Assert.Equal(tableName, reader.GetString(0));
            Assert.Equal("Id", reader.GetString(1));
            Assert.Equal((connection.DataTypeAlwaysString ? typeof(string) : typeof(int)).FullName, reader.GetString(2));

            Assert.False(reader.Read());
        }

    }

}
