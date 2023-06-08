using Data.Common.FileException;
using Data.Common.Utils.ConnectionString;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data.FileClient;

namespace EFCore.Common.Storage.Internal;

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
        var count = (int)rawSqlCommandBuilder
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

    protected void Create<TFileConnection, TFileParameter>(Func<string, TFileConnection> createConnection)
        where TFileConnection : FileConnection<TFileParameter>
        where TFileParameter : FileParameter<TFileParameter>, new() =>
            AdminExecuteNonQuery<TFileConnection, TFileParameter>(
                createConnection, databaseName => $"CREATE DATABASE '{databaseName}'"
            );            

    protected void Delete<TFileConnection, TFileParameter>(Func<string, TFileConnection> createConnection)
        where TFileConnection : FileConnection<TFileParameter>
        where TFileParameter : FileParameter<TFileParameter>, new() =>
            AdminExecuteNonQuery<TFileConnection, TFileParameter>(
                createConnection, databaseName => $"DROP DATABASE '{databaseName}'"
            );            

    private void AdminExecuteNonQuery<TFileConnection, TFileParameter>(
            Func<string, TFileConnection> createConnection,
            Func<string, string> createSqlStatement)
        where TFileConnection : FileConnection<TFileParameter>
        where TFileParameter : FileParameter<TFileParameter>, new()
    {
        var adminConnectionString = new FileConnectionString() { DataSource = ":Admin:" }.ConnectionString;
        using (var adminConnection = createConnection(adminConnectionString))
        {
            adminConnection.Open();

            var command = adminConnection.CreateCommand();

            var databaseName = new FileConnectionString(Dependencies.Connection.ConnectionString).DataSource;
            command.CommandText = createSqlStatement(databaseName);

            var executeResult = command.ExecuteNonQuery()!;

            adminConnection.Close();
        }
    }
}
