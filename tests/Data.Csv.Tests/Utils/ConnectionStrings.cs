namespace Data.Csv.Tests;

public static class ConnectionStrings
{
    private static string Folder = Path.Combine("Sources", "Folder");
    public static string FolderAsDBConnectionString = $"Data Source={Folder}";

    public static string AddFormatted(string connectionString, bool formatted) => 
        connectionString += $"; Formatted={formatted}";
}