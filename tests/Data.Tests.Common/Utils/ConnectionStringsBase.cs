namespace Data.Tests.Common.Utils;

public abstract class ConnectionStringsBase : IConnectionStrings
{
    public abstract string Extension { get; }

    protected virtual string Folder => Path.Combine("Sources", "Folder");
    protected virtual string File => Path.Combine("Sources", $"database.{Extension}");
    protected virtual string eComDataBase => Path.Combine("Sources", $"ecommerce.{Extension}");

    public virtual ConnectionString FolderAsDBConnectionString => Folder;

    public virtual ConnectionString FileAsDBConnectionString => File;

    public virtual ConnectionString eComDBConnectionString => eComDataBase;

    public static ConnectionStringsBase Instance => throw new NotImplementedException();
}
