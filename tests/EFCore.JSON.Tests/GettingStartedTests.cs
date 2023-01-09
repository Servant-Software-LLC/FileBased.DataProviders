using EFCore.Common.Tests;
using EFCore.FileBasedProviders.Common;
using EFCore.JSON.Tests.Models;
using Xunit;

namespace EFCore.JSON.Tests;

/// <summary>
/// Unit tests created based off of the Getting Started page in EF Core.  REF: https://learn.microsoft.com/en-us/ef/core/get-started/overview/first-app?tabs=netcore-cli
/// </summary>
public class GettingStartedTests : GettingStartedBaseTests<BloggingContext>
{

    [Fact]
    public void Create() => Logic_Create(ConnectionString.Format(CreateTempFolder()));


}
