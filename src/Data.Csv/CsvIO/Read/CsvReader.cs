using Data.Csv.Utils;
using Microsoft.Data.Analysis;
using System.Data.CsvClient;

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
                dataTable = FillDataTable(tableName);
            }

            DataSet!.Tables.Add(dataTable ?? new DataTable(tableName));
        }
    }

    protected override void UpdateFromFolder(string tableName)
    {
        // Determine if the table exists in the DataSet, if not create it.
        var dataTable = DataSet!.Tables[tableName];
        if (dataTable == null)
        {
            // Fill the existing DataTable with the new data
            DataSet!.Tables.Remove(tableName);
        }

        var newDataTable = FillDataTable(tableName);
        DataSet!.Tables.Add(newDataTable);
    }
    #endregion

    #region File Read Update

    protected override void ReadFromFile() =>
        throw new NotSupportedException("FileAsDatabase is not supported for the CSV provider.");

    protected override void UpdateFromFile() =>
        throw new NotSupportedException("FileAsDatabase is not supported for the CSV provider.");

    #endregion

    // Read the data from the folder to create a DataTable
    private DataTable FillDataTable(string tableName, int numberOfRowsToReadForInference = 10)
    {
        // Load CSV into DataFrame to infer data types
        DataFrame dataFrame = CsvDataFrameLoader.LoadDataFrame(fileConnection.DataSourceProvider, tableName, numberOfRowsToReadForInference,
                                                               fileConnection.PreferredFloatingPointDataType);

        DataTable results = new DataTable(tableName);
        FillDataTable(dataFrame, results);
        return results;
    }

    private DataTable FillDataTable(DataFrame dataFrame, DataTable dataTable)
    {
        // Add columns to DataTable
        foreach (var column in dataFrame.Columns)
        {
            // Use column.Name for the DataColumn name and column.DataType for its type
            var dataType = column.DataType;
            dataTable.Columns.Add(column.Name, dataType);
        }

        // Add rows to DataTable
        foreach (var row in dataFrame.Rows)
        {
            DataRow newRow = dataTable.NewRow();

            for (int i = 0; i < dataFrame.Columns.Count; i++)
            {
                // Check for empty strings and set them to DBNull
                if (row[i] is string value && string.IsNullOrEmpty(value) || row[i] == null)
                {
                    newRow[i] = DBNull.Value;
                }
                else
                {
                    // Convert value to the correct type
                    var convertedValue = row[i] == DBNull.Value ? DBNull.Value : row[i];
                    newRow[i] = convertedValue;
                }
            }

            dataTable.Rows.Add(newRow);
        }

        return dataTable;
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
