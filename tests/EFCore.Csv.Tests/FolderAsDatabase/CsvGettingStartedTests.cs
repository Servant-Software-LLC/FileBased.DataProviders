using Data.Common.Extension;
using Data.Common.Utils.ConnectionString;
using EFCore.Common.Tests.Utils;
using EFCore.Csv.Tests.Utils;
using Microsoft.Extensions.Logging;
using System.Reflection;
using Xunit;
using GettingStarted = EFCore.Common.Tests.GettingStartedTests<EFCore.Csv.Tests.Models.BloggingContext>;

namespace EFCore.Csv.Tests.FolderAsDatabase;

/// <summary>
/// Unit tests created based off of the Getting Started page in EF Core.  REF: https://learn.microsoft.com/en-us/ef/core/get-started/overview/first-app?tabs=netcore-cli
/// </summary>
public class CsvGettingStartedTests
{
    [Fact]
    public void Create_AddBlog()
    {
        var sandboxId = $"{GetType().FullName}.{MethodBase.GetCurrentMethod()!.Name}";
        string connectionString = ConnectionStrings.Instance.gettingStartedFolderDB.Sandbox("Sandbox", sandboxId);
        GettingStarted.Create_AddBlog(connectionString);
    }

    [Fact]
    public void Create_AddBlog_StreamedDataSource()
    {
        string connectionString = FileConnectionString.CustomDataSource;
        var dataSourceProvider = CustomDataSourceFactory.VirtualGettingStartedFolderAsDB(ConnectionStrings.Instance.Extension);
        GettingStarted.Create_AddBlog(connectionString, dataSourceProvider);
    }

    [Fact(Skip = "Temp: Not needed for current goal")]
    public void Read_FirstBlog()
    {
        var sandboxId = $"{GetType().FullName}.{MethodBase.GetCurrentMethod()!.Name}";
        GettingStarted.Read_FirstBlog(ConnectionStrings.Instance.gettingStartedFolderDB.Sandbox("Sandbox", sandboxId));
    }

    [Fact(Skip = "Temp: Not needed for current goal")]
    public void Read_FirstBlog_StreamedDataSource()
    {
        string connectionString = FileConnectionString.CustomDataSource;
        var dataSourceProvider = CustomDataSourceFactory.VirtualGettingStartedFolderAsDB(ConnectionStrings.Instance.Extension);
        GettingStarted.Read_FirstBlog(connectionString, dataSourceProvider);
    }

    [Fact(Skip = "Temp: Not needed for current goal")]
    public void Update_UpdateBlogAddPost()
    {
        var sandboxId = $"{GetType().FullName}.{MethodBase.GetCurrentMethod()!.Name}";
        GettingStarted.Update_UpdateBlogAddPost(ConnectionStrings.Instance.gettingStartedFolderDB.Sandbox("Sandbox", sandboxId));
    }

    [Fact(Skip = "Temp: Not needed for current goal")]
    public void Delete_DeleteBlog()
    {
        var sandboxId = $"{GetType().FullName}.{MethodBase.GetCurrentMethod()!.Name}";
        GettingStarted.Delete_DeleteBlog(ConnectionStrings.Instance.gettingStartedFolderDB.Sandbox("Sandbox", sandboxId).AddLogging(LogLevel.Debug));
    }

}
