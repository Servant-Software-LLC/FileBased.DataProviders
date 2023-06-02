using Microsoft.Extensions.Logging;

namespace System.Data.CsvClient;
public class CsvTransaction : FileTransaction<CsvParameter>
{
    private ILogger<CsvTransaction> log => ((IFileConnection)connection).LoggerServices.CreateLogger<CsvTransaction>();
    private readonly CsvConnection connection;

    public CsvTransaction(CsvConnection connection, IsolationLevel isolationLevel = 0) 
        : base(connection, isolationLevel)
    {
        this.connection = connection;
    }

    public override CsvCommand CreateCommand(string cmdText)
    {
        log.LogDebug($"{GetType()}.{nameof(CreateCommand)}() called.  CommandText = {cmdText}");
        return new(cmdText, connection, this);
    }
}