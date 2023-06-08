using EFCore.Common.Storage.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data.CsvClient;

namespace EFCore.Csv.Storage.Internal;


public class CsvDatabaseCreator : FileDatabaseCreator
{
    //TODO: Remove if not used after implementation is fully complete.  It may be that we don't even need all these derived classes and can 
    //      make FileDatabaseCreator non-abstract and register it.
    private readonly ICsvRelationalConnection connection;

    public CsvDatabaseCreator(
        RelationalDatabaseCreatorDependencies dependencies,
        ICsvRelationalConnection connection,
        IRawSqlCommandBuilder rawSqlCommandBuilder)
        : base(dependencies, rawSqlCommandBuilder)
    {
        this.connection = connection ?? throw new ArgumentNullException(nameof(connection));
    }

    public override void Create() =>
        Create<CsvConnection, CsvParameter>(connString => new CsvConnection(connString));

    public override void Delete() =>
        Delete<CsvConnection, CsvParameter>(connString => new CsvConnection(connString));

}
