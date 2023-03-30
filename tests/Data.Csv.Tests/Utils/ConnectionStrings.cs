using Data.Tests.Common.Utils;

namespace Data.Csv.Tests;

public class ConnectionStrings : ConnectionStringsBase
{
    public override string Extension => "cvs";

    public override ConnectionString FileAsDBConnectionString => throw new NotImplementedException();

    protected override string eComFolderDataBase
        => Path.Combine(SourcesFolder, $"eCom");

    public override ConnectionString eComFileDBConnectionString
        => throw new InvalidOperationException("File as database is not supported in csv provider");

    public override ConnectionString eComFolderDBConnectionString
        => eComFolderDataBase;

    public new static ConnectionStrings Instance => new ConnectionStrings();
}