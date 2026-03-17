# AGENTS.md

This file provides guidance to AI coding agents (including OpenAI Codex) when working in this repository.

## Repository Overview

FileBased.DataProviders is a collection of ADO.NET data providers that treat file formats (CSV, JSON, XML, XLS) as relational databases. It implements `IDbConnection`, `IDbCommand`, `IDataReader`, and related interfaces from `System.Data`.

## Review Guidelines

When reviewing pull requests, focus on the following by priority:

### P0 — Must fix (security, data corruption, crashes)
- SQL injection vulnerabilities in query parsing or command text handling
- Exposed secrets or credentials in code or connection strings
- Data corruption when writing CSV/JSON/XML/XLS files
- Unhandled exceptions that crash the provider without a meaningful error
- Incorrect disposal of streams, connections, or readers (memory/file handle leaks)

### P1 — Should fix (logic bugs, resource leaks, incorrect behavior)
- Incorrect type inference for CSV/JSON columns (e.g., numeric values typed as strings or vice versa)
- Off-by-one errors or boundary conditions in streaming/chunked reads
- Broken `IDataReader` contract (e.g., wrong `FieldCount`, incorrect `Read()` return values)
- Missing `using`/`Dispose` on `IDisposable` objects
- Async/await antipatterns (blocking on `.Result`, missing `ConfigureAwait`, `async void`)
- Thread-safety issues in shared state

### P2 — Nice to fix (skip unless trivial)
- Naming consistency with existing conventions
- Missing XML doc comments on new public API surface
- Minor performance improvements

## What to Skip
- Code style and formatting (enforced by `.editorconfig`)
- Suggestions to add tests unless a bug is clearly untested
- Refactoring suggestions unrelated to the PR's scope
