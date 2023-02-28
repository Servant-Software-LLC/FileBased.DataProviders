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






//var con = new JsonConnection(jsonConnectionString);
//string query = "select name,email from employees";
//var selectCommand = new JsonCommand(query);
//var nameParameter = new JsonParameter("empname", "Shahid");
//selectCommand.Parameters.Add(nameParameter);
//selectCommand.Connection = con;
//JsonDataAdapter dataAdpater = new JsonDataAdapter(selectCommand);

//var dataset = new DataSet();
//dataset.Tables.Add(new DataTable() { TableName = "Table" });
////var cmd = new SqlCommand("DELETE from customer where id=6", con);
////dataAdpater.UpdateCommand = cmd;


//dataAdpater.Fill(dataset);
//dataset.Tables[0].Rows.Remove(dataset.Tables[0].Rows[0]);
//dataset.Tables[0].Rows[0][1] = "shahhd";

//var updateQuery = "update employees set email=@empname";
//var updateCommand = new JsonCommand(updateQuery, con);
//nameParameter.Value = "Shhaid Updarted";
//nameParameter.SourceColumn = "email";
//updateCommand.Parameters.Add(nameParameter);
//dataAdpater.UpdateCommand = updateCommand;


//dataAdpater.Update(dataset);
//con.Open();










string sqlConnectionStr = "Server=Localhost\\SQLEXPRESS;Database=EcommerceDB;Trusted_Connection=True;MultipleActiveResultSets=True";
var con = new SqlConnection(sqlConnectionStr);
string query = "select name,email from customer;";
var dataAdpater = new SqlDataAdapter(query, con);
//dataAdpater.SelectCommand.Parameters.AddWithValue("id", 6);
var dataset = new DataSet();
dataset.Tables.Add(new DataTable() { TableName = "Table34" });
var cmd = new SqlCommand("update customer set id=6", con);
dataAdpater.UpdateCommand = cmd;


var data=dataAdpater.FillSchema(dataset,SchemaType.Mapped);
dataset.Tables[0].Rows.Remove(dataset.Tables[0].Rows[0]);
dataAdpater.Update(dataset);
con.Open();
























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
