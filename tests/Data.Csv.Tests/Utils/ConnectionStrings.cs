using Data.Tests.Common.Utils;

namespace Data.Csv.Tests;

public class ConnectionStrings : ConnectionStringsBase
{
    public override string Extension => "cvs";

    public override ConnectionString FileAsDBConnectionString => throw new NotImplementedException();

    //TODO:  This will be provided in a 'Folder as Database' style.
    public override ConnectionString eComDBConnectionString => throw new NotImplementedException();

    public new static ConnectionStrings Instance => new ConnectionStrings();
}