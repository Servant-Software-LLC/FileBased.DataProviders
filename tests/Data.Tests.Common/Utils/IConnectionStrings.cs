namespace Data.Tests.Common.Utils;

public interface IConnectionStrings
{
    ConnectionString FolderAsDBConnectionString { get; }
    ConnectionString FileAsDBConnectionString { get; }
    ConnectionString eComFileDBConnectionString { get; }
    ConnectionString eComFolderDBConnectionString { get; }

    static abstract ConnectionStringsBase Instance { get; }
}