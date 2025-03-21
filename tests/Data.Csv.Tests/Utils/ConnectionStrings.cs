using Data.Common.Utils.ConnectionString;
using Data.Tests.Common.Utils;

namespace Data.Csv.Tests;

public class ConnectionStrings : ConnectionStringsBase
{
    public override string Extension => "csv";

    public override FileConnectionString FileAsDB => throw new NotImplementedException();
    public override FileConnectionString EmptyWithTablesFileAsDB => throw new NotImplementedException();
    public override FileConnectionString eComFileDB => throw new NotImplementedException();
    public override FileConnectionString gettingStartedFileDB => throw new NotImplementedException();
    public override FileConnectionString bogusFileDB => throw new NotImplementedException();

    public new static ConnectionStrings Instance => new ConnectionStrings();
}