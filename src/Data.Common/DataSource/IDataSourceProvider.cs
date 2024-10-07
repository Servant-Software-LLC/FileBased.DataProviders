namespace Data.Common.DataSource;

public interface IDataSourceProvider
{
    DataSourceType DataSourceType { get; }

    bool StorageExists(string tableName);

    TextReader GetTextReader(string tableName);
    
    TextWriter GetTextWriter(string tableName);

    string StorageIdentifier(string tableName);
}
