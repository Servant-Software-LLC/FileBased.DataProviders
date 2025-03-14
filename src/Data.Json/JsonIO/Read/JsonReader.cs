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
    private const int guessRows = 10;
    private const int bufferSize = 4096;

    public JsonReader(JsonConnection jsonConnection)
        : base(jsonConnection)
    {
    }

    #region Folder Read/Update

    protected override void ReadFromFolder(string tableName)
    {
        StreamReader streamReaderTable = Read(tableName);

        // For folder mode, the file itself is a JSON array representing a table.
        VirtualDataTable virtualDataTable = PrepareDataTable(streamReaderTable.BaseStream, tableName);
        DataSet!.Tables.Add(virtualDataTable);
    }

    protected override void UpdateFromFolder(string tableName)
    {
        DataSet.RemoveWithDisposal(tableName);

        StreamReader streamReaderTable = Read(tableName);

        // Replace the Rows enumerable with a new one based on the updated JSON array.
        var reloadedVirtualDataTable = PrepareDataTable(streamReaderTable.BaseStream, tableName);
        DataSet!.Tables.Add(reloadedVirtualDataTable);
    }

    private VirtualDataTable PrepareDataTable(Stream stream, string tableName, int pageSize = 1000)
    {
        JsonVirtualDataTable virtualDataTable = new(stream, tableName, guessRows, ((JsonConnection)fileConnection).GuessTypeFunction, bufferSize);

        return virtualDataTable;
    }

    #endregion

    #region File Read/Update

    protected override void ReadFromFile()
    {
        StreamReader streamReaderTable = Read(string.Empty);
        JsonDatabaseStreamSplitter jsonDatabaseStreamSplitter = new(streamReaderTable.BaseStream);

        //Gather the previous table schemas, because a JSON dataset without data does not have a schema stored in the JSON content.
        //Instead, the schema may have been added by the consumer of this library by using CREATE TABLE and ALTER TABLE statements.
        var previousTableSchemas = GatherPreviousTableSchemas();

        DataSet?.Dispose();

        // For file mode, the JSON document's root is an object with properties as tables.
        DataSet = new JsonDatabaseVirtualDataSet(jsonDatabaseStreamSplitter, previousTableSchemas, guessRows, ((JsonConnection)fileConnection).GuessTypeFunction, bufferSize);
    }

    protected override void UpdateFromFile()
    {
        StreamReader streamReaderTable = Read(string.Empty);
        JsonDatabaseStreamSplitter jsonDatabaseStreamSplitter = new(streamReaderTable.BaseStream);

        //Gather the previous table schemas, because a JSON dataset without data does not have a schema stored in the JSON content.
        //Instead, the schema may have been added by the consumer of this library by using CREATE TABLE and ALTER TABLE statements.
        var previousTableSchemas = GatherPreviousTableSchemas();
        DataSet?.Dispose();

        // For file mode, the JSON document's root is an object with properties as tables.
        DataSet = new JsonDatabaseVirtualDataSet(jsonDatabaseStreamSplitter, previousTableSchemas, guessRows, ((JsonConnection)fileConnection).GuessTypeFunction, bufferSize);
    }

    #endregion

    private StreamReader Read(string tableName)
    {
        StreamReader textReader = fileConnection.DataSourceProvider.GetTextReader(tableName);
        return textReader;
    }

    private IDictionary<string, DataColumnCollection> GatherPreviousTableSchemas()
    {
        Dictionary<string, DataColumnCollection> results = new();
        if (DataSet != null)
        {
            foreach (var table in DataSet.Tables)
            {
                results[table.TableName] = table.Columns;
            }
        }

        return results;
    }
}
