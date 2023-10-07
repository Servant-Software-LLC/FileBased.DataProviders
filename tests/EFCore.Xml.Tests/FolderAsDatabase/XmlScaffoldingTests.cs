using System.Reflection;
using Xunit;
using Data.Common.Extension;
using EFCore.Common.Tests;
using EFCore.Xml.Tests.Utils;
using EFCore.Xml.Design.Internal;

namespace EFCore.Xml.Tests.FolderAsDatabase;

public class XmlScaffoldingTests
{
    [Fact]
    public void ValidateScaffolding()
    {
        var sandboxId = $"{GetType().FullName}.{MethodBase.GetCurrentMethod()!.Name}";
        var connectionString = ConnectionStrings.Instance.FolderAsDB.Sandbox("Sandbox", sandboxId);
        ScaffoldingTests.ValidateScaffolding(connectionString, new XmlDesignTimeServices());
    }
}
