using Dapper;
using System.Data.CsvClient;
using Xunit;

namespace Data.Csv.Tests;

public class DapperIntegrationTests
{
    [Fact(Skip = "TODO: Since CSV provider does not have FileAsDatabase, we need to have source files for the individual tables that are in the ecommerce database")]
    public void Query_StronglyTyped()
    {
        // Arrange
        var connection = new CsvConnection(ConnectionStrings.FolderAsDBConnectionString);
        connection.Open();

        // Act
        var rows = connection.
            Query<Records>("SELECT c.CustomerName, [o].[OrderDate], [oi].[Quantity], [p].[Name] FROM [Customers c] INNER JOIN [Orders o] ON [c].[ID] = [o].[CustomerID] INNER JOIN [OrderItems oi] ON [o].[ID] = [oi].[OrderID] INNER JOIN [Products p] ON [p].[ID] = [oi].[ProductID]").
            ToList();

        foreach (var item in rows)
        {
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
