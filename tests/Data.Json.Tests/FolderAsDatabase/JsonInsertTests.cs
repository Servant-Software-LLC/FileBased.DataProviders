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
        InsertTests.Insert_ShouldInsertData(() => new JsonConnection(ConnectionStrings.Instance.FolderAsDBConnectionString.Sandbox("Sandbox", sandboxId)));
    }

    [Fact]
    public void Insert_JsonShouldBeFormatted()
    {
        InsertTests.Insert_ShouldBeFormatted(() =>
        new JsonConnection(ConnectionStrings.Instance
        .FolderAsDBConnectionString.AddFormatted(true)));
    }
}