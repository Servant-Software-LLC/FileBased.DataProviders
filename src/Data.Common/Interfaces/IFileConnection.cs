using Data.Common.Utils;

namespace Data.Common.Interfaces;

public interface IFileConnection : IDbConnection
{
    string FileExtension { get; }
    bool? Formatted { get; }
    PathType PathType { get; }
    bool FolderAsDatabase { get; }
    bool AdminMode { get; }
    FileReader FileReader { get; }
    LoggerServices LoggerServices { get; }
    bool CaseInsensitive { get; }
}
