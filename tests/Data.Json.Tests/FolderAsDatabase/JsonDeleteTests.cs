using Data.Tests.Common;
using System.Data.JsonClient;
using System.Reflection;
using Xunit;

namespace Data.Json.Tests.FolderAsDatabase;

/// <summary>
/// These tests indirectly exercise the <see cref="JsonIO.Delete.JsonDelete"/> class via the <see cref="JsonCommand" calls to the different forms of the Execute methods/>. 
/// </summary>
public class JsonDeleteTests
{
    [Fact]
    public void Delete_ShouldDeleteData()
    {
        var sandboxId = $"{GetType().FullName}.{MethodBase.GetCurrentMethod()!.Name}";
        DeleteTests.Delete_ShouldDeleteData(() => new JsonConnection(ConnectionStrings.Instance.FolderAsDBConnectionString.Sandbox("Sandbox", sandboxId)));
    }

}
