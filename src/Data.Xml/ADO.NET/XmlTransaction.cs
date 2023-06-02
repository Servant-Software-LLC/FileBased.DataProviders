using Microsoft.Extensions.Logging;

namespace System.Data.XmlClient;

public class XmlTransaction : FileTransaction<XmlParameter>
{
    private ILogger<XmlTransaction> log => ((IFileConnection)connection).LoggerServices.CreateLogger<XmlTransaction>();
    private readonly XmlConnection connection;

    public XmlTransaction(XmlConnection connection, IsolationLevel isolationLevel = 0) 
        : base(connection, isolationLevel)
    {
        this.connection = connection;
    }

    public override XmlCommand CreateCommand(string cmdText)
    {
        log.LogDebug($"{GetType()}.{nameof(CreateCommand)}() called.  CommandText = {cmdText}");
        return new(cmdText, connection, this);
    }

}