namespace Data.Json.Interfaces;

public interface IConnectionStringProperties
{
    string ConnectionString { get; }

    public string? DataSource { get; }
    public bool Formatted { get; }
}
