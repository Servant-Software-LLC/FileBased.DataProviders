using Data.Common.FileException;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Storage;

namespace EFCore.Json.Storage.Internal;


public class JsonDatabaseCreator : RelationalDatabaseCreator
{
    private readonly IRawSqlCommandBuilder rawSqlCommandBuilder;
    private readonly IJsonRelationalConnection connection;

    public JsonDatabaseCreator(
        RelationalDatabaseCreatorDependencies dependencies,
        IJsonRelationalConnection connection,
        IRawSqlCommandBuilder rawSqlCommandBuilder)
        : base(dependencies)
    {
        this.rawSqlCommandBuilder = rawSqlCommandBuilder;
        this.connection = connection;
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

    public override bool Exists()
    {
        var dbConnection = connection.DbConnection;

        try
        {
            dbConnection.Open();
        }
        catch (InvalidConnectionStringException)
        {
            return false;
        }

        return true;
    }

    public override bool HasTables()
    {
#error Waiting on https://github.com/Servant-Software-LLC/ADO.NET.FileBased.DataProviders/issues/26 to be complete

        var count = (long)rawSqlCommandBuilder
            .Build("SELECT COUNT(*) FROM \"sqlite_master\" WHERE \"type\" = 'table' AND \"rootpage\" IS NOT NULL;")
            .ExecuteScalar(
                new RelationalCommandParameterObject(
                    Dependencies.Connection,
                    null,
                    null,
                    null,
                    Dependencies.CommandLogger, CommandSource.Migrations))!;

        return count != 0;
    }
}
