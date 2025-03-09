using Data.Common.DataSource;
using Data.Common.Utils.ConnectionString;
using System.Data.CsvClient;
using System.Text;

namespace Data.Csv.Tests.Utils;

/// <summary>
/// Simplify writing unit tests where a unit test just needs to provide the CSV content as a string
/// </summary>
internal static class CsvConnectionFactory
{
    public static CsvConnection GetCsvConnection(string tableName, string csvContent)
    {
        byte[] fileBytes = Encoding.UTF8.GetBytes(csvContent);
        MemoryStream fileStream = new MemoryStream(fileBytes);
        var connection = new CsvConnection(FileConnectionString.CustomDataSource);
        StreamedDataSource dataSourceProvider = new(tableName, fileStream);

        connection.DataSourceProvider = dataSourceProvider;

        return connection;
    }
}
