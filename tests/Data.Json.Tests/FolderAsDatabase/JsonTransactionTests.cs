using Data.Common.Extension;
using Data.Tests.Common;
using System.Data;
using System.Data.JsonClient;
using System.Reflection;
using Xunit;

namespace Data.Json.Tests.FolderAsDatabase;

public partial class JsonTransactionTests
{
    [Fact]
    public void Transaction_ShouldInsertDataIntoDatabase()
    {
        var sandboxId = $"{GetType().FullName}.{MethodBase.GetCurrentMethod()!.Name}";
        TransactionTests.Transaction_ShouldInsertDataIntoDatabase(
            () => new JsonConnection(ConnectionStrings.Instance.FolderAsDB.Sandbox("Sandbox", sandboxId))
        );
    }

    [Fact]
    public void Transaction_ShouldDeleteDataFromDatabase()
    {
        var sandboxId = $"{GetType().FullName}.{MethodBase.GetCurrentMethod()!.Name}";
        TransactionTests.Transaction_ShouldDeleteDataFromDatabase(
            () => new JsonConnection(ConnectionStrings.Instance.FolderAsDB.Sandbox("Sandbox", sandboxId))
        );
    }

    [Fact]
    public void Transaction_ShouldUpdateDataInDatabase()
    {
        var sandboxId = $"{GetType().FullName}.{MethodBase.GetCurrentMethod()!.Name}";
        TransactionTests.Transaction_ShouldUpdateDataInDatabase(
            () => new JsonConnection(ConnectionStrings.Instance.FolderAsDB.Sandbox("Sandbox", sandboxId))
        );
    }

    [Fact]
    public void Transaction_ShouldRollbackWhenExceptionIsThrown()
    {
        TransactionTests.
            Transaction_ShouldRollbackWhenExceptionIsThrown(() => new JsonConnection(ConnectionStrings.Instance.FolderAsDBConnectionString));
    }
}