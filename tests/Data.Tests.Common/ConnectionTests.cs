using Data.Common.Utils.ConnectionString;
using Data.Tests.Common.Utils;
using System.Data.FileClient;

namespace Data.Tests.Common;

public static class ConnectionTests
{
    public static void OpenConnection_AdminMode_Success<TFileParameter>(Func<Func<ConnectionStringsBase, FileConnectionString>, FileConnection<TFileParameter>> createConnection)
        where TFileParameter : FileParameter<TFileParameter>, new()
    {
        // Arrange
        var connection = createConnection(connString => connString.Admin);

        // Act (and Assert that no exception occurs)
        connection.Open();

    }
}
