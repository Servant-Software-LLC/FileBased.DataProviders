using Microsoft.Data.Analysis;
using Data.Common.DataSource;
                                                            
namespace Data.Csv.Utils;

internal static class CsvDataFrameLoader
{
    public static DataFrame LoadDataFrame(IDataSourceProvider dataSourceProvider, string tableName, int numberOfLines)
    {
        using var streamReader = dataSourceProvider.GetTextReader(tableName);
        var stream = streamReader.BaseStream;

        // Need to ensure that each line of CSV text has the proper number of commas (like its header)
        using CsvTransformStream transformStream = new CsvTransformStream(streamReader);
        var dataFrame = DataFrame.LoadCsv(transformStream, guessRows: numberOfLines);

        //If the CSV doesn't have data (but it has column names), the DataFrame shows no columns.  
        if (dataFrame.Columns.Count == 0)
        {
            if (!string.IsNullOrEmpty(transformStream.HeaderLine))
            {
                var columnNames = transformStream.HeaderLine.Split(',');
                var columns = columnNames.Select<string, DataFrameColumn>(name => new StringDataFrameColumn(name, 0));

                return new DataFrame(columns);
            }
        }

        return dataFrame;
    }
}