using Data.Tests.Common;
using System.Data.JsonClient;
using System.Reflection;
using Xunit;

namespace Data.Json.Tests.FolderAsDatabase;

/// <summary>
/// These tests indirectly exercise the <see cref="JsonIO.Write.JsonUpdate"/> class via calls to <see cref="JsonCommand.ExecuteNonQuery" />/>. 
/// </summary>
public class JsonUpdateTests
{
    [Fact]
    public void Update_ShouldUpdateData()
    {
        var sandboxId = $"{GetType().FullName}.{MethodBase.GetCurrentMethod()!.Name}";
        UpdateTests.Update_ShouldUpdateData(
            () => new JsonConnection(ConnectionStrings.Instance.FolderAsDBConnectionString.Sandbox("Sandbox", sandboxId))
        );
    }

}
