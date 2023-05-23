using Data.Common.Utils.ConnectionString;
using Data.Tests.Common;
using System.Data.XmlClient;
using Xunit;

namespace Data.Xml.Tests.Admin;

public class XmlConnectionTests
{
    [Fact]
    public void OpenConnection_Success()
    {
        ConnectionTests.OpenConnection_AdminMode_Success(getConnectionString =>
        {
            FileConnectionString connectionString = getConnectionString(ConnectionStrings.Instance);
            return new XmlConnection(connectionString);
        });
    }

}
