using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace Data.Xls.Utils;

public static class XslxReader
{
    public static IDictionary<string, List<string>> GetColumnValues(Stream stream, int numberOfRows)
    {
        Dictionary<string, List<string>> columnsOfData = new();

        using (SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Open(stream, false))
        {
            WorksheetPart worksheetPart = spreadsheetDocument.WorkbookPart!.WorksheetParts.First();
            using (OpenXmlReader reader = OpenXmlReader.Create(worksheetPart))
            {
                bool headerRowRead = false;
                List<string> headers = new();
                int rowsRead = 0;

                while (reader.Read())
                {
                    if (reader.ElementType == typeof(Row) && reader.IsStartElement)
                    {
                        Row row = (Row)reader.LoadCurrentElement()!;

                        if (!headerRowRead)
                        {
                            // Read the first row as headers
                            foreach (Cell cell in row.Elements<Cell>())
                            {
                                string header = spreadsheetDocument.WorkbookPart.GetCellValue(cell);
                                headers.Add(header);
                                columnsOfData[header] = new List<string>();
                            }

                            headerRowRead = true;
                        }
                        else if (rowsRead < numberOfRows)
                        {
                            int columnIndex = 0;

                            foreach (Cell cell in row.Elements<Cell>())
                            {
                                if (columnIndex < headers.Count)
                                {
                                    string value = spreadsheetDocument.WorkbookPart.GetCellValue(cell);
                                    columnsOfData[headers[columnIndex]].Add(value);
                                }

                                columnIndex++;
                            }

                            rowsRead++;
                        }

                        if (rowsRead >= numberOfRows)
                        {
                            break;
                        }
                    }
                }
            }
        }

        // Reset stream if needed
        if (stream.CanSeek)
        {
            stream.Seek(0, SeekOrigin.Begin);
        }

        return columnsOfData;
    }
    
    public static IEnumerable<DataRow> GetRowsIterator(Stream stream, XlsVirtualDataTable table)
    {
        using (SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Open(stream, false))
        {
            var workbookPart = spreadsheetDocument.WorkbookPart!;
            var worksheetPart = workbookPart.WorksheetParts.First();

            using (var reader = OpenXmlReader.Create(worksheetPart))
            {
                bool headerSkipped = false;
                while (reader.Read())
                {
                    if (reader.ElementType == typeof(Row))
                    {
                        Row row = (Row)reader.LoadCurrentElement();

                        if (!headerSkipped)
                        {
                            headerSkipped = true;
                            continue; // Skip the first row (header)
                        }

                        var dataRow = table.NewRow();
                        
                        int columnIndex = 0;
                        foreach (Cell cell in row.Elements<Cell>())
                        {
                            string? value = spreadsheetDocument.WorkbookPart.GetCellValue(cell);
                            dataRow[table.Columns[columnIndex++].ColumnName] = value;
                        }

                        yield return dataRow;
                    }
                }
            }
        }
    }
}