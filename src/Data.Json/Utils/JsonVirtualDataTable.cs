using System.Text.RegularExpressions;
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
    private readonly Func<IEnumerable<JsonElement>, Type> guessTypeFunction;

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
    public JsonVirtualDataTable(Stream stream, string tableName, int guessRows, Func<IEnumerable<JsonElement>, Type> guessTypeFunction, int bufferSize)
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

        var reader = new StreamJsonReader(stream, bufferSize);
        reader.Preamble = new MemoryStream();

        var columnsOfData = GatherGuessingRows(reader);
        var columnTypes = GuessTypes(columnsOfData);

        // Now, we can build the schema of columns
        foreach (var columnType in columnTypes)
        {
            AddColumn(columnType.Key, columnType.Value);
        }

        // Re-create the stream so that it returns the preamble bytes first,
        // then continues with the remainder of the original stream.
        reader.Preamble.Position = 0;
        stream = new ConcatStream(reader.Preamble, stream);
    }

    private IDictionary<string, List<JsonElement>> GatherGuessingRows(StreamJsonReader reader)
    {
        Dictionary<string, List<JsonElement>> columnsOfData = new();

        int rowsGathered = 0;
        while (rowsGathered < guessRows && reader.Read())
        {
            // If we're at the start of the array
            if (reader.TokenType == JsonTokenType.StartArray)
            {
                // Skip the array start.
                continue;
            }

            // If we've got an object within the array
            if (reader.TokenType == JsonTokenType.StartObject)
            {
                rowsGathered++;

                try
                {
                    // Attempt to parse the entire object.
                    JsonDocument doc = reader.ParseCurrentValue();  // Note: Do not dispose, because JsonElement holds a reference to it and needs to live on in the return value.
                    JsonElement firstObj = doc.RootElement;
                    // Infer schema from firstObj.
                    DataTable schemaTable = new DataTable(TableName);
                    foreach (JsonProperty property in firstObj.EnumerateObject())
                    {
                        if (!columnsOfData.TryGetValue(property.Name, out List<JsonElement> columnData))
                        {
                            columnData = new List<JsonElement>();
                            columnsOfData[property.Name] = columnData;
                        }

                        columnData.Add(property.Value);
                    }

                    continue;
                }
                catch (Exception)
                {
                    // Incomplete object; continue reading more bytes.
                    //TODO:  Do we halt completely??  Log this error or something else?
                }
            }

            // If we're at the end of the array
            if (reader.TokenType == JsonTokenType.EndArray)
            {
                //Nothing more to do.
                break;
            }

        }

        return columnsOfData;
    }

    private IDictionary<string, Type> GuessTypes(IDictionary<string, List<JsonElement>> columnsOfData)
    {
        Dictionary<string, Type> columnsType = new();

        //Keep in mind that JSON objects in an array can have different properties, therefore, we need to process them individually.
        foreach (var columnData in columnsOfData)
        {
            Type columnType = guessTypeFunction(columnData.Value);

            if (columnsType.ContainsKey(columnData.Key))
            {
                //TODO:  There are two properties that have the same name, but different casing?  Log this error or something else?
                continue;
            }

            columnsType[columnData.Key] = columnType;
        }

        return columnsType;
    }

    private void AddColumn(string columnName, Type columnType)
    {
        var existingColumnIndex = Columns.IndexOf(columnName);
        if (existingColumnIndex == -1)
        {
            if (columnType == null)
            {
                throw new ArgumentOutOfRangeException(nameof(columnType), $"Unable to AddColumn({columnName}) in JsonVirtualDataTable because {nameof(columnType)} is null");
            }

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


    private Type DefaultGuessTypeFunction(IEnumerable<JsonElement> values)
    {
        Type finalGuess = null;
        foreach(JsonElement value in values)
        {
            var rowGuessType = MapJsonValueKindToType(value);
            if (finalGuess == null) 
            {
                finalGuess = rowGuessType;
                continue;
            }

            finalGuess = CompatibleType(finalGuess, rowGuessType);
        }

        return finalGuess;
    }

    /// <summary>
    /// Determines the Type which is compatible for both a and b.
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    private static Type CompatibleType(Type a, Type b)
    {
        if (a == null)
            return b;
        if (b == null)
            return a;

        if (a == b)
            return a;

        if (a == typeof(string) || b == typeof(string))
            return typeof(string);

        if ((a == typeof(double) && b == typeof(bool)) ||
            (a == typeof(bool) && b == typeof(double)))
            return typeof(string);

        if (a == typeof(DateTime) || b == typeof(DateTime))
            return typeof(string);

        throw new InvalidOperationException($"Unexpected types to compare. a={a.FullName} b={b.FullName}");
    }

    private static Type MapJsonValueKindToType(JsonElement element) =>
        element.ValueKind switch
        {
            JsonValueKind.Number => typeof(double),
            JsonValueKind.True => typeof(bool),
            JsonValueKind.False => typeof(bool),
            JsonValueKind.String => DateTimeTryParse(element.GetString()) ? typeof(DateTime) : typeof(string),
            JsonValueKind.Null => null,
            _ => typeof(string)
        };
    
    private static bool DateTimeTryParse(string columnValue)
    {
        // Ensures that the string begins with a date format where components are separated by - / or .
        const string datePattern = @"^(?:\d{1,4}[-/.]\d{1,2}[-/.]\d{1,4}).*$";

        if (DateTime.TryParse(columnValue, out DateTime _) &&
            Regex.IsMatch(columnValue, datePattern))
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Returns an enumerable of DataRow objects by incrementally reading JSON objects from the stream.
    /// </summary>
    private IEnumerable<DataRow> EnumerateRows()
    {
        bool insideArray = false;
        var reader = new StreamJsonReader(stream, bufferSize);
        while (reader.Read())
        {
            if (!insideArray) 
            {
                // Since https://github.com/Servant-Software-LLC/FileBased.DataProviders/issues/95, we allow a single JSON object that isn't in an array.
                insideArray = reader.TokenType is JsonTokenType.StartArray or JsonTokenType.StartObject;

                if (reader.TokenType == JsonTokenType.StartArray)
                {
                    continue;
                } 
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
                        foreach (DataColumn dataColumn in Columns)
                        {
                            if (obj.TryGetProperty(dataColumn.ColumnName, out JsonElement prop))
                            {
                                row[dataColumn.ColumnName] = ConvertJsonElement(prop, dataColumn.DataType);
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
        try
        {
            if (element.ValueKind == JsonValueKind.Null)
                return DBNull.Value;

            if (targetType == typeof(string))
                return element.ToString();
            if (targetType == typeof(double))
                return element.TryGetDouble(out double d) ? d : 0.0;
            if (targetType == typeof(bool))
                return element.GetBoolean();
            if (targetType == typeof(DateTime))
                return DateTime.SpecifyKind(DateTime.Parse(element.GetString()), DateTimeKind.Utc);

            return element.ToString();
        }
        catch (Exception ex)
        {
            throw new InvalidDataException($"Unable to get the value of JsonElement of type {element.GetType()} with a kind of {element.ValueKind}.  TargetType was {targetType}", ex);
        }
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
