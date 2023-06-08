using EFCore.Common.Storage.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data.JsonClient;

namespace EFCore.Json.Storage.Internal;


public class JsonDatabaseCreator : FileDatabaseCreator
{
    //TODO: Remove if not used after implementation is fully complete.  It may be that we don't even need all these derived classes and can 
    //      make FileDatabaseCreator non-abstract and register it.
    private readonly IJsonRelationalConnection connection;

    public JsonDatabaseCreator(
        RelationalDatabaseCreatorDependencies dependencies,
        IJsonRelationalConnection connection,
        IRawSqlCommandBuilder rawSqlCommandBuilder)
        : base(dependencies, rawSqlCommandBuilder)
    {
        this.connection = connection ?? throw new ArgumentNullException(nameof(connection));
    }

    public override void Create() =>
        Create<JsonConnection, JsonParameter>(connString => new JsonConnection(connString));

    public override void Delete() =>
        Delete<JsonConnection, JsonParameter>(connString => new JsonConnection(connString));

}
