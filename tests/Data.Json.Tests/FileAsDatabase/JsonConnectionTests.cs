using Data.Tests.Common;
using System.Data.JsonClient;
using Xunit;

namespace Data.Json.Tests.FileAsDatabase;

public class JsonConnectionTests
{
    [Fact]
    public void Open_DatabaseExists() =>
        ConnectionTests.Open_DatabaseExists(() => new JsonConnection(ConnectionStrings.Instance.FileAsDB));

    [Fact]
    public void Open_DatabaseDoesNotExist() =>
        ConnectionTests.Open_DatabaseDoesNotExist(() => new JsonConnection(ConnectionStrings.Instance.bogusFileDB));

    [Fact]
    public void Open_DatabaseDoesNotExist_AutoCreate() =>
        ConnectionTests.Open_DatabaseDoesNotExist_AutoCreate(() => new JsonConnection(ConnectionStrings.Instance.bogusFileDB));

    [Fact]
    public void ChangeDatabase_DatabaseExists() =>
        ConnectionTests.ChangeDatabase_DatabaseExists(() =>
            new JsonConnection(ConnectionStrings.Instance.FileAsDB), ConnectionStrings.Instance.eComFileDataBase);

    [Fact]
    public void ChangeDatabase_DatabaseDoesNotExist() =>
        ConnectionTests.ChangeDatabase_DatabaseDoesNotExist(() =>
            new JsonConnection(ConnectionStrings.Instance.FileAsDB), ConnectionStrings.Instance.bogusFileDB);

    [Fact]
    public void ChangeDatabase_DatabaseDoesNotExist_AutoCreate() =>
        ConnectionTests.ChangeDatabase_DatabaseDoesNotExist_AutoCreate(() =>
            new JsonConnection(ConnectionStrings.Instance.FileAsDB), ConnectionStrings.Instance.bogusFileDB);
}
