using Data.Common.DataSource;
using Data.Common.Utils.ConnectionString;

namespace Data.Tests.Common.Utils;

public abstract class ConnectionStringsBase : IConnectionStrings
{
    public abstract string Extension { get; }

    public DatabaseFullPaths Database => new DatabaseFullPaths(Extension);

    public virtual FileConnectionString Admin => new FileConnectionString() { DataSource = $":{nameof(DataSourceType.Admin)}:" };
    public virtual FileConnectionString FolderAsDB => new FileConnectionString() { DataSource = Database.Folder };
    
    public virtual FileConnectionString WithDateTimeFolder => new FileConnectionString() { DataSource = Database.WithDateTimeFolder };

    public virtual FileConnectionString FileAsDB => new FileConnectionString() { DataSource = Database.File };
    public virtual FileConnectionString WithDateTimeFileAsDB => new FileConnectionString() { DataSource = Database.WithDateTime };
    public virtual FileConnectionString EmptyWithTablesFolderAsDB => new FileConnectionString() { DataSource = Database.FolderEmptyWithTables };
    public virtual FileConnectionString EmptyWithTablesFileAsDB => new FileConnectionString() { DataSource = Database.FileEmptyWithTables };

    public virtual FileConnectionString eComFileDB => new FileConnectionString() { DataSource = Database.eComFileDataBase };

    public virtual FileConnectionString eComFolderDB => new FileConnectionString() { DataSource = Database.eComFolderDataBase };

    public virtual FileConnectionString gettingStartedFileDB => new FileConnectionString() { DataSource = Database.gettingStartedFileDataBase };

    public virtual FileConnectionString gettingStartedFolderDB => new FileConnectionString() { DataSource = Database.gettingStartedFolderDataBase };

    public virtual FileConnectionString bogusFileDB => new FileConnectionString() { DataSource = Database.bogusFileDataBase };

    public virtual FileConnectionString bogusFolderDB => new FileConnectionString() { DataSource = Database.bogusFolderDataBase };

    public virtual FileConnectionString withTrailingCommaDB => new FileConnectionString() { DataSource = Database.withTrailingCommaFolderDataBase };

    public static ConnectionStringsBase Instance => throw new NotImplementedException();

}