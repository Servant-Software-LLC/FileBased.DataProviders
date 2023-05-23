using Data.Common.Utils.ConnectionString;
using Data.Tests.Common;
using System.Data.JsonClient;
using Xunit;

namespace Data.Json.Tests.Admin;

public class JsonConnectionTests
{
    [Fact]
    public void OpenConnection_Success()
    {
        ConnectionTests.OpenConnection_AdminMode_Success(getConnectionString =>
        {
            FileConnectionString connectionString = getConnectionString(ConnectionStrings.Instance);
            return new JsonConnection(connectionString);
        });
    }

}
