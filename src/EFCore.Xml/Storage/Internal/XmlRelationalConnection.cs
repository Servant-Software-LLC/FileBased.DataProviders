using Microsoft.EntityFrameworkCore.Storage;
using System.Data.Common;
using System.Data.XmlClient;

namespace EFCore.Xml.Storage.Internal;

public class XmlRelationalConnection : RelationalConnection, IXmlRelationalConnection
{
    public XmlRelationalConnection(RelationalConnectionDependencies dependencies)
        : base(dependencies)
    {
    }

    protected override DbConnection CreateDbConnection() => new XmlConnection(ConnectionString);
}
