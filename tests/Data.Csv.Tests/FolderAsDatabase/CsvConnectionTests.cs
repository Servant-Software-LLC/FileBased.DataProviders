﻿using Data.Tests.Common;
using System.Data.CsvClient;
using Xunit;

namespace Data.Csv.Tests.FolderAsDatabase;

public class CsvConnectionTests
{
    [Fact]
    public void Open_DatabaseExists() => 
        ConnectionTests.Open_DatabaseExists(() => new CsvConnection(ConnectionStrings.Instance.FolderAsDB));

    [Fact]
    public void Open_DatabaseDoesNotExist() =>
        ConnectionTests.Open_DatabaseDoesNotExist(() => new CsvConnection(ConnectionStrings.Instance.bogusFolderDB));

    [Fact]
    public void Open_DatabaseDoesNotExist_AutoCreate() =>
        ConnectionTests.Open_DatabaseDoesNotExist_AutoCreate(() => new CsvConnection(ConnectionStrings.Instance.bogusFolderDB));

    [Fact]
    public void ChangeDatabase_DatabaseExists() =>
        ConnectionTests.ChangeDatabase_DatabaseExists(() => 
            new CsvConnection(ConnectionStrings.Instance.FolderAsDB), ConnectionStrings.Instance.Database.eComFolderDataBase);

    [Fact]
    public void ChangeDatabase_DatabaseDoesNotExist() => 
        ConnectionTests.ChangeDatabase_DatabaseDoesNotExist(() =>
            new CsvConnection(ConnectionStrings.Instance.FolderAsDB), ConnectionStrings.Instance.bogusFolderDB);

    [Fact]
    public void ChangeDatabase_DatabaseDoesNotExist_AutoCreate() =>
        ConnectionTests.ChangeDatabase_DatabaseDoesNotExist_AutoCreate(() =>
            new CsvConnection(ConnectionStrings.Instance.FolderAsDB), ConnectionStrings.Instance.bogusFolderDB);

    [Fact]
    public void GetSchema_Tables() =>
        ConnectionTests.GetSchema_Tables(() =>
            new CsvConnection(ConnectionStrings.Instance.FolderAsDB));

    [Fact]
    public void GetSchema_Columns() =>
        ConnectionTests.GetSchema_Columns(() =>
            new CsvConnection(ConnectionStrings.Instance.FolderAsDB));
}
