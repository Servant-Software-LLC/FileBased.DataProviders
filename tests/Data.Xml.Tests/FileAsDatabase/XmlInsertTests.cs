using Data.Common.Extension;
using Data.Tests.Common;
using System.Data.XmlClient;
using System.Reflection;
using Xunit;

namespace Data.Xml.Tests.FileAsDatabase;

public class XmlInsertTests
{
    [Fact]
    public void Insert_ShouldInsertData()
    {
        var sandboxId = $"{GetType().FullName}.{MethodBase.GetCurrentMethod()!.Name}";
        InsertTests.Insert_ShouldInsertData(() => new XmlConnection(ConnectionStrings.Instance.FileAsDB.Sandbox("Sandbox", sandboxId)));
    }

    [Fact]
    public void Insert_ShouldInsertNullData()
    {
        var sandboxId = $"{GetType().FullName}.{MethodBase.GetCurrentMethod()!.Name}";
        InsertTests.Insert_ShouldInsertNullData(() => new XmlConnection(ConnectionStrings.Instance.FileAsDB.Sandbox("Sandbox", sandboxId)));
    }

    [Fact]
    public void Insert_JsonShouldBeFormatted()
    {
        InsertTests.Insert_ShouldBeFormattedForFile(() =>
        new XmlConnection(ConnectionStrings.Instance.FileAsDB.AddFormatted(true)));
    }

    [Fact]
    public void Insert_IndentityColumn_NoLastRow()
    {
        var sandboxId = $"{GetType().FullName}.{MethodBase.GetCurrentMethod()!.Name}";
        InsertTests.Insert_IndentityColumn_NoLastRow(
            () => new XmlConnection(ConnectionStrings.Instance.gettingStartedFileDB.Sandbox("Sandbox", sandboxId)));

    }

    [Fact]
    public void Insert_IndentityColumn_LastRow_Decimal()
    {
        var sandboxId = $"{GetType().FullName}.{MethodBase.GetCurrentMethod()!.Name}";
        InsertTests.Insert_IndentityColumn_LastRow_Decimal(
            () => new XmlConnection(ConnectionStrings.Instance.FileAsDB.Sandbox("Sandbox", sandboxId)));
    }

}