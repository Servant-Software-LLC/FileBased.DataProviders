using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

public static class ExcelHelper
{
    public static MemoryStream CreateExcelStream(List<string> headers, List<Dictionary<string, object>> rows)
    {
        var memoryStream = new MemoryStream();
        using (SpreadsheetDocument document =
               SpreadsheetDocument.Create(memoryStream, SpreadsheetDocumentType.Workbook, true))
        {
            WorkbookPart workbookPart = document.AddWorkbookPart();
            workbookPart.Workbook = new Workbook();
            WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
            SheetData sheetData = new SheetData();

            sheetData.Append(CreateHeaderRow(headers));
            sheetData.Append(CreateDataRows(headers, rows));
            workbookPart.AppendSheet(worksheetPart, sheetData);
            workbookPart.Workbook.Save();
        }

        memoryStream.Seek(0, SeekOrigin.Begin);
        return memoryStream;
    }

    private static void AppendSheet(this WorkbookPart workbookPart, WorksheetPart worksheetPart, SheetData sheetData)
    {
        worksheetPart.Worksheet = new Worksheet(sheetData);
        Sheets sheets = workbookPart.Workbook.AppendChild(new Sheets());
        sheets.Append(new Sheet()
        {
            Id = workbookPart.GetIdOfPart(worksheetPart),
            SheetId = 1,
            Name = "Sheet1"
        });
    }

    private static List<Row> CreateDataRows(List<string> headers, List<Dictionary<string, object>> rows)
    {
        var newRows = new System.Collections.Generic.List<Row>();
        uint rowIndex = 2;
        foreach (var row in rows)
        {
            var dataRow = new Row { RowIndex = rowIndex };
            for (int colIndex = 0; colIndex < headers.Count; colIndex++)
            {
                var colLetter = GetExcelColumnName(colIndex + 1);
                var cellRef = $"{colLetter}{rowIndex}";
                var cellValue = row.TryGetValue(headers[colIndex], out var val) ? val : string.Empty;
                var (dataType, value) = GetCellTypeAndValue(cellValue);

                dataRow.Append(new Cell
                {
                    CellReference = cellRef,
                    DataType = dataType,
                    CellValue = new CellValue(value)
                });
            }

            newRows.Add(dataRow);
            rowIndex++;
        }

        return newRows;
    }

    private static Row CreateHeaderRow(List<string> headers)
    {
        var headerRow = new Row { RowIndex = 1 };
        for (int colIndex = 0; colIndex < headers.Count; colIndex++)
        {
            var colLetter = GetExcelColumnName(colIndex + 1);
            var cellRef = $"{colLetter}1";

            headerRow.Append(new Cell
            {
                CellReference = cellRef,
                DataType = CellValues.String,
                CellValue = new CellValue(headers[colIndex])
            });
        }

        return headerRow;
    }

    private static Cell CreateTextCell(string text)
    {
        return new Cell
        {
            DataType = CellValues.String,
            CellValue = new CellValue(text)
        };
    }

    private static string GetExcelColumnName(int index)
    {
        const string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        string columnName = string.Empty;

        while (index > 0)
        {
            index--;
            columnName = letters[index % 26] + columnName;
            index /= 26;
        }

        return columnName;
    }

    private static (EnumValue<CellValues>? dataType, string value) GetCellTypeAndValue(object cellValue)
    {
        if (cellValue is null)
        {
            return (CellValues.String, string.Empty);
        }

        if (cellValue is string str)
        {
            return (CellValues.String, str);
        }

        if (cellValue is bool boolean)
        {
            return (CellValues.Boolean, boolean ? "1" : "0");
        }

        if (cellValue is DateTime dateTime)
        {
            // Excel stores dates as numbers (OADate)
            return (null, dateTime.ToOADate().ToString(System.Globalization.CultureInfo.InvariantCulture));
        }

        if (cellValue is int or double or decimal or float)
        {
            return (null, Convert.ToString(cellValue, System.Globalization.CultureInfo.InvariantCulture));
        }

        // Fallback to string
        return (CellValues.String, cellValue.ToString()!);
    }
}