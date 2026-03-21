using Data.Common.DataSource;
using EFCore.Common.Tests.Models;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace EFCore.Common.Tests;

/// <summary>
/// Tests verifying that LINQ query translators correctly translate string methods,
/// member access, and aggregate functions to SQL equivalents.
/// Uses pre-seeded data: Blogs with Url "http://blogs.msdn.com/adonet" (Id=1) and
/// "https://www.billboard.com/" (Id=2), one Post "Hello World" (BlogId=1).
/// </summary>
public static class LinqTranslatorTests<TBloggingContext> where TBloggingContext : BloggingContextBase, new()
{
    private static TBloggingContext CreateContext(string connectionString, IDataSourceProvider dataSourceProvider = null)
    {
        var db = new TBloggingContext();
        db.ConnectionString = connectionString;
        if (dataSourceProvider != null)
            db.DataSourceProvider = dataSourceProvider;
        return db;
    }

    public static void StringContains_FiltersCorrectly(string connectionString, IDataSourceProvider dataSourceProvider = null)
    {
        using var db = CreateContext(connectionString, dataSourceProvider);

        var results = db.Blogs.Where(b => b.Url.Contains("billboard")).ToList();
        Assert.Single(results);
        Assert.Contains("billboard", results[0].Url);
    }

    public static void StringStartsWith_FiltersCorrectly(string connectionString, IDataSourceProvider dataSourceProvider = null)
    {
        using var db = CreateContext(connectionString, dataSourceProvider);

        var results = db.Blogs.Where(b => b.Url.StartsWith("https")).ToList();
        Assert.Single(results);
        Assert.StartsWith("https", results[0].Url);
    }

    public static void StringEndsWith_FiltersCorrectly(string connectionString, IDataSourceProvider dataSourceProvider = null)
    {
        using var db = CreateContext(connectionString, dataSourceProvider);

        var results = db.Blogs.Where(b => b.Url.EndsWith("adonet")).ToList();
        Assert.Single(results);
        Assert.EndsWith("adonet", results[0].Url);
    }

    public static void StringToUpper_TranslatesCorrectly(string connectionString, IDataSourceProvider dataSourceProvider = null)
    {
        using var db = CreateContext(connectionString, dataSourceProvider);

        var results = db.Blogs
            .Select(b => b.Url.ToUpper())
            .ToList();

        Assert.Equal(2, results.Count);
        Assert.All(results, url => Assert.Equal(url, url.ToUpper()));
    }

    public static void StringToLower_TranslatesCorrectly(string connectionString, IDataSourceProvider dataSourceProvider = null)
    {
        using var db = CreateContext(connectionString, dataSourceProvider);

        var results = db.Blogs
            .Select(b => b.Url.ToLower())
            .ToList();

        Assert.Equal(2, results.Count);
        Assert.All(results, url => Assert.Equal(url, url.ToLower()));
    }

    public static void EFLike_FiltersCorrectly(string connectionString, IDataSourceProvider dataSourceProvider = null)
    {
        using var db = CreateContext(connectionString, dataSourceProvider);

        var results = db.Blogs
            .Where(b => EF.Functions.Like(b.Url, "%billboard%"))
            .ToList();

        Assert.Single(results);
        Assert.Contains("billboard", results[0].Url);
    }

    public static void StringLength_FiltersCorrectly(string connectionString, IDataSourceProvider dataSourceProvider = null)
    {
        using var db = CreateContext(connectionString, dataSourceProvider);

        var results = db.Blogs
            .Where(b => b.Url.Length > 30)
            .ToList();

        // "http://blogs.msdn.com/adonet" = 29 chars, "https://www.billboard.com/" = 25 chars
        Assert.Empty(results);

        var results2 = db.Blogs
            .Where(b => b.Url.Length > 20)
            .ToList();
        Assert.Equal(2, results2.Count);
    }

    public static void Count_Aggregate_Works(string connectionString, IDataSourceProvider dataSourceProvider = null)
    {
        using var db = CreateContext(connectionString, dataSourceProvider);

        var count = db.Blogs.Count();
        Assert.Equal(2, count);
    }

    public static void CountWithPredicate_Aggregate_Works(string connectionString, IDataSourceProvider dataSourceProvider = null)
    {
        using var db = CreateContext(connectionString, dataSourceProvider);

        var count = db.Blogs.Count(b => b.Url.Contains("billboard"));
        Assert.Equal(1, count);
    }

    public static void Max_Aggregate_Works(string connectionString, IDataSourceProvider dataSourceProvider = null)
    {
        using var db = CreateContext(connectionString, dataSourceProvider);

        var maxId = db.Blogs.Max(b => b.BlogId);
        Assert.Equal(2, maxId);
    }

    public static void Min_Aggregate_Works(string connectionString, IDataSourceProvider dataSourceProvider = null)
    {
        using var db = CreateContext(connectionString, dataSourceProvider);

        var minId = db.Blogs.Min(b => b.BlogId);
        Assert.Equal(1, minId);
    }
}
