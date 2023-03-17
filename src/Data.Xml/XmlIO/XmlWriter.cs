internal class XmlDataSetWriter : IDataSetWriter
{
    private readonly FileConnection fileConnection;
    private readonly FileQuery fileQuery;

    public XmlDataSetWriter(FileConnection fileConnection, FileQuery fileQuery)
    {
        this.fileConnection = fileConnection;
        this.fileQuery = fileQuery;
    }

    public void WriteDataSet(DataSet dataSet)
    {
        if (fileConnection.PathType == PathType.Directory)
        {
            SaveFolderAsDB(fileQuery.TableName, dataSet);
        }
        else
        {
            SaveToFile(dataSet);
        }
    }

    private void SaveToFile(DataSet dataSet)
    {
        using (var fileStream = new FileStream(fileConnection.Database, FileMode.Create, FileAccess.Write))
        {
            dataSet.WriteXml(fileStream, XmlWriteMode.WriteSchema);
        }
    }

    private void SaveFolderAsDB(string? tableName, DataSet dataSet)
    {
        var tablesToWrite = dataSet!.Tables.Cast<DataTable>();
        if (!string.IsNullOrEmpty(tableName))
            tablesToWrite = tablesToWrite.Where(t => t.TableName == tableName);

        foreach (DataTable table in tablesToWrite)
        {
            var path = fileConnection.GetTablePath(table.TableName);
            using (var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write))
            {
                table.WriteXml(fileStream, XmlWriteMode.WriteSchema);
            }
        }
    }
}
