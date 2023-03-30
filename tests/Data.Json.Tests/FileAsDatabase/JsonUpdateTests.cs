using Data.Common.Extension;
using Data.Tests.Common;
using System.Data.JsonClient;
using System.Reflection;
using Xunit;

namespace Data.Json.Tests.FileAsDatabase;

public class JsonUpdateTests
{
    [Fact]
    public void Update_ShouldUpdateData()
    {
        var sandboxId = $"{GetType().FullName}.{MethodBase.GetCurrentMethod()!.Name}";
        UpdateTests.Update_ShouldUpdateData(
            () => new JsonConnection(ConnectionStrings.Instance.FileAsDB.Sandbox("Sandbox", sandboxId))
        );
    }

}
