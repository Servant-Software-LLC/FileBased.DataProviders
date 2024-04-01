using Microsoft.Extensions.Logging;

namespace System.Data.JsonClient;
/// <summary>
/// Represents a transaction to be performed in a JSON database.
/// </summary>
public class JsonTransaction : FileTransaction<JsonParameter>
{
    private ILogger<JsonTransaction> log => ((IFileConnection)connection).LoggerServices.CreateLogger<JsonTransaction>();
    private readonly JsonConnection connection;

    /// <summary>
    /// Initializes a new instance of the <see cref="JsonTransaction"/> class with a connection and isolation level.
    /// </summary>
    /// <param name="connection">The connection to associate with the transaction.</param>
    /// <param name="isolationLevel">The isolation level of the transaction.</param>
    public JsonTransaction(JsonConnection connection, IsolationLevel isolationLevel = 0)
        : base(connection, isolationLevel)
    {
        this.connection = connection;
    }

#if NET7_0_OR_GREATER    // .NET 7 implementation that supports covariant return types

    /// <summary>
    /// Creates a new <see cref="JsonCommand"/> with the specified command text and associates it with the current transaction.
    /// </summary>
    /// <param name="cmdText">The text of the command.</param>
    /// <returns>The created <see cref="JsonCommand"/>.</returns>
    public override JsonCommand CreateCommand(string cmdText)
    {
        log.LogDebug($"{GetType()}.{nameof(CreateCommand)}() called.  CommandText = {cmdText}");
        return new(cmdText, connection, this);
    }

#else       // .NET Standard 2.0 compliant implementation.This method must return FileCommand<JsonParameter>, not JsonCommand.

    /// <summary>
    /// Creates a new <see cref="JsonCommand"/> with the specified command text and associates it with the current transaction.
    /// </summary>
    /// <param name="cmdText">The text of the command.</param>
    /// <returns>The created <see cref="JsonCommand"/>.</returns>
    public override FileCommand<JsonParameter> CreateCommand(string cmdText)
    {
        log.LogDebug($"{GetType()}.{nameof(CreateCommand)}() called.  CommandText = {cmdText}");
        return new JsonCommand(cmdText, connection, this);
    }

#endif

}
