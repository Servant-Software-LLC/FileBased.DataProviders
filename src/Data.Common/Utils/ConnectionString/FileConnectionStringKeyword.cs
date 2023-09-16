namespace Data.Common.Utils.ConnectionString;

public enum FileConnectionStringKeyword
{
    [Alias("Data Source")]
    DataSource,
    Formatted,
    [Alias("Log")]
    LogLevel,
    CreateIfNotExist,
}
