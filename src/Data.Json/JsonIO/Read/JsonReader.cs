using Data.Json.Utils;
using SqlBuildingBlocks.POCOs;
using System.Data.JsonClient;

namespace Data.Json.JsonIO.Read;

/// <summary>
/// A JSON reader that supports both Folder and File modes by leveraging virtualized data table implementations.
/// For folder mode, the JSON file is assumed to be an array of objects representing a single table.
/// For file mode, the JSON document is assumed to be an object whose properties are arrays (tables).
/// </summary>
internal class JsonReader : FileReader
{
    public JsonReader(JsonConnection jsonConnection)
        : base(jsonConnection)
    {
    }

    #region Folder Read/Update

    protected override void ReadFromFolder(string tableName)
    {
        StreamReader streamReaderTable = Read(tableName);

        // For folder mode, the file itself is a JSON array representing a table.
        VirtualDataTable virtualDataTable = new JsonVirtualDataTable(streamReaderTable.BaseStream, tableName);
        DataSet!.Tables.Add(virtualDataTable);
    }

    protected override void UpdateFromFolder(string tableName)
    {
        StreamReader streamReaderTable = Read(tableName);
 
        var existingTable = DataSet!.Tables[tableName];
        if (existingTable == null)
        {
            existingTable = new JsonVirtualDataTable(streamReaderTable.BaseStream, tableName);
            DataSet!.Tables.Add(existingTable);
        }
        else
        {
            DataSet.RemoveWithDisposal(tableName);

            // Replace the Rows enumerable with a new one based on the updated JSON array.
            DataSet!.Tables.Add(new JsonVirtualDataTable(streamReaderTable.BaseStream, tableName));
        }
    }

    #endregion

    #region File Read/Update

    protected override void ReadFromFile()
    {
        StreamReader streamReaderTable = Read(string.Empty);
        JsonDatabaseStreamSplitter jsonDatabaseStreamSplitter = new(streamReaderTable.BaseStream);

        DataSet?.Dispose();

        // For file mode, the JSON document's root is an object with properties as tables.
        DataSet = new JsonDatabaseVirtualDataSet(jsonDatabaseStreamSplitter);
    }

    protected override void UpdateFromFile()
    {
        StreamReader streamReaderTable = Read(string.Empty);
        JsonDatabaseStreamSplitter jsonDatabaseStreamSplitter = new(streamReaderTable.BaseStream);

        DataSet?.Dispose();

        // For file mode, the JSON document's root is an object with properties as tables.
        DataSet = new JsonDatabaseVirtualDataSet(jsonDatabaseStreamSplitter);
    }

    #endregion

    private StreamReader Read(string tableName)
    {
        StreamReader textReader = fileConnection.DataSourceProvider.GetTextReader(tableName);
        return textReader;
    }
}
