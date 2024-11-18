namespace Data.Common.Interfaces;

public interface IConnectionStringProperties
{
    string ConnectionString { get; }

    string DataSource { get; }
    bool? Formatted { get; }
}
