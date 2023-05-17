using Data.Common.Utils.ConnectionString;

namespace EFCore.Common.Tests.Utils;

public interface IConnectionStrings
{
    FileConnectionString FolderAsDB { get; }
    FileConnectionString FileAsDB { get; }

    static abstract ConnectionStringsBase Instance { get; }
}
