using DocumentFormat.OpenXml.Spreadsheet;

namespace Data.Xls.Utils;

public static class CellExtensions
{
    public static int GetColumnIndex(this Cell cell)
    {
        var cellRef = cell.CellReference?.Value;
        
        if (string.IsNullOrEmpty(cellRef))
            return -1; // Invalid reference

        var columnRef = new string(cellRef.TakeWhile(char.IsLetter).ToArray()); // Extract letters

        return ColumnLetterToIndex(columnRef);
    }

    private static int ColumnLetterToIndex(string column)
    {
        int index = 0;

        foreach (char c in column)
        {
            index = index * 26 + (c - 'A' + 1);
        }

        return index - 1; // Zero-based index
    }
}