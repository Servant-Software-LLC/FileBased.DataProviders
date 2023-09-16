using Data.Tests.Common;
using System.Data.XmlClient;
using Xunit;

namespace Data.Xml.Tests.FolderAsDatabase;

public class XmlConnectionTests
{
    [Fact]
    public void Open_DatabaseExists() =>
        ConnectionTests.Open_DatabaseExists(() => new XmlConnection(ConnectionStrings.Instance.FolderAsDB));

    [Fact]
    public void Open_DatabaseDoesNotExist() =>
        ConnectionTests.Open_DatabaseDoesNotExist(() => new XmlConnection(ConnectionStrings.Instance.bogusFolderDB));

    [Fact]
    public void Open_DatabaseDoesNotExist_AutoCreate() =>
        ConnectionTests.Open_DatabaseDoesNotExist_AutoCreate(() => new XmlConnection(ConnectionStrings.Instance.bogusFolderDB));

    [Fact]
    public void ChangeDatabase_DatabaseExists() =>
        ConnectionTests.ChangeDatabase_DatabaseExists(() =>
            new XmlConnection(ConnectionStrings.Instance.FolderAsDB), ConnectionStrings.Instance.eComFolderDataBase);

    [Fact]
    public void ChangeDatabase_DatabaseDoesNotExist() =>
        ConnectionTests.ChangeDatabase_DatabaseDoesNotExist(() =>
            new XmlConnection(ConnectionStrings.Instance.FolderAsDB), ConnectionStrings.Instance.bogusFolderDB);

    [Fact]
    public void ChangeDatabase_DatabaseDoesNotExist_AutoCreate() =>
        ConnectionTests.ChangeDatabase_DatabaseDoesNotExist_AutoCreate(() =>
            new XmlConnection(ConnectionStrings.Instance.FolderAsDB), ConnectionStrings.Instance.bogusFolderDB);
}
