using Data.Common.DataSource;
using Microsoft.EntityFrameworkCore;

namespace EFCore.Common.Tests.Models;

public abstract class TypeMappingContextBase : DbContext
{
    public string ConnectionString { get; set; }

    public IDataSourceProvider DataSourceProvider { get; set; }

    public DbSet<TypeMappingEntity> TypeMappingEntities { get; set; }

    protected abstract override void OnConfiguring(DbContextOptionsBuilder options);
}
