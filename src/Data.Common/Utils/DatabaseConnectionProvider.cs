using SqlBuildingBlocks.Interfaces;

namespace Data.Common.Utils;

internal class DatabaseConnectionProvider : IDatabaseConnectionProvider
{
    private readonly IFileConnection fileConnection;
    public DatabaseConnectionProvider(IFileConnection fileConnection) => this.fileConnection = fileConnection;

    public string DefaultDatabase => fileConnection.Database;

    public bool CaseInsensitive => fileConnection.CaseInsensitive;
}
