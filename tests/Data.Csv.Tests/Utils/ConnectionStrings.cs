using Data.Common.Extension;
using Data.Common.Utils.ConnectionString;
using Data.Tests.Common.Utils;

namespace Data.Csv.Tests;

public class ConnectionStrings : ConnectionStringsBase
{
    public override string Extension => "cvs";

    public override FileConnectionString FileAsDBConnectionString => throw new NotImplementedException();

    protected override string eComDataBase => Path.Combine(FileConnectionStringTestsExtensions.SourcesFolder, $"eCom");
    public override FileConnectionString eComDBConnectionString => new FileConnectionString() { DataSource = eComDataBase };

    public new static ConnectionStrings Instance => new ConnectionStrings();
}