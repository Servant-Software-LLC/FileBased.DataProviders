using Data.Common.Extension;
using Data.Common.Utils.ConnectionString;
using EFCore.Common.Tests.Utils;
using EFCore.Json.Tests.Utils;
using System.Reflection;
using Xunit;
using GettingStarted = EFCore.Common.Tests.GettingStartedTests<EFCore.Json.Tests.Models.BloggingContext>;

namespace EFCore.Json.Tests.FolderAsDatabase;

/// <summary>
/// Unit tests created based off of the Getting Started page in EF Core.  REF: https://learn.microsoft.com/en-us/ef/core/get-started/overview/first-app?tabs=netcore-cli
/// </summary>
public class JsonGettingStartedTests
{
    [Fact]
    public void Create_AddBlog()
    {
        var sandboxId = $"{GetType().FullName}.{MethodBase.GetCurrentMethod()!.Name}";
        GettingStarted.Create_AddBlog(ConnectionStrings.Instance.gettingStartedFolderDB.Sandbox("Sandbox", sandboxId));
    }

    [Fact]
    public void Create_AddBlog_StreamedDataSource()
    {
        string connectionString = FileConnectionString.CustomDataSource;
        var dataSourceProvider = CustomDataSourceFactory.VirtualGettingStartedFolderAsDB(ConnectionStrings.Instance.Extension);
        GettingStarted.Create_AddBlog(connectionString, dataSourceProvider);
    }

    [Fact]
    public void Read_FirstBlog()
    {
        var sandboxId = $"{GetType().FullName}.{MethodBase.GetCurrentMethod()!.Name}";
        GettingStarted.Read_FirstBlog(ConnectionStrings.Instance.gettingStartedWithDataFolderDB.Sandbox("Sandbox", sandboxId));
    }

    [Fact]
    public void Read_FirstBlog_AfterAddingData()
    {
        var sandboxId = $"{GetType().FullName}.{MethodBase.GetCurrentMethod()!.Name}";
        GettingStarted.Read_FirstBlog_AfterAddingData(ConnectionStrings.Instance.gettingStartedFolderDB.Sandbox("Sandbox", sandboxId));
    }

    [Fact]
    public void Read_FirstBlog_StreamedDataSource()
    {
        string connectionString = FileConnectionString.CustomDataSource;
        var dataSourceProvider = CustomDataSourceFactory.VirtualGettingStartedWithDataFolderAsDB(ConnectionStrings.Instance.Extension);
        GettingStarted.Read_FirstBlog(connectionString, dataSourceProvider);
    }

    [Fact]
    public void Read_FirstBlog_AfterAddingData_StreamedDataSource()
    {
        string connectionString = FileConnectionString.CustomDataSource;
        var dataSourceProvider = CustomDataSourceFactory.VirtualGettingStartedFolderAsDB(ConnectionStrings.Instance.Extension);
        GettingStarted.Read_FirstBlog_AfterAddingData(connectionString, dataSourceProvider);
    }

    [Fact]
    public void Update_UpdateBlogAddPost()
    {
        var sandboxId = $"{GetType().FullName}.{MethodBase.GetCurrentMethod()!.Name}";
        GettingStarted.Update_UpdateBlogAddPost(ConnectionStrings.Instance.gettingStartedFolderDB.Sandbox("Sandbox", sandboxId));
    }

    [Fact]
    public void Delete_DeleteBlog()
    {
        var sandboxId = $"{GetType().FullName}.{MethodBase.GetCurrentMethod()!.Name}";
        GettingStarted.Delete_DeleteBlog(ConnectionStrings.Instance.gettingStartedFolderDB.Sandbox("Sandbox", sandboxId));
    }


}
