using EFCore.Common.Storage.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data.XmlClient;

namespace EFCore.Xml.Storage.Internal;


public class XmlDatabaseCreator : FileDatabaseCreator
{
    //TODO: Remove if not used after implementation is fully complete.  It may be that we don't even need all these derived classes and can 
    //      make FileDatabaseCreator non-abstract and register it.
    private readonly IXmlRelationalConnection connection;

    public XmlDatabaseCreator(
        RelationalDatabaseCreatorDependencies dependencies,
        IXmlRelationalConnection connection,
        IRawSqlCommandBuilder rawSqlCommandBuilder)
        : base(dependencies, rawSqlCommandBuilder)
    {
        this.connection = connection ?? throw new ArgumentNullException(nameof(connection));
    }

    public override void Create() => 
        Create<XmlConnection, XmlParameter>(connString => new XmlConnection(connString));

    public override void Delete() =>
        Delete<XmlConnection, XmlParameter>(connString => new XmlConnection(connString));

}
