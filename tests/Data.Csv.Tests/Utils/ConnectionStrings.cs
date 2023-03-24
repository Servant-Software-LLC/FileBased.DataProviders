using Data.Tests.Common.Utils;

namespace Data.Csv.Tests;

public class ConnectionStrings : ConnectionStringsBase
{
    public override string Extension => "cvs";

    public override ConnectionString FileAsDBConnectionString => throw new NotImplementedException();

    //TODO:  This will be provided in a 'Folder as Database' style.

    protected override string eComDataBase => Path.Combine(SourcesFolder, $"eCom");
    public override ConnectionString eComDBConnectionString => eComDataBase;

    public new static ConnectionStrings Instance => new ConnectionStrings();
}