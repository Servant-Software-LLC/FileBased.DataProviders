using Data.Common.Extension;
using Data.Common.Utils.ConnectionString;

namespace Data.Tests.Common.Utils;

public abstract class ConnectionStringsBase : IConnectionStrings
{
    public abstract string Extension { get; }

    protected virtual string Folder => Path.Combine(FileConnectionStringTestsExtensions.SourcesFolder, "Folder");
    protected virtual string File => Path.Combine(FileConnectionStringTestsExtensions.SourcesFolder, $"database.{Extension}");
    public virtual string eComFileDataBase => Path.Combine(FileConnectionStringTestsExtensions.SourcesFolder, $"ecommerce.{Extension}");
    public virtual string eComFolderDataBase => Path.Combine(FileConnectionStringTestsExtensions.SourcesFolder, $"eCom");
    protected virtual string FileEmptyWithTables => Path.Combine(FileConnectionStringTestsExtensions.SourcesFolder, $"emptyDatabase.{Extension}");

    public virtual FileConnectionString FolderAsDB => new FileConnectionString() { DataSource = Folder };

    public virtual FileConnectionString FileAsDB => new FileConnectionString() { DataSource = File };
    public virtual FileConnectionString EmptyWithTablesFileAsDB => new FileConnectionString() { DataSource = FileEmptyWithTables };

    public virtual FileConnectionString eComFileDB => new FileConnectionString() { DataSource = eComFileDataBase };

    public virtual FileConnectionString eComFolderDB => new FileConnectionString() { DataSource = eComFolderDataBase };

    public static ConnectionStringsBase Instance => throw new NotImplementedException();
}