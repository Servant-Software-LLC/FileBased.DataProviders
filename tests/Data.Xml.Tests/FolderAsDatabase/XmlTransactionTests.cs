using Data.Common.Extension;
using Data.Tests.Common;
using System.Data.XmlClient;
using System.Reflection;
using Xunit;

namespace Data.Xml.Tests.FolderAsDatabase;

public partial class XmlTransactionTests
{
    [Fact]
    public void Transaction_ShouldInsertDataIntoDatabase()
    {
        var sandboxId = $"{GetType().FullName}.{MethodBase.GetCurrentMethod()!.Name}";
        TransactionTests.Transaction_ShouldInsertDataIntoDatabase(
            () => new XmlConnection(ConnectionStrings.Instance.FolderAsDB.Sandbox("Sandbox", sandboxId))
        );
    }

    [Fact]
    public void Transaction_ShouldDeleteDataFromDatabase()
    {
        var sandboxId = $"{GetType().FullName}.{MethodBase.GetCurrentMethod()!.Name}";
        TransactionTests.Transaction_ShouldDeleteDataFromDatabase(
            () => new XmlConnection(ConnectionStrings.Instance.FolderAsDB.Sandbox("Sandbox", sandboxId))
        );
    }

    [Fact]
    public void Transaction_ShouldUpdateDataInDatabase()
    {
        var sandboxId = $"{GetType().FullName}.{MethodBase.GetCurrentMethod()!.Name}";
        TransactionTests.Transaction_ShouldUpdateDataInDatabase(
            () => new XmlConnection(ConnectionStrings.Instance.FolderAsDB.Sandbox("Sandbox", sandboxId))
        );
    }

    [Fact]
    public void Transaction_ShouldRollbackWhenExceptionIsThrown()
    {
        TransactionTests.
                Transaction_ShouldRollbackWhenExceptionIsThrown(
            () => new XmlConnection(ConnectionStrings.Instance.FolderAsDB));
    }

    [Fact]
    public void Transaction_MultipleInserts_GeneratingIdentity()
    {
        var sandboxId = $"{GetType().FullName}.{MethodBase.GetCurrentMethod()!.Name}";
        TransactionTests.
            Transaction_MultipleInserts_GeneratingIdentity(() => new XmlConnection(ConnectionStrings.Instance.gettingStartedFolderDB.Sandbox("Sandbox", sandboxId)));
    }

}