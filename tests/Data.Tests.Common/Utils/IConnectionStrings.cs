using Data.Common.Utils.ConnectionString;

namespace Data.Tests.Common.Utils;

public interface IConnectionStrings
{
    FileConnectionString FolderAsDBConnectionString { get; }
    FileConnectionString FileAsDBConnectionString { get; }
    FileConnectionString eComDBConnectionString { get; }

    static abstract ConnectionStringsBase Instance { get; }
}