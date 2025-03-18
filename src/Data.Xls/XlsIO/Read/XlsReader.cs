using Data.Xls.Utils;
using SqlBuildingBlocks.POCOs;
using System.Data.XlsClient;
using Data.Common.Utils.ConnectionString;
using DocumentFormat.OpenXml.Wordprocessing;

namespace Data.Xls.XlsIO.Read;

internal class XlsReader : FileReader
{
    private const int pageSize = 4096;

    public XlsReader(XlsConnection connection) 
        : base(connection)
    {
    }

    protected override void ReadFromFile()
    {
        StreamReader streamReaderTable = Read(string.Empty);

        DataSet?.Dispose();

        XlsConnection xlsConnection = (XlsConnection)fileConnection;
        var databaseName = Path.GetFileNameWithoutExtension(xlsConnection.Database);
        DataSet = new XlsDatabaseVirtualDataSet(streamReaderTable.BaseStream, databaseName, xlsConnection.GuessTypeRows, pageSize,
                                                fileConnection.PreferredFloatingPointDataType, xlsConnection.GuessTypeFunction);
    }

    protected override void UpdateFromFile()
    {
        StreamReader streamReaderTable = Read(string.Empty);

        DataSet?.Dispose();

        XlsConnection xlsConnection = (XlsConnection)fileConnection;
        var databaseName = Path.GetFileNameWithoutExtension(xlsConnection.Database);
        DataSet = new XlsDatabaseVirtualDataSet(streamReaderTable.BaseStream, databaseName, xlsConnection.GuessTypeRows, pageSize, fileConnection.PreferredFloatingPointDataType, xlsConnection.GuessTypeFunction);
    }

    protected override void ReadFromFolder(string tableName) =>
        throw new NotSupportedException("FolderAsDatabase is not supported for the XLS provider.");

    protected override void UpdateFromFolder(string tableName) =>
        throw new NotSupportedException("FolderAsDatabase is not supported for the XLS provider.");

    private StreamReader Read(string tableName)
    {
        StreamReader textReader = fileConnection.DataSourceProvider.GetTextReader(tableName);
        return textReader;
    }

}
