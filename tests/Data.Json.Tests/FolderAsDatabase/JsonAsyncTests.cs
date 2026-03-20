using Data.Tests.Common;
using System.Data.JsonClient;
using Xunit;

namespace Data.Json.Tests.FolderAsDatabase;

public class JsonAsyncTests
{
    [Fact]
    public Task OpenAsync_ShouldOpenConnection() =>
        AsyncTests.OpenAsync_ShouldOpenConnection(() => new JsonConnection(ConnectionStrings.Instance.FolderAsDB));

    [Fact]
    public Task OpenAsync_WithCancellation_ShouldThrow() =>
        AsyncTests.OpenAsync_WithCancellation_ShouldThrow(() => new JsonConnection(ConnectionStrings.Instance.FolderAsDB));

    [Fact]
    public Task ExecuteNonQueryAsync_ShouldInsertRow() =>
        AsyncTests.ExecuteNonQueryAsync_ShouldInsertRow(() => new JsonConnection(ConnectionStrings.Instance.FolderAsDB));

    [Fact]
    public Task ExecuteScalarAsync_ShouldReturnValue() =>
        AsyncTests.ExecuteScalarAsync_ShouldReturnValue(() => new JsonConnection(ConnectionStrings.Instance.FolderAsDB));

    [Fact]
    public Task ExecuteReaderAsync_ShouldReadData() =>
        AsyncTests.ExecuteReaderAsync_ShouldReadData(() => new JsonConnection(ConnectionStrings.Instance.FolderAsDB));

    [Fact]
    public Task ReadAsync_ShouldIterateRows() =>
        AsyncTests.ReadAsync_ShouldIterateRows(() => new JsonConnection(ConnectionStrings.Instance.FolderAsDB));

    [Fact]
    public Task TransactionAsync_CommitAsync_ShouldPersist() =>
        AsyncTests.TransactionAsync_CommitAsync_ShouldPersist(() => new JsonConnection(ConnectionStrings.Instance.FolderAsDB));

    [Fact]
    public Task TransactionAsync_RollbackAsync_ShouldDiscard() =>
        AsyncTests.TransactionAsync_RollbackAsync_ShouldDiscard(() => new JsonConnection(ConnectionStrings.Instance.FolderAsDB));

    [Fact]
    public Task GetFieldValueAsync_ShouldReturnTypedValue() =>
        AsyncTests.GetFieldValueAsync_ShouldReturnTypedValue(() => new JsonConnection(ConnectionStrings.Instance.FolderAsDB));
}
