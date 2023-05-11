using CsvHelper.Configuration;
using System.Data.CsvClient;
using System.Globalization;

namespace Data.Csv.CsvIO.Read;

internal class CsvReader : FileReader<CsvParameter>
{    
    public CsvReader(CsvConnection connection) 
        : base(connection)
    {
    }

    void FillDataTable(string path,DataTable dataTable)
    {
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            MissingFieldFound = null,
        };

        using (var reader = new StreamReader(path))
        using (var csv = new CsvHelper.CsvReader(reader, config))
        {
            using (var dr = new CsvHelper.CsvDataReader(csv))
            {
                dataTable.Load(dr);
            }
        }
    }
   
    #region Folder Read Update
    protected override void ReadFromFolder(IEnumerable<string> tableNames)
    {
        foreach (var name in tableNames)
        {
            var path = fileConnection.GetTablePath(name);
            var dataTable = new DataTable(name);
            FillDataTable(path,dataTable);
            DataSet!.Tables.Add(dataTable);
        }
    }

    protected override void UpdateFromFolder(string tableName)
    {
        var path = fileConnection.GetTablePath(tableName);
        var dataTable = new DataTable(tableName);
        FillDataTable(path, dataTable);
        //remove if exist
        if (DataSet!.Tables[tableName]!=null)
        DataSet!.Tables.Remove(tableName);
        DataSet!.Tables.Add(dataTable);
    }
    #endregion

    #region File Read Update

    protected override void ReadFromFile() => 
        throw new NotSupportedException("FileAsDatabase is not supported for the CSV provider.");

    protected override void UpdateFromFile() =>
        throw new NotSupportedException("FileAsDatabase is not supported for the CSV provider.");

    #endregion
}
