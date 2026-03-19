using Data.Csv.Utils;
using System.Data.Common;

namespace System.Data.CsvClient;

public sealed class CsvClientFactory : DbProviderFactory
{
    public static readonly CsvClientFactory Instance = new CsvClientFactory();

    private CsvClientFactory() { }

    public override DbCommand CreateCommand() => new CsvCommand();

    public override DbCommandBuilder CreateCommandBuilder() => new CsvCommandBuilder();

    public override DbConnection CreateConnection() => new CsvConnection();

    public override DbConnectionStringBuilder CreateConnectionStringBuilder() => new CsvConnectionStringBuilder();

    public override DbDataAdapter CreateDataAdapter() => new CsvDataAdapter();

    public override DbParameter CreateParameter() => new CsvParameter();
}
