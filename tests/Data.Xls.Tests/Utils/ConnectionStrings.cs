using Data.Common.Utils.ConnectionString;
using Data.Tests.Common.Utils;

namespace Data.Xls.Tests;

public class ConnectionStrings : ConnectionStringsBase
{
    public override string Extension => "xlsx";

    public FileConnectionString WithSpaceCellAsDB => new FileConnectionString { DataSource = Database.WithSpaceCell };

    public new static ConnectionStrings Instance => new ConnectionStrings();
}