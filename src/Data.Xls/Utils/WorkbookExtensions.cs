using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace Data.Xls.Utils;

public static class WorkbookExtensions
{
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

        return value;
    }
}