using Data.Csv.Utils;
using SqlBuildingBlocks.POCOs;
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
            VirtualDataTable dataTable = null;

            //Check if the contents of the file contains any non-whitespace characters
            if (HasNonWhitespaceCharacter(textReader))
            {
                dataTable = PrepareDataTable(tableName);
            }

            DataSet!.Tables.Add(dataTable ?? new VirtualDataTable(tableName));
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

        var newDataTable = PrepareDataTable(tableName);
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
    private VirtualDataTable PrepareDataTable(string tableName, int pageSize = 1000, int numberOfRowsToReadForInference = 10)
    {
        CsvVirtualDataTable virtualDataTable = new(fileConnection.DataSourceProvider, tableName, pageSize, numberOfRowsToReadForInference, 
                                                   fileConnection.PreferredFloatingPointDataType);

        return virtualDataTable;
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
