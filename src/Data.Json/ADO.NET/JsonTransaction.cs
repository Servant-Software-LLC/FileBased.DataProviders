using Microsoft.Extensions.Logging;

namespace System.Data.JsonClient;
public class JsonTransaction : FileTransaction<JsonParameter>
{
    private ILogger<JsonTransaction> log => ((IFileConnection)connection).LoggerServices.CreateLogger<JsonTransaction>();
    private readonly JsonConnection connection;

    public JsonTransaction(JsonConnection connection, IsolationLevel isolationLevel = 0) 
        : base(connection, isolationLevel)
    {
        this.connection = connection;
    }

    public override JsonCommand CreateCommand(string cmdText)
    {
        log.LogDebug($"{GetType()}.{nameof(CreateCommand)}() called.  CommandText = {cmdText}");
        return new(cmdText, connection, this);
    }
}