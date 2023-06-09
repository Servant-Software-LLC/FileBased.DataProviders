using Data.Common.Utils;

namespace System.Data.XmlClient;

public class XmlCommand : FileCommand<XmlParameter>
{
    public XmlCommand()
    {
    }

    public XmlCommand(string command) : base(command)
    {
    }

    public XmlCommand(XmlConnection connection) : base(connection)
    {
    }

    public XmlCommand(string cmdText, XmlConnection connection) : base(cmdText, connection)
    {
    }

    public XmlCommand(string cmdText, XmlConnection connection, XmlTransaction transaction) 
        : base(cmdText, connection, transaction)
    {
    }

    public override XmlParameter CreateParameter() => new();
    public override XmlParameter CreateParameter(string parameterName, object value) => new(parameterName, value);

    public override XmlDataAdapter CreateAdapter() => new(this);

    protected override FileWriter CreateWriter(FileStatement fileStatement) => fileStatement switch
    {
        FileDelete deleteStatement => new XmlDelete(deleteStatement, (XmlConnection)Connection!, this),
        FileInsert insertStatement => new XmlInsert(insertStatement, (XmlConnection)Connection!, this),
        FileUpdate updateStatement => new XmlUpdate(updateStatement, (XmlConnection)Connection!, this),

        _ => throw new InvalidOperationException("query not supported")
    };

    protected override XmlDataReader CreateDataReader(IEnumerable<FileStatement> fileStatements, LoggerServices loggerServices) => 
        new(fileStatements, FileConnection.FileReader, CreateWriter, loggerServices);

}