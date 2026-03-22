using Data.Common.Extension;
using Data.Tests.Common;
using Data.Tests.Common.Utils;
using System.Data.XmlClient;
using System.Reflection;
using Xunit;

namespace Data.Xml.Tests.FolderAsDatabase;

/// <summary>
/// These tests indirectly exercise the <see cref="JsonIO.Write.JsonInsert"/> class via calls to <see cref="XmlCommand.ExecuteNonQuery" />.
/// </summary>
public class XmlInsertTests
{
    [Fact]
    public void Insert_ShouldInsertData()
    {
        var sandboxId = $"{GetType().FullName}.{MethodBase.GetCurrentMethod()!.Name}";
        InsertTests.Insert_ShouldInsertData(() => new XmlConnection(ConnectionStrings.Instance.FolderAsDB.Sandbox("Sandbox", sandboxId)));
    }

    [Fact]
    public void Insert_ShouldInsertData_CustomDataSource()
    {
        var sandboxId = $"{GetType().FullName}.{MethodBase.GetCurrentMethod()!.Name}";
        InsertTests.Insert_ShouldInsertData(() =>
            CustomDataSourceFactory.VirtualFolderAsDB((connectionString) => new XmlConnection(connectionString)));
    }

    [Fact]
    public void Insert_ShouldInsertNullData()
    {
        var sandboxId = $"{GetType().FullName}.{MethodBase.GetCurrentMethod()!.Name}";
        InsertTests.Insert_ShouldInsertNullData(() => new XmlConnection(ConnectionStrings.Instance.FolderAsDB.Sandbox("Sandbox", sandboxId)));
    }

    [Fact]
    public void Insert_ShouldBeFormatted()
    {
        var sandboxId = $"{GetType().FullName}.{MethodBase.GetCurrentMethod()!.Name}";
        var sandboxConnectionString = ConnectionStrings.Instance.FolderAsDB.AddFormatted(true).Sandbox("Sandbox", sandboxId);

        InsertTests.Insert_ShouldBeFormatted(() => new XmlConnection(sandboxConnectionString));
    }

    //This is a special case.  In XML without an XSD, if a table does not have any rows in it, then we have no schema information on the columns or their data types.
    //Inserting the first row into this table will then determine the columns (along with their data types) in this table.
    [Fact]
    public void Insert_ShouldInsertDataIntoEmptyTables()
    {
        var sandboxId = $"{GetType().FullName}.{MethodBase.GetCurrentMethod()!.Name}";
        var sandboxConnectionString = ConnectionStrings.Instance.EmptyWithTablesFolderAsDB.Sandbox("Sandbox", sandboxId);

        //Setup the tables by inserting the first row into them.
        InsertTests.Insert_ShouldInsertData(() => new XmlConnection(sandboxConnectionString));

        //Assert by reading from the tables.
        InsertTests.Insert_ShouldInsertDataIntoEmptyTables(() => new XmlConnection(sandboxConnectionString));
    }

    [Fact]
    public void Insert_IndentityColumn_NoLastRow()
    {
        var sandboxId = $"{GetType().FullName}.{MethodBase.GetCurrentMethod()!.Name}";
        InsertTests.Insert_IndentityColumn_NoLastRow(
            () => new XmlConnection(ConnectionStrings.Instance.gettingStartedFolderDB.Sandbox("Sandbox", sandboxId)));

    }

    [Fact]
    public void Insert_IndentityColumn_LastRow_Decimal()
    {
        var sandboxId = $"{GetType().FullName}.{MethodBase.GetCurrentMethod()!.Name}";
        InsertTests.Insert_IndentityColumn_LastRow_Decimal(
            () => new XmlConnection(ConnectionStrings.Instance.FolderAsDB.Sandbox("Sandbox", sandboxId)));
    }

}