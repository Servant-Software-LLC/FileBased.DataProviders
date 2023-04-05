using Data.Common.Extension;
using Data.Tests.Common;
using System.Data;
using System.Data.CsvClient;
using System.Reflection;
using Xunit;

namespace Data.Csv.Tests.FolderAsDatabase;

public partial class CsvTransactionTests
{
    [Fact]
    public void Transaction_ShouldInsertDataIntoDatabase()
    {
        var sandboxId = $"{GetType().FullName}.{MethodBase.GetCurrentMethod()!.Name}";
        TransactionTests.Transaction_ShouldInsertDataIntoDatabase(
            () => new CsvConnection(ConnectionStrings.Instance.FolderAsDBConnectionString.Sandbox("Sandbox", sandboxId))
        );
    }

    [Fact]
    public void Transaction_ShouldDeleteDataFromDatabase()
    {
        var sandboxId = $"{GetType().FullName}.{MethodBase.GetCurrentMethod()!.Name}";
        TransactionTests.Transaction_ShouldDeleteDataFromDatabase(
            () => new CsvConnection(ConnectionStrings.Instance.FolderAsDBConnectionString.Sandbox("Sandbox", sandboxId))
        );
    }

    [Fact]
    public void Transaction_ShouldUpdateDataInDatabase()
    {
        var sandboxId = $"{GetType().FullName}.{MethodBase.GetCurrentMethod()!.Name}";
        TransactionTests.Transaction_ShouldUpdateDataInDatabase(
            () => new CsvConnection(ConnectionStrings.Instance.FolderAsDBConnectionString.Sandbox("Sandbox", sandboxId))
        );
    }

    [Fact]
    public void Transaction_ShouldRollbackWhenExceptionIsThrown()
    {
        TransactionTests.
                Transaction_ShouldRollbackWhenExceptionIsThrown(() => new CsvConnection(ConnectionStrings.Instance.FolderAsDBConnectionString));
    }
}