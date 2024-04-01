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

#if NET7_0_OR_GREATER    // .NET 7 implementation that supports covariant return types

    /// <inheritdoc />
    public override XmlCommand CreateCommand(string cmdText)
    {
        log.LogDebug($"{GetType()}.{nameof(CreateCommand)}() called.  CommandText = {cmdText}");
        return new(cmdText, connection, this);
    }


#else       // .NET Standard 2.0 compliant implementation.This method must return FileCommand<XmlParameter>, not XmlCommand.

    /// <inheritdoc />
    public override FileCommand<XmlParameter> CreateCommand(string cmdText)
    {
        log.LogDebug($"{GetType()}.{nameof(CreateCommand)}() called.  CommandText = {cmdText}");
        return new XmlCommand(cmdText, connection, this);
    }

#endif


}
