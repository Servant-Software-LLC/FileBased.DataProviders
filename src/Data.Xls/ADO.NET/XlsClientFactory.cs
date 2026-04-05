using Data.Xls.Utils;
using System.Data.Common;

namespace System.Data.XlsClient;

public sealed class XlsClientFactory : DbProviderFactory
{
    public static readonly XlsClientFactory Instance = new XlsClientFactory();

#if NET7_0_OR_GREATER
    static XlsClientFactory()
    {
        DbProviderFactories.RegisterFactory("System.Data.XlsClient", Instance);
    }
#endif

    private XlsClientFactory() { }

    public override DbCommand CreateCommand() => new XlsCommand();

    public override DbCommandBuilder CreateCommandBuilder() => new XlsCommandBuilder();

    public override DbConnection CreateConnection() => new XlsConnection();

    public override DbConnectionStringBuilder CreateConnectionStringBuilder() => new XlsConnectionStringBuilder();

    public override DbDataAdapter CreateDataAdapter() => throw new InvalidOperationException("The XSL provider does not support adapters.");

    public override DbParameter CreateParameter() => new XlsParameter();
}
