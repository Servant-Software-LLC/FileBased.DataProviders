using Data.Common.Extension;
using Data.Tests.Common;
using System.Data;
using System.Data.JsonClient;
using System.Reflection;
using Xunit;

namespace Data.Json.Tests.FileAsDatabase;

public class JsonInsertTests
{
    [Fact]
    public void Insert_ShouldInsertData()
    {
        var sandboxId = $"{GetType().FullName}.{MethodBase.GetCurrentMethod()!.Name}";
        InsertTests.Insert_ShouldInsertData(() => new JsonConnection(ConnectionStrings.Instance.FileAsDB.Sandbox("Sandbox", sandboxId)));
    }

    //This is a special case.  In json, if a table does not have any rows in it, then we have no schema information on the columns or their data types.
    //Inserting the first row into this table will then determine the columns (along with their data types) in this table.
    [Fact]
    public void Insert_ShouldInsertDataIntoEmptyTables()
    {
        var sandboxId = $"{GetType().FullName}.{MethodBase.GetCurrentMethod()!.Name}";
        var sandboxConnectionString = ConnectionStrings.Instance.EmptyWithTablesFileAsDB.Sandbox("Sandbox", sandboxId);
        InsertTests.Insert_ShouldInsertData(() => new JsonConnection(sandboxConnectionString));

        //Extra assertions to verify that our JSON tables now have a schema 
        using (var connection = new JsonConnection(sandboxConnectionString))
        {
            //Copy the contents of the tables into a DataSet
            var adapter = connection.CreateDataAdapter("SELECT * FROM locations");
            var dataset = new DataSet();
            adapter.Fill(dataset);

            //Verify that the DataColumns are of the correct schema type.
            //Table: locations
            var locationsTable = dataset.Tables[0];
            var idColumn = locationsTable.Columns["id"];
            Assert.Equal(typeof(decimal), idColumn.DataType);
            var cityColumn = locationsTable.Columns["city"];
            Assert.Equal(typeof(string), cityColumn.DataType);
            var stateColumn = locationsTable.Columns["state"];
            Assert.Equal(typeof(string), stateColumn.DataType);
            var zipColumn = locationsTable.Columns["zip"];
            Assert.Equal(typeof(decimal), zipColumn.DataType);

            adapter = connection.CreateDataAdapter("SELECT * FROM employees");
            adapter.Fill(dataset);

            //Table: employees
            var employeesTable = dataset.Tables[0];
            var nameColumn = employeesTable.Columns["name"];
            Assert.Equal(typeof(string), nameColumn.DataType);
            var emailColumn = employeesTable.Columns["email"];
            Assert.Equal(typeof(string), emailColumn.DataType);
            var salaryColumn = employeesTable.Columns["salary"];
            Assert.Equal(typeof(decimal), salaryColumn.DataType);
            var marriedColumn = employeesTable.Columns["married"];

            //NOTE:  When https://github.com/Servant-Software-LLC/ADO.NET.FileBased.DataProviders/issues/22 is solved, then 
            //       this assert will fail, because it needs to have an expectation of:
            // Assert.Equal(typeof(boolean), marriedColumn.DataType);
            Assert.Equal(typeof(string), marriedColumn.DataType);
        }

    }

    [Fact]
    public void Insert_JsonShouldBeFormatted()
    {
        InsertTests.Insert_ShouldBeFormattedForFile(() =>
        new JsonConnection(ConnectionStrings.Instance
        .FileAsDB.AddFormatted(true)));
    }

    [Fact]
    public void Insert_IndentityColumn_NoLastRow()
    {
        //NOTE: Without a single row, there is no way for the Json Provider to 'know' of an indentity column
    }

    [Fact]
    public void Insert_IndentityColumn_LastRow_Decimal()
    {
        var sandboxId = $"{GetType().FullName}.{MethodBase.GetCurrentMethod()!.Name}";
        InsertTests.Insert_IndentityColumn_LastRow_Decimal(
            () => new JsonConnection(ConnectionStrings.Instance.FileAsDB.Sandbox("Sandbox", sandboxId))
        );
    }

}