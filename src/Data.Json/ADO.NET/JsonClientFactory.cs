using Data.Json.Utils;
using System.Data.Common;

namespace System.Data.JsonClient;

public sealed class JsonClientFactory : DbProviderFactory
{
    public static readonly JsonClientFactory Instance = new JsonClientFactory();

    private JsonClientFactory() { }

    public override DbCommand CreateCommand() => new JsonCommand();

    /// <summary>
    /// Intendes to be addressed in future versions.  REF: https://github.com/Servant-Software-LLC/FileBased.DataProviders/issues/75
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotSupportedException"></exception>
    public override DbCommandBuilder CreateCommandBuilder() => throw new NotSupportedException("CommandBuilder is not implemented for this provider.");

    public override DbConnection CreateConnection() => new JsonConnection();

    public override DbConnectionStringBuilder CreateConnectionStringBuilder() => new JsonConnectionStringBuilder();

    public override DbDataAdapter CreateDataAdapter() => new JsonDataAdapter();

    public override DbParameter CreateParameter() => new JsonParameter();

}
