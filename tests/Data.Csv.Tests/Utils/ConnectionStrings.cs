using Data.Common.Extension;
using Data.Common.Utils.ConnectionString;
using Data.Tests.Common.Utils;

namespace Data.Csv.Tests;

public class ConnectionStrings : ConnectionStringsBase
{
    public override string Extension => "cvs";

    public override FileConnectionString FileAsDB => throw new NotImplementedException();

    public override FileConnectionString FileAsDB_eCom => throw new NotImplementedException();

    public new static ConnectionStrings Instance => new ConnectionStrings();
}