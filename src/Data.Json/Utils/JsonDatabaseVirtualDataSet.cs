using SqlBuildingBlocks.POCOs;

namespace Data.Json.Utils;

/// <summary>
/// A VirtualDataSet implementation that builds tables from a JSON document whose root is an object.
/// Each property is assumed to have an array of objects representing table rows.
/// This implementation accepts a <see cref="JsonDatabaseStreamSplitter"/>, which splits a large JSON file into 
/// streams for each table, and creates a VirtualDataTable for each table.
/// </summary>
public class JsonDatabaseVirtualDataSet : VirtualDataSet, IDisposable
{
    private readonly JsonDatabaseStreamSplitter splitter;

    /// <summary>
    /// Initializes a new instance of the <see cref="JsonDatabaseVirtualDataSet"/> class using a JSON stream splitter.
    /// </summary>
    /// <param name="splitter">
    /// A <see cref="JsonDatabaseStreamSplitter"/> that returns substreams for each table within a large JSON document.
    /// </param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="splitter"/> is null.</exception>
    public JsonDatabaseVirtualDataSet(JsonDatabaseStreamSplitter splitter)
    {
        this.splitter = splitter ?? throw new ArgumentNullException(nameof(splitter));

        // Get a dictionary mapping table names to streams representing each table's JSON array.
        IDictionary<string, Stream> tableStreams = splitter.GetTableStreams();
        foreach (var kvp in tableStreams)
        {
            // Create a virtual table for each table stream.
            var virtualTable = new JsonVirtualDataTable(kvp.Value, kvp.Key);

            Tables.Add(virtualTable);
        }
    }

    void IDisposable.Dispose()
    {
        splitter.Dispose();
        base.Dispose();
    }
}
