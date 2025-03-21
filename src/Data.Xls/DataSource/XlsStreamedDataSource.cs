using Data.Common.DataSource;
using Data.Xls.Utils;

namespace Data.Xls.DataSource;

public class XlsStreamedDataSource : StreamedDataSource, IDisposable
{
    private readonly XlsDatabaseStreamSplitter splitter;

    public XlsStreamedDataSource(string database, Stream xlsStream)
        : base(Path.GetFileNameWithoutExtension(database))  // Needs to look like a folder to get the proper database name in this workaround.
    {
        splitter = new(xlsStream);

        foreach(KeyValuePair<string, Stream> tableStreams in splitter.GetTableStreams())
        {
            tables.AddTable(tableStreams.Key, () => tableStreams.Value);
        }
    }

    void IDisposable.Dispose()
    {
        splitter.Dispose();
        base.Dispose();
    }
}
