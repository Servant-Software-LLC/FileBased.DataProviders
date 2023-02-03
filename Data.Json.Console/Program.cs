// See https://aka.ms/new-console-template for more information
using Dapper;
using System.Data.JsonClient;

Console.WriteLine("Hello, World!");
string connectionString = @"Data Source=C:\Users\GAC\source\repos\ADO.NET.FileBased.DataProviders\tests\Data.Json.Tests\Sources\database.json;";

var connection = new JsonConnection(connectionString);
//var command = new JsonCommand("SELECT [NAME],[SALARY] FROM [employees]", connection);
connection.Open();
var rows=connection.ExecuteScalar<int>("SELECT COUNT(*) FROM [employees] where salary=10 OR name='Mike'");
Console.ReadLine();
//// Act
//connection.Open();
//var reader = command.ExecuteReader();
//var fieldCount = reader.FieldCount;
