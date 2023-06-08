using EFCore.Common.Tests.Utils;

namespace EFCore.Json.Tests.Utils;

public class ConnectionStrings : ConnectionStringsBase
{
    public override string Extension => "json";

    public new static ConnectionStrings Instance => new ConnectionStrings();

}
