using System.Data.Common;

namespace System.Data.XmlClient;

public sealed class XmlClientFactory : DbProviderFactory
{
    public static readonly XmlClientFactory Instance = new XmlClientFactory();

    private XmlClientFactory() { }

    public override DbCommand CreateCommand() => new XmlCommand();

    /// <summary>
    /// Intendes to be addressed in future versions.  REF: https://github.com/Servant-Software-LLC/FileBased.DataProviders/issues/75
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotSupportedException"></exception>
    public override DbCommandBuilder CreateCommandBuilder() => throw new NotSupportedException("CommandBuilder is not implemented for this provider.");

    public override DbConnection CreateConnection() => new XmlConnection();

    /// <summary>
    /// Intendes to be addressed in future versions.  REF: https://github.com/Servant-Software-LLC/FileBased.DataProviders/issues/74
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotSupportedException"></exception>
    public override DbConnectionStringBuilder CreateConnectionStringBuilder() => throw new NotSupportedException("ConnectionStringBuilder is not implemented for this provider.");

    public override DbDataAdapter CreateDataAdapter() => new XmlDataAdapter();

    public override DbParameter CreateParameter() => new XmlParameter();

}
