using System.Reflection;
using Xunit;
using Data.Common.Extension;
using EFCore.Common.Tests;
using EFCore.Xml.Tests.Utils;
using EFCore.Xml.Design.Internal;

namespace EFCore.Xml.Tests.FolderAsDatabase;

/// <summary>
/// This class contains tests for validating the scaffolding of XML files as a database.
/// </summary>
public class XmlScaffoldingTests
{
    /// <summary>
    /// Validates the scaffolding of XML files as a database.
    /// </summary>
    [Fact]
    public void ValidateScaffolding()
    {
        var sandboxId = $"{GetType().FullName}.{MethodBase.GetCurrentMethod()!.Name}";
        var connectionString = ConnectionStrings.Instance.gettingStartedFolderDB.Sandbox("Sandbox", sandboxId);
        ScaffoldingTests.ValidateScaffolding(connectionString, new XmlDesignTimeServices());
    }
}
