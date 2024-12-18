using Microsoft.Data.Analysis;
using Data.Common.DataSource;
using Data.Common.Utils.ConnectionString;

namespace Data.Csv.Utils;

internal static class CsvDataFrameLoader
{
    public static DataFrame LoadDataFrame(IDataSourceProvider dataSourceProvider, string tableName, int guessRows, FloatingPointDataType preferredFloatingPointDataType)
    {
        using var streamReader = dataSourceProvider.GetTextReader(tableName);

        // Need to ensure that each line of CSV text has the proper number of commas (like its header)
        using CsvTransformStream transformStream = new CsvTransformStream(streamReader);

        // NOTE: The DataFrame, when guessing at data types, only pulls in floating point numbers with a precision of 'float'.  Therefore, if the caller wants a higher
        //       precision, then we need to 
        var numberOfRowsToRead = preferredFloatingPointDataType == FloatingPointDataType.Float ? -1 : guessRows;
        var dataFrame = DataFrame.LoadCsv(transformStream, numberOfRowsToRead: numberOfRowsToRead, guessRows: guessRows);

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

        //If the precision is just floating point, then we're done.
        if (preferredFloatingPointDataType == FloatingPointDataType.Float)
            return dataFrame;

        //Get a list of the column data types.
        var columnTypes = dataFrame.Columns.Select(column => preferredFloatingPointDataType.GetClrType(column.DataType)).ToArray();

        transformStream.Seek(0, SeekOrigin.Begin);
        var secondPassDataFrame = DataFrame.LoadCsv(transformStream, dataTypes: columnTypes);
        return secondPassDataFrame;
    }
}
