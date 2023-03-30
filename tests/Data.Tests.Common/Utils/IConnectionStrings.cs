using Data.Common.Utils.ConnectionString;

namespace Data.Tests.Common.Utils;

public interface IConnectionStrings
{
    FileConnectionString FolderAsDB { get; }
    FileConnectionString FileAsDB { get; }
    FileConnectionString EmptyWithTablesFileAsDB { get; }
    FileConnectionString eComFileDB { get; }
    FileConnectionString eComFolderDB { get; }

    static abstract ConnectionStringsBase Instance { get; }
}