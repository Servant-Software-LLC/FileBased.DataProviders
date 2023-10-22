using Microsoft.Extensions.Logging;

namespace System.Data.XmlClient;


/// <summary>
/// Represents a transaction to be performed at an XML file.
/// </summary>
public class XmlTransaction : FileTransaction<XmlParameter>
{
    private ILogger<XmlTransaction> log => ((IFileConnection)connection).LoggerServices.CreateLogger<XmlTransaction>();
    private readonly XmlConnection connection;

    /// <summary>
    /// Initializes a new instance of the <see cref="XmlTransaction"/> class with a connection and an isolation level.
    /// </summary>
    /// <param name="connection">The connection to use for the transaction.</param>
    /// <param name="isolationLevel">The isolation level to use for the transaction.</param>
    public XmlTransaction(XmlConnection connection, IsolationLevel isolationLevel = 0)
        : base(connection, isolationLevel)
    {
        this.connection = connection;
    }

    /// <inheritdoc />
    public override XmlCommand CreateCommand(string cmdText)
    {
        log.LogDebug($"{GetType()}.{nameof(CreateCommand)}() called.  CommandText = {cmdText}");
        return new(cmdText, connection, this);
    }

}
