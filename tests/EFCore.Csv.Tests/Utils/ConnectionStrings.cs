using Data.Common.Utils.ConnectionString;
using EFCore.Common.Tests.Utils;

namespace EFCore.Csv.Tests.Utils;

public class ConnectionStrings : ConnectionStringsBase
{
    public override string Extension => "csv";

    public override FileConnectionString gettingStartedFileDB => throw new NotImplementedException();
    public override FileConnectionString gettingStartedWithDataFileDB => throw new NotImplementedException();

    public new static ConnectionStrings Instance => new ConnectionStrings();
}
