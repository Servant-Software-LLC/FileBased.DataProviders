using Data.Tests.Common.Utils;

namespace Data.Json.Tests;

public class ConnectionStrings : ConnectionStringsBase
{
    public override string Extension => "json";

    public new static ConnectionStrings Instance => new ConnectionStrings();
}