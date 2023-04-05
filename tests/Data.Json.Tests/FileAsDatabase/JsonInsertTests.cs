using Data.Tests.Common;
using System.Data.JsonClient;
using System.Reflection;
using Xunit;

namespace Data.Json.Tests.FileAsDatabase;

public class JsonInsertTests
{
    [Fact]
    public void Insert_ShouldInsertData()
    {
        var sandboxId = $"{GetType().FullName}.{MethodBase.GetCurrentMethod()!.Name}";
        InsertTests.Insert_ShouldInsertData(() => new JsonConnection(ConnectionStrings.Instance.FileAsDBConnectionString.Sandbox("Sandbox", sandboxId)));
    }

    [Fact]
    public void Insert_JsonShouldBeFormatted()
    {
        InsertTests.Insert_ShouldBeFormattedForFile(() =>
        new JsonConnection(ConnectionStrings.Instance
        .FileAsDBConnectionString.AddFormatted(true)));
    }
}