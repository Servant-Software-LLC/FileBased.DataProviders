using Data.Common.Extension;
using Data.Tests.Common;
using System.Data.JsonClient;
using System.Reflection;
using Xunit;

namespace Data.Json.Tests.FileAsDatabase;

public class JsonCreateTableTests
{
    /// <summary>
    /// When an exception occurs while creating a table, the JSON file ends up with an empty array.  SettingsOnADO later calls
    /// CREATE TABLE on the same file, but throws because the file exists.  This scenario can also occur when after a CREATE TABLE
    /// occurs and then a DROP COLUMN occurs.  Therefore, the CREATE TABLE statement should be able to handle an empty array.
    /// </summary>
    [Fact]
    public void CreateTable_WhenNoColumns_ShouldWork()
    {
        var sandboxId = $"{GetType().FullName}.{MethodBase.GetCurrentMethod()!.Name}";
        CreateTableTests.CreateTable_WhenNoColumns_ShouldWork(
                       () => new JsonConnection(ConnectionStrings.Instance.FileAsDB.Sandbox("Sandbox", sandboxId)));

    }
}
