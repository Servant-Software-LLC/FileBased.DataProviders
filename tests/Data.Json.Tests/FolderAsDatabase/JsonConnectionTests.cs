using Data.Tests.Common;
using System.Data.JsonClient;
using Xunit;

namespace Data.Json.Tests.FolderAsDatabase;

public class JsonConnectionTests
{
    [Fact]
    public void Open_DatabaseExists() =>
        ConnectionTests.Open_DatabaseExists(() => new JsonConnection(ConnectionStrings.Instance.FolderAsDB));

    [Fact]
    public void Open_DatabaseDoesNotExist() =>
        ConnectionTests.Open_DatabaseDoesNotExist(() => new JsonConnection(ConnectionStrings.Instance.bogusFolderDB));

    [Fact]
    public void Open_DatabaseDoesNotExist_AutoCreate() =>
        ConnectionTests.Open_DatabaseDoesNotExist_AutoCreate(() => new JsonConnection(ConnectionStrings.Instance.bogusFolderDB));

    [Fact]
    public void ChangeDatabase_DatabaseExists() =>
        ConnectionTests.ChangeDatabase_DatabaseExists(() =>
            new JsonConnection(ConnectionStrings.Instance.FolderAsDB), ConnectionStrings.Instance.eComFolderDataBase);

    [Fact]
    public void ChangeDatabase_DatabaseDoesNotExist() =>
        ConnectionTests.ChangeDatabase_DatabaseDoesNotExist(() =>
            new JsonConnection(ConnectionStrings.Instance.FolderAsDB), ConnectionStrings.Instance.bogusFolderDB);

    [Fact]
    public void ChangeDatabase_DatabaseDoesNotExist_AutoCreate() =>
        ConnectionTests.ChangeDatabase_DatabaseDoesNotExist_AutoCreate(() =>
            new JsonConnection(ConnectionStrings.Instance.FolderAsDB), ConnectionStrings.Instance.bogusFolderDB);

}
