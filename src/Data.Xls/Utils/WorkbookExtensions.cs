using System.Globalization;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace Data.Xls.Utils;

public static class WorkbookExtensions
{
    private const uint CustomDateNumberFormat = 164;
    private const uint MinDateNumberFormat = 14;
    private const uint MaxDateNumberFormat = 22;

    public static string GetCellValue(this WorkbookPart workbook, Cell cell)
    {
        if (cell == null) return "";
        string value = cell.InnerText;

        if (cell.DataType != null && cell.DataType.Value == CellValues.SharedString)
        {
            var stringTable = workbook.SharedStringTablePart.SharedStringTable;
            return stringTable.ChildElements[int.Parse(value)].InnerText;
        }

        if (cell.DataType != null && cell.DataType.Value == CellValues.Boolean)
        {
            return value == "1" ? "true" : "false";
        }

        // Handle potential Date (when DataType is null)
        if (TryGetDateTimeValue(workbook, cell, out var dateTimeValue))
            return dateTimeValue;

        return value;
    }

    private static bool TryGetDateTimeValue(WorkbookPart workbook, Cell cell, out string dateTimeValue)
    {
        dateTimeValue = cell.InnerText;

        if (cell.DataType != null || cell.StyleIndex == null) 
            return false;
        
        var styles = workbook.WorkbookStylesPart.Stylesheet;
        var cellFormat = styles.CellFormats.ElementAt((int)cell.StyleIndex.Value) as CellFormat;

        if (cellFormat != null)
        {
            var numberFormatId = cellFormat.NumberFormatId?.Value ?? 0;
            
            if (numberFormatId is (>= MinDateNumberFormat and <= MaxDateNumberFormat) or CustomDateNumberFormat)
            {
                if (double.TryParse(cell.InnerText, out double oadate))
                {
                    DateTime dateValue = DateTime.FromOADate(oadate);
                    dateTimeValue = dateValue.ToString(CultureInfo.InvariantCulture);
                    return true;
                }
            }
        }

        return false;
    }
}