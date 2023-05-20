using EFCore.Common.Storage.Internal;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Storage;

namespace EFCore.Json.Storage.Internal;


public class JsonDatabaseCreator : FileDatabaseCreator
{
    //TODO: Remove if not used after implementation is fully complete.
    private readonly IJsonRelationalConnection connection;

    public JsonDatabaseCreator(
        RelationalDatabaseCreatorDependencies dependencies,
        IJsonRelationalConnection connection,
        IRawSqlCommandBuilder rawSqlCommandBuilder)
        : base(dependencies, rawSqlCommandBuilder)
    {
        this.connection = connection ?? throw new ArgumentNullException(nameof(connection));
    }

    public override void Create()
    {
#error Waiting on https://github.com/Servant-Software-LLC/ADO.NET.FileBased.DataProviders/issues/23 to be complete

        Dependencies.Connection.Open();

        rawSqlCommandBuilder.Build("PRAGMA journal_mode = 'wal';")
            .ExecuteNonQuery(
                new RelationalCommandParameterObject(
                    Dependencies.Connection,
                    null,
                    null,
                    null,
                    Dependencies.CommandLogger, CommandSource.Migrations));

        Dependencies.Connection.Close();
    }

    public override void Delete()
    {
#error Waiting on https://github.com/Servant-Software-LLC/ADO.NET.FileBased.DataProviders/issues/24 to be complete

        string? path = null;

        Dependencies.Connection.Open();
        try
        {
            path = Dependencies.Connection.DbConnection.DataSource;
        }
        catch
        {
            // any exceptions here can be ignored
        }
        finally
        {
            Dependencies.Connection.Close();
        }
    }

}
