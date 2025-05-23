﻿using Data.Common.Extension;

namespace Data.Tests.Common.Utils;

public class DatabaseFullPaths
{
    private string extension;

    public DatabaseFullPaths(string extension)
    {
        this.extension = !string.IsNullOrEmpty(extension) ? extension : throw new ArgumentNullException(nameof(extension));
    }

    public string DatabaseFileName => "database";
    public string Folder => Path.Combine(FileConnectionStringTestsExtensions.SourcesFolder, "Folder");
    public string WithDateTimeFolder => Path.Combine(FileConnectionStringTestsExtensions.SourcesFolder, "WithDateTime");
    public string LargeFolder => Path.Combine(FileConnectionStringTestsExtensions.SourcesFolder, "LargeFolder");
    public string File => Path.Combine(FileConnectionStringTestsExtensions.SourcesFolder, $"{DatabaseFileName}.{extension}");
    public string WithDateTime => Path.Combine(FileConnectionStringTestsExtensions.SourcesFolder, $"withDateTime.{extension}");
    public string WithFormula => Path.Combine(FileConnectionStringTestsExtensions.SourcesFolder, $"WithFormula.{extension}");
    public string EmptyCells => Path.Combine(FileConnectionStringTestsExtensions.SourcesFolder, $"emptyCells.{extension}");
    public string CellsWithComma => Path.Combine(FileConnectionStringTestsExtensions.SourcesFolder, $"cellsWithComma.{extension}");
    public string eComFileDataBase => Path.Combine(FileConnectionStringTestsExtensions.SourcesFolder, $"ecommerce.{extension}");
    public string eComFolderDataBase => Path.Combine(FileConnectionStringTestsExtensions.SourcesFolder, $"eCom");
    public string FolderEmptyWithTables => Path.Combine(FileConnectionStringTestsExtensions.SourcesFolder, "EmptyDatabase");
    public string FileEmptyWithTables => Path.Combine(FileConnectionStringTestsExtensions.SourcesFolder, $"emptyDatabase.{extension}");
    public string gettingStartedFileDataBase => Path.Combine(FileConnectionStringTestsExtensions.SourcesFolder, $"GettingStarted.{extension}");
    public string gettingStartedFolderDataBase => Path.Combine(FileConnectionStringTestsExtensions.SourcesFolder, $"GettingStarted");
    public string bogusFileDataBase => Path.Combine(FileConnectionStringTestsExtensions.SourcesFolder, $"Folder_BOGUS.{Guid.NewGuid()}.{extension}");
    public string bogusFolderDataBase => Path.Combine(FileConnectionStringTestsExtensions.SourcesFolder, $"Database_BOGUS.{Guid.NewGuid()}");
    public string withTrailingCommaFolderDataBase => Path.Combine(FileConnectionStringTestsExtensions.SourcesFolder, $"WithTrailingComma");

}
