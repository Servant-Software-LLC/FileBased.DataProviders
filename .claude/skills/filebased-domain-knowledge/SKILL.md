---
name: filebased-domain-knowledge
description: |
  FileBased.DataProviders product knowledge for Servant Software LLC agents. Use when working on anything
  related to FileBased.DataProviders:
  - Product strategy, roadmap, or feature planning
  - Technical architecture discussions or documentation
  - Market analysis, competitive positioning, or customer interviews
  - Developer experience, onboarding, or API questions
  - Understanding current maturity, gaps, or productization needs
  - Integration with MockDB, SettingsOnADO, or other consuming projects

  Provides: product purpose, architecture overview, asset inventory, maturity assessment, and productization gaps.

  SELF-UPDATING: When your work changes, advances, or extends FileBased.DataProviders in ways that affect
  this knowledge (new providers, features, assets, maturity changes, resolved gaps, etc.), you MUST update
  this skill and its reference files to reflect the new state before completing your task. This keeps the
  knowledge accurate for future agents. Update the specific section(s) affected -- do not rewrite unchanged content.
---

# FileBased.DataProviders Domain Knowledge

## Quick Reference

**What is FileBased.DataProviders?** A collection of ADO.NET and EF Core data providers that treat common serializable file formats (JSON, XML, CSV, XLS) as relational databases. Enables standard SQL CRUD operations against file-based data through the familiar ADO.NET and EF Core interfaces.

## Core Product Facts

- **Primary Interface**: ADO.NET (DbConnection/DbCommand/DbDataReader) for all formats
- **EF Core Support**: Alpha-stage providers for JSON, CSV, XML (optimized for SettingsOnEF and MockDB)
- **Supported Formats**: JSON, XML, CSV, XLS (Excel read-only)
- **Database Modes**: Folder-as-database (directory = DB, file = table) and File-as-database (single file = DB)
- **SQL Parsing**: Via SqlBuildingBlocks library (Irony-based)
- **Tech Stack**: .NET 8 + netstandard2.0, CsvHelper, DocumentFormat.OpenXml, Serilog
- **License**: MIT
- **Distribution**: NuGet (9 packages under ServantSoftware.* namespace)

## Key Assets

| Asset | Status |
|-------|--------|
| JSON ADO.NET Provider | Production (folder + file modes, type inference, VirtualDataTable) |
| CSV ADO.NET Provider | Production (folder mode, RFC 4180, CsvHelper) |
| XML ADO.NET Provider | Production (folder + file modes, XSD typing) |
| XLS ADO.NET Provider | Early stage (read-only, OpenXml) |
| Data.Common Framework | Production (base classes, interfaces, file I/O pipeline) |
| EF Core JSON Provider | Alpha |
| EF Core CSV Provider | Alpha |
| EF Core XML Provider | Alpha |
| Dapper Compatibility | Verified (JSON provider) |
| NuGet Distribution | 9 packages published |

## Ecosystem Role

FileBased.DataProviders is a foundational OSS dependency for Servant Software LLC's product stack:

- **MockDB** -- Uses FileBased.DataProviders for data import/export and file-based storage
- **SettingsOnADO** -- Uses Data.Json for JSON-file-based settings persistence
- **SqlBuildingBlocks** -- Provides the SQL parsing engine that FileBased.DataProviders consumes

## Maturity Assessment

| Dimension | Score | Notes |
|-----------|-------|-------|
| Problem/Solution Fit | 4/5 | Clear value for SQL-over-files use case |
| Core Engineering | 4/5 | Well-structured provider hierarchy, extensible architecture |
| ADO.NET Providers | 4/5 | JSON/CSV/XML production-ready; XLS read-only |
| EF Core Providers | 2/5 | Alpha stage, limited feature coverage |
| Quality & Reliability | 3/5 | Good test coverage, Dapper integration verified |
| Documentation | 3/5 | READMEs per format, but no comprehensive API docs |
| Commercial Readiness | 3/5 | MIT licensed, NuGet published, CI/CD pipeline |

## Top Gaps

### P0 (Critical)
| Gap | Impact |
|-----|--------|
| EF Core providers are alpha | Limits EF Core adoption for file-based scenarios |
| XLS provider is read-only | No write support for Excel format |

### P1 (Important)
| Gap | Impact |
|-----|--------|
| No comprehensive API documentation | Developer onboarding friction |
| Type system varies by format | CSV has no types; JSON infers; XML requires XSD |
| No file-locking/concurrent write strategy | Data integrity risk in multi-process scenarios |

## Detailed References

See [references/](references/) for:
- Architecture Overview
- Asset Inventory
- Maturity Assessment
- Gaps & Next Steps
