using CsvHelper.Configuration;
using CsvHelper;
using Microsoft.Data.Analysis;
using System.Globalization;
using System.Text;

namespace Data.Csv.Utils;

internal static class CsvDataFrameLoader
{
    public static DataFrame LoadDataFrameWithQuotedFields(string filePath, long numberOfLines)
    {
        // Step 1: Read the first N lines
        var records = ReadFirstNLines(filePath, numberOfLines);

        if (records.Count == 1)
        {
            if (records[0] is string[] columnNames)
            {
                return UnableToDetermineColumnTypes(columnNames);
            }
        }

        // Step 2: Write records to CSV string with quoted fields
        var csvContent = WriteRecordsWithQuotedFields(records);

        // Step 3: Load DataFrame from CSV string
        var df = LoadDataFrameFromCsvString(csvContent);

        return df;
    }

    private static DataFrame UnableToDetermineColumnTypes(string[] columnNames)
    {
        DataFrame df = new DataFrame();

        // Loop through the column names and add StringDataFrameColumns to the DataFrame
        foreach (string columnName in columnNames)
        {
            // Create a new column with string data type
            var column = new StringDataFrameColumn(columnName);
            df.Columns.Add(column);
        }

        return df;
    }

    private static List<dynamic> ReadFirstNLines(string filePath, long numberOfLines)
    {
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            MissingFieldFound = null,
            BadDataFound = null,
            IgnoreBlankLines = true,
            TrimOptions = TrimOptions.Trim,
        };

        using (var reader = new StreamReader(filePath))
        using (var csv = new CsvHelper.CsvReader(reader, config))
        {
            var records = new List<dynamic>();

            // Read the header
            if (!csv.Read() || !csv.ReadHeader())
                throw new InvalidOperationException("CSV file does not contain a valid header.");

            records.Add(csv.HeaderRecord); // Add header as the first record

            // Read the next N-1 lines
            int linesRead = 0;
            while (csv.Read())
            {
                var record = csv.Parser.Record; // Read the current record as an array
                records.Add((string[])record.Clone());
                linesRead++;
                if (linesRead >= numberOfLines - 1) // Subtract 1 because we've already read the header
                    break;
            }

            return records;
        }
    }

    private static string WriteRecordsWithQuotedFields(List<dynamic> records)
    {
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            ShouldQuote = args => (args.Row.Row > 1), // Quote all data fields, skipping the header
        };

        var stringBuilder = new StringBuilder();

        int recordsInHeader = ((string[])records[0]).Length;
        using (var writer = new StringWriter(stringBuilder))
        using (var csv = new CsvWriter(writer, config))
        {
            // Enumerate the records
            foreach (var record in records)
            {
                if (record is not string[])
                {
                    throw new Exception("Invalid record type.");
                }

                foreach (var field in (string[])record)
                {
                    csv.WriteField(field);
                }

                // Write empty fields for missing fields
                int missingFields = recordsInHeader - ((string[])record).Length;
                for (int i = 0; i < missingFields; i++)
                {
                    csv.WriteField(string.Empty);
                }

                csv.NextRecord();
            }
        }

        return stringBuilder.ToString();
    }

    private static DataFrame LoadDataFrameFromCsvString(string csvContent)
    {
        var df = DataFrame.LoadCsvFromString(csvContent);
        return df;
    }
}