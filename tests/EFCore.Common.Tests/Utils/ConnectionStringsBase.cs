using Data.Common.Extension;
using Data.Common.Utils.ConnectionString;

namespace EFCore.Common.Tests.Utils;

public abstract class ConnectionStringsBase : IConnectionStrings
{
    public abstract string Extension { get; }

    protected virtual string gettingStartedFolderDataBase => Path.Combine(FileConnectionStringTestsExtensions.SourcesFolder, "GettingStarted");
    protected virtual string gettingStartedFileDataBase => Path.Combine(FileConnectionStringTestsExtensions.SourcesFolder, $"GettingStarted.{Extension}");
    public virtual string gettingStartedWithDataFolderDataBase => Path.Combine(FileConnectionStringTestsExtensions.SourcesFolder, $"GettingStartedWithData");
    public virtual string gettingStartedWithDataFileDataBase => Path.Combine(FileConnectionStringTestsExtensions.SourcesFolder, $"GettingStartedWithData.{Extension}");


    public virtual FileConnectionString gettingStartedFolderDB => new FileConnectionString() { DataSource = gettingStartedFolderDataBase };

    public virtual FileConnectionString gettingStartedFileDB => new FileConnectionString() { DataSource = gettingStartedFileDataBase };
    public virtual FileConnectionString gettingStartedWithDataFolderDB => new FileConnectionString() { DataSource = gettingStartedWithDataFolderDataBase };
    public virtual FileConnectionString gettingStartedWithDataFileDB => new FileConnectionString() { DataSource = gettingStartedWithDataFileDataBase };


    public static ConnectionStringsBase Instance => throw new NotImplementedException();
}
