using Data.Csv.Utils;
using SqlBuildingBlocks.POCOs;
using System.Data.XlsClient;

namespace Data.Xls.XlsIO.Read;

internal class XlsReader : FileReader
{
    private const int pageSize = 4096;

    public XlsReader(XlsConnection connection) 
        : base(connection)
    {
    }

    protected override void ReadFromFile() =>
        throw new NotSupportedException("ReadFromFile should not be called.  XLS provider makes collection of streams for each table.");

    protected override void UpdateFromFile() =>
        throw new NotSupportedException("UpdateFromFile should not be called.  XLS provider makes collection of streams for each table.");

    protected override void ReadFromFolder(string tableName)
    {
        StreamReader streamReaderTable = Read(tableName);

        //The CsvVirtualDataTable will assume ownership of disposing the textReader, because the
        //IEnumerable<DataRow> Rows within it could still be fetching from this reader.
        VirtualDataTable dataTable = PrepareDataTable(streamReaderTable, tableName);

        DataSet!.Tables.Add(dataTable ?? new VirtualDataTable(new DataTable(tableName)));
    }

    protected override void UpdateFromFolder(string tableName)
    {
        //Remove the table if it exists
        DataSet?.Dispose();
        DataSet = new();

        StreamReader streamReaderTable = Read(tableName);

        //The CsvVirtualDataTable will assume ownership of disposing the textReader, because the
        //IEnumerable<DataRow> Rows within it could still be fetching from this reader.
        var newDataTable = PrepareDataTable(streamReaderTable, tableName);
        DataSet!.Tables.Add(newDataTable);
    }

    private StreamReader Read(string tableName)
    {
        StreamReader textReader = fileConnection.DataSourceProvider.GetTextReader(tableName);
        return textReader;
    }

    // Read the data from the folder to create a DataTable
    private VirtualDataTable PrepareDataTable(StreamReader streamReader, string tableName)
    {
        XlsConnection xlsConnection = (XlsConnection)fileConnection;
        char separator = ',';  //Since the XlsSheetStream composed the stream as comma separated, we are ensured that the separator is a comma and don't need to detect it.
        CsvVirtualDataTable virtualDataTable = new(streamReader, tableName, pageSize, xlsConnection.GuessTypeRows,
                                                   fileConnection.PreferredFloatingPointDataType, xlsConnection.GuessTypeFunction,
                                                   separator);

        return virtualDataTable;
    }

}
