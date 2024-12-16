using System.Reflection;
using Xunit;
using Data.Common.Extension;
using EFCore.Common.Tests;
using EFCore.Xml.Tests.Utils;
using EFCore.Xml.Design.Internal;

namespace EFCore.Xml.Tests.FileAsDatabase;

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
        var connectionString = ConnectionStrings.Instance.gettingStartedFileDB.Sandbox("Sandbox", sandboxId);
        ScaffoldingTests.ValidateScaffolding(connectionString,  new XmlDesignTimeServices());
    }

    [Fact]
    public void Scaffolding_AllColumnsPresent()
    {
        var sandboxId = $"{GetType().FullName}.{MethodBase.GetCurrentMethod()!.Name}";
        string connectionString = ConnectionStrings.Instance.gettingStartedWithDataFileDB.Sandbox("Sandbox", sandboxId);
        ScaffoldingTests.ValidateScaffolding(connectionString, new XmlDesignTimeServices(), "MockDB.RemoteTable.Xml.GettingStartedWithData", null,
            scaffoldedModel =>
            {
                Assert.NotNull(scaffoldedModel);
                Assert.NotEmpty(scaffoldedModel.AdditionalFiles);
                Assert.NotNull(scaffoldedModel.ContextFile);

                var firstAdditionalFile = scaffoldedModel.AdditionalFiles[0];
                Assert.Contains("BlogId { get; set; }", firstAdditionalFile.Code);
                Assert.Contains("Url { get; set; }", firstAdditionalFile.Code);

                var secondAdditionalFile = scaffoldedModel.AdditionalFiles[1];
                Assert.Contains("BlogId { get; set; }", secondAdditionalFile.Code);
                Assert.Contains("Content { get; set; }", secondAdditionalFile.Code);
                Assert.Contains("PostId { get; set; }", secondAdditionalFile.Code);
                Assert.Contains("Title { get; set; }", secondAdditionalFile.Code);
            }
        );
    }
}
