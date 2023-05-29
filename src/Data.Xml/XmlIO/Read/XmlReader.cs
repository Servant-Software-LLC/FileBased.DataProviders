using System.Data.XmlClient;

namespace Data.Xml.XmlIO.Read;

internal class XmlReader : FileReader
{
    public XmlReader(XmlConnection connection) 
        : 
        base(connection)
    {
    }
    protected override void ReadFromFolder(IEnumerable<string> tableNames)
    {
        foreach (var name in tableNames)
        {
            var path = fileConnection.GetTablePath(name);
            var dataSet = new DataSet();
            dataSet.ReadXml(path);
            dataSet.Tables[0].TableName = name;
            DataSet!.Tables.Add(dataSet.Tables[0].Copy());
            dataSet.Dispose();
        }
    }
    protected override void UpdateFromFolder(string tableName)
    {
        var path = fileConnection.GetTablePath(tableName);
        var dataSet = new DataSet();
        dataSet.ReadXml(path);
      
      
        var oldDataTable = DataSet!.Tables[tableName];
        var newDataTable = dataSet.Tables[0].Copy();
        if (oldDataTable != null)
        {
            oldDataTable = newDataTable.Copy();
        }
        else
        {
            DataSet!.Tables.Add(newDataTable);
            oldDataTable = newDataTable;
        }
        oldDataTable.TableName = tableName;
        dataSet.Dispose();
    }

    protected override void ReadFromFile()
    {
      
        DataSet = new DataSet();
        DataSet.ReadXml(fileConnection.Database);
    }
    protected override void UpdateFromFile()
    {
        DataSet!.Clear();
        DataSet.ReadXml(fileConnection.Database);
    }
}
