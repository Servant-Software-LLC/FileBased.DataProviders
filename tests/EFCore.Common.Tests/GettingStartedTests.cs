using EFCore.Common.Tests.Models;
using Xunit;

namespace EFCore.Common.Tests;

public static class GettingStartedTests<TBloggingContext> where TBloggingContext : BloggingContextBase, new()
{
    public static void Create_AddBlog(string connectionString) 
    {
        using var db = new TBloggingContext();
        db.ConnectionString = connectionString;

        db.Add(new Blog { Url = "http://blogs.msdn.com/adonet" });
        db.SaveChanges();
    }

    public static void Read_FirstBlog(string connectionString)
    {
        using var db = new TBloggingContext();
        db.ConnectionString = connectionString;
        db.Database.EnsureCreated();

        var firstBlog = new Blog { Url = "http://blogs.msdn.com/adonet" };
        db.Add(firstBlog);
        db.Add(new Blog { Url = "https://www.billboard.com/" });
        db.Add(new Blog { Url = "https://www.wired.com/" });
        db.SaveChanges();

        var blog = db.Blogs
            .OrderBy(b => b.BlogId)
            .First();

        Assert.Equal(firstBlog.Url, blog.Url);
    }

    public static void Update_UpdateBlogAddPost(string connectionString)
    {
        using var db = new TBloggingContext();
        db.ConnectionString = connectionString;
        db.Database.EnsureCreated();

        db.Add(new Blog { Url = "http://blogs.msdn.com/adonet" });
        db.SaveChanges();

        var blog = db.Blogs
            .OrderBy(b => b.BlogId)
            .First();
        var updatedUrl = "https://devblogs.microsoft.com/dotnet";
        blog.Url = updatedUrl;
        blog.Posts.Add(
            new Post { Title = "Hello World", Content = "I wrote an app using EF Core!" });
        db.SaveChanges();

        blog = db.Blogs
            .OrderBy(b => b.BlogId)
            .First();

        Assert.Equal(updatedUrl, blog.Url);
        Assert.True(blog.Posts.Any());
    }

    public static void Delete_DeleteBlog(string connectionString)
    {
        using var db = new TBloggingContext();
        db.ConnectionString = connectionString;
        db.Database.EnsureCreated();

        var firstBlog = new Blog { Url = "http://blogs.msdn.com/adonet" };
        db.Add(firstBlog);
        var secondBlog = "https://www.billboard.com/";
        db.Add(new Blog { Url = secondBlog });
        var thirdBlog = "https://www.wired.com/";
        db.Add(new Blog { Url = thirdBlog });
        db.SaveChanges();

        var blog = db.Blogs
            .OrderBy(b => b.BlogId)
            .First();

        Assert.Equal(firstBlog, blog);
        db.Remove(blog);
        db.SaveChanges();

        var blogs = db.Blogs.OrderBy(b => b.BlogId).ToList();
        Assert.Equal(2, db.Blogs.Count());
        Assert.Equal(secondBlog, blogs[0].Url);
        Assert.Equal(thirdBlog, blogs[1].Url);
    }

}
