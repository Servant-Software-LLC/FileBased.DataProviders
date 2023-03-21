namespace Data.Xml.Tests;

public static class ConnectionStrings
{
    private static string Folder = Path.Combine("Sources", "Folder");
    private static string File = Path.Combine("Sources", "database.xml");
    private static string eComDataBase = Path.Combine("Sources", "ecommerce.xml");

    public static string FolderAsDBConnectionString = $"Data Source={Folder}";
    public static string FileAsDBConnectionString = $"Data Source={File}";

    public static string eComDBConnectionString = $"Data Source={eComDataBase}";

    public static string AddFormatted(this string connectionString, bool formatted) => connectionString += $"; Formatted={formatted}";
}