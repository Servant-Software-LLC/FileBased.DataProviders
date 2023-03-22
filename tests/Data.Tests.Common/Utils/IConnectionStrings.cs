namespace Data.Tests.Common.Utils;

public interface IConnectionStrings
{
    ConnectionString FolderAsDBConnectionString { get; }
    ConnectionString FileAsDBConnectionString { get; }
    ConnectionString eComDBConnectionString { get; }

    static abstract ConnectionStringsBase Instance { get; }
}