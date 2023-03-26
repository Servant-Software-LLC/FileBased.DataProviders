using Data.Tests.Common.Utils;

namespace Data.Xml.Tests;

public class ConnectionStrings : ConnectionStringsBase
{
    public override string Extension => "xml";

    public new static ConnectionStrings Instance => new ConnectionStrings();
}