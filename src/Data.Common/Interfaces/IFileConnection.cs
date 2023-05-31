namespace Data.Common.Interfaces;

public interface IFileConnection : IDbConnection
{
    string FileExtension { get; }
    bool? Formatted { get; }
    PathType PathType { get; }
    bool FolderAsDatabase { get; }
    bool AdminMode { get; }
    FileReader FileReader { get; }
}
