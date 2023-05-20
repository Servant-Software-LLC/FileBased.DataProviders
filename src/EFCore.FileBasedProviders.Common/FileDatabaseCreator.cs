using Data.Common.FileException;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Storage;

namespace EFCore.Common;

public abstract class FileDatabaseCreator : RelationalDatabaseCreator
{
    protected readonly IRawSqlCommandBuilder rawSqlCommandBuilder;

    public FileDatabaseCreator(
        RelationalDatabaseCreatorDependencies dependencies,
        IRawSqlCommandBuilder rawSqlCommandBuilder)
        : base(dependencies)
    {
        this.rawSqlCommandBuilder = rawSqlCommandBuilder ?? throw new ArgumentNullException(nameof(rawSqlCommandBuilder));
    }

    public override bool Exists()
    {
        var dbConnection = Dependencies.Connection.DbConnection;

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
        var count = (long)rawSqlCommandBuilder
            .Build("SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE'")
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
