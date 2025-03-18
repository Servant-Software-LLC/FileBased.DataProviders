using Data.Tests.Common;
using System.Data.XlsClient;
using Xunit;

namespace Data.Xls.Tests.FileAsDatabase;

public class XlsConnectionTests
{
    [Fact]
    public void Open_DatabaseExists() =>
        ConnectionTests.Open_DatabaseExists(() => new XlsConnection(ConnectionStrings.Instance.FileAsDB));

    [Fact]
    public void Open_DatabaseDoesNotExist() =>
        ConnectionTests.Open_DatabaseDoesNotExist(() => new XlsConnection(ConnectionStrings.Instance.bogusFileDB));

    [Fact]
    public void Open_DatabaseDoesNotExist_AutoCreate() =>
        ConnectionTests.Open_DatabaseDoesNotExist_AutoCreate(() =>
            new XlsConnection(ConnectionStrings.Instance.bogusFileDB));

    [Fact]
    public void ChangeDatabase_DatabaseExists() =>
        ConnectionTests.ChangeDatabase_DatabaseExists(() =>
                new XlsConnection(ConnectionStrings.Instance.FileAsDB),
            ConnectionStrings.Instance.Database.eComFileDataBase);

    [Fact]
    public void ChangeDatabase_DatabaseDoesNotExist() =>
        ConnectionTests.ChangeDatabase_DatabaseDoesNotExist(() =>
            new XlsConnection(ConnectionStrings.Instance.FileAsDB), ConnectionStrings.Instance.bogusFileDB);

    [Fact]
    public void ChangeDatabase_DatabaseDoesNotExist_AutoCreate() =>
        ConnectionTests.ChangeDatabase_DatabaseDoesNotExist_AutoCreate(() =>
            new XlsConnection(ConnectionStrings.Instance.FileAsDB), ConnectionStrings.Instance.bogusFileDB);

    [Fact]
    public void GetSchema_Tables() =>
        ConnectionTests.GetSchema_Tables(() =>
            new XlsConnection(ConnectionStrings.Instance.FileAsDB), ConnectionStrings.Instance.Database.DatabaseFileName);

    [Fact]
    public void GetSchema_Columns() =>
        ConnectionTests.GetSchema_Columns(() =>
            new XlsConnection(ConnectionStrings.Instance.FileAsDB), ConnectionStrings.Instance.Database.DatabaseFileName);
}