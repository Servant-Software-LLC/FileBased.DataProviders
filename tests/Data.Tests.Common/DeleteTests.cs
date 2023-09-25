using System.Data.FileClient;
using Xunit;

namespace Data.Tests.Common;

public static class DeleteTests
{
    public static void Delete_ShouldDeleteData<TFileParameter>(Func<FileConnection<TFileParameter>> createFileConnection)
        where TFileParameter : FileParameter<TFileParameter>, new()
    {
        // Arrange
        var connection = createFileConnection();
        var command = connection.CreateCommand();
        connection.Open();

        // Insert data in both tables
        command.CommandText = "INSERT INTO employees (name,email,salary,married) values ('Malizba','johndoe@email.com',50000,'true')";
        command.ExecuteNonQuery();

        command.CommandText = "INSERT INTO locations (id,city,state,zip) values (1350,'New York','NY',10001)";
        command.ExecuteNonQuery();

        // Act
        command.CommandText = "DELETE FROM employees where name='Malizba'";
        int employeesDeleted = command.ExecuteNonQuery();

        command.CommandText = "DELETE FROM locations where id=1350";
        int locationsDeleted = command.ExecuteNonQuery();

        // Assert
        Assert.Equal(1, employeesDeleted);
        Assert.Equal(1, locationsDeleted);
    }

    public static void Delete_WithReturning<TFileParameter>(Func<FileConnection<TFileParameter>> createFileConnection)
        where TFileParameter : FileParameter<TFileParameter>, new()
    {
        // Arrange
        var connection = createFileConnection();
        var command = connection.CreateCommand();
        connection.Open();

        // Insert data in both tables
        command.CommandText = @"DELETE FROM ""SomeSetting""
WHERE ""Id"" = @p0
RETURNING 1;";


        command.Parameters.Add(command.CreateParameter("p0", 1));
        var reader = command.ExecuteReader();

        Assert.True(reader.Read());
        var fieldCount = reader.FieldCount;
        Assert.Equal(1, fieldCount);

        Assert.Equal(0, reader[0]);
    }
}
