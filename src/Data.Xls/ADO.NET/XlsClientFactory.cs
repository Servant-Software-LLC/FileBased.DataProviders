using System.Data.Common;

namespace System.Data.XlsClient;

public sealed class XlsClientFactory : DbProviderFactory
{
    public static readonly XlsClientFactory Instance = new XlsClientFactory();

    private XlsClientFactory() { }

    public override DbCommand CreateCommand() => new XlsCommand();

    /// <summary>
    /// Intendes to be addressed in future versions.  REF: https://github.com/Servant-Software-LLC/FileBased.DataProviders/issues/75
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotSupportedException"></exception>
    public override DbCommandBuilder CreateCommandBuilder() => throw new NotSupportedException("CommandBuilder is not implemented for this provider.");

    public override DbConnection CreateConnection() => new XlsConnection();

    /// <summary>
    /// Intendes to be addressed in future versions.  REF: https://github.com/Servant-Software-LLC/FileBased.DataProviders/issues/74
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotSupportedException"></exception>
    public override DbConnectionStringBuilder CreateConnectionStringBuilder() => throw new NotSupportedException("ConnectionStringBuilder is not implemented for this provider.");

    public override DbDataAdapter CreateDataAdapter() => throw new InvalidOperationException("The XSL provider does not support adapters.");

    public override DbParameter CreateParameter() => new XlsParameter();
}
