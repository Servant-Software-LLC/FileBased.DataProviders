using EFCore.Common.Infrastructure.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data.Common;
using System.Data.XmlClient;

namespace EFCore.Xml.Storage.Internal;

public class XmlRelationalConnection : FileRelationalConnection, IXmlRelationalConnection
{
    public XmlRelationalConnection(RelationalConnectionDependencies dependencies)
        : base(dependencies)
    {
    }

    protected override DbConnection CreateDbConnection()
    {
        var connection = new XmlConnection(ConnectionString);
        if (DataSourceProvider != null)
            connection.DataSourceProvider = DataSourceProvider;

        return connection;
    }
}
