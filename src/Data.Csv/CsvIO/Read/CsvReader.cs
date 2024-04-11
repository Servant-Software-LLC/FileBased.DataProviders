using CsvHelper.Configuration;
using System.Data.CsvClient;
using System.Globalization;

namespace Data.Csv.CsvIO.Read;

internal class CsvReader : FileReader
{    
    public CsvReader(CsvConnection connection) 
        : base(connection)
    {
    }

    void FillDataTable(string path, DataTable dataTable)
    {
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            MissingFieldFound = null,
        };

        using (var reader = new StreamReader(path))
        using (var csv = new CsvHelper.CsvReader(reader, config))
        {
            using (var dataReader = new CsvHelper.CsvDataReader(csv))
            {
                dataTable.Load(dataReader);
            }
        }

        //Post processing to replace String.Empty with DBNull.Value on DataTable, because for some unknown reason,
        //the values in the DataRows when there are two consecutive commas mostly end up being a DBNull, but for a
        //unit test, Insert_ShouldInsertNullData, it will sometimes be String.Empty.
        foreach (DataRow row in dataTable.Rows)
        {
            foreach (DataColumn column in dataTable.Columns)
            {
                if (row[column] is string value && string.IsNullOrEmpty(value))
                {
                    //Temporarily toggle the ReadOnly property to false, so we can change its value.
                    bool wasReadOnly = column.ReadOnly;
                    column.ReadOnly = false;
                    row[column] = DBNull.Value;
                    column.ReadOnly = wasReadOnly;
                }
            }
        }

    }

    #region Folder Read Update
    protected override void ReadFromFolder(string tableName)
    {
        var path = fileConnection.GetTablePath(tableName);
        var dataTable = new DataTable(tableName);
        FillDataTable(path,dataTable);
        DataSet!.Tables.Add(dataTable);
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
