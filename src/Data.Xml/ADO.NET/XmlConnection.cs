using Data.Xml.XmlIO;
using System.Data.Common;
using System.Xml.Linq;

namespace System.Data.XmlClient;

public class XmlConnection : FileConnection<XmlParameter>
{
    public XmlConnection(string connectionString) : 
        base(connectionString)
    {
        FileReader = new XmlReader(this);
    }


    public override string FileExtension => "xml";

    public override XmlTransaction BeginTransaction() => new(this, default);

    public override XmlTransaction BeginTransaction(IsolationLevel il) => BeginTransaction();
    protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel) => BeginTransaction(isolationLevel);

    public override XmlDataAdapter CreateDataAdapter(string query) => new(query, this);

    public override XmlCommand CreateCommand() => new(this);
    public override XmlCommand CreateCommand(string cmdText) => new(cmdText, this);

    protected override void CreateFileAsDatabase(string databaseFileName)
    {
        var doc = new XDocument(new XElement("DataSet"));
        doc.Save(databaseFileName);
    }

    protected override Func<FileStatement, IDataSetWriter> CreateDataSetWriter => fileStatement => new XmlDataSetWriter(this, fileStatement);
}