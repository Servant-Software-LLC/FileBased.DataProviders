using Data.Tests.Common;
using Data.Common.Extension;
using System.Data.JsonClient;
using System.Reflection;
using Xunit;
using Data.Common.Utils.ConnectionString;
using Data.Common.DataSource;
using System.Data.Common;

namespace Data.Json.Tests.FileAsDatabase;

public class JsonAlterTableTests
{
    [Fact]
    public void AddColumn_EmptyTable_ColumnAdded()
    {
        var sandboxId = $"{GetType().FullName}.{MethodBase.GetCurrentMethod()!.Name}";
        AlterTableTests.AddColumn_EmptyTable_ColumnAdded(
                       () => new JsonConnection(ConnectionStrings.Instance.FileAsDB.Sandbox("Sandbox", sandboxId)));
    }

    [Fact]
    public void DropColumn_ColumnExists_Dropped()
    {
        var sandboxId = $"{GetType().FullName}.{MethodBase.GetCurrentMethod()!.Name}";
        AlterTableTests.DropColumn_ColumnExists_Dropped(
                       () => new JsonConnection(ConnectionStrings.Instance.FileAsDB.Sandbox("Sandbox", sandboxId)));
    }

    [Fact]
    public void DropColumn_SettingsOnADO_Support()
    {
        const string jsonString = @"
[
  {
    ""CaseSensitive"": true
    /*This is the database name used in the MockDB GraphQL API when it is not provided for the Name field of the Database object.*/,
    ""DefaultDatabaseName"": ""NewMockDB"",
    ""MaxAllTableRowCount"": 3000,
    ""SeedDatabases"": true,
	""SettingToBeDeprecated"": 1234
  }
]
";
        const string tableName = "GeneralSettings";

        var connection = new JsonConnection(FileConnectionString.CustomDataSource);
        TableStreamedDataSource dataSourceProvider = new("MyDatabase", tableName, jsonString);
        connection.DataSourceProvider = dataSourceProvider;
        connection.Open();

        string sql = $"ALTER TABLE {tableName} DROP COLUMN SettingToBeDeprecated;";

        // Execute the command
        using (DbCommand command = connection.CreateCommand())
        {
            command.CommandText = sql;

            command.ExecuteNonQuery();
        }

    }
}
