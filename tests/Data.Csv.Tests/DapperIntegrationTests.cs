using Dapper;
using System.Data.CsvClient;
using Xunit;

namespace Data.Csv.Tests;

public class DapperIntegrationTests
{
    [Fact]
    public void Query_StronglyTyped()
    {
        // Arrange
        var connection = new CsvConnection(ConnectionStrings.Instance.eComFolderDB);
        connection.Open();

        // Act
        var rows = connection.
            Query<Records>(@"
SELECT c.CustomerName, [o].[OrderDate], [oi].[Quantity], [p].[Name] 
    FROM [Customers] c 
    INNER JOIN [Orders] o ON [c].[ID] = [o].[CustomerID] 
    INNER JOIN [OrderItems] oi ON [o].[ID] = [oi].[OrderID] 
    INNER JOIN [Products] p ON [p].[ID] = [oi].[ProductID]
").
            ToList();

        Assert.Equal(40, rows.Count);

        foreach (var item in rows)
        {
            //To make sure that the INNER JOIN succeeded, make sure that we have data from each table involved.
            Assert.False(string.IsNullOrEmpty(item.CustomerName));
            Assert.False(item.OrderDate == default);
            Assert.False(string.IsNullOrEmpty(item.Quantity));
            Assert.False(string.IsNullOrEmpty(item.Name));
        }
    }


    private class Records
    {
        public string? CustomerName { get; set; }
        public DateTime OrderDate { get; set; }
        public string? Quantity { get; set; }
        public string? Name { get; set; }
    }
}
