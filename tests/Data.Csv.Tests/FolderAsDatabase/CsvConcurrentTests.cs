using Data.Common.Extension;
using Data.Tests.Common;
using System.Data.CsvClient;
using System.Reflection;
using Xunit;

namespace Data.Csv.Tests.FolderAsDatabase;

public class CsvConcurrentTests
{
    [Fact]
    public Task ConcurrentReads_ShouldAllSucceed() =>
        ConcurrentTests.ConcurrentReads_ShouldAllSucceed(
            () => new CsvConnection(ConnectionStrings.Instance.FolderAsDB));

    [Fact]
    public Task ConcurrentWrites_ShouldNotCorruptData()
    {
        var sandboxId = $"{GetType().FullName}.{MethodBase.GetCurrentMethod()!.Name}";
        var sandboxed = ConnectionStrings.Instance.FolderAsDB.Sandbox("Sandbox", sandboxId);
        return ConcurrentTests.ConcurrentWrites_ShouldNotCorruptData(
            cs => new CsvConnection(cs), sandboxed);
    }

    [Fact]
    public Task ConcurrentReadersAndWriter_ShouldNotInterfere()
    {
        var sandboxId = $"{GetType().FullName}.{MethodBase.GetCurrentMethod()!.Name}";
        var sandboxed = ConnectionStrings.Instance.FolderAsDB.Sandbox("Sandbox", sandboxId);
        return ConcurrentTests.ConcurrentReadersAndWriter_ShouldNotInterfere(
            cs => new CsvConnection(cs), sandboxed);
    }

    [Fact]
    public Task ConcurrentTransactions_ShouldNotDeadlock()
    {
        var sandboxId = $"{GetType().FullName}.{MethodBase.GetCurrentMethod()!.Name}";
        var sandboxed = ConnectionStrings.Instance.FolderAsDB.Sandbox("Sandbox", sandboxId);
        return ConcurrentTests.ConcurrentTransactions_ShouldNotDeadlock(
            cs => new CsvConnection(cs), sandboxed);
    }

    [Fact]
    public Task ConcurrentIdentityGeneration_ShouldProduceUniqueIds()
    {
        var sandboxId = $"{GetType().FullName}.{MethodBase.GetCurrentMethod()!.Name}";
        var sandboxed = ConnectionStrings.Instance.gettingStartedFolderDB.Sandbox("Sandbox", sandboxId);
        return ConcurrentTests.ConcurrentIdentityGeneration_ShouldProduceUniqueIds(
            cs => new CsvConnection(cs), sandboxed);
    }

    [Fact]
    public Task CrossDatabaseOperations_ShouldNotBlock()
    {
        var sandboxId1 = $"{GetType().FullName}.{MethodBase.GetCurrentMethod()!.Name}.db1";
        var sandboxId2 = $"{GetType().FullName}.{MethodBase.GetCurrentMethod()!.Name}.db2";
        return ConcurrentTests.CrossDatabaseOperations_ShouldNotBlock(
            () => new CsvConnection(ConnectionStrings.Instance.FolderAsDB.Sandbox("Sandbox", sandboxId1)),
            () => new CsvConnection(ConnectionStrings.Instance.FolderAsDB.Sandbox("Sandbox", sandboxId2)));
    }
}
