namespace EFCore.FileBasedProviders.Common;

public class ConnectionString
{
    public static string Format(string dataSource) => $"DataSource={dataSource};";
}