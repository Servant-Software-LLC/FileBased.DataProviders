using Data.Tests.Common;
using System.Data.CsvClient;
using Xunit;

namespace Data.Csv.Tests.FolderAsDatabase;

public class CsvAsyncTests
{
    [Fact]
    public Task OpenAsync_ShouldOpenConnection() =>
        AsyncTests.OpenAsync_ShouldOpenConnection(() => new CsvConnection(ConnectionStrings.Instance.FolderAsDB));

    [Fact]
    public Task OpenAsync_WithCancellation_ShouldThrow() =>
        AsyncTests.OpenAsync_WithCancellation_ShouldThrow(() => new CsvConnection(ConnectionStrings.Instance.FolderAsDB));

    [Fact]
    public Task ExecuteNonQueryAsync_ShouldInsertRow() =>
        AsyncTests.ExecuteNonQueryAsync_ShouldInsertRow(() => new CsvConnection(ConnectionStrings.Instance.FolderAsDB));

    [Fact]
    public Task ExecuteScalarAsync_ShouldReturnValue() =>
        AsyncTests.ExecuteScalarAsync_ShouldReturnValue(() => new CsvConnection(ConnectionStrings.Instance.FolderAsDB));

    [Fact]
    public Task ExecuteReaderAsync_ShouldReadData() =>
        AsyncTests.ExecuteReaderAsync_ShouldReadData(() => new CsvConnection(ConnectionStrings.Instance.FolderAsDB));

    [Fact]
    public Task ReadAsync_ShouldIterateRows() =>
        AsyncTests.ReadAsync_ShouldIterateRows(() => new CsvConnection(ConnectionStrings.Instance.FolderAsDB));

    [Fact]
    public Task TransactionAsync_CommitAsync_ShouldPersist() =>
        AsyncTests.TransactionAsync_CommitAsync_ShouldPersist(() => new CsvConnection(ConnectionStrings.Instance.FolderAsDB));

    [Fact]
    public Task TransactionAsync_RollbackAsync_ShouldDiscard() =>
        AsyncTests.TransactionAsync_RollbackAsync_ShouldDiscard(() => new CsvConnection(ConnectionStrings.Instance.FolderAsDB));

    [Fact]
    public Task GetFieldValueAsync_ShouldReturnTypedValue() =>
        AsyncTests.GetFieldValueAsync_ShouldReturnTypedValue(() => new CsvConnection(ConnectionStrings.Instance.FolderAsDB));
}
