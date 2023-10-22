using Data.Common.Utils.ConnectionString;

namespace EFCore.Common.Tests.Utils;

public interface IConnectionStrings
{
    FileConnectionString gettingStartedFolderDB { get; }
    FileConnectionString gettingStartedFileDB { get; }

    static abstract ConnectionStringsBase Instance { get; }
}
