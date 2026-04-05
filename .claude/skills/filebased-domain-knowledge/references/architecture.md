# FileBased.DataProviders Architecture Overview

## Architectural Posture

FileBased.DataProviders is a collection of ADO.NET and EF Core data providers that present file-based data (JSON, XML, CSV, XLS) through standard relational database interfaces. It enables SQL CRUD operations against files using the familiar DbConnection/DbCommand/DbDataReader stack, with optional EF Core integration.

## Primary Architectural Building Blocks

### ADO.NET Provider Layer
- **Data.Common**: Base classes (`FileConnection<T>`, `FileCommand<T>`, `FileDataReader`, `FileTransaction<T>`, `FileParameter<T>`) implementing ADO.NET contracts. All format-specific providers inherit from these.
- **Format Providers**: JSON, CSV, XML, XLS -- each implements format-specific connection, command, reader, and I/O classes.

### Data Source Abstraction
- **IDataSourceProvider**: Decouples storage backend from data parsing. Implementations include FileSystemDataSource (production), StreamedDataSource (testing), TableStreamedDataSource (per-table testing).
- **DataSourceType**: Directory (folder-as-database) or File (file-as-database) modes.
- **FileSystemWatcher**: Integration for external change detection (StartWatching/StopWatching lifecycle).

### File I/O Pipeline
- **FileReader** (abstract): Template method pattern -- subclasses override `ReadFromFolder()` and `ReadFromFile()` for format-specific parsing. Produces VirtualDataSet/VirtualDataTable for lazy-loaded in-memory representation.
- **FileWriter** (abstract): Handles INSERT, UPDATE, DELETE operations. Returns Result with RecordsAffected.
- **IDataSetWriter**: Extensibility point for custom serialization back to format.

### SQL Statement Processing
- **FileStatementCreator**: Parses SQL using SqlBuildingBlocks library (Irony-based grammar).
- **FileStatement hierarchy**: FileSelect, FileInsert, FileUpdate, FileDelete, FileCreateTable, FileAddColumn, FileDropColumn, FileCreateDatabase, FileDropDatabase, FileAdminStatement.

### EF Core Integration Layer
- **EFCore.Common**: Shared infrastructure (FileOptionsExtension, FileTypeMappingSource, FileRelationalConnection, FileDatabaseCreator, FileScaffoldingModelFactory).
- **Format-specific EF Core providers**: UseJson(), UseCsv(), UseXml() extensions for DbContextOptionsBuilder.

### Connection String Infrastructure
- **FileConnectionString**: Parses DataSource, LogLevel, Formatted, CreateIfNotExist, PreferredFloatingPointDataType keywords with alias support and validation.

## Design Patterns

| Pattern | Usage |
|---------|-------|
| Template Method | FileReader/FileWriter abstract base with format-specific overrides |
| Factory | JsonClientFactory (DbProviderFactory), FileStatementCreator |
| Strategy | IDataSourceProvider implementations, IDataSetWriter, GuessTypeFunction |
| Adapter | FileDataAdapter bridging ADO.NET to file operations |
| Decorator | VirtualDataTable (lazy-loading), BOMHandlingStream, TransactionScopedRows |
| Options (EF Core) | FileOptionsExtension for provider configuration |

## Format-Specific Architecture

| Format | Reader | Type System | Modes | Special |
|--------|--------|-------------|-------|---------|
| JSON | JsonReader + JsonVirtualDataTable | Inferred (GuessTypeFunction, sample rows) | Folder + File | Lazy-loading, BOM handling |
| CSV | CsvReader (CsvHelper) | All strings | Folder only | RFC 4180, header-as-columns |
| XML | XmlReader | XSD-typed or string default | Folder + File | XPath navigation, XmlReader type conversion |
| XLS | XlsReader (OpenXml) | Cell format types | Folder only | Read-only, DocumentFormat.OpenXml |

## Ecosystem Dependencies

- **SqlBuildingBlocks** -- SQL parsing (Irony-based grammar to logical classes)
- **CsvHelper** -- CSV parsing (RFC 4180)
- **DocumentFormat.OpenXml** -- Excel file reading
- **Serilog** -- Logging
- **xUnit + FluentAssertions + Moq** -- Testing

## Strengths

1. Unified ADO.NET interface across all file formats
2. Same SQL subset works against JSON, CSV, XML, and XLS
3. Extensible architecture (new formats via FileReader/FileWriter subclasses)
4. Both folder-as-database and file-as-database modes
5. Lazy-loading via VirtualDataTable for large files
6. Transaction support with rollback capability

## Architecture Risks

1. EF Core providers are alpha -- limited feature coverage
2. XLS provider is read-only
3. Type system inconsistency across formats (CSV has none, JSON infers, XML requires XSD)
4. No built-in concurrent write protection
5. SQL support limited to what SqlBuildingBlocks parses
