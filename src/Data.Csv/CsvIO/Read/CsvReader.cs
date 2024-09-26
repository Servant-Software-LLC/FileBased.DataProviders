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

    // Read the data from the folder to create a DataTable
    private DataTable FillDataTable(string path, string tableName)
    {
        //Determine the schema of the DataTable
        DataTable results = new DataTable(tableName);
        CreateDataTableSchema(path, results);

        FillDataTable(path, results);
        return results;
    }

    // Fill the DataTable with the data from the CSV file
    private void FillDataTable(string path, DataTable dataTable)
    {
        bool hasHeader = dataTable.Columns.Count > 0;

        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            MissingFieldFound = null,
            HasHeaderRecord = hasHeader
        };

        // Read the raw data into a temporary DataTable
        var tempDataTable = new DataTable();
        using (var reader = new StreamReader(path))
        using (var csv = new CsvHelper.CsvReader(reader, config))
        {
            // Initialize CsvDataReader with the CsvReader and DataTable schema
            using (var dataReader = new CsvHelper.CsvDataReader(csv))
            {
                // Load data into DataTable
                tempDataTable.Load(dataReader);
            }
        }

        // Copy the data from the temporary DataTable to the results DataTable
        foreach (DataRow row in tempDataTable.Rows)
        {
            var newRow = dataTable.NewRow();
            foreach (DataColumn column in dataTable.Columns)
            {
                // Check for empty strings and set them to DBNull
                if (row[column.ColumnName] is string value && string.IsNullOrEmpty(value) || row[column.ColumnName] == null)
                {
                    //Temporarily toggle the ReadOnly property to false, so we can change its value.
                    bool wasReadOnly = column.ReadOnly;
                    column.ReadOnly = false;
                    newRow[column.ColumnName] = DBNull.Value;
                    column.ReadOnly = wasReadOnly;
                }
                else
                {
                    var rawValue = row[column.ColumnName];
                    var expectedType = dataTable.Columns[column.ColumnName].DataType;

                    // Convert value to the correct type
                    var convertedValue = rawValue == DBNull.Value ? DBNull.Value : Convert.ChangeType(rawValue, expectedType);
                    newRow[column.ColumnName] = convertedValue;
                }
            }

            dataTable.Rows.Add(newRow);
        }
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

        // Determine if the table exists in the DataSet, if not create it.
        var dataTable = DataSet!.Tables[tableName];
        if (dataTable == null)
        {
            var newDataTable = FillDataTable(path, tableName);
            DataSet!.Tables.Add(newDataTable);
            return;
        }

        // Fill the existing DataTable with the new data
        FillDataTable(path, dataTable);
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
