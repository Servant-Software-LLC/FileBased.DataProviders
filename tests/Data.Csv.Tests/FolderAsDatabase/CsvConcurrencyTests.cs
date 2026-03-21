using Data.Common.Extension;
using Data.Tests.Common;
using System.Data.CsvClient;
using System.Reflection;
using Xunit;

namespace Data.Csv.Tests.FolderAsDatabase;

public class CsvConcurrencyTests
{
    [Fact]
    public void ConcurrentSelects_ShouldNotDeadlock()
    {
        var sandboxId = $"{GetType().FullName}.{MethodBase.GetCurrentMethod()!.Name}";
        var connStr = ConnectionStrings.Instance.FolderAsDB.Sandbox("Sandbox", sandboxId);
        ConcurrencyTests.ConcurrentSelects_ShouldNotDeadlock(connStr, cs => new CsvConnection(cs));
    }

    [Fact]
    public void SelectDuringMutations_ShouldNotDeadlock()
    {
        var sandboxId = $"{GetType().FullName}.{MethodBase.GetCurrentMethod()!.Name}";
        var connStr = ConnectionStrings.Instance.FolderAsDB.Sandbox("Sandbox", sandboxId);
        ConcurrencyTests.SelectDuringMutations_ShouldNotDeadlock(connStr, cs => new CsvConnection(cs));
    }

    [Fact]
    public void ConcurrentTransactions_ShouldNotDeadlock()
    {
        var sandboxId = $"{GetType().FullName}.{MethodBase.GetCurrentMethod()!.Name}";
        var connStr = ConnectionStrings.Instance.FolderAsDB.Sandbox("Sandbox", sandboxId);
        ConcurrencyTests.ConcurrentTransactions_ShouldNotDeadlock(connStr, cs => new CsvConnection(cs));
    }

    [Fact]
    public void ConcurrentInserts_WithIdentity_ShouldGenerateUniqueIds()
    {
        var sandboxId = $"{GetType().FullName}.{MethodBase.GetCurrentMethod()!.Name}";
        var connStr = ConnectionStrings.Instance.gettingStartedFolderDB.Sandbox("Sandbox", sandboxId);
        ConcurrencyTests.ConcurrentInserts_WithIdentity_ShouldGenerateUniqueIds(connStr, cs => new CsvConnection(cs));
    }
}
