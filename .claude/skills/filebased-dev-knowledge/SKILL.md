---
name: filebased-dev-knowledge
description: |
  Developer-specific knowledge for working in the FileBased.DataProviders codebase. Use when implementing
  features, fixing bugs, adding format providers, writing tests, or navigating the project structure.
  Covers: solution layout, ADO.NET provider patterns, EF Core integration, data source abstraction,
  file I/O pipeline, connection strings, testing patterns, and practical workflows.

  SELF-UPDATING: When your work changes, advances, or extends FileBased.DataProviders in ways that affect
  this knowledge (new projects, providers, DI registrations, testing patterns, build steps, etc.), you
  MUST update this skill to reflect the new state before completing your task. This keeps the knowledge
  accurate for future agents. Update the specific section(s) affected -- do not rewrite unchanged content.
---

# FileBased.DataProviders Developer Knowledge

## Solution Layout

```
src/
  Data.Common/              -- Base classes & interfaces for all file-based ADO.NET providers
  Data.Json/                -- JSON ADO.NET provider (folder-as-DB & file-as-DB modes)
  Data.Csv/                 -- CSV ADO.NET provider (folder-as-DB, RFC 4180 compliant)
  Data.Xml/                 -- XML ADO.NET provider (folder-as-DB & file-as-DB, XSD typed)
  Data.Xls/                 -- Excel ADO.NET provider (read-only, OpenXml-based)
  EFCore.Common/            -- Shared EF Core infrastructure for file-based providers
  EFCore.Json/              -- EF Core JSON provider (alpha)
  EFCore.Csv/               -- EF Core CSV provider (alpha)
  EFCore.Xml/               -- EF Core XML provider (alpha)

tests/
  Data.Tests.Common/        -- Shared test utilities, POCOs, assertion extensions
  Data.Json.Tests/          -- JSON provider tests (includes Dapper integration)
  Data.Csv.Tests/           -- CSV provider tests
  Data.Xml.Tests/           -- XML provider tests
  Data.Xls.Tests/           -- Excel provider tests
  EFCore.Common.Tests/      -- EF Core shared tests
  EFCore.Json.Tests/        -- EF Core JSON tests
  EFCore.Csv.Tests/         -- EF Core CSV tests
  EFCore.Xml.Tests/         -- EF Core XML tests
```

## Build & SDK

- **SDK**: .NET 7.0 (see global.json) with MSBuild Traversal 3.0.2
- **ADO.NET TFMs**: net8.0 + netstandard2.0 (multi-target)
- **EF Core TFMs**: net8.0 only
- **Test TFMs**: net8.0
- **Language**: C# 11 with nullable enabled
- **Package pins**: Packages.props centralizes versions (SqlBuildingBlocks 1.0.0.130, CsvHelper 33.0.1, EF Core 6.0.36, DocumentFormat.OpenXml 3.3.0)

### Running Tests
```powershell
# Build
dotnet build --configuration Release

# All tests
dotnet test --configuration Release
```

## ADO.NET Provider Architecture

### Class Hierarchy

Each format provider follows the same pattern:

```
FileConnection<TFileParameter> : DbConnection
  ├─ JsonConnection, CsvConnection, XmlConnection, XlsConnection
FileCommand<TFileParameter> : DbCommand
  ├─ JsonCommand, CsvCommand, XmlCommand, XlsCommand
FileDataReader : DbDataReader
  ├─ JsonDataReader, CsvDataReader, XmlDataReader, XlsDataReader
FileTransaction<TFileParameter> : DbTransaction
FileParameter<TFileParameter> : DbParameter
FileParameterCollection<TFileParameter> : DbParameterCollection
FileDataAdapter : DbDataAdapter
```

### Adding a New Format Provider

1. **Create `Data.[Format]/` project** targeting net8.0;netstandard2.0
2. **Implement ADO.NET classes**: `[Format]Connection`, `[Format]Command`, `[Format]DataReader`, `[Format]Transaction`, `[Format]Parameter`, `[Format]ParameterCollection`
3. **Implement FileReader subclass** in `[Format]IO/Read/` -- override `ReadFromFolder()` and `ReadFromFile()`
4. **Implement FileWriter subclass** in `[Format]IO/` -- handle INSERT/UPDATE/DELETE
5. **Implement IDataSetWriter** for serialization back to format
6. **Add tests** in `tests/Data.[Format].Tests/` mirroring existing test structure
7. **Optionally add EFCore provider** in `src/EFCore.[Format]/`

### Connection String Keywords

| Keyword | Aliases | Purpose |
|---------|---------|---------|
| `DataSource` | `Data Source` | Path to file or folder (required) |
| `LogLevel` | | Serilog log level (default: None) |
| `Formatted` | | Pretty-print output (boolean) |
| `CreateIfNotExist` | | Auto-create missing databases (boolean) |
| `PreferredFloatingPointDataType` | | Double or Decimal selection |

### Database Modes

| Mode | Description | Supported By |
|------|-------------|--------------|
| **Folder-as-Database** | Directory = database, one file per table | JSON, CSV, XML, XLS |
| **File-as-Database** | Single file = database, internal structure = tables | JSON, XML |

## File I/O Pipeline

### Read Path

```
FileConnection.Open()
  └─ FileReader (abstract)
       ├─ ReadFromFolder(tableName)   [folder-as-DB: one file per table]
       └─ ReadFromFile()              [file-as-DB: parse internal structure]
           └─ VirtualDataSet / VirtualDataTable  [lazy-loaded in-memory representation]
```

### Write Path

```
FileCommand.ExecuteNonQuery() / ExecuteReader()
  └─ FileStatementCreator.Create(sql)  [SqlBuildingBlocks parses SQL → FileStatement]
       └─ FileStatement subtype (FileInsert, FileUpdate, FileDelete, FileCreateTable, ...)
            └─ FileWriter.Execute()
                 └─ IDataSetWriter.Write()  [serializes back to format]
```

### SQL Statement Types

```
FileStatement (abstract)
  ├─ FileSelect         -- SELECT queries
  ├─ FileInsert         -- INSERT operations
  ├─ FileUpdate         -- UPDATE operations
  ├─ FileDelete         -- DELETE operations
  ├─ FileCreateTable    -- CREATE TABLE
  ├─ FileAddColumn      -- ALTER TABLE ADD COLUMN
  ├─ FileDropColumn     -- ALTER TABLE DROP COLUMN
  ├─ FileCreateDatabase -- CREATE DATABASE
  ├─ FileDropDatabase   -- DROP DATABASE
  └─ FileAdminStatement -- Administrative operations
```

## Data Source Provider Abstraction

`IDataSourceProvider` decouples storage from parsing:

```csharp
public interface IDataSourceProvider
{
    string Database { get; }
    DataSourceType DataSourceType { get; }  // Directory or File
    bool StorageExists(string tableName);
    IEnumerable<string> GetTableNames();
    StreamReader GetTextReader(string tableName);
    TextWriter GetTextWriter(string tableName);
    event DataSourceEventHandler Changed;     // FileSystemWatcher integration
}
```

**Implementations**:
- `FileSystemDataSource` -- Local file system (production)
- `StreamedDataSource` -- In-memory streams (testing)
- `TableStreamedDataSource` -- Per-table streams (testing)

## EF Core Integration

### Registration Pattern

```csharp
protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
{
    optionsBuilder.UseJson(connectionString);    // or UseCsv(), UseXml()
}
```

### Key EF Core Classes

| Class | Purpose |
|-------|---------|
| `FileOptionsExtension` | Stores IDataSourceProvider, registers services |
| `FileTypeMappingSource` | Maps EF Core types to provider types |
| `FileRelationalConnection` | EF Core connection wrapper |
| `FileDatabaseCreator` | Database creation/deletion |
| `FileScaffoldingModelFactory` | Model generation from source files |
| `FileDesignTimeServices` | Design-time service registration |

## Key Interfaces

| Interface | Location | Purpose |
|-----------|----------|---------|
| `IFileConnection` | Data.Common | Public connection contract |
| `IFileCommand` | Data.Common | Command contract |
| `IDataSourceProvider` | Data.Common | Storage backend abstraction |
| `IDataSetWriter` | Data.Common | Format-specific serialization |
| `IConnectionStringProperties` | Data.Common | Connection string parser contract |
| `ITableSchemaProvider` | Data.Common | Schema metadata provider |
| `IContainsReturning` | Data.Common | RETURNING clause support |

## Format-Specific Behaviors

| Format | Type System | Modes | Special Features |
|--------|-------------|-------|-----------------|
| JSON | Inferred from content (GuessTypeFunction) | Folder + File | VirtualDataTable lazy-loading, BOM handling |
| CSV | All strings (no types) | Folder only | CsvHelper RFC 4180, header-as-columns |
| XML | Strongly typed via XSD | Folder + File | XmlReader type conversion |
| XLS | Cell format types | Folder only | Read-only, DocumentFormat.OpenXml |

## Testing Patterns

### Test Structure

Each format provider test project mirrors the ADO.NET structure:
```
Data.[Format].Tests/
  Admin/                  -- Connection string, admin operation tests
  FileAsDatabase/         -- File-as-DB mode tests (JSON, XML only)
  FolderAsDatabase/       -- Folder-as-DB mode tests
  Sources/                -- Test data files
  Utils/                  -- Format-specific test helpers
```

### Test Stack
- **xUnit** for test framework
- **FluentAssertions** for assertions
- **Moq** for mocking
- **Dapper** for ORM integration tests (in JSON tests)
- **coverlet** for code coverage (XPlat Cobertura format)

### Test Categories
- Connection string parsing
- Connection lifecycle
- CRUD operations (SELECT, INSERT, UPDATE, DELETE)
- DDL operations (CREATE TABLE, ALTER TABLE)
- Transaction support
- Large file handling (5MB+ files)
- Edge cases (UTF-8 BOM, trailing commas, DateTime formats)
- Dapper integration
- EF Core DbContext operations

## CI/CD

- **Trigger**: Push/PR to main, manual dispatch
- **Runner**: ubuntu-latest, .NET 8.0.x
- **Version**: 1.0.0.${{github.run_number}} (via UpdateVersion.ps1)
- **Packages published to NuGet.org**: Data.Common, Data.Json, Data.Csv, Data.Xml, Data.Xls, EFCore.Common, EFCore.Json, EFCore.Csv, EFCore.Xml
- **Coverage**: Cobertura XML, 40-60% thresholds, PR comment
