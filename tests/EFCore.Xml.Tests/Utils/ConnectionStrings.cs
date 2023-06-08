using EFCore.Common.Tests.Utils;

namespace EFCore.Xml.Tests.Utils;

public class ConnectionStrings : ConnectionStringsBase
{
    public override string Extension => "xml";

    public new static ConnectionStrings Instance => new ConnectionStrings();

}
