using Dapper;
using System.Data.JsonClient;

string connectionString = @"Data Source=C:\Users\GAC\source\repos\ADO.NET.FileBased.DataProviders\tests\Data.Json.Tests\Sources\ecommerce.json;";

var connection = new JsonConnection(connectionString);
connection.Open();
var rows = connection.Query<Records>("SELECT c.CustomerName, [o].[OrderDate], [oi].[Quantity], [p].[Name] FROM [Customers c] INNER JOIN [Orders o] ON [c].[ID] = [o].[CustomerID] INNER JOIN [OrderItems oi] ON [o].[ID] = [oi].[OrderID] INNER JOIN [Products p] ON [p].[ID] = [oi].[ProductID]");

Console.Write($"{"Customer Name",-30}");
Console.Write($"{"Order Date",-30}");
Console.Write($"{"Quantity",-30}");
Console.Write($"{"Name",-30}");

foreach (var item in rows)
{
    Console.WriteLine();
    Console.Write($"{item.CustomerName, -30}");
    Console.Write($"{item.OrderDate.ToString(), -30}");
    Console.Write($"{item.Quantity, -30}");
    Console.Write($"{item.Name, -30}");
}
Console.ReadLine();
Console.WriteLine();

public class Records
{
    public string CustomerName { get; set; }
    public DateTime OrderDate { get; set; }
    public string Quantity { get; set; }
    public string Name { get; set; }
}
