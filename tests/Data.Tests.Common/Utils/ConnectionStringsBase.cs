namespace Data.Tests.Common.Utils;

public abstract class ConnectionStringsBase : IConnectionStrings
{
    public const string SourcesFolder = "Sources";
    public const string SourcesPristineCopy = "Sources.Pristine";

    public abstract string Extension { get; }

    protected virtual string Folder => Path.Combine(SourcesFolder, "Folder");
    protected virtual string File => Path.Combine(SourcesFolder, $"database.{Extension}");
    protected virtual string eComDataBase => Path.Combine(SourcesFolder, $"ecommerce.{Extension}");

    public virtual ConnectionString FolderAsDBConnectionString => Folder;

    public virtual ConnectionString FileAsDBConnectionString => File;

    public virtual ConnectionString eComDBConnectionString => eComDataBase;

    public static ConnectionStringsBase Instance => throw new NotImplementedException();
}
