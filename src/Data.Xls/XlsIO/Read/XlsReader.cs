using Data.Xls.Utils;
using SqlBuildingBlocks.POCOs;
using System.Data.XlsClient;
using Data.Common.Utils.ConnectionString;

namespace Data.Xls.XlsIO.Read;

internal class XlsReader : FileReader
{
    public XlsReader(XlsConnection connection) 
        : base(connection)
    {
    }

    protected override void ReadFromFile()
    {
        StreamReader streamReaderTable = Read(string.Empty);

        DataSet?.Dispose();

        XlsConnection xlsConnection = (XlsConnection)fileConnection;

        //TODO: 
        DataSet = new XlsDatabaseVirtualDataSet(/*streamReaderTable.BaseStream, previousTableSchemas, jsonConnection.GuessTypeRows, jsonConnection.GuessTypeFunction, bufferSize*/);
    }

    protected override void UpdateFromFile()
    {
        StreamReader streamReaderTable = Read(string.Empty);

        DataSet?.Dispose();

        XlsConnection jsonConnection = (XlsConnection)fileConnection;
        DataSet = new XlsDatabaseVirtualDataSet(/*jsonDatabaseStreamSplitter, previousTableSchemas, jsonConnection.GuessTypeRows, jsonConnection.GuessTypeFunction, bufferSize*/);
    }

    protected override void ReadFromFolder(string tableName) =>
        throw new NotSupportedException("FolderAsDatabase is not supported for the XLS provider.");

    protected override void UpdateFromFolder(string tableName) =>
        throw new NotSupportedException("FolderAsDatabase is not supported for the XLS provider.");

    // Read the data from the folder to create a DataTable
    private VirtualDataTable PrepareDataTable(StreamReader streamReader, string tableName, int pageSize = 1000)
    {
        XlsConnection csvConnection = (XlsConnection)fileConnection;
        XlsVirtualDataTable virtualDataTable = new(streamReader.BaseStream, tableName, csvConnection.GuessTypeRows, FloatingPointDataType.Double, csvConnection.GuessTypeFunction);

        return virtualDataTable;
    }

    private StreamReader Read(string tableName)
    {
        StreamReader textReader = fileConnection.DataSourceProvider.GetTextReader(tableName);
        return textReader;
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
