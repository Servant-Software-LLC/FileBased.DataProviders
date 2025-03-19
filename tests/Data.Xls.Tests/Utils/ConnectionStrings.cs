using Data.Tests.Common.Utils;

namespace Data.Xls.Tests;

public class ConnectionStrings : ConnectionStringsBase
{
    public override string Extension => "xlsx";

    public new static ConnectionStrings Instance => new ConnectionStrings();
}