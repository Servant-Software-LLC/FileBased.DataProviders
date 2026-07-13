using Data.Xls.Utils;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Xunit;

namespace Data.Xls.Tests.Utils;

public class WorkbookExtensionsTests
{
    [Fact]
    public void GetCellValue_ReturnsKanjiText_WhenSharedStringHasNoPhoneticAnnotation()
    {
        using var ms = new MemoryStream();
        using var document = SpreadsheetDocument.Create(ms, SpreadsheetDocumentType.Workbook);
        WorkbookPart workbookPart = GivenWorkbookWithSharedString(document,
            new SharedStringItem(new Text("株式会社ホープス")));
        Cell cell = GivenSharedStringCell(index: 0);

        string result = workbookPart.GetCellValue(cell);

        ThenResultEquals(result, "株式会社ホープス");
    }

    [Fact]
    public void GetCellValue_ExcludesPhoneticAnnotation_WhenSharedStringHasRphElement_RegressionForTOSCA52141()
    {
        using var ms = new MemoryStream();
        using var document = SpreadsheetDocument.Create(ms, SpreadsheetDocumentType.Workbook);
        WorkbookPart workbookPart = GivenWorkbookWithPhoneticSharedString(document,
            kanjiText: "株式会社ホープス",
            phoneticText: "カブシキガイシャ");
        Cell cell = GivenSharedStringCell(index: 0);

        string result = workbookPart.GetCellValue(cell);

        ThenResultEquals(result, "株式会社ホープス");
    }

    [Fact]
    public void GetCellValue_ConcatenatesRunsAndExcludesPhoneticAnnotation_WhenSharedStringUsesMultipleRuns_RegressionForTOSCA52141()
    {
        using var ms = new MemoryStream();
        using var document = SpreadsheetDocument.Create(ms, SpreadsheetDocumentType.Workbook);
        WorkbookPart workbookPart = GivenWorkbookWithMultiRunPhoneticSharedString(document,
            runTexts: new[] { "株式", "会社ホープス" },
            phoneticText: "カブシキガイシャホープス");
        Cell cell = GivenSharedStringCell(index: 0);

        string result = workbookPart.GetCellValue(cell);

        ThenResultEquals(result, "株式会社ホープス");
    }

    private static WorkbookPart GivenWorkbookWithSharedString(SpreadsheetDocument document, SharedStringItem item)
    {
        var workbookPart = document.AddWorkbookPart();
        var sharedStringPart = workbookPart.AddNewPart<SharedStringTablePart>();
        var table = new SharedStringTable();
        table.AppendChild(item);
        sharedStringPart.SharedStringTable = table;
        return workbookPart;
    }

    private static WorkbookPart GivenWorkbookWithPhoneticSharedString(
        SpreadsheetDocument document, string kanjiText, string phoneticText)
    {
        var item = new SharedStringItem();
        item.AppendChild(new Text(kanjiText));
        var phoneticRun = new PhoneticRun { BaseTextStartIndex = 0, EndingBaseIndex = (uint)kanjiText.Length };
        phoneticRun.AppendChild(new Text(phoneticText));
        item.AppendChild(phoneticRun);
        return GivenWorkbookWithSharedString(document, item);
    }

    private static WorkbookPart GivenWorkbookWithMultiRunPhoneticSharedString(
        SpreadsheetDocument document, string[] runTexts, string phoneticText)
    {
        var item = new SharedStringItem();
        foreach (var runText in runTexts)
        {
            var run = new Run();
            run.AppendChild(new Text(runText));
            item.AppendChild(run);
        }
        var phoneticRun = new PhoneticRun { BaseTextStartIndex = 0, EndingBaseIndex = (uint)string.Concat(runTexts).Length };
        phoneticRun.AppendChild(new Text(phoneticText));
        item.AppendChild(phoneticRun);
        return GivenWorkbookWithSharedString(document, item);
    }

    private static Cell GivenSharedStringCell(int index)
    {
        return new Cell
        {
            DataType = CellValues.SharedString,
            CellValue = new CellValue(index.ToString())
        };
    }

    private static void ThenResultEquals(string actual, string expected)
    {
        Assert.Equal(expected, actual);
    }
}
