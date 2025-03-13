using Data.Common.Utils;
using SqlBuildingBlocks.POCOs;

namespace Data.Json.Utils;

/// <summary>
/// Represents a virtualized JSON data table built from a streaming JSON source.
/// The JSON is expected to be a top-level array of objects.
/// The schema (i.e. column names and data types) is inferred from the first object.
/// Rows are produced on demand without loading the entire file into memory.
/// </summary>
public class JsonVirtualDataTable : VirtualDataTable, IDisposable, IFreeStreams
{
    private Stream stream;
    private readonly int bufferSize;
    private readonly int guessRows;
    private readonly Func<IEnumerable<(string, JsonValueKind)>, Type> guessTypeFunction;

    /// <summary>
    /// Initializes a new instance of the <see cref="JsonVirtualDataTable"/> class.
    /// </summary>
    /// <param name="stream">
    /// A stream containing the JSON. This stream need not be seekable.
    /// </param>
    /// <param name="tableName">
    /// The table name.
    /// </param>
    /// <param name="bufferSize">
    /// The buffer size to use when reading from the stream.
    /// </param>
    public JsonVirtualDataTable(Stream stream, string tableName, int guessRows, Func<IEnumerable<(string, JsonValueKind)>, Type> guessTypeFunction, int bufferSize)
        : base(tableName)
    {
        this.stream = stream ?? throw new ArgumentNullException(nameof(stream));
        this.guessRows = guessRows > 0 ? guessRows : throw new ArgumentOutOfRangeException(nameof(guessRows), $"Guess rows must be 1 or greater.  Value: {guessRows}");
        this.guessTypeFunction = guessTypeFunction ?? DefaultGuessTypeFunction;
        this.bufferSize = bufferSize;

        // Read a “preamble” to determine the schema.
        DetermineColumns();

        // Determine the schema and column data types using the first page.
        Rows = EnumerateRows();
    }

    /// <summary>
    /// Reads enough from the underlying stream to capture the opening of the JSON array and the first object,
    /// uses that first object to infer the schema, and then resets the stream so that enumeration will start
    /// from the beginning of the JSON.
    /// </summary>
    private void DetermineColumns()
    {
        // We'll accumulate data until we can parse the first complete object.
        bool foundFirstObject = false;

        var reader = new StreamJsonReader(stream, bufferSize);
        reader.Preamble = new MemoryStream();
        while (!foundFirstObject && reader.Read())
        {
            if (reader.TokenType == JsonTokenType.StartArray)
            {
                // Skip the array start.
                continue;
            }
            if (reader.TokenType == JsonTokenType.StartObject)
            {
                try
                {
                    // Attempt to parse the entire object.
                    using (JsonDocument doc = reader.ParseCurrentValue())
                    {
                        JsonElement firstObj = doc.RootElement;
                        // Infer schema from firstObj.
                        DataTable schemaTable = new DataTable(TableName);
                        foreach (JsonProperty prop in firstObj.EnumerateObject())
                        {
                            Type type = MapJsonValueKindToType(prop.Value.ValueKind);
                            AddColumn(prop.Name, type);
                        }
                        foundFirstObject = true;
                        break;
                    }
                }
                catch (Exception)
                {
                    // Incomplete object; continue reading more bytes.
                    //TODO:  Do we halt completely??  Log this error or something else?
                }
            }
        }

        // Now, we must re-create the stream so that it returns the preamble bytes first,
        // then continues with the remainder of the original stream.
        reader.Preamble.Position = 0;
        stream = new ConcatStream(reader.Preamble, stream);
    }

    private void AddColumn(string columnName, Type columnType)
    {
        var existingColumnIndex = Columns.IndexOf(columnName);
        if (existingColumnIndex == -1)
        {
            //This is a new column, which is the common scenario
            Columns.Add(columnName, columnType);
            return;
        }

        var existingColumn = Columns[existingColumnIndex];
        if (existingColumn.DataType != columnType) 
        {
            throw new InvalidOperationException($"The column {columnName} already exists in this virtual table. Further, the data types don't match.  Existing: {existingColumn.DataType.FullName} New: {columnType.FullName}");
        }
    }


    private Type DefaultGuessTypeFunction(IEnumerable<(string, JsonValueKind)> values)
    {
        Type finalGuess = null;
        foreach((string stringValue, JsonValueKind valueKind) in values)
        {
            var rowGuessType = MapJsonValueKindToType(valueKind);
            if (finalGuess == null) 
            {
                finalGuess = rowGuessType;
                continue;
            }

            finalGuess = CompatibleType(finalGuess, rowGuessType);
        }

        return finalGuess;
    }

    private static Type CompatibleType(Type a, Type b)
    {
        if (a == null)
            throw new ArgumentNullException(nameof(a));
        if (b == null) 
            throw new ArgumentNullException(nameof (b));

        if (a == b)
            return a;

        if (a == typeof(string) || b == typeof(string))
            return typeof(string);

        if ((a == typeof(double) && b == typeof(bool)) ||
            (a == typeof(bool) && b == typeof(double)))
            return typeof(double);

        throw new InvalidOperationException($"Unexpected types to compare. a={a.FullName} b={b.FullName}");
    }

    private static Type MapJsonValueKindToType(JsonValueKind kind) =>
        kind switch
        {
            JsonValueKind.Number => typeof(double),
            JsonValueKind.True => typeof(bool),
            JsonValueKind.False => typeof(bool),
            _ => typeof(string)
        };

    /// <summary>
    /// Returns an enumerable of DataRow objects by incrementally reading JSON objects from the stream.
    /// </summary>
    private IEnumerable<DataRow> EnumerateRows()
    {
        bool insideArray = false;
        var reader = new StreamJsonReader(stream, bufferSize);
        while (reader.Read())
        {
            if (!insideArray && reader.TokenType == JsonTokenType.StartArray)
            {
                insideArray = true;
                continue;
            }
            if (insideArray)
            {
                if (reader.TokenType == JsonTokenType.EndArray)
                    break;

                if (reader.TokenType == JsonTokenType.StartObject)
                {
                    // Parse the complete JSON object.
                    using (JsonDocument doc = reader.ParseCurrentValue())
                    {
                        JsonElement obj = doc.RootElement;

                        // Create a new DataRow from the schema.
                        DataRow row = NewRow();
                        foreach (DataColumn col in Columns)
                        {
                            if (obj.TryGetProperty(col.ColumnName, out JsonElement prop))
                            {
                                row[col.ColumnName] = ConvertJsonElement(prop, col.DataType);
                            }
                        }

                        yield return row;
                    }
                }
            }
        }
    }

    private static object ConvertJsonElement(JsonElement element, Type targetType)
    {
        if (element.ValueKind == JsonValueKind.Null)
            return DBNull.Value;

        if (targetType == typeof(string))
            return element.ToString();
        if (targetType == typeof(double))
            return element.TryGetDouble(out double d) ? d : 0.0;
        if (targetType == typeof(bool))
            return element.GetBoolean();
        return element.ToString();
    }

    public void Dispose()
    {
        FreeStreams();
    }

    public void FreeStreams()
    {
        Rows = null;    //These Rows cannot enumerate on the disposed stream anymore.
        stream.Dispose();
    }
}
