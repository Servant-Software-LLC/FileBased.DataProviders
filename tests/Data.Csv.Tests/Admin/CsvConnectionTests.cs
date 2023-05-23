using Data.Common.Utils.ConnectionString;
using Data.Tests.Common;
using System.Data.CsvClient;
using Xunit;

namespace Data.Csv.Tests.Admin;

public class CsvConnectionTests
{
    [Fact]
    public void OpenConnection_Success()
    {
        ConnectionTests.OpenConnection_AdminMode_Success(getConnectionString =>
        {
            FileConnectionString connectionString = getConnectionString(ConnectionStrings.Instance);
            return new CsvConnection(connectionString);
        });
    }
}
