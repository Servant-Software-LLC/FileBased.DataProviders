﻿using Data.Common.Extension;
using EFCore.Json.Tests.Utils;
using Microsoft.Extensions.Logging;
using System.Reflection;
using Xunit;
using GettingStarted = EFCore.Common.Tests.GettingStartedTests<EFCore.Json.Tests.Models.BloggingContext>;

namespace EFCore.Json.Tests.FileAsDatabase;

/// <summary>
/// Unit tests created based off of the Getting Started page in EF Core.  REF: https://learn.microsoft.com/en-us/ef/core/get-started/overview/first-app?tabs=netcore-cli
/// </summary>
public class JsonGettingStartedTests
{
    [Fact]
    public void Create_AddBlog()
    {
        var sandboxId = $"{GetType().FullName}.{MethodBase.GetCurrentMethod()!.Name}";
        GettingStarted.Create_AddBlog(ConnectionStrings.Instance.gettingStartedFileDB.Sandbox("Sandbox", sandboxId).AddLogging(LogLevel.Debug));
    }

    [Fact(Skip = "Temp: Not needed for current goal")]
    public void Read_FirstBlog()
    {
        var sandboxId = $"{GetType().FullName}.{MethodBase.GetCurrentMethod()!.Name}";
        GettingStarted.Read_FirstBlog(ConnectionStrings.Instance.gettingStartedFileDB.Sandbox("Sandbox", sandboxId));
    }

    [Fact(Skip = "Temp: Not needed for current goal")]
    public void Update_UpdateBlogAddPost()
    {
        var sandboxId = $"{GetType().FullName}.{MethodBase.GetCurrentMethod()!.Name}";
        GettingStarted.Update_UpdateBlogAddPost(ConnectionStrings.Instance.gettingStartedFileDB.Sandbox("Sandbox", sandboxId));
    }

    [Fact(Skip = "Temp: Not needed for current goal")]
    public void Delete_DeleteBlog()
    {
        var sandboxId = $"{GetType().FullName}.{MethodBase.GetCurrentMethod()!.Name}";
        GettingStarted.Delete_DeleteBlog(ConnectionStrings.Instance.gettingStartedFileDB.Sandbox("Sandbox", sandboxId));
    }


}
