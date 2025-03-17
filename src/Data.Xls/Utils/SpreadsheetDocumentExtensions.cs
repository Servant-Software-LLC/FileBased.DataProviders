using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace Data.Xls.Utils;

public static class SpreadsheetDocumentExtensions
{
    public static string GetCellValue(this SpreadsheetDocument doc, Cell cell)
    {
        if (cell == null) return "";
        string value = cell.InnerText;

        if (cell.DataType != null && cell.DataType.Value == CellValues.SharedString)
        {
            var stringTable = doc.WorkbookPart.SharedStringTablePart.SharedStringTable;
            return stringTable.ChildElements[int.Parse(value)].InnerText;
        }

        return value;
    }
}