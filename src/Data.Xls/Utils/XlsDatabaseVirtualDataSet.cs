using SqlBuildingBlocks.POCOs;

namespace Data.Xls.Utils;

/// <summary>
/// A VirtualDataSet implementation that builds tables from a JSON document whose root is an object.
/// Each property is assumed to have an array of objects representing table rows.
/// This implementation accepts a <see cref="XlsDatabaseStreamSplitter"/>, which splits a large JSON file into 
/// streams for each table, and creates a VirtualDataTable for each table.
/// </summary>
public class XlsDatabaseVirtualDataSet : VirtualDataSet, IDisposable, IFreeStreams
{
    /// <summary>
    /// Initializes a new instance of the <see cref="XlsDatabaseVirtualDataSet"/> class using a JSON stream splitter.
    /// </summary>
    /// <param name="splitter">
    /// A <see cref="XlsDatabaseStreamSplitter"/> that returns substreams for each table within a large JSON document.
    /// </param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="splitter"/> is null.</exception>
    public XlsDatabaseVirtualDataSet(/*XlsDatabaseStreamSplitter splitter, IDictionary<string, DataColumnCollection> previousTableSchemas, int guessRows, Func<IEnumerable<XlsElement>, Type> guessTypeFunction, int bufferSize*/)
    {
        //foreach (var kvp in tableStreams)
        //{
        //    // Create a virtual table for each table stream.
        //    var virtualTable = new XlsVirtualDataTable(kvp.Value, kvp.Key, guessRows, guessTypeFunction, bufferSize);

        //    //If this virtual data table has no columns, then the XLS content was an empty array.  In this
        //    //case, we need to copy the schema from the previous table.
        //    if (virtualTable.Columns.Count == 0)
        //    {
        //        if (previousTableSchemas.TryGetValue(virtualTable.TableName, out DataColumnCollection dataColumnCollection))
        //        {
        //            foreach (DataColumn dataColumn in dataColumnCollection)
        //            {
        //                virtualTable.Columns.Add(dataColumn.ColumnName, dataColumn.DataType);
        //            }
        //        }
        //    }

        //    Tables.Add(virtualTable);
        //}
    }

    public void FreeStreams()
    {
        //splitter.Dispose();
    }

    void IDisposable.Dispose()
    {
        FreeStreams();
        base.Dispose();
    }
}
