using Microsoft.Data.Analysis;
using Data.Common.DataSource;

namespace Data.Csv.Utils;

internal static class CsvDataFrameLoader
{
    public static DataFrame LoadDataFrame(IDataSourceProvider dataSourceProvider, string tableName, long numberOfLines)
    {
        using var streamReader = dataSourceProvider.GetTextReader(tableName);
        var stream = streamReader.BaseStream;

        IEnumerable<Type> types = DetermineDataTypes(numberOfLines, streamReader);

        //Reset the stream to the beginning because the DataFrame only uses X number of lines to determine column types.
        stream.Seek(0, SeekOrigin.Begin);

        // Need to ensure that each line of CSV text has the proper number of commas (like its header)
        using CsvTransformStream transformStream = new CsvTransformStream(streamReader);
        return DataFrame.LoadCsv(transformStream, dataTypes: types.ToArray());
    }

    private static IEnumerable<Type> DetermineDataTypes(long numberOfLines, StreamReader streamReader)
    {
        // Need to ensure that each line of CSV text has the proper number of commas (like its header)
        using (CsvTransformStream transformStream = new CsvTransformStream(streamReader))
        {
            //Let DataFrame determine the column data types.
            var originalDataFrame = DataFrame.LoadCsv(transformStream, numberOfRowsToRead: numberOfLines);

            //If the CSV doesn't have data (but it has column names), the DataFrame shows no columns.  
            if (originalDataFrame.Columns.Count == 0)
            {
                if (!string.IsNullOrEmpty(transformStream.HeaderLine))
                {
                    var columnNames = transformStream.HeaderLine.Split(',');
                    return columnNames.Select(name => typeof(string));
                }
            }


            return originalDataFrame.Columns.Select(column => column.DataType);
        }
    }
}