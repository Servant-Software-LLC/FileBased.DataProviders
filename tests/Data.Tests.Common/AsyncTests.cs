using Data.Common.Utils.ConnectionString;
using Data.Tests.Common.Utils;
using System.Data;
using System.Data.FileClient;
using Xunit;

namespace Data.Tests.Common;

/// <summary>
/// Tests that exercise async ADO.NET operations on file-based providers.
/// </summary>
public static class AsyncTests
{
    public static async Task OpenAsync_ShouldOpenConnection<TFileParameter>(Func<FileConnection<TFileParameter>> createConnection)
        where TFileParameter : FileParameter<TFileParameter>, new()
    {
        // Arrange
        using var connection = createConnection();

        // Act
        await connection.OpenAsync();

        // Assert
        Assert.Equal(ConnectionState.Open, connection.State);
    }

    public static async Task OpenAsync_WithCancellation_ShouldThrow<TFileParameter>(Func<FileConnection<TFileParameter>> createConnection)
        where TFileParameter : FileParameter<TFileParameter>, new()
    {
        // Arrange
        using var connection = createConnection();
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(() => connection.OpenAsync(cts.Token));
    }

    public static async Task ExecuteNonQueryAsync_ShouldInsertRow<TFileParameter>(Func<FileConnection<TFileParameter>> createConnection)
        where TFileParameter : FileParameter<TFileParameter>, new()
    {
        // Arrange
        using var connection = createConnection();
        await connection.OpenAsync();

        using var command = connection.CreateCommand();
        command.CommandText = "INSERT INTO locations (id, city, state, zip) VALUES (9999, 'AsyncCity', 'AC', 99999)";

        // Act
        var rowsAffected = await command.ExecuteNonQueryAsync();

        // Assert
        Assert.Equal(1, rowsAffected);
    }

    public static async Task ExecuteScalarAsync_ShouldReturnValue<TFileParameter>(Func<FileConnection<TFileParameter>> createConnection)
        where TFileParameter : FileParameter<TFileParameter>, new()
    {
        // Arrange
        using var connection = createConnection();
        await connection.OpenAsync();

        using var command = connection.CreateCommand();
        command.CommandText = "SELECT COUNT(*) FROM locations";

        // Act
        var result = await command.ExecuteScalarAsync();

        // Assert
        Assert.NotNull(result);
        Assert.True((int)result > 0);
    }

    public static async Task ExecuteReaderAsync_ShouldReadData<TFileParameter>(Func<FileConnection<TFileParameter>> createConnection)
        where TFileParameter : FileParameter<TFileParameter>, new()
    {
        // Arrange
        using var connection = createConnection();
        await connection.OpenAsync();

        using var command = connection.CreateCommand();
        command.CommandText = "SELECT city, state FROM locations";

        // Act
        using var reader = await command.ExecuteReaderAsync();

        // Assert
        Assert.True(await reader.ReadAsync());
        Assert.Equal(2, reader.FieldCount);

        var city = reader.GetString(0);
        Assert.False(string.IsNullOrEmpty(city));
    }

    public static async Task ReadAsync_ShouldIterateRows<TFileParameter>(Func<FileConnection<TFileParameter>> createConnection)
        where TFileParameter : FileParameter<TFileParameter>, new()
    {
        // Arrange
        using var connection = createConnection();
        await connection.OpenAsync();

        using var command = connection.CreateCommand();
        command.CommandText = "SELECT city FROM locations";

        using var reader = await command.ExecuteReaderAsync();

        // Act
        int rowCount = 0;
        while (await reader.ReadAsync())
        {
            rowCount++;
            Assert.False(await reader.IsDBNullAsync(0));
        }

        // Assert
        Assert.True(rowCount > 0);
    }

    public static async Task TransactionAsync_CommitAsync_ShouldPersist<TFileParameter>(Func<FileConnection<TFileParameter>> createConnection)
        where TFileParameter : FileParameter<TFileParameter>, new()
    {
        // Arrange
        using var connection = createConnection();
        await connection.OpenAsync();
        using var transaction = connection.BeginTransaction();

        var command = transaction.CreateCommand("INSERT INTO locations (id, city, state, zip) VALUES (8888, 'TxnCity', 'TX', 88888)");

        // Act
        var rowsAffected = command.ExecuteNonQuery();

#if !NETSTANDARD2_0
        await transaction.CommitAsync();
#else
        transaction.Commit();
#endif

        // Assert
        Assert.Equal(1, rowsAffected);

        // Verify the data was committed
        var adapter = connection.CreateDataAdapter("SELECT * FROM locations WHERE city = 'TxnCity'");
        var dataSet = new DataSet();
        adapter.Fill(dataSet);
        Assert.Equal(1, dataSet.Tables[0].Rows.Count);
    }

    public static async Task TransactionAsync_RollbackAsync_ShouldDiscard<TFileParameter>(Func<FileConnection<TFileParameter>> createConnection)
        where TFileParameter : FileParameter<TFileParameter>, new()
    {
        // Arrange
        using var connection = createConnection();
        await connection.OpenAsync();
        using var transaction = connection.BeginTransaction();

        var command = transaction.CreateCommand("INSERT INTO locations (id, city, state, zip) VALUES (7777, 'RollCity', 'RC', 77777)");
        command.ExecuteNonQuery();

        // Act
#if !NETSTANDARD2_0
        await transaction.RollbackAsync();
#else
        transaction.Rollback();
#endif

        // Assert - data should not be persisted after rollback
        var selectCommand = connection.CreateCommand();
        selectCommand.CommandText = "SELECT COUNT(*) FROM locations WHERE city = 'RollCity'";
        var count = (int)selectCommand.ExecuteScalar()!;
        Assert.Equal(0, count);
    }

    public static async Task GetFieldValueAsync_ShouldReturnTypedValue<TFileParameter>(Func<FileConnection<TFileParameter>> createConnection)
        where TFileParameter : FileParameter<TFileParameter>, new()
    {
        // Arrange
        using var connection = createConnection();
        await connection.OpenAsync();

        using var command = connection.CreateCommand();
        command.CommandText = "SELECT city FROM locations";

        using var reader = await command.ExecuteReaderAsync();
        Assert.True(await reader.ReadAsync());

        // Act
        var value = await reader.GetFieldValueAsync<string>(0);

        // Assert
        Assert.False(string.IsNullOrEmpty(value));
    }
}
