using Data.Tests.Common;
using System.Data.XlsClient;
using Xunit;

namespace Data.Xls.Tests;

public class XlsClientFactoryTests
{
    [Fact]
    public void GetFactory_FromConnection_ReturnsXlsClientFactory()
    {
        using var connection = new XlsConnection();
        ClientFactoryTests.GetFactory_FromConnection_ReturnsSameInstance(connection, XlsClientFactory.Instance);
    }
}
