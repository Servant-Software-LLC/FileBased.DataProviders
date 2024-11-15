namespace Data.Common.Utils.ConnectionString;

public enum FileConnectionStringKeyword
{
    [Alias("Data Source")]
    DataSource,
    Formatted,
    [Alias("FloatingPoint")]
    PreferredFloatingPointDataType,
    [Alias("Log")]
    LogLevel,
    CreateIfNotExist,
}
