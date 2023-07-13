using Data.Common.Enum;
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
    protected virtual string FolderEmptyWithTables => Path.Combine(FileConnectionStringTestsExtensions.SourcesFolder, "EmptyDatabase");
    protected virtual string FileEmptyWithTables => Path.Combine(FileConnectionStringTestsExtensions.SourcesFolder, $"emptyDatabase.{Extension}");
    public virtual string gettingStartedFileDataBase => Path.Combine(FileConnectionStringTestsExtensions.SourcesFolder, $"GettingStarted.{Extension}");
    public virtual string gettingStartedFolderDataBase => Path.Combine(FileConnectionStringTestsExtensions.SourcesFolder, $"GettingStarted");
    public virtual string gettingStartedWithDataFileDataBase => Path.Combine(FileConnectionStringTestsExtensions.SourcesFolder, $"GettingStartedWithData.{Extension}");
    public virtual string gettingStartedWithDataFolderDataBase => Path.Combine(FileConnectionStringTestsExtensions.SourcesFolder, $"GettingStartedWithData");

    public virtual FileConnectionString Admin => new FileConnectionString() { DataSource = $":{nameof(PathType.Admin)}:" };
    public virtual FileConnectionString FolderAsDB => new FileConnectionString() { DataSource = Folder };

    public virtual FileConnectionString FileAsDB => new FileConnectionString() { DataSource = File };

    public virtual FileConnectionString EmptyWithTablesFolderAsDB => new FileConnectionString() { DataSource = FolderEmptyWithTables };
    public virtual FileConnectionString EmptyWithTablesFileAsDB => new FileConnectionString() { DataSource = FileEmptyWithTables };

    public virtual FileConnectionString eComFileDB => new FileConnectionString() { DataSource = eComFileDataBase };

    public virtual FileConnectionString eComFolderDB => new FileConnectionString() { DataSource = eComFolderDataBase };

    public virtual FileConnectionString gettingStartedFileDB => new FileConnectionString() { DataSource = gettingStartedFileDataBase };

    public virtual FileConnectionString gettingStartedFolderDB => new FileConnectionString() { DataSource = gettingStartedFolderDataBase };
    public virtual FileConnectionString gettingStartedWithDataFileDB => new FileConnectionString() { DataSource = gettingStartedWithDataFileDataBase };

    public virtual FileConnectionString gettingStartedWithDataFolderDB => new FileConnectionString() { DataSource = gettingStartedWithDataFolderDataBase };

    public static ConnectionStringsBase Instance => throw new NotImplementedException();
}