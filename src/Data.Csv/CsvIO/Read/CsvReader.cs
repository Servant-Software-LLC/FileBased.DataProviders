using Data.Common.Extension;
using CsvHelper.Configuration;
using Data.Common.Utils.ConnectionString;
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

    #region Folder Read Update
    protected override void ReadFromFolder(string tableName)
    {
        using (var textReader = fileConnection.DataSourceProvider.GetTextReader(tableName))
        {
            DataTable dataTable = null;

            //Check if the contents of the file contains any non-whitespace characters
            if (HasNonWhitespaceCharacter(textReader))
            {
                dataTable = FillDataTable(textReader, tableName);
            }

            DataSet!.Tables.Add(dataTable ?? new DataTable(tableName));
        }
    }

    protected override void UpdateFromFolder(string tableName)
    {
        using (var textReader = fileConnection.DataSourceProvider.GetTextReader(tableName))
        {
            // Determine if the table exists in the DataSet, if not create it.
            var dataTable = DataSet!.Tables[tableName];
            if (dataTable == null)
            {
                var newDataTable = FillDataTable(textReader, tableName);
                DataSet!.Tables.Add(newDataTable);
                return;
            }

            // Fill the existing DataTable with the new data
            dataTable.Rows.Clear();
            FillDataTable(textReader, dataTable);
        }
    }
    #endregion

    #region File Read Update

    protected override void ReadFromFile() =>
        throw new NotSupportedException("FileAsDatabase is not supported for the CSV provider.");

    protected override void UpdateFromFile() =>
        throw new NotSupportedException("FileAsDatabase is not supported for the CSV provider.");

    #endregion

    // Read the data from the folder to create a DataTable
    private DataTable FillDataTable(TextReader textReader, string tableName)
    {
        //Determine the schema of the DataTable
        DataTable results = new DataTable(tableName);
        CreateDataTableSchema(results);

        FillDataTable(textReader, results);
        return results;
    }

    // Fill the DataTable with the data from the CSV file
    private void FillDataTable(TextReader textReader, DataTable dataTable)
    {
        bool hasHeader = dataTable.Columns.Count > 0;

        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            MissingFieldFound = null,
            HasHeaderRecord = hasHeader
        };

        // Read the raw data into a temporary DataTable
        var tempDataTable = new DataTable();
        using (var reader = fileConnection.DataSourceProvider.GetTextReader(dataTable.TableName))
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


    private void CreateDataTableSchema(DataTable dataTable, long numberOfRowsToReadForInference = 10)
    {
        using (var textReader = fileConnection.DataSourceProvider.GetTextReader(dataTable.TableName))
        {
            // Load CSV into DataFrame to infer data types
            DataFrame df = CsvDataFrameLoader.LoadDataFrameWithQuotedFields(textReader, numberOfRowsToReadForInference);

            foreach (DataFrameColumn column in df.Columns)
            {
                DataColumn dataColumn = new DataColumn
                {
                    ColumnName = column.Name,
                    DataType = fileConnection.PreferredFloatingPointDataType.GetClrType(column.DataType)
                };
                dataTable.Columns.Add(dataColumn);
            }
        }
    }

    private static bool HasNonWhitespaceCharacter(TextReader textReader)
    {
        int character;
        while ((character = textReader.Read()) != -1)
        {
            if (!char.IsWhiteSpace((char)character))
            {
                return true; // Short-circuit as soon as we find a non-whitespace character
            }
        }

        return false; // No non-whitespace character found
    }
}
