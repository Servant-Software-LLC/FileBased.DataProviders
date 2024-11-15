using SqlBuildingBlocks;
using System.Data;
using System.Data.XmlClient;

namespace Data.Xml.XmlIO.Read;

internal class XmlReader : FileReader
{
    public XmlReader(XmlConnection connection) 
        : 
        base(connection)
    {
    }
    protected override void ReadFromFolder(string tableName)
    {
        using (var textReader = fileConnection.DataSourceProvider.GetTextReader(tableName))
        using (var tempDataSet = new DataSet())
        {            
            tempDataSet.ReadXml(textReader);
            tempDataSet.Tables[0].TableName = tableName;
            DataSet!.Tables.Add(tempDataSet.Tables[0].Copy());
        }
    }

    protected override void UpdateFromFolder(string tableName)
    {
        using (var textReader = fileConnection.DataSourceProvider.GetTextReader(tableName))
        using (var tempDataSet = new DataSet())
        {
            tempDataSet.ReadXml(textReader);

            var oldDataTable = DataSet!.Tables[tableName];
            var newDataTable = tempDataSet.Tables[0].Copy();
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
        }
    }

    protected override void ReadFromFile()
    {
        using (var textReader = fileConnection.DataSourceProvider.GetTextReader(string.Empty))
        {
            DataSet = new DataSet();
            DataSet.ReadXml(textReader);
        }
    }

    protected override void UpdateFromFile()
    {
        using (var textReader = fileConnection.DataSourceProvider.GetTextReader(string.Empty))
        {
            DataSet!.Clear();
            DataSet.ReadXml(textReader);
        }
    }
}
