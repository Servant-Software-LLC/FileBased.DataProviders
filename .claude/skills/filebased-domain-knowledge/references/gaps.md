# FileBased.DataProviders Productization Gaps & Recommended Next Steps

## Top Productization Gaps

### P0 (Critical)

| Gap Area | Current Signal | Impact |
|----------|----------------|--------|
| **EF Core Provider Maturity** | Alpha stage for JSON/CSV/XML; limited feature coverage | Blocks EF Core adoption; constrains MockDB and SettingsOnEF integration |
| **XLS Write Support** | Read-only provider; no INSERT/UPDATE/DELETE | Incomplete Excel format support |

### P1 (Important)

| Gap Area | Current Signal | Impact |
|----------|----------------|--------|
| **API Documentation** | READMEs per format but no comprehensive API reference | Developer onboarding friction |
| **Type System Consistency** | CSV has no types; JSON infers; XML requires XSD | Confusing cross-format behavior |
| **Concurrent Write Safety** | No file-locking or multi-process write strategy | Data integrity risk in shared-file scenarios |
| **SQL Feature Coverage** | Bounded by SqlBuildingBlocks parser | Some SQL operations unsupported |
| **Performance Benchmarks** | BenchmarkDotNet dependency exists but no published results | No baseline for large-file performance |

### P2 (Nice to Have)

| Gap Area | Current Signal | Impact |
|----------|----------------|--------|
| **Additional Formats** | Only JSON/CSV/XML/XLS supported | No YAML, TOML, Parquet, etc. |
| **Streaming Write** | Current write path materializes full dataset | Memory pressure for very large files |
| **Change Detection** | FileSystemWatcher integration exists but limited | Incomplete external modification handling |

## Recommended Next Steps

### Workstream A -- EF Core Maturity
1. Expand EF Core feature coverage (migrations, complex queries, relationships)
2. Add EF Core specification compliance tests
3. Graduate JSON/CSV/XML providers from alpha to stable

### Workstream B -- Format Completeness
1. Add XLS write support
2. Standardize type inference across formats
3. Document type mapping behavior per format

### Workstream C -- Robustness
1. Implement file-locking strategy for concurrent access
2. Publish performance benchmarks
3. Add stress tests for large-file scenarios

### Workstream D -- Developer Experience
1. Generate API reference documentation
2. Create quickstart guides with common scenarios
3. Add sample projects demonstrating Dapper and EF Core usage
