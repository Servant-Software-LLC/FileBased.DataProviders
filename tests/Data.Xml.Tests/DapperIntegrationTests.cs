using Dapper;
using System.Data.XmlClient;
using Xunit;

namespace Data.Xml.Tests;

public class DapperIntegrationTests
{
    [Fact]
    public void Query_StronglyTyped()
    {
        // Arrange
        var connection = new XmlConnection(ConnectionStrings.Instance.FileAsDB_eCom);
        connection.Open();

        // Act
        var rows = connection.
            Query<Records>("SELECT c.CustomerName, [o].[OrderDate], [oi].[Quantity], [p].[Name] FROM [Customers c] INNER JOIN [Orders o] ON [c].[ID] = [o].[CustomerID] INNER JOIN [OrderItems oi] ON [o].[ID] = [oi].[OrderID] INNER JOIN [Products p] ON [p].[ID] = [oi].[ProductID]").
            ToList();

        Assert.True(rows.Any());

        foreach (var item in rows)
        {
            //To make sure that the INNER JOIN succeeded, make sure that we have data from each table involved.
            Assert.False(string.IsNullOrEmpty(item.CustomerName));
            Assert.False(item.OrderDate == default);
            Assert.False(item.Quantity == 0);
            Assert.False(string.IsNullOrEmpty(item.Name));
        }

    }


    private class Records
    {
        public string? CustomerName { get; set; }
        public DateTime OrderDate { get; set; }
        public int Quantity { get; set; }
        public string? Name { get; set; }
    }

}
