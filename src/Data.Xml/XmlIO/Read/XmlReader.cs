using SqlBuildingBlocks.POCOs;
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

            //TODO:  Need to support large data files (https://github.com/Servant-Software-LLC/FileBased.DataProviders/issues/83) 
            VirtualDataTable virtualDataTable = new(tempDataSet.Tables[0]);
            DataSet!.Tables.Add(virtualDataTable);
        }
    }

    protected override void UpdateFromFolder(string tableName)
    {
        using (var textReader = fileConnection.DataSourceProvider.GetTextReader(tableName))
        using (var tempDataSet = new DataSet())
        {
            tempDataSet.ReadXml(textReader);
            var newDataTable = tempDataSet.Tables[0];

            //If the table is already in the VirtualDataSet, then remove it.
            DataSet!.RemoveWithDisposal(tableName);

            //TODO:  Need to support large data files (https://github.com/Servant-Software-LLC/FileBased.DataProviders/issues/83) 
            VirtualDataTable virtualDataTable = new(newDataTable);
            DataSet!.Tables.Add(virtualDataTable);
        }
    }

    protected override void ReadFromFile()
    {
        using (var textReader = fileConnection.DataSourceProvider.GetTextReader(string.Empty))
        using (var tempDataSet = new DataSet())
        {
            tempDataSet.ReadXml(textReader);

            DataSet = new VirtualDataSet();
            foreach (DataTable dataTable in tempDataSet.Tables)
            {
                //TODO:  Need to support large data files (https://github.com/Servant-Software-LLC/FileBased.DataProviders/issues/83) 
                VirtualDataTable virtualDataTable = new(dataTable);
                DataSet.Tables.Add(virtualDataTable);
            }
        }
    }

    protected override void UpdateFromFile()
    {
        using (var textReader = fileConnection.DataSourceProvider.GetTextReader(string.Empty))
        using (var tempDataSet = new DataSet())
        {
            //Create empty DataTables in our tempDataSet that have its schema.
            foreach (VirtualDataTable virtualDataTable in DataSet!.Tables)
            {
                var dataTable = virtualDataTable.CreateEmptyDataTable();
                tempDataSet.Tables.Add(dataTable);
            }

            tempDataSet.ReadXml(textReader);

            DataSet = new VirtualDataSet();
            foreach (DataTable dataTable in tempDataSet.Tables)
            {
                //TODO:  Need to support large data files (https://github.com/Servant-Software-LLC/FileBased.DataProviders/issues/83) 
                VirtualDataTable virtualDataTable = new(dataTable);
                DataSet.Tables.Add(virtualDataTable);
            }
        }
    }
}
