using Data.Common.Extension;
using Data.Common.Utils.ConnectionString;

namespace Data.Tests.Common.Utils;

public abstract class ConnectionStringsBase : IConnectionStrings
{
    public abstract string Extension { get; }

    protected virtual string Folder => Path.Combine(FileConnectionStringTestsExtensions.SourcesFolder, "Folder");
    protected virtual string File => Path.Combine(FileConnectionStringTestsExtensions.SourcesFolder, $"database.{Extension}");
    protected virtual string FileEmptyWithTables => Path.Combine(FileConnectionStringTestsExtensions.SourcesFolder, $"emptyDatabase.{Extension}");
    protected virtual string eComFile => Path.Combine(FileConnectionStringTestsExtensions.SourcesFolder, $"ecommerce.{Extension}");
    protected virtual string eComFolder => Path.Combine(FileConnectionStringTestsExtensions.SourcesFolder, $"eCom");

    public virtual FileConnectionString FolderAsDB => new FileConnectionString() { DataSource = Folder };

    public virtual FileConnectionString FileAsDB => new FileConnectionString() { DataSource = File };
    public virtual FileConnectionString FileAsDBEmptyWithTables => new FileConnectionString() { DataSource = FileEmptyWithTables };

    public virtual FileConnectionString FileAsDB_eCom => new FileConnectionString() { DataSource = eComFile };
    public virtual FileConnectionString FolderAsDB_eCom => new FileConnectionString() { DataSource = eComFolder };

    public static ConnectionStringsBase Instance => throw new NotImplementedException();
}
