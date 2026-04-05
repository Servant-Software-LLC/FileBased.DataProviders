---
name: filebased-qa-knowledge
description: |
  QA-specific knowledge for testing the FileBased.DataProviders library. Use when planning test
  strategies, evaluating coverage gaps, writing acceptance criteria, investigating regressions,
  designing test matrices, or assessing release readiness.
  Covers: test inventory, coverage analysis, risk-based testing priorities, edge case catalog,
  cross-format test matrix, integration testing concerns, known fragile areas, and test environment setup.

  SELF-UPDATING: When your work changes, advances, or extends testing in FileBased.DataProviders
  (new test projects, coverage changes, discovered edge cases, resolved defects, etc.), you MUST
  update this skill to reflect the new state before completing your task.
---

# FileBased.DataProviders QA Knowledge

## Test Inventory

### Test Projects (9 total)

| Project | Scope | Type |
|---------|-------|------|
| Data.Tests.Common | Shared POCOs, assertion extensions, test utilities | Support |
| Data.Json.Tests | JSON ADO.NET provider | Unit + Integration + Dapper |
| Data.Csv.Tests | CSV ADO.NET provider | Unit + Integration |
| Data.Xml.Tests | XML ADO.NET provider | Unit + Integration |
| Data.Xls.Tests | Excel ADO.NET provider | Unit + Integration |
| EFCore.Common.Tests | Shared EF Core infrastructure | Unit |
| EFCore.Json.Tests | EF Core JSON provider | Integration |
| EFCore.Csv.Tests | EF Core CSV provider | Integration |
| EFCore.Xml.Tests | EF Core XML provider | Integration |

### Test Stack
- **Framework**: xUnit 2.9.2
- **Assertions**: FluentAssertions 6.0.0
- **Mocking**: Moq 4.20.72
- **ORM Integration**: Dapper 2.0.123
- **Coverage**: coverlet (XPlat Cobertura), 40-60% threshold
- **Performance**: BenchmarkDotNet 0.13.4 (available but no published baselines)

### Running Tests
```powershell
dotnet test --configuration Release
dotnet test --configuration Release --collect:"XPlat Code Coverage"
```

## Cross-Format Test Matrix

This is the most important QA lens for this library. Every SQL operation should be verified across
all applicable formats.

### Operation Coverage by Format

| Operation | JSON | CSV | XML | XLS | Notes |
|-----------|------|-----|-----|-----|-------|
| SELECT | Y | Y | Y | Y | Core read path |
| INSERT | Y | Y | Y | -- | XLS is read-only |
| UPDATE | Y | Y | Y | -- | XLS is read-only |
| DELETE | Y | Y | Y | -- | XLS is read-only |
| CREATE TABLE | Y | Y | Y | -- | Schema creation |
| ALTER TABLE ADD COLUMN | Y | Y | Y | -- | Schema evolution |
| ALTER TABLE DROP COLUMN | Y | Y | Y | -- | Schema evolution |
| CREATE DATABASE | Y | Y | Y | -- | New folder/file |
| DROP DATABASE | Y | Y | Y | -- | Remove folder/file |
| Transactions | Y | Y | Y | -- | Rollback verified |
| Dapper Integration | Y | ? | ? | ? | Only JSON verified |
| EF Core Integration | Y | Y | Y | -- | Alpha coverage |

**Key gap**: Dapper integration only verified for JSON. CSV, XML, and XLS lack Dapper tests.

### Database Mode Coverage

| Mode | JSON | CSV | XML | XLS |
|------|------|-----|-----|-----|
| Folder-as-Database | Y | Y | Y | Y |
| File-as-Database | Y | -- | Y | -- |

**Key gap**: CSV and XLS only support folder mode. Ensure no file-as-DB code paths are accidentally
exercised for these formats.

## Risk-Based Testing Priorities

### P0 -- Data Integrity Risks

1. **Write operations lose data silently**
   - UPDATE that partially succeeds (some columns written, crash mid-write)
   - Transaction rollback fails to restore original file content
   - Concurrent read during write returns incomplete data
   - Test: Write, kill mid-operation, verify file is either fully old or fully new

2. **Type conversion data loss**
   - JSON type inference picks wrong type (number stored as string, or vice versa)
   - CSV has no type system -- round-trip of typed data may lose precision
   - XML without XSD defaults to string -- typed data loses its type
   - Test: Round-trip DateTime, decimal, bool, null through each format

3. **Character encoding corruption**
   - UTF-8 BOM handling (JSON has specific BOM tests -- verify others)
   - Non-ASCII characters in file paths, table names, column values
   - Test: Insert emoji, CJK characters, RTL text, verify round-trip

### P1 -- Schema & Structure Risks

4. **Schema inference inconsistency**
   - JSON GuessTypeFunction produces different types depending on sample size (GuessTypeRows)
   - Column ordering differs between formats
   - Empty tables/files produce different schema than populated ones
   - Test: Create table, add rows, verify schema; also verify schema of empty table

5. **File system edge cases**
   - Very long file paths (near OS limits)
   - Special characters in table names (spaces, dots, slashes)
   - Read-only file system
   - File locked by another process
   - Missing parent directory with CreateIfNotExist=false vs true

6. **Large file behavior**
   - 5MB+ file tests exist for JSON -- verify memory doesn't explode
   - VirtualDataTable lazy-loading correctness under pagination
   - Test: 100K+ rows, verify streaming reads don't materialize all in memory

### P2 -- Compatibility & Integration Risks

7. **SqlBuildingBlocks parser boundaries**
   - SQL that SqlBuildingBlocks can parse but the format provider can't execute
   - SQL that SqlBuildingBlocks rejects but is valid for the target format
   - Test: Document the supported SQL subset per provider

8. **EF Core alpha limitations**
   - Which EF Core operations work vs throw?
   - Do migrations work at all?
   - Scaffold from existing file -- does it produce correct model?
   - Test: Standard EF Core CRUD, verify what fails

## Edge Case Catalog

### JSON-Specific
- Trailing commas in JSON arrays/objects
- JSON with BOM (UTF-8 byte order mark)
- Nested JSON objects (non-flat structure)
- Mixed types in same column across rows
- Empty JSON array `[]` as a table
- JSON null vs missing property vs empty string
- Very large numbers (beyond double precision)
- DateTime format variations (ISO 8601, Unix epoch, custom)

### CSV-Specific
- Quoted fields containing delimiters, newlines, quotes
- Empty rows (all delimiters, no values)
- Header-only file (no data rows)
- Inconsistent column count across rows
- Trailing newline at end of file
- CRLF vs LF line endings
- Fields with leading/trailing whitespace

### XML-Specific
- XSD present vs absent (typed vs untyped)
- XML namespaces
- CDATA sections
- Empty elements vs self-closing elements
- Attributes vs child elements (which maps to columns?)
- Very deeply nested XML structure
- XML declaration encoding mismatch with file encoding

### XLS-Specific
- Multiple sheets (each = table)
- Merged cells
- Empty rows/columns in middle of data
- Formula cells (return computed value or formula text?)
- Hidden sheets/columns
- Very large workbooks

## Known Fragile Areas

1. **ClientPayload offset tracking** -- Inherited from how SqlBuildingBlocks constructs logical
   statements. Off-by-one in field parsing cascades through subsequent reads.

2. **VirtualDataTable lazy-loading** -- JsonVirtualDataTable streams on-demand. If the underlying
   file changes between reads, results may be inconsistent.

3. **FileSystemWatcher reliability** -- The IDataSourceProvider Changed event relies on OS-level
   file watchers, which are unreliable on some platforms (especially network drives, WSL).

4. **Transaction isolation** -- TransactionScopedRows tracks changes, but concurrent commands on
   the same connection may see uncommitted state.

## Test Environment Setup

### Prerequisites
- .NET 8 SDK
- No external services required (all file-based)

### Test Data Sources
Located in each test project under `Sources/`:
- Pre-built JSON files (ecommerce.json, database.json)
- Folder-based data structures (eCom/ with Customers.json, Orders.json)
- Large test files (Large_5MB.json)
- Edge case files (WithBOM/, WithTrailingComma/, WithDateTime/)

### Test Isolation
- Each test should create its own temp directory or use unique file names
- JSON tests use sandbox pattern (unique test directory per test method)
- No shared mutable state between tests

## Coverage Gaps to Investigate

1. **No performance regression tests** -- BenchmarkDotNet is a dependency but no published baselines
2. **No stress/load tests** -- Concurrent read/write, many simultaneous connections
3. **No cross-platform tests** -- CI runs on ubuntu-latest only; no Windows/macOS verification
4. **No negative tests for XLS write** -- XLS is read-only; verify write attempts throw clearly
5. **No Dapper tests for CSV/XML/XLS** -- Only JSON has Dapper integration tests
6. **EF Core specification tests** -- EFCore.Common.Tests exists but EF Core spec compliance is limited

## Release Readiness Checklist

- [ ] All test projects pass (`dotnet test --configuration Release`)
- [ ] Code coverage meets threshold (40% yellow, 60% green)
- [ ] Cross-format CRUD matrix passes (SELECT/INSERT/UPDATE/DELETE per format)
- [ ] Large file tests pass (5MB+)
- [ ] Edge case tests pass (BOM, trailing commas, DateTime)
- [ ] No new compiler warnings (TreatWarningsAsErrors not enabled in this repo)
- [ ] NuGet packages build successfully (`dotnet pack --configuration Release`)
- [ ] Version updated via UpdateVersion.ps1
