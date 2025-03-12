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
        var textReader = fileConnection.DataSourceProvider.GetTextReader(tableName);

        VirtualDataTable dataTable = null;

        //Check if the contents of the file contains any non-whitespace characters
        if (HasNonWhitespaceCharacter(textReader))
        {
            //The CsvVirtualDataTable will assume ownership of disposing the textReader, because the
            //IEnumerable<DataRow> Rows within it could still be fetching from this reader.
            dataTable = PrepareDataTable(textReader, tableName);
        }
        else
        {
            textReader.Dispose();
        }

        DataSet!.Tables.Add(dataTable ?? new VirtualDataTable(new DataTable(tableName)));
    }

    protected override void UpdateFromFolder(string tableName)
    {
        //Remove the table if it exists
        DataSet!.RemoveWithDisposal(tableName);

        var textReader = fileConnection.DataSourceProvider.GetTextReader(tableName);

        //The CsvVirtualDataTable will assume ownership of disposing the textReader, because the
        //IEnumerable<DataRow> Rows within it could still be fetching from this reader.
        var newDataTable = PrepareDataTable(textReader, tableName);
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
    private VirtualDataTable PrepareDataTable(StreamReader streamReader, string tableName, int pageSize = 1000, int numberOfRowsToReadForInference = 10)
    {
        CsvVirtualDataTable virtualDataTable = new(streamReader, tableName, pageSize, numberOfRowsToReadForInference, 
                                                   fileConnection.PreferredFloatingPointDataType, ((CsvConnection)fileConnection).GuessTypeFunction);

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
