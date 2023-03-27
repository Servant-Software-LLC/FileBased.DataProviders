using Data.Common.Extension;
using Data.Tests.Common;
using System.Data.CsvClient;
using System.Reflection;
using Xunit;

namespace Data.Csv.Tests.FolderAsDatabase;

/// <summary>
/// These tests indirectly exercise the <see cref="JsonIO.Write.JsonUpdate"/> class via calls to <see cref="CsvCommand.ExecuteNonQuery" />/>. 
/// </summary>
public class CsvUpdateTests
{
    [Fact]
    public void Update_ShouldUpdateData()
    {
        var sandboxId = $"{GetType().FullName}.{MethodBase.GetCurrentMethod()!.Name}";
        UpdateTests.Update_ShouldUpdateData(
            () => new CsvConnection(ConnectionStrings.Instance.FolderAsDBConnectionString.Sandbox("Sandbox", sandboxId))
        );
    }

}
