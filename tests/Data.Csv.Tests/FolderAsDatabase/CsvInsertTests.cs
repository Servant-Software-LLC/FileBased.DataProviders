using Data.Common.Extension;
using Data.Tests.Common;
using Data.Tests.Common.Utils;
using System.Data.CsvClient;
using System.Reflection;
using Xunit;

namespace Data.Csv.Tests.FolderAsDatabase;

/// <summary>
/// These tests indirectly exercise the <see cref="JsonIO.Write.JsonInsert"/> class via calls to <see cref="CsvCommand.ExecuteNonQuery" />.
/// </summary>
public class CsvInsertTests
{
    [Fact]
    public void Insert_ShouldInsertData()
    {
        var sandboxId = $"{GetType().FullName}.{MethodBase.GetCurrentMethod()!.Name}";
        InsertTests.Insert_ShouldInsertData(() => new CsvConnection(ConnectionStrings.Instance.FolderAsDB.Sandbox("Sandbox", sandboxId)));
    }

    [Fact]
    public void Insert_ShouldInsertData_CustomDataSource()
    {
        var sandboxId = $"{GetType().FullName}.{MethodBase.GetCurrentMethod()!.Name}";
        InsertTests.Insert_ShouldInsertData(() =>
            CustomDataSourceFactory.VirtualFolderAsDB((connectionString) => new CsvConnection(connectionString)));
    }

    [Fact]
    public void Insert_ShouldInsertNullData()
    {
        var sandboxId = $"{GetType().FullName}.{MethodBase.GetCurrentMethod()!.Name}";
        InsertTests.Insert_ShouldInsertNullData(() => new CsvConnection(ConnectionStrings.Instance.FolderAsDB.Sandbox("Sandbox", sandboxId)));
    }

    [Fact]
    public void Insert_ShouldBeFormatted()
    {
        var sandboxId = $"{GetType().FullName}.{MethodBase.GetCurrentMethod()!.Name}";
        var sandboxConnectionString = ConnectionStrings.Instance.FolderAsDB.AddFormatted(true).Sandbox("Sandbox", sandboxId);

        InsertTests.Insert_ShouldBeFormatted(() => new CsvConnection(sandboxConnectionString));
    }

    [Fact]
    public void Insert_IndentityColumn_NoLastRow()
    {
        //TODO: Without a single row, there is no way for the Csv Provider to 'know' of an indentity column
    }

    [Fact]
    public void Insert_IndentityColumn_LastRow()
    {
        var sandboxId = $"{GetType().FullName}.{MethodBase.GetCurrentMethod()!.Name}";
        InsertTests.Insert_IndentityColumn_LastRow_Decimal(
            () => new CsvConnection(ConnectionStrings.Instance.FolderAsDB.Sandbox("Sandbox", sandboxId)));
    }
}