using Data.Json.Utils;
using System.Data.Common;

namespace System.Data.JsonClient;

public sealed class JsonClientFactory : DbProviderFactory
{
    public static readonly JsonClientFactory Instance = new JsonClientFactory();

    private JsonClientFactory() { }

    public override DbCommand CreateCommand() => new JsonCommand();

    public override DbCommandBuilder CreateCommandBuilder() => new JsonCommandBuilder();

    public override DbConnection CreateConnection() => new JsonConnection();

    public override DbConnectionStringBuilder CreateConnectionStringBuilder() => new JsonConnectionStringBuilder();

    public override DbDataAdapter CreateDataAdapter() => new JsonDataAdapter();

    public override DbParameter CreateParameter() => new JsonParameter();

}
