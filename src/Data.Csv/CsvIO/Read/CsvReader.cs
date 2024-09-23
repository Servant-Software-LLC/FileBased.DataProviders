using CsvHelper.Configuration;
using Data.Csv.Utils;
using Microsoft.Data.Analysis;
using System.Data.CsvClient;
using System.Globalization;

namespace Data.Csv.CsvIO.Read;

internal class CsvReader : FileReader
{    
    public CsvReader(CsvConnection connection) 
        : base(connection)
    {
    }

    private DataTable FillDataTable(string path, string tableName)
    {
        //Determine the schema of the DataTable
        DataTable results = new DataTable(tableName);
        CreateDataTableSchema(path, results);

        bool hasHeader = results.Columns.Count > 0;

        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            MissingFieldFound = null,
            HasHeaderRecord = hasHeader
        };

        // Read the raw data into a temporary DataTable
        var dataTable = new DataTable();
        using (var reader = new StreamReader(path))
        using (var csv = new CsvHelper.CsvReader(reader, config))
        {
            // Initialize CsvDataReader with the CsvReader and DataTable schema
            using (var dataReader = new CsvHelper.CsvDataReader(csv))
            {
                // Load data into DataTable
                dataTable.Load(dataReader);
            }
        }

        // Copy the data from the temporary DataTable to the results DataTable
        foreach (DataRow row in dataTable.Rows)
        {
            var newRow = results.NewRow();
            foreach (DataColumn column in dataTable.Columns)
            {
                // Check for empty strings and set them to DBNull
                if (row[column] is string value && string.IsNullOrEmpty(value) || row[column] == null)
                {
                    //Temporarily toggle the ReadOnly property to false, so we can change its value.
                    bool wasReadOnly = column.ReadOnly;
                    column.ReadOnly = false;
                    newRow[column.ColumnName] = DBNull.Value;
                    column.ReadOnly = wasReadOnly;
                }
                else
                {
                    var rawValue = row[column];
                    var expectedType = results.Columns[column.ColumnName].DataType;

                    // Convert value to the correct type
                    var convertedValue = rawValue == DBNull.Value ? DBNull.Value : Convert.ChangeType(rawValue, expectedType);
                    newRow[column.ColumnName] = convertedValue;
                }
            }

            results.Rows.Add(newRow);
        }

        return results;
    }

    private void CreateDataTableSchema(string path, DataTable dataTable, long numberOfRowsToReadForInference = 10)
    {


        // Load CSV into DataFrame to infer data types
        DataFrame df = CsvDataFrameLoader.LoadDataFrameWithQuotedFields(path, numberOfRowsToReadForInference);

        foreach (DataFrameColumn column in df.Columns)
        {
            DataColumn dataColumn = new DataColumn
            {
                ColumnName = column.Name,
                DataType = GetClrType(column.DataType)
            };
            dataTable.Columns.Add(dataColumn);
        }
    }

    private Type GetClrType(Type dataFrameColumnType)
    {
        // Convert float to decimal
        if (dataFrameColumnType == typeof(float))
            return typeof(decimal);

        return dataFrameColumnType;
    }

    #region Folder Read Update
    protected override void ReadFromFolder(string tableName)
    {
        var path = fileConnection.GetTablePath(tableName);

        DataTable dataTable = null;

        //Check if the contents of the file contains any non-whitespace characters
        if (HasNonWhitespaceCharacter(path))
        {
            dataTable = FillDataTable(path, tableName);
        }

        DataSet!.Tables.Add(dataTable ?? new DataTable(tableName));
    }

    protected override void UpdateFromFolder(string tableName)
    {
        var path = fileConnection.GetTablePath(tableName);
        var dataTable = FillDataTable(path, tableName);
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

    private static bool HasNonWhitespaceCharacter(string path)
    {
        using (StreamReader reader = new StreamReader(path))
        {
            int character;
            while ((character = reader.Read()) != -1)
            {
                if (!char.IsWhiteSpace((char)character))
                {
                    return true; // Short-circuit as soon as we find a non-whitespace character
                }
            }
        }
        return false; // No non-whitespace character found
    }
}
