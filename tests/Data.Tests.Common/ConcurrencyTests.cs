using Data.Common.Extension;
using Data.Common.Utils.ConnectionString;
using Data.Tests.Common.Extensions;
using System.Data;
using System.Data.FileClient;
using Xunit;

namespace Data.Tests.Common;

/// <summary>
/// Tests that exercise concurrent read/write operations on file-based providers.
/// These verify thread safety and absence of deadlocks under parallel access.
/// </summary>
public static class ConcurrencyTests
{
    /// <summary>
    /// Multiple concurrent SELECT operations should not deadlock or corrupt data.
    /// </summary>
    public static void ConcurrentSelects_ShouldNotDeadlock<TFileParameter>(
        FileConnectionString connectionString,
        Func<FileConnectionString, FileConnection<TFileParameter>> createConnection)
        where TFileParameter : FileParameter<TFileParameter>, new()
    {
        const int concurrency = 10;
        var exceptions = new List<Exception>();
        var barrier = new Barrier(concurrency);

        var tasks = Enumerable.Range(0, concurrency).Select(_ => Task.Run(() =>
        {
            try
            {
                barrier.SignalAndWait(TimeSpan.FromSeconds(10));
                using var connection = createConnection(connectionString);
                connection.Open();

                var adapter = connection.CreateDataAdapter("SELECT * FROM employees");
                var dataSet = new DataSet();
                adapter.Fill(dataSet);

                Assert.True(dataSet.Tables[0].Rows.Count > 0, "SELECT should return rows");
            }
            catch (Exception ex)
            {
                lock (exceptions) { exceptions.Add(ex); }
            }
        })).ToArray();

        var completed = Task.WaitAll(tasks, TimeSpan.FromSeconds(30));
        Assert.True(completed, "Concurrent SELECTs should complete without deadlock within 30s");

        if (exceptions.Count > 0)
            throw new AggregateException("Concurrent SELECT failures", exceptions);
    }

    /// <summary>
    /// SELECT operations running concurrently with INSERT/UPDATE/DELETE should not deadlock.
    /// </summary>
    public static void SelectDuringMutations_ShouldNotDeadlock<TFileParameter>(
        FileConnectionString connectionString,
        Func<FileConnectionString, FileConnection<TFileParameter>> createConnection)
        where TFileParameter : FileParameter<TFileParameter>, new()
    {
        const int readers = 5;
        const int writers = 3;
        var exceptions = new List<Exception>();
        var barrier = new Barrier(readers + writers);

        var readerTasks = Enumerable.Range(0, readers).Select(_ => Task.Run(() =>
        {
            try
            {
                barrier.SignalAndWait(TimeSpan.FromSeconds(10));
                using var connection = createConnection(connectionString);
                connection.Open();

                var adapter = connection.CreateDataAdapter("SELECT * FROM employees");
                var dataSet = new DataSet();
                adapter.Fill(dataSet);

                Assert.NotNull(dataSet.Tables[0]);
            }
            catch (Exception ex)
            {
                lock (exceptions) { exceptions.Add(ex); }
            }
        })).ToArray();

        var writerTasks = Enumerable.Range(0, writers).Select(i => Task.Run(() =>
        {
            try
            {
                barrier.SignalAndWait(TimeSpan.FromSeconds(10));
                using var connection = createConnection(connectionString);
                connection.Open();

                var cmd = connection.CreateCommand($"INSERT INTO employees (name, salary) VALUES ('Concurrent_{i}', {50000 + i})");
                cmd.ExecuteNonQuery();

                cmd = connection.CreateCommand($"UPDATE employees SET salary = {60000 + i} WHERE name = 'Concurrent_{i}'");
                cmd.ExecuteNonQuery();

                cmd = connection.CreateCommand($"DELETE FROM employees WHERE name = 'Concurrent_{i}'");
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                lock (exceptions) { exceptions.Add(ex); }
            }
        })).ToArray();

        var allTasks = readerTasks.Concat(writerTasks).ToArray();
        var completed = Task.WaitAll(allTasks, TimeSpan.FromSeconds(60));
        Assert.True(completed, "Concurrent reads/writes should complete without deadlock within 60s");

        if (exceptions.Count > 0)
            throw new AggregateException("Concurrent read/write failures", exceptions);
    }

    /// <summary>
    /// Multiple concurrent transactions against the same table should not deadlock.
    /// Each transaction inserts, commits, and completes without hanging. IO errors
    /// from concurrent file access are tolerated (known limitation of file-based
    /// storage), but deadlocks (timeout) are not.
    /// </summary>
    public static void ConcurrentTransactions_ShouldNotDeadlock<TFileParameter>(
        FileConnectionString connectionString,
        Func<FileConnectionString, FileConnection<TFileParameter>> createConnection)
        where TFileParameter : FileParameter<TFileParameter>, new()
    {
        const int concurrency = 5;
        var ioErrors = new List<Exception>();
        var nonIoErrors = new List<Exception>();
        var barrier = new Barrier(concurrency);
        var completedCount = 0;

        var tasks = Enumerable.Range(0, concurrency).Select(i => Task.Run(() =>
        {
            try
            {
                barrier.SignalAndWait(TimeSpan.FromSeconds(10));
                using var connection = createConnection(connectionString);
                connection.Open();
                using var transaction = connection.BeginTransaction();

                var cmd = transaction.CreateCommand($"INSERT INTO employees (name, salary) VALUES ('TxnWorker_{i}', {70000 + i})");
                cmd.ExecuteNonQuery();

                transaction.Commit();
                Interlocked.Increment(ref completedCount);
            }
            catch (Exception ex) when (IsFileAccessError(ex))
            {
                // IO errors from concurrent file access are a known limitation,
                // not a deadlock. Track but don't fail the test for these.
                lock (ioErrors) { ioErrors.Add(ex); }
            }
            catch (Exception ex)
            {
                lock (nonIoErrors) { nonIoErrors.Add(ex); }
            }
        })).ToArray();

        var completed = Task.WaitAll(tasks, TimeSpan.FromSeconds(60));
        Assert.True(completed, "Concurrent transactions should complete without deadlock within 60s");

        if (nonIoErrors.Count > 0)
            throw new AggregateException("Concurrent transaction failures (non-IO)", nonIoErrors);

        // At least one transaction should succeed
        Assert.True(completedCount > 0,
            $"At least one transaction should commit successfully. IO errors: {ioErrors.Count}");
    }

    private static bool IsFileAccessError(Exception ex)
    {
        if (ex is IOException || ex is System.Xml.XmlException)
            return true;

        // Check inner exceptions for IO/XML errors (often wrapped)
        var inner = ex.InnerException;
        while (inner != null)
        {
            if (inner is IOException || inner is System.Xml.XmlException)
                return true;
            inner = inner.InnerException;
        }

        // Check for TableNotFoundException wrapping an IO error
        return ex.GetType().Name == "TableNotFoundException" && ex.InnerException != null && IsFileAccessError(ex.InnerException);
    }

    /// <summary>
    /// Concurrent INSERTs with identity generation should produce unique IDs.
    /// </summary>
    public static void ConcurrentInserts_WithIdentity_ShouldGenerateUniqueIds<TFileParameter>(
        FileConnectionString connectionString,
        Func<FileConnectionString, FileConnection<TFileParameter>> createConnection)
        where TFileParameter : FileParameter<TFileParameter>, new()
    {
        const int concurrency = 5;
        var exceptions = new List<Exception>();
        var barrier = new Barrier(concurrency);
        var generatedIds = new System.Collections.Concurrent.ConcurrentBag<object>();

        var tasks = Enumerable.Range(0, concurrency).Select(i => Task.Run(() =>
        {
            try
            {
                barrier.SignalAndWait(TimeSpan.FromSeconds(10));
                using var connection = createConnection(connectionString);
                connection.Open();
                using var transaction = connection.BeginTransaction();

                var commandText = "INSERT INTO \"Blogs\" (\"Url\") VALUES (@p0); SELECT \"BlogId\" FROM \"Blogs\" WHERE ROW_COUNT() = 1 AND \"BlogId\"=LAST_INSERT_ID();";
                var cmd = connection.CreateCommand(commandText);
                cmd.Parameters.Add(cmd.CreateParameter("p0", $"http://blog{i}.example.com"));
                using var reader = cmd.ExecuteReader();

                Assert.True(reader.Read(), $"INSERT with identity should return a row for worker {i}");
                var id = reader["BlogId"];
                Assert.NotNull(id);
                generatedIds.Add(id);

                transaction.Commit();
            }
            catch (Exception ex)
            {
                lock (exceptions) { exceptions.Add(ex); }
            }
        })).ToArray();

        var completed = Task.WaitAll(tasks, TimeSpan.FromSeconds(60));
        Assert.True(completed, "Concurrent identity inserts should complete without deadlock within 60s");

        if (exceptions.Count > 0)
            throw new AggregateException("Concurrent identity insert failures", exceptions);

        var idList = generatedIds.ToList();
        Assert.Equal(concurrency, idList.Distinct().Count());
    }
}
