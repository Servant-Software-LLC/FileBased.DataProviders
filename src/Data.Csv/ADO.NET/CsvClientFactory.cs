using Data.Csv.Utils;
using System.Data.Common;

namespace System.Data.CsvClient;

public sealed class CsvClientFactory : DbProviderFactory
{
    public static readonly CsvClientFactory Instance = new CsvClientFactory();

    private CsvClientFactory() { }

    public override DbCommand CreateCommand() => new CsvCommand();

    /// <summary>
    /// Intendes to be addressed in future versions.  REF: https://github.com/Servant-Software-LLC/FileBased.DataProviders/issues/75
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotSupportedException"></exception>
    public override DbCommandBuilder CreateCommandBuilder() => throw new NotSupportedException("CommandBuilder is not implemented for this provider.");

    public override DbConnection CreateConnection() => new CsvConnection();

    public override DbConnectionStringBuilder CreateConnectionStringBuilder() => new CsvConnectionStringBuilder();

    public override DbDataAdapter CreateDataAdapter() => new CsvDataAdapter();

    public override DbParameter CreateParameter() => new CsvParameter();
}
