namespace Data.Json.Tests;

public static class ConnectionStrings
{
    private static string Folder = Path.Combine("Sources", "Folder");
    private static string File = Path.Combine("Sources", "database.json");

    public static string FolderAsDBConnectionString = $"Data Source={Folder}";
    public static string FileAsDBConnectionString = $"Data Source={File}";
}