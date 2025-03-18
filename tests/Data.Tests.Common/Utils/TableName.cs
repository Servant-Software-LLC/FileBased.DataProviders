namespace Data.Tests.Common.Utils;

public static class TableName
{
    public static string GetTableName(string databaseName, string tableName) 
    {
        return string.IsNullOrWhiteSpace(databaseName) ? tableName : $"{databaseName} - {tableName}";
    }
}