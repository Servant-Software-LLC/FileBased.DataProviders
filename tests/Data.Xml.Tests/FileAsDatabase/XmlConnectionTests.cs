using Data.Tests.Common;
using System.Data.XmlClient;
using Xunit;

namespace Data.Xml.Tests.FileAsDatabase;

public class XmlConnectionTests
{
    [Fact]
    public void Open_DatabaseExists() =>
        ConnectionTests.Open_DatabaseExists(() => new XmlConnection(ConnectionStrings.Instance.FileAsDB));

    [Fact]
    public void Open_DatabaseDoesNotExist() =>
        ConnectionTests.Open_DatabaseDoesNotExist(() => new XmlConnection(ConnectionStrings.Instance.bogusFileDB));

    [Fact]
    public void Open_DatabaseDoesNotExist_AutoCreate() =>
        ConnectionTests.Open_DatabaseDoesNotExist_AutoCreate(() => new XmlConnection(ConnectionStrings.Instance.bogusFileDB));

    [Fact]
    public void ChangeDatabase_DatabaseExists() =>
        ConnectionTests.ChangeDatabase_DatabaseExists(() =>
            new XmlConnection(ConnectionStrings.Instance.FileAsDB), ConnectionStrings.Instance.eComFileDataBase);

    [Fact]
    public void ChangeDatabase_DatabaseDoesNotExist() =>
        ConnectionTests.ChangeDatabase_DatabaseDoesNotExist(() =>
            new XmlConnection(ConnectionStrings.Instance.FileAsDB), ConnectionStrings.Instance.bogusFileDB);

    [Fact]
    public void ChangeDatabase_DatabaseDoesNotExist_AutoCreate() =>
        ConnectionTests.ChangeDatabase_DatabaseDoesNotExist_AutoCreate(() =>
            new XmlConnection(ConnectionStrings.Instance.FileAsDB), ConnectionStrings.Instance.bogusFileDB);
}
