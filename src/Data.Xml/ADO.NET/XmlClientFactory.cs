using Data.Xml.Utils;
using System.Data.Common;

namespace System.Data.XmlClient;

public sealed class XmlClientFactory : DbProviderFactory
{
    public static readonly XmlClientFactory Instance = new XmlClientFactory();

    private XmlClientFactory() { }

    public override DbCommand CreateCommand() => new XmlCommand();

    public override DbCommandBuilder CreateCommandBuilder() => new XmlCommandBuilder();

    public override DbConnection CreateConnection() => new XmlConnection();

    public override DbConnectionStringBuilder CreateConnectionStringBuilder() => new XmlConnectionStringBuilder();

    public override DbDataAdapter CreateDataAdapter() => new XmlDataAdapter();

    public override DbParameter CreateParameter() => new XmlParameter();

}
