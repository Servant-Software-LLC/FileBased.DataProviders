using Data.Common.Utils.ConnectionString;
using EFCore.Common.Tests.Utils;

namespace EFCore.Csv.Tests.Utils;

public class ConnectionStrings : ConnectionStringsBase
{
    public override string Extension => "cvs";

    public override FileConnectionString FileAsDB => throw new NotImplementedException();

    public new static ConnectionStrings Instance => new ConnectionStrings();
}
