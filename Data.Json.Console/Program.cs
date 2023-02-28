using Dapper;
using System;
using System.Data;
using System.Data.JsonClient;
using System.Data.SqlClient;
using System.Reflection;


var rootFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
for (int i = 0; i < 4; i++)
    rootFolder = Directory.GetParent(rootFolder!)!.FullName;
var jsonFile = Path.Combine(rootFolder!, "tests", "Data.Json.Tests", "Sources", "database.json");

string jsonConnectionString = $"Data Source={jsonFile};";


rootFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
for (int i = 0; i < 4; i++)
    rootFolder = Directory.GetParent(rootFolder!)!.FullName;
jsonFile = Path.Combine(rootFolder!, "tests", "Data.Json.Tests", "Sources", "ecommerce.json");

jsonConnectionString = $"Data Source={jsonFile};";

var connection = new JsonConnection(jsonConnectionString);
connection.Open();
var rows = connection.Query<Records>("SELECT c.CustomerName, [o].[OrderDate], [oi].[Quantity], [p].[Name] FROM [Customers c] INNER JOIN [Orders o] ON [c].[ID] = [o].[CustomerID] INNER JOIN [OrderItems oi] ON [o].[ID] = [oi].[OrderID] INNER JOIN [Products p] ON [p].[ID] = [oi].[ProductID]");

Console.Write($"{"Customer Name",-30}");
Console.Write($"{"Order Date",-30}");
Console.Write($"{"Quantity",-30}");
Console.Write($"{"Name",-30}");

foreach (var item in rows)
{
    Console.WriteLine();
    Console.Write($"{item.CustomerName,-30}");
    Console.Write($"{item.OrderDate.ToString(),-30}");
    Console.Write($"{item.Quantity,-30}");
    Console.Write($"{item.Name,-30}");
}
Console.ReadLine();
Console.WriteLine();

public class Records
{
    public string? CustomerName { get; set; }
    public DateTime OrderDate { get; set; }
    public string? Quantity { get; set; }
    public string? Name { get; set; }
}
