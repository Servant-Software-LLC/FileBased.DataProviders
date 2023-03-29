using Data.Common.Utils.ConnectionString;

namespace Data.Tests.Common.Utils;

public interface IConnectionStrings
{
    FileConnectionString FolderAsDB { get; }
    FileConnectionString FileAsDB { get; }
    FileConnectionString FileAsDBEmptyWithTables { get; }
    FileConnectionString FolderAsDB_eCom { get; }
    FileConnectionString FileAsDB_eCom { get; }

    static abstract ConnectionStringsBase Instance { get; }
}