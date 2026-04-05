# FileBased.DataProviders Asset Inventory

## Core Product Assets

| Asset Area | Observed Asset | Status |
|------------|----------------|--------|
| ADO.NET Framework | Data.Common base classes & interfaces | Production |
| JSON Provider | Data.Json (folder + file modes, type inference, VirtualDataTable) | Production |
| CSV Provider | Data.Csv (folder mode, RFC 4180, CsvHelper) | Production |
| XML Provider | Data.Xml (folder + file modes, XSD typing) | Production |
| XLS Provider | Data.Xls (read-only, DocumentFormat.OpenXml) | Early stage |
| EF Core Framework | EFCore.Common (options, type mapping, scaffolding) | Alpha |
| EF Core JSON | EFCore.Json (UseJson extension) | Alpha |
| EF Core CSV | EFCore.Csv (UseCsv extension) | Alpha |
| EF Core XML | EFCore.Xml (UseXml extension) | Alpha |
| Dapper Compatibility | Verified with JSON provider | Tested |
| Connection String Parser | FileConnectionString with keyword aliases | Production |
| Data Source Abstraction | IDataSourceProvider (FileSystem, Streamed, TableStreamed) | Production |
| File I/O Pipeline | FileReader/FileWriter with format-specific implementations | Production |
| SQL Statement Processing | FileStatementCreator + FileStatement hierarchy | Production |

## Test Assets

| Test Type | Projects | Coverage |
|-----------|----------|----------|
| Unit Tests | 9 test projects (one per source project) | Comprehensive |
| Dapper Integration | Data.Json.Tests includes ORM tests | Verified |
| Large File Tests | 5MB+ file handling | Verified |
| Edge Case Tests | UTF-8 BOM, trailing commas, DateTime formats | Verified |
| EF Core Tests | 3 test projects (JSON, CSV, XML) | Basic |

## NuGet Packages (9 total)

| Package | NuGet ID |
|---------|----------|
| Data.Common | ServantSoftware.Data.Common |
| Data.Json | ServantSoftware.Data.Json |
| Data.Csv | ServantSoftware.Data.Csv |
| Data.Xml | ServantSoftware.Data.Xml |
| Data.Xls | ServantSoftware.Data.Xls |
| EFCore.Common | ServantSoftware.EFCore.FileBasedProviders.Common |
| EFCore.Json | ServantSoftware.EFCore.Json |
| EFCore.Csv | ServantSoftware.EFCore.Csv |
| EFCore.Xml | ServantSoftware.EFCore.Xml |

## Dependencies

- SqlBuildingBlocks (SQL parsing, Irony-based)
- CsvHelper (CSV parsing)
- DocumentFormat.OpenXml (Excel reading)
- Microsoft.Data.Analysis (CSV data analysis)
- Serilog (logging)
- xUnit + FluentAssertions + Moq (testing)
- Dapper (integration testing)
- BenchmarkDotNet (performance testing)
