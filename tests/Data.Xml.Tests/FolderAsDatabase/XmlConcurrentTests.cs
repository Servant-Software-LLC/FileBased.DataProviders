using Data.Common.Extension;
using Data.Tests.Common;
using System.Data.XmlClient;
using System.Reflection;
using Xunit;

namespace Data.Xml.Tests.FolderAsDatabase;

public class XmlConcurrentTests
{
    [Fact]
    public Task ConcurrentReads_ShouldAllSucceed() =>
        ConcurrentTests.ConcurrentReads_ShouldAllSucceed(
            () => new XmlConnection(ConnectionStrings.Instance.FolderAsDB));

    [Fact]
    public Task ConcurrentWrites_ShouldNotCorruptData()
    {
        var sandboxId = $"{GetType().FullName}.{MethodBase.GetCurrentMethod()!.Name}";
        var sandboxed = ConnectionStrings.Instance.FolderAsDB.Sandbox("Sandbox", sandboxId);
        return ConcurrentTests.ConcurrentWrites_ShouldNotCorruptData(
            cs => new XmlConnection(cs), sandboxed);
    }

    [Fact]
    public Task ConcurrentReadersAndWriter_ShouldNotInterfere()
    {
        var sandboxId = $"{GetType().FullName}.{MethodBase.GetCurrentMethod()!.Name}";
        var sandboxed = ConnectionStrings.Instance.FolderAsDB.Sandbox("Sandbox", sandboxId);
        return ConcurrentTests.ConcurrentReadersAndWriter_ShouldNotInterfere(
            cs => new XmlConnection(cs), sandboxed);
    }

    [Fact]
    public Task ConcurrentTransactions_ShouldNotDeadlock()
    {
        var sandboxId = $"{GetType().FullName}.{MethodBase.GetCurrentMethod()!.Name}";
        var sandboxed = ConnectionStrings.Instance.FolderAsDB.Sandbox("Sandbox", sandboxId);
        return ConcurrentTests.ConcurrentTransactions_ShouldNotDeadlock(
            cs => new XmlConnection(cs), sandboxed);
    }

    [Fact]
    public Task ConcurrentIdentityGeneration_ShouldProduceUniqueIds()
    {
        var sandboxId = $"{GetType().FullName}.{MethodBase.GetCurrentMethod()!.Name}";
        var sandboxed = ConnectionStrings.Instance.gettingStartedFolderDB.Sandbox("Sandbox", sandboxId);
        return ConcurrentTests.ConcurrentIdentityGeneration_ShouldProduceUniqueIds(
            cs => new XmlConnection(cs), sandboxed);
    }

    [Fact]
    public Task CrossDatabaseOperations_ShouldNotBlock()
    {
        var sandboxId1 = $"{GetType().FullName}.{MethodBase.GetCurrentMethod()!.Name}.db1";
        var sandboxId2 = $"{GetType().FullName}.{MethodBase.GetCurrentMethod()!.Name}.db2";
        return ConcurrentTests.CrossDatabaseOperations_ShouldNotBlock(
            () => new XmlConnection(ConnectionStrings.Instance.FolderAsDB.Sandbox("Sandbox", sandboxId1)),
            () => new XmlConnection(ConnectionStrings.Instance.FolderAsDB.Sandbox("Sandbox", sandboxId2)));
    }
}
