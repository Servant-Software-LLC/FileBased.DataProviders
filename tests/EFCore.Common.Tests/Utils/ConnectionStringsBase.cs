using Data.Common.Extension;
using Data.Common.Utils.ConnectionString;

namespace EFCore.Common.Tests.Utils;

public abstract class ConnectionStringsBase : IConnectionStrings
{
    public abstract string Extension { get; }

    protected virtual string Folder => Path.Combine(FileConnectionStringTestsExtensions.SourcesFolder, "GettingStarted");
    protected virtual string File => Path.Combine(FileConnectionStringTestsExtensions.SourcesFolder, $"GettingStarted.{Extension}");


    public virtual FileConnectionString FolderAsDB => new FileConnectionString() { DataSource = Folder };

    public virtual FileConnectionString FileAsDB => new FileConnectionString() { DataSource = File };

    public static ConnectionStringsBase Instance => throw new NotImplementedException();
}
