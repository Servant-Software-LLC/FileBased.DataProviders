using Data.Common.DataSource;
using Microsoft.EntityFrameworkCore;

namespace EFCore.Common.Tests.Models;

// Model taken from Getting Started with EF Core - https://learn.microsoft.com/en-us/ef/core/get-started/overview/first-app?tabs=netcore-cli
public abstract class BloggingContextBase : DbContext
{
    public string ConnectionString { get; set; } 

    public IDataSourceProvider DataSourceProvider { get; set; }

    public DbSet<Blog> Blogs { get; set; }
    public DbSet<Post> Posts { get; set; }

    protected abstract override void OnConfiguring(DbContextOptionsBuilder options);
}
