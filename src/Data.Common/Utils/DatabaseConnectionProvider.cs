using SqlBuildingBlocks.Interfaces;

namespace Data.Common.Utils;

internal class DatabaseConnectionProvider : IDatabaseConnectionProvider
{
    private readonly IFileConnection fileConnection;
    public DatabaseConnectionProvider(IFileConnection fileConnection) => this.fileConnection = fileConnection;

    /// <summary>
    /// Default database name.  Since for a file connection, the database can be a path (absolute or relative) to the 
    /// folder or file, we just want a simple name for the database.  Use the simple file/folder name
    /// </summary>
    public string DefaultDatabase => Path.GetFileNameWithoutExtension(fileConnection.Database);

    public bool CaseInsensitive => fileConnection.CaseInsensitive;
}
