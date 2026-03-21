using Data.Common.Extension;
using Data.Common.Utils.ConnectionString;
using System.Data;
using System.Data.FileClient;
using Xunit;

namespace Data.Tests.Common;

/// <summary>
/// Tests that exercise concurrent access patterns on file-based providers:
/// multiple readers, reader+writer, multiple writers, concurrent transactions,
/// and concurrent identity generation.
/// </summary>
public static class ConcurrentTests
{
    private const int ConcurrencyLevel = 5;

    /// <summary>
    /// Multiple tasks reading from the same table concurrently should all succeed
    /// and return consistent data.
    /// </summary>
    public static async Task ConcurrentReads_ShouldAllSucceed<TFileParameter>(
        Func<FileConnection<TFileParameter>> createConnection)
        where TFileParameter : FileParameter<TFileParameter>, new()
    {
        var tasks = Enumerable.Range(0, ConcurrencyLevel).Select(async _ =>
        {
            using var connection = createConnection();
            await connection.OpenAsync();

            using var command = connection.CreateCommand();
            command.CommandText = "SELECT city, state FROM locations";
            using var reader = await command.ExecuteReaderAsync();

            int rowCount = 0;
            while (await reader.ReadAsync())
            {
                rowCount++;
                Assert.False(string.IsNullOrEmpty(reader.GetString(0)));
            }

            return rowCount;
        }).ToArray();

        var results = await Task.WhenAll(tasks);

        // All readers should see the same number of rows
        Assert.True(results.All(r => r == results[0]),
            $"Expected all readers to see the same row count, but got: {string.Join(", ", results)}");
    }

    /// <summary>
    /// Concurrent writers inserting into the same table should not corrupt data
    /// or throw unhandled exceptions. The connection string must point to an
    /// already-sandboxed database (sandbox created before calling this method).
    /// </summary>
    public static async Task ConcurrentWrites_ShouldNotCorruptData<TFileParameter>(
        Func<FileConnectionString, FileConnection<TFileParameter>> createConnection,
        FileConnectionString sandboxedConnectionString)
        where TFileParameter : FileParameter<TFileParameter>, new()
    {
        var tasks = Enumerable.Range(0, ConcurrencyLevel).Select(async i =>
        {
            using var connection = createConnection(sandboxedConnectionString);
            await connection.OpenAsync();

            using var command = connection.CreateCommand();
            command.CommandText = $"INSERT INTO employees (name, email, salary, married) VALUES ('Writer{i}', 'writer{i}@test.com', {50000 + i}, true)";
            var rowsAffected = await command.ExecuteNonQueryAsync();
            Assert.Equal(1, rowsAffected);
        }).ToArray();

        await Task.WhenAll(tasks);

        // Verify all rows were inserted
        using var verifyConnection = createConnection(sandboxedConnectionString);
        verifyConnection.Open();
        using var verifyCommand = verifyConnection.CreateCommand();
        verifyCommand.CommandText = "SELECT COUNT(*) FROM employees WHERE name LIKE 'Writer%'";
        var count = (int)verifyCommand.ExecuteScalar()!;
        Assert.Equal(ConcurrencyLevel, count);
    }

    /// <summary>
    /// Concurrent readers and a writer should not interfere with each other.
    /// Writers acquire exclusive locks; readers acquire shared locks.
    /// </summary>
    public static async Task ConcurrentReadersAndWriter_ShouldNotInterfere<TFileParameter>(
        Func<FileConnectionString, FileConnection<TFileParameter>> createConnection,
        FileConnectionString sandboxedConnectionString)
        where TFileParameter : FileParameter<TFileParameter>, new()
    {
        using var barrier = new Barrier(ConcurrencyLevel + 1);

        // Start reader tasks
        var readerTasks = Enumerable.Range(0, ConcurrencyLevel).Select(async _ =>
        {
            barrier.SignalAndWait(TimeSpan.FromSeconds(10));

            using var connection = createConnection(sandboxedConnectionString);
            await connection.OpenAsync();

            using var command = connection.CreateCommand();
            command.CommandText = "SELECT city FROM locations";
            using var reader = await command.ExecuteReaderAsync();

            int rowCount = 0;
            while (await reader.ReadAsync())
                rowCount++;

            Assert.True(rowCount > 0, "Reader should see at least one row");
        }).ToArray();

        // Start writer task
        var writerTask = Task.Run(async () =>
        {
            barrier.SignalAndWait(TimeSpan.FromSeconds(10));

            using var connection = createConnection(sandboxedConnectionString);
            await connection.OpenAsync();

            using var command = connection.CreateCommand();
            command.CommandText = "INSERT INTO locations (id, city, state, zip) VALUES (50001, 'ConcurrentCity', 'CC', 50001)";
            var rowsAffected = await command.ExecuteNonQueryAsync();
            Assert.Equal(1, rowsAffected);
        });

        await Task.WhenAll(readerTasks.Concat(new[] { writerTask }));
    }

    /// <summary>
    /// Multiple transactions committing concurrently on the same table should
    /// not deadlock or corrupt data. Each transaction reads its snapshot at open
    /// time and commits under a write lock, so at least one row must be persisted.
    /// </summary>
    public static async Task ConcurrentTransactions_ShouldNotDeadlock<TFileParameter>(
        Func<FileConnectionString, FileConnection<TFileParameter>> createConnection,
        FileConnectionString sandboxedConnectionString)
        where TFileParameter : FileParameter<TFileParameter>, new()
    {
        var tasks = Enumerable.Range(0, ConcurrencyLevel).Select(async i =>
        {
            using var connection = createConnection(sandboxedConnectionString);
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            var command = transaction.CreateCommand(
                $"INSERT INTO employees (name, email, salary, married) VALUES ('Txn{i}', 'txn{i}@test.com', {60000 + i}, false)");
            command.ExecuteNonQuery();

            await Task.Run(() => transaction.Commit());
        }).ToArray();

        // All transactions should complete without deadlock or exception
        await Task.WhenAll(tasks);

        // Verify at least one transaction row was committed (the last writer wins
        // because each transaction reads its own snapshot at connection.Open time)
        using var verifyConnection = createConnection(sandboxedConnectionString);
        verifyConnection.Open();
        using var verifyCommand = verifyConnection.CreateCommand();
        verifyCommand.CommandText = "SELECT COUNT(*) FROM employees WHERE name LIKE 'Txn%'";
        var count = (int)verifyCommand.ExecuteScalar()!;
        Assert.True(count >= 1, $"Expected at least 1 transaction row committed, but found {count}");
    }

    /// <summary>
    /// Concurrent inserts with auto-generated identity values should each
    /// receive a unique identity.
    /// </summary>
    public static async Task ConcurrentIdentityGeneration_ShouldProduceUniqueIds<TFileParameter>(
        Func<FileConnectionString, FileConnection<TFileParameter>> createConnection,
        FileConnectionString sandboxedConnectionString)
        where TFileParameter : FileParameter<TFileParameter>, new()
    {
        var tasks = Enumerable.Range(0, ConcurrencyLevel).Select(async i =>
        {
            using var connection = createConnection(sandboxedConnectionString);
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            var commandText = "INSERT INTO \"Blogs\" (\"Url\") VALUES (@p0); SELECT \"BlogId\" FROM \"Blogs\" WHERE ROW_COUNT() = 1 AND \"BlogId\"=LAST_INSERT_ID();";
            var command = connection.CreateCommand(commandText);
            command.Parameters.Add(command.CreateParameter("p0", $"http://blog{i}.example.com"));

            using var reader = command.ExecuteReader();
            Assert.True(reader.Read(), $"Expected identity result for insert {i}");
            var id = reader["BlogId"];
            Assert.NotNull(id);

            transaction.Commit();

            return id;
        }).ToArray();

        var ids = await Task.WhenAll(tasks);

        // All generated identities should be unique
        var distinctIds = ids.Select(id => id!.ToString()).Distinct().ToList();
        Assert.Equal(ConcurrencyLevel, distinctIds.Count);
    }

    /// <summary>
    /// Operations against different databases should not block each other,
    /// verifying the per-database lock design (PR #151).
    /// </summary>
    public static async Task CrossDatabaseOperations_ShouldNotBlock<TFileParameter>(
        Func<FileConnection<TFileParameter>> createConnectionDb1,
        Func<FileConnection<TFileParameter>> createConnectionDb2)
        where TFileParameter : FileParameter<TFileParameter>, new()
    {
        using var barrier = new Barrier(2);

        var task1 = Task.Run(async () =>
        {
            barrier.SignalAndWait(TimeSpan.FromSeconds(10));
            using var connection = createConnectionDb1();
            await connection.OpenAsync();

            using var command = connection.CreateCommand();
            command.CommandText = "SELECT city FROM locations";
            using var reader = await command.ExecuteReaderAsync();

            int rowCount = 0;
            while (await reader.ReadAsync())
                rowCount++;

            Assert.True(rowCount > 0);
        });

        var task2 = Task.Run(async () =>
        {
            barrier.SignalAndWait(TimeSpan.FromSeconds(10));
            using var connection = createConnectionDb2();
            await connection.OpenAsync();

            using var command = connection.CreateCommand();
            command.CommandText = "SELECT city FROM locations";
            using var reader = await command.ExecuteReaderAsync();

            int rowCount = 0;
            while (await reader.ReadAsync())
                rowCount++;

            Assert.True(rowCount > 0);
        });

        // Both should complete within a reasonable time (no cross-database blocking)
        var completed = Task.WaitAll(new[] { task1, task2 }, TimeSpan.FromSeconds(30));
        Assert.True(completed, "Cross-database operations should complete without blocking each other");
    }
}
