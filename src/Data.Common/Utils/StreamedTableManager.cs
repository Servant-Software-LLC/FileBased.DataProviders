namespace Data.Common.Utils;

public class StreamedTableManager : IDisposable
{
    private const int streamResetsAllowed = 4;
    private readonly Dictionary<string, StreamHolder> tables = new();

    public void AddTable(string tableName, Func<Stream> streamCreationFunc)
    {
        if (string.IsNullOrEmpty(tableName))
            throw new ArgumentNullException(nameof(tableName));

        if (streamCreationFunc is null)
            throw new ArgumentNullException(nameof(streamCreationFunc));

        tables[tableName] = new StreamHolder() { StreamCreationFunc = streamCreationFunc };
    }

    /// <summary>
    /// Checks if storage exists for the specified table.
    /// </summary>
    /// <param name="tableName">The name of the table to check.</param>
    /// <returns>True if the table exists in the data source; otherwise, false.</returns>
    public bool StorageExists(string tableName) => tables.ContainsKey(tableName);

    public IEnumerable<string> GetTableNames() => tables.Keys;

    public BufferedResetStream GetReadingStream(string tableName)
    {
        if (tables.TryGetValue(tableName, out StreamHolder streamHolder))
        {
            if (streamHolder.LatestWriteStream != null)
            {
                streamHolder.LatestWriteStream.Seek(0, SeekOrigin.Begin);

                // This was a write stream.  Wrap it and return it.
                return new BufferedResetStream(streamHolder.LatestWriteStream, streamResetsAllowed);
            }

            var stream = streamHolder.StreamCreationFunc();

            //Move to the beginning of the stream.
            stream.Seek(0, SeekOrigin.Begin);

            var bufferedStream = new BufferedResetStream(stream, streamResetsAllowed);
            streamHolder.LatestReadStream = bufferedStream;

            return bufferedStream;
        }

        throw new FileNotFoundException($"No table stream for {tableName}.");
    }

    public MemoryStream GetWritingStream(string tableName)
    {
        if (tables.TryGetValue(tableName, out StreamHolder streamHolder))
        {
            //If there is already an existing stream, dispose of it.  It may be locking up some resource.
            streamHolder.LatestReadStream?.Close();
            streamHolder.LatestReadStream?.Dispose();
            tables.Remove(tableName);
        }

        // Once we've created a write stream, we aren't going to create read streams anymore
        streamHolder = new();
        tables.Add(tableName, streamHolder);

        MemoryStream memoryStream = new();
        streamHolder.LatestWriteStream = memoryStream;
        return memoryStream;
    }

    public void Dispose()
    {
        foreach (StreamHolder streamHolder in tables.Values)
        {
            streamHolder.LatestReadStream?.Close();
            streamHolder.LatestReadStream?.Dispose();
            streamHolder.LatestWriteStream?.Close();
            streamHolder.LatestWriteStream?.Dispose();
        }
    }
}
