using EFCore.Common.Tests;
using EFCore.FileBasedProviders.Common;
using EFCore.Json.Tests.Models;
using Xunit;

namespace EFCore.Json.Tests;

/// <summary>
/// Unit tests created based off of the Getting Started page in EF Core.  REF: https://learn.microsoft.com/en-us/ef/core/get-started/overview/first-app?tabs=netcore-cli
/// </summary>
public class GettingStartedTests : GettingStartedBaseTests<BloggingContext>
{

    [Fact]
    public void Create() => Logic_Create(ConnectionString.Format(CreateTempFolder()));


}
