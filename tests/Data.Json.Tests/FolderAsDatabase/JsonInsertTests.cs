using Data.Common.Extension;
using Data.Tests.Common;
using System.Data.JsonClient;
using System.Reflection;
using Xunit;

namespace Data.Json.Tests.FolderAsDatabase;

/// <summary>
/// These tests indirectly exercise the <see cref="JsonIO.Write.JsonInsert"/> class via calls to <see cref="JsonCommand.ExecuteNonQuery" />.
/// </summary>
public class JsonInsertTests
{
    [Fact]
    public void Insert_ShouldInsertData()
    {
        var sandboxId = $"{GetType().FullName}.{MethodBase.GetCurrentMethod()!.Name}";
        InsertTests.Insert_ShouldInsertData(() => new JsonConnection(ConnectionStrings.Instance.FolderAsDB.Sandbox("Sandbox", sandboxId)));
    }

    [Fact]
    public void Insert_ShouldInsertNullData()
    {
        var sandboxId = $"{GetType().FullName}.{MethodBase.GetCurrentMethod()!.Name}";
        InsertTests.Insert_ShouldInsertNullData(() => new JsonConnection(ConnectionStrings.Instance.FolderAsDB.Sandbox("Sandbox", sandboxId)));
    }

    //This is a special case.  In json, if a table does not have any rows in it, then we have no schema information on the columns or their data types.
    //Inserting the first row into this table will then determine the columns (along with their data types) in this table.
    [Fact]
    public void Insert_ShouldInsertDataIntoEmptyTables()
    {
        var sandboxId = $"{GetType().FullName}.{MethodBase.GetCurrentMethod()!.Name}";
        var sandboxConnectionString = ConnectionStrings.Instance.EmptyWithTablesFolderAsDB.Sandbox("Sandbox", sandboxId);

        //Setup the tables by inserting the first row into them.
        InsertTests.Insert_ShouldInsertData(() => new JsonConnection(sandboxConnectionString));

        //Assert by reading from the tables.
        InsertTests.Insert_ShouldInsertDataIntoEmptyTables(() => new JsonConnection(sandboxConnectionString));
    }

    [Fact]
    public void Insert_JsonShouldBeFormatted()
    {
        InsertTests.Insert_ShouldBeFormatted(() =>
        new JsonConnection(ConnectionStrings.Instance
        .FolderAsDB.AddFormatted(true)));
    }

    [Fact]
    public void Insert_IndentityColumn_NoLastRow()
    {
        //NOTE: Without a single row, there is no way for the Json Provider to 'know' of an indentity column
    }

    [Fact]
    public void Insert_IndentityColumn_LastRow()
    {
        var sandboxId = $"{GetType().FullName}.{MethodBase.GetCurrentMethod()!.Name}";
        InsertTests.Insert_IndentityColumn_LastRow_Decimal(
            () => new JsonConnection(ConnectionStrings.Instance.FolderAsDB.Sandbox("Sandbox", sandboxId)));
    }

}