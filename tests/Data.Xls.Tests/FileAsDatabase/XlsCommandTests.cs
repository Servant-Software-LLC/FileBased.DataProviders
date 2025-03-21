using Data.Tests.Common;
using Data.Common.Extension;
using Data.Tests.Common.Utils;
using System.Data.XlsClient;
using System.Reflection;
using Xunit;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml;

namespace Data.Xls.Tests.FileAsDatabase;

/// <summary>
/// Tests that exercise the <see cref="XlsCommand"/> class while using the 'File as Database' approach.
/// </summary>
public class XlsCommandTests
{
    [Fact]
    public void ExecuteScalar_ShouldReturnFirstRowFirstColumn()
    {
        CommandTests.ExecuteScalar_ShouldReturnFirstRowFirstColumn(
          () => new XlsConnection(ConnectionStrings.Instance.FileAsDB));
    }

    [Fact]
    public void ExecuteScalar_ShouldCountRecords()
    {
        CommandTests.ExecuteScalar_ShouldCountRecords(
           () => new XlsConnection(ConnectionStrings.Instance.FileAsDB));
    }

    [Fact]
    public void ExecuteScalar_ShouldCountSchemaTableRecords()
    {
        CommandTests.ExecuteScalar_ShouldCountSchemaTableRecords(
           () => new XlsConnection(ConnectionStrings.Instance.
           FileAsDB));
    }

    [Fact]
    public void ExecuteNonQuery_CreateDatabase_WithExisting()
    {
        var tempFolder = FileUtils.GetTempFolderName();
        var databaseName = Path.Combine(tempFolder, "MyDatabase.xls");
        CreateEmptyXlsx(databaseName);

        CommandTests.ExecuteNonQuery_Admin_CreateDatabase(getConnectionString =>
        {
            var connectionString = getConnectionString(ConnectionStrings.Instance);
            return new XlsConnection(connectionString);
        }, databaseName, 0);
    }

    [Fact]
    public void ExecuteNonQuery_CreateDatabase_NewDatabase()
    {
        var tempFolder = FileUtils.GetTempFolderName();
        var databaseName = Path.Combine(tempFolder, "MyDatabase.xls");

        CommandTests.ExecuteNonQuery_Admin_CreateDatabase(getConnectionString =>
        {
            var connectionString = getConnectionString(ConnectionStrings.Instance);
            return new XlsConnection(connectionString);
        }, databaseName, 1);
    }

    [Fact]
    public void ExecuteNonQuery_DropDatabase_WithExisting()
    {
        var tempFolder = FileUtils.GetTempFolderName();
        var databaseName = Path.Combine(tempFolder, "MyDatabase.xls");
        var fileStream = File.Create(databaseName);
        fileStream.Close();

        CommandTests.ExecuteNonQuery_Admin_DropDatabase(getConnectionString =>
        {
            var connectionString = getConnectionString(ConnectionStrings.Instance);
            return new XlsConnection(connectionString);
        }, databaseName, 1);
    }

    [Fact]
    public void ExecuteNonQuery_DropDatabase_NewDatabase()
    {
        var tempFolder = FileUtils.GetTempFolderName();
        var databaseName = Path.Combine(tempFolder, "MyDatabase.xls");

        CommandTests.ExecuteNonQuery_Admin_DropDatabase(getConnectionString =>
        {
            var connectionString = getConnectionString(ConnectionStrings.Instance);
            return new XlsConnection(connectionString);
        }, databaseName, 0);
    }

    [Fact (Skip = "XSLX table create not implemented")]
    public void ExecuteNonQuery_CreateTable()
    {
        var sandboxId = $"{GetType().FullName}.{MethodBase.GetCurrentMethod()!.Name}";
        CommandTests.ExecuteNonQuery_CreateTable(
          () => new XlsConnection(ConnectionStrings.Instance.FileAsDB.Sandbox("Sandbox", sandboxId)));
    }

    private static void CreateEmptyXlsx(string filePath)
    {
        // Create a new SpreadsheetDocument (XLSX) in read/write mode.
        using (SpreadsheetDocument document = SpreadsheetDocument.Create(filePath, SpreadsheetDocumentType.Workbook))
        {
            // Add a WorkbookPart to the document.
            WorkbookPart workbookPart = document.AddWorkbookPart();
            workbookPart.Workbook = new Workbook();

            // Add a WorksheetPart to the WorkbookPart.
            WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
            // Create an empty worksheet with a SheetData element.
            worksheetPart.Worksheet = new Worksheet(new SheetData());

            // Create the Sheets collection and add it to the workbook.
            Sheets sheets = document.WorkbookPart!.Workbook.AppendChild(new Sheets());

            // Append a new Sheet to the Sheets collection.
            Sheet sheet = new Sheet()
            {
                Id = document.WorkbookPart.GetIdOfPart(worksheetPart),
                SheetId = 1,
                Name = "Sheet1"
            };
            sheets.Append(sheet);

            // Save the workbook.
            workbookPart.Workbook.Save();
        }
    }

}