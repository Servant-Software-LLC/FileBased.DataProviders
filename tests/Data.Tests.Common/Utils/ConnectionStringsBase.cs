namespace Data.Tests.Common.Utils;

public abstract class ConnectionStringsBase : IConnectionStrings
{
    public const string SourcesFolder = "Sources";
    public const string SourcesPristineCopy = "Sources.Pristine";

    public abstract string Extension { get; }

    protected virtual string Folder => Path.Combine(SourcesFolder, "Folder");
    protected virtual string File => Path.Combine(SourcesFolder, $"database.{Extension}");
    protected virtual string eComFileDataBase => Path.Combine(SourcesFolder, $"ecommerce.{Extension}");
    protected virtual string eComFolderDataBase => Path.Combine(SourcesFolder, $"eCom");

    public virtual ConnectionString FolderAsDBConnectionString => Folder;

    public virtual ConnectionString FileAsDBConnectionString => File;

    public virtual ConnectionString eComFileDBConnectionString => eComFileDataBase;

    public static ConnectionStringsBase Instance => throw new NotImplementedException();

    public virtual ConnectionString eComFolderDBConnectionString
        => eComFolderDataBase;
}