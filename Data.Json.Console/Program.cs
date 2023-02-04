
//DataSet ecommerceDS = new DataSet("EcommerceDB");

//// Create customers table
//DataTable customers = new DataTable("Customers");
//customers.Columns.Add("customer_id", typeof(int));
//customers.Columns.Add("first_name", typeof(string));
//customers.Columns.Add("last_name", typeof(string));
//customers.Columns.Add("email", typeof(string));
//customers.PrimaryKey = new DataColumn[] { customers.Columns["customer_id"] };
//ecommerceDS.Tables.Add(customers);

//// Add data to customers table
//customers.Rows.Add(1, "John", "Doe", "johndoe@email.com");
//customers.Rows.Add(2, "Jane", "Doe", "janedoe@email.com");
//customers.Rows.Add(3, "Jim", "Smith", "jimsmith@email.com");
//customers.Rows.Add(4, "Jane", "Smith", "janesmith@email.com");

//// Create orders table
//DataTable orders = new DataTable("Orders");
//orders.Columns.Add("order_id", typeof(int));
//orders.Columns.Add("customer_id", typeof(int));
//orders.Columns.Add("order_date", typeof(DateTime));
//orders.PrimaryKey = new DataColumn[] { orders.Columns["order_id"] };
//ecommerceDS.Tables.Add(orders);

//// Add data to orders table
//orders.Rows.Add(1, 1, new DateTime(2022, 01, 01));
//orders.Rows.Add(2, 1, new DateTime(2022, 01, 02));
//orders.Rows.Add(3, 2, new DateTime(2022, 02, 01));
//orders.Rows.Add(4, 3, new DateTime(2022, 03, 01));

//// Create order_items table
//DataTable order_items = new DataTable("Order_Items");
//order_items.Columns.Add("order_item_id", typeof(int));
//order_items.Columns.Add("order_id", typeof(int));
//order_items.Columns.Add("product_id", typeof(int));
//order_items.Columns.Add("quantity", typeof(int));
//order_items.PrimaryKey = new DataColumn[] { order_items.Columns["order_item_id"] };
//ecommerceDS.Tables.Add(order_items);

//// Add data to order_items table
//order_items.Rows.Add(1, 1, 101, 2);
//order_items.Rows.Add(2, 1, 102, 3);
//order_items.Rows.Add(3, 2, 101, 1);
//order_items.Rows.Add(4, 2, 102, 2);
//order_items.Rows.Add(5, 3, 101, 1);
//order_items.Rows.Add(6, 4, 101, 1);
//order_items.Rows.Add(7, 4, 102, 1);
//order_items.Rows.Add(8, 4, 103, 1);


//var join = new datat("Orders", "customer_id", "customer_id");
//var innserJoin = new DataTableInnerJoin("Order_Items", "order_id", "order_id");
//join.InnerJoin.Add(innserJoin);

//RecursiveTableJoin Join = new RecursiveTableJoin(join,"Customers");
//var f = Join.Join(ecommerceDS);

//foreach (DataColumn col in f.Columns)
//{
//    Console.Write($"{col.ColumnName,-20}");
//}
//foreach (DataRow row in f.Rows)
//{
//    Console.WriteLine();
//    foreach (DataColumn col in f.Columns)
//    {
//        Console.Write($"{row[col.ColumnName],-20}");

//    }
//}

using Dapper;
using System.Data.JsonClient;

string connectionString = @"Data Source=C:\Users\GAC\source\repos\ADO.NET.FileBased.DataProviders\tests\Data.Json.Tests\Sources\ecommerce.json;";

var connection = new JsonConnection(connectionString);
//var command = new JsonCommand("SELECT [NAME],[SALARY] FROM [employees]", connection);
connection.Open();
var rows = connection.Query("SELECT * from [OrderItems]");
var rows2 = connection.Query("SELECT * from [Orders]");

Console.ReadLine();

//connection.Open();
//var reader = command.ExecuteReader();
//var fieldCount = reader.FieldCount;

