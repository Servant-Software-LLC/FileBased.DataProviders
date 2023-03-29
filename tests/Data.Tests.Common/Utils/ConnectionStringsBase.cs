using Data.Common.Extension;
using Data.Common.Utils.ConnectionString;

namespace Data.Tests.Common.Utils;

public abstract class ConnectionStringsBase : IConnectionStrings
{
    public abstract string Extension { get; }

    protected virtual string Folder => Path.Combine(FileConnectionStringTestsExtensions.SourcesFolder, "Folder");
    protected virtual string File => Path.Combine(FileConnectionStringTestsExtensions.SourcesFolder, $"database.{Extension}");
    protected virtual string FileEmptyWithTables => Path.Combine(FileConnectionStringTestsExtensions.SourcesFolder, $"emptyDatabase.{Extension}");
    protected virtual string eComDataBase => Path.Combine(FileConnectionStringTestsExtensions.SourcesFolder, $"ecommerce.{Extension}");

    public virtual FileConnectionString FolderAsDBConnectionString => new FileConnectionString() { DataSource = Folder };

    public virtual FileConnectionString FileAsDBConnectionString => new FileConnectionString() { DataSource = File };
    public virtual FileConnectionString FileAsDBEmptyWithTablesConnectionString => new FileConnectionString() { DataSource = FileEmptyWithTables };

    public virtual FileConnectionString eComDBConnectionString => new FileConnectionString() { DataSource = eComDataBase };

    public static ConnectionStringsBase Instance => throw new NotImplementedException();
}
