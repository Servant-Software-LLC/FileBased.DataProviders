using Data.Common.Extension;
using Data.Tests.Common;
using System.Data.JsonClient;
using System.Reflection;
using Xunit;

namespace Data.Json.Tests.FileAsDatabase;

public class JsonDeleteTests
{
    [Fact]
    public void Delete_ShouldDeleteData()
    {
        var sandboxId = $"{GetType().FullName}.{MethodBase.GetCurrentMethod()!.Name}";
        DeleteTests.Delete_ShouldDeleteData(() => new JsonConnection(ConnectionStrings.Instance.FileAsDB.Sandbox("Sandbox", sandboxId)));
    }

}
